using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The three mutually exclusive camera states (see docs/systems/camera.md section 7).
/// Transitions are driven externally via the public Enter*/Exit* methods on
/// <see cref="IsometricCameraController"/>; the controller never polls for player state.
/// </summary>
public enum CameraState
{
    NormalFollow,
    Death,
    Planning
}

/// <summary>
/// Drives the fixed isometric camera rig (see docs/systems/camera.md). Lives on the
/// CameraRig GameObject; the orthographic Camera is a child offset back along local Z.
/// The rig rotation is locked to the isometric angles on Awake and never changes at runtime.
///
/// Behaviour per state:
///  - NormalFollow: snappy follow of the player plus a smoothed, mouse-driven aim offset.
///  - Death:        rig frozen at the point of death, zooms out, aim offset disabled.
///  - Planning:     rig recentres on the castle tile, zooms out, follow + aim disabled.
///
/// All tunable values are exposed in the Inspector (section 4). Uses Unity's new Input System.
/// </summary>
public class IsometricCameraController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tunable camera values (zoom, aim, lerp speeds, iso angles). See docs/systems/camera.md section 4.")]
    [SerializeField] CameraSettings settings;

    [Header("Scene References")]
    [Tooltip("The child orthographic Camera whose size this controller drives. Auto-found in Awake if left empty.")]
    [SerializeField] Camera cam;

    [Tooltip("The player transform the rig follows during NormalFollow. Wired in the Inspector.")]
    [SerializeField] Transform playerTarget;

    [Tooltip("Hex Grid used to resolve the castle (center cell) world position for planning mode.")]
    [SerializeField] Grid grid;

    // The castle is always painted at the center cell by BaseSpawner (BaseSpawner.Center).
    static readonly Vector3Int CastleCell = Vector3Int.zero;

    CameraState _state;
    Vector3 _baseCenterPoint;     // resolved castle world position (planning mode target)
    Vector3 _currentAimOffset;    // smoothed aim offset currently applied to the rig
    float _targetOrthoSize;       // orthographic size the current state is easing toward

    /// <summary>
    /// Caches references, locks the isometric rotation, resolves the castle world position,
    /// initialises the zoom, and snaps the rig to the player so the first frame has no pop.
    /// </summary>
    void Awake()
    {
        if (settings == null)
        {
            Debug.LogError("[IsometricCameraController] No CameraSettings assigned; disabling controller.", this);
            enabled = false;
            return;
        }

        if (cam == null)
            cam = GetComponentInChildren<Camera>();

        // Lock the isometric framing once; rotation is fixed and never touched again at runtime.
        transform.rotation = Quaternion.Euler(settings.cameraElevationAngle, settings.cameraYawAngle, 0f);

        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = settings.normalZoomSize;
        }
        _targetOrthoSize = settings.normalZoomSize;

        // TODO: Design mismatch — the doc resolves BaseCenterPoint via BuildingManager or a
        // tagged castle GameObject, but neither exists in the project yet. The castle is always
        // painted at the fixed center cell (Vector3Int.zero) by BaseSpawner, so we resolve its
        // world position from the Grid instead. Revisit if a BuildingManager is added later.
        _baseCenterPoint = grid != null ? grid.CellToWorld(CastleCell) : transform.position;

        // Start locked to the player so there is no visible snap on the first frame.
        if (playerTarget != null)
            transform.position = playerTarget.position;

        _state = CameraState.NormalFollow;
    }

    void Update()
    {
        switch (_state)
        {
            case CameraState.NormalFollow: UpdateNormalFollow(); break;
            case CameraState.Death: UpdateDeath(); break;
            case CameraState.Planning: UpdatePlanning(); break;
        }

        UpdateZoom();
    }

    /// <summary>
    /// Snappy player follow plus a smoothed, screen-space aim offset. The aim offset is
    /// smoothed independently (aimOffsetSpeed), added to the player position, and the combined
    /// target is then lerped at the high follow speed so movement feels nearly instant.
    /// </summary>
    void UpdateNormalFollow()
    {
        if (playerTarget == null)
            return;

        Vector3 targetAimOffset = CalculateAimOffset();
        _currentAimOffset = Vector3.Lerp(_currentAimOffset, targetAimOffset, settings.aimOffsetSpeed * Time.deltaTime);

        Vector3 targetPosition = playerTarget.position + _currentAimOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, settings.playerFollowSpeed * Time.deltaTime);
    }

    // Rig position is frozen at the point of death; only the zoom-out (UpdateZoom) animates.
    void UpdateDeath() { }

    // Eases the rig to the castle centre while the zoom-out animates; follow and aim are disabled.
    void UpdatePlanning()
    {
        transform.position = Vector3.Lerp(transform.position, _baseCenterPoint, settings.planningTransitionSpeed * Time.deltaTime);
    }

    /// <summary>Eases the camera's orthographic size toward the current state's target each frame.</summary>
    void UpdateZoom()
    {
        if (cam == null)
            return;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, _targetOrthoSize, settings.zoomSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Reads the mouse via the new Input System and converts it into a world-space XZ offset.
    /// The mouse is measured relative to screen centre, normalized to [-1, 1] per axis and
    /// clamped to unit magnitude, then rotated by the camera yaw so the offset lies flat on the
    /// world XZ plane and scales linearly with distance from centre up to aimOffsetMaxDistance.
    /// </summary>
    Vector3 CalculateAimOffset()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null)
            return Vector3.zero;

        Vector2 mousePosition = mouse.position.ReadValue();
        float halfWidth = Screen.width * 0.5f;
        float halfHeight = Screen.height * 0.5f;
        float normalizedX = (mousePosition.x - halfWidth) / halfWidth;
        float normalizedY = (mousePosition.y - halfHeight) / halfHeight;

        // Clamp magnitude (not each axis) so the offset direction is preserved on multi-monitor setups.
        float normalizedMagnitude = Mathf.Clamp01(new Vector2(normalizedX, normalizedY).magnitude);

        // Project the screen vector onto the world XZ plane using the rig's own flattened basis
        // (see camera.md §6): screen-right maps to the rig's right, screen-up maps to the rig's
        // forward. Deriving the offset from the rig's actual right/forward vectors is
        // orientation-correct regardless of yaw sign conventions or grid swizzle handedness.
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;
        right.y = 0f;
        forward.y = 0f;
        right.Normalize();
        forward.Normalize();

        Vector3 worldOffset = right * normalizedX + forward * normalizedY;
        return worldOffset.normalized * (normalizedMagnitude * settings.aimOffsetMaxDistance);
    }

    // ── Public state transitions (called by external systems: player health, planning UI) ──

    /// <summary>Player died: freeze the rig at its current position and zoom out to DeathZoomSize.</summary>
    public void EnterDeathState()
    {
        _state = CameraState.Death;
        _targetOrthoSize = settings.deathZoomSize;
    }

    /// <summary>
    /// Respawn complete: snap the rig to the (new) player spawn position while still zoomed out,
    /// clear the aim offset, and return to NormalFollow so the zoom eases back to NormalZoomSize.
    /// </summary>
    public void EnterRespawnState()
    {
        if (playerTarget != null)
            transform.position = playerTarget.position;

        _currentAimOffset = Vector3.zero;
        _targetOrthoSize = settings.normalZoomSize;
        _state = CameraState.NormalFollow;
    }

    /// <summary>Entered planning/building: recentre the rig on the castle and zoom out to PlanningZoomSize.</summary>
    public void EnterPlanningState()
    {
        _state = CameraState.Planning;
        _targetOrthoSize = settings.planningZoomSize;
    }

    /// <summary>
    /// Exited planning/building: snap the rig back to the player instantly, clear the aim offset,
    /// and return to NormalFollow so the zoom eases back to NormalZoomSize.
    /// </summary>
    public void ExitPlanningState()
    {
        if (playerTarget != null)
            transform.position = playerTarget.position;

        _currentAimOffset = Vector3.zero;
        _targetOrthoSize = settings.normalZoomSize;
        _state = CameraState.NormalFollow;
    }
}
