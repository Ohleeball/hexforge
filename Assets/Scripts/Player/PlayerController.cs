using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

/// <summary>
/// Drives the player character during COMBAT mode (see docs/systems/player_character.md):
/// WASD free movement across walkable hex tiles with axis-separated sliding against
/// unwalkable cells, mouse-driven facing independent of movement, and idle/walk animation.
/// Movement is physics-driven via Rigidbody.MovePosition in FixedUpdate. All tunable
/// values come from <see cref="PlayerData"/> — nothing is hardcoded here.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerData data;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Grid grid;

    Rigidbody rb;
    Animator animator;
    Camera mainCamera;

    // Retained so the character keeps its last valid heading when the cursor leaves the map.
    Vector3 lastFacingDirection = Vector3.forward;

    // Current planar (X/Z) velocity, ramped toward the input target by acceleration/deceleration.
    Vector3 currentVelocity;

    // Speed (units/sec) below which the character is considered stopped for animation purposes.
    const float MovingThreshold = 0.05f;

    static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
    }

    // Movement and walkability run on the physics step so the collider interacts
    // correctly with enemies and wall geometry.
    void FixedUpdate()
    {
        // Movement is COMBAT-only; the player is frozen during PLANNING.
        if (GameManager.Instance != null && GameManager.Instance.State == GameState.PLANNING)
            return;

        // Ramp velocity toward the input-scaled target (accelerate while pressing, decelerate to rest when idle).
        Vector3 inputDirection = ReadInput();
        Vector3 targetVelocity = inputDirection * data.moveSpeed;
        float rate = inputDirection.sqrMagnitude > 0f ? data.acceleration : data.deceleration;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);

        if (currentVelocity.sqrMagnitude > 0f)
        {
            Vector3 step = currentVelocity * Time.fixedDeltaTime;
            Vector3 resolved = ResolveMove(rb.position, step);

            // If a wall blocked an axis, zero that velocity component so it doesn't accumulate against the wall.
            Vector3 actualStep = resolved - rb.position;
            if (step.x != 0f && Mathf.Abs(actualStep.x) < 1e-5f) currentVelocity.x = 0f;
            if (step.z != 0f && Mathf.Abs(actualStep.z) < 1e-5f) currentVelocity.z = 0f;

            rb.MovePosition(resolved);
        }

        if (animator != null)
            animator.SetBool(IsMovingHash, currentVelocity.sqrMagnitude > MovingThreshold * MovingThreshold);
    }

    // Facing is decoupled from movement: the character always snaps to look at the
    // ground point under the mouse cursor.
    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mainCamera == null || mouse == null)
            return;

        // Manual intersection of the mouse ray with the Y = 0 ground plane.
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 lookDirection = ray.GetPoint(distance) - transform.position;
            lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude > 0.0001f)
                lastFacingDirection = lookDirection.normalized;
        }

        // Instant snap, no lerp. If the ray missed, lastFacingDirection is unchanged.
        transform.rotation = Quaternion.LookRotation(lastFacingDirection, Vector3.up);
    }

    /// <summary>
    /// Reads WASD/arrow input as an X/Z direction, normalized so diagonals aren't faster, then
    /// rotates it into camera space so the input axes line up with what the player sees on screen
    /// (see player_character.md — Movement). The camera.md isometric rig is yawed 45°, so raw
    /// world-space WASD would walk diagonally on screen; mapping the input through the camera's
    /// flattened right/forward basis keeps the controls screen-relative.
    /// </summary>
    Vector3 ReadInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector3.zero;

        float x = 0f;
        float z = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) z -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) z += 1f;

        Vector3 input = new Vector3(x, 0f, z);
        if (input.sqrMagnitude > 1f)
            input = input.normalized;

        // Fall back to raw world-space input if there is no camera to align against.
        if (mainCamera == null)
            return input;

        // Flatten the camera's basis onto the XZ plane so movement stays horizontal.
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        return cameraRight * input.x + cameraForward * input.z;
    }

    /// <summary>
    /// Axis-separated sliding. Tries the full move first; if the target cell is unwalkable,
    /// tries the X component alone, then the Z component alone, applying whichever lands on
    /// a walkable cell. If both axes are individually blocked, the player does not move.
    /// </summary>
    Vector3 ResolveMove(Vector3 current, Vector3 step)
    {
        Vector3 full = current + step;
        if (IsWalkable(full))
            return full;

        Vector3 xOnly = current + new Vector3(step.x, 0f, 0f);
        if (step.x != 0f && IsWalkable(xOnly))
            return xOnly;

        Vector3 zOnly = current + new Vector3(0f, 0f, step.z);
        if (step.z != 0f && IsWalkable(zOnly))
            return zOnly;

        return current;
    }

    /// <summary>True if the tile at the given world position exists and is walkable.</summary>
    bool IsWalkable(Vector3 worldPosition)
    {
        Vector3Int cell = grid.WorldToCell(worldPosition);
        HexTile tile = tilemap.GetTile<HexTile>(cell);
        return tile != null && tile.isWalkable;
    }
}
