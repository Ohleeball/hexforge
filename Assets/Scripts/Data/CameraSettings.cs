using UnityEngine;

/// <summary>
/// Tunable values for the isometric camera (see docs/systems/camera.md section 4). All camera
/// balance numbers live here so they can be adjusted in the Inspector without touching
/// <see cref="IsometricCameraController"/>. Scene references (camera, player, grid) are not
/// settings and remain on the controller component itself.
/// </summary>
[CreateAssetMenu(fileName = "CameraSettings", menuName = "HexForge/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    [Header("Zoom (orthographic size)")]
    [Tooltip("Orthographic size during normal gameplay.")]
    public float normalZoomSize = 8.0f;

    [Tooltip("Orthographic size during the death state (zoomed out).")]
    public float deathZoomSize = 14.0f;

    [Tooltip("Orthographic size during planning/building mode (zoomed out to show the full base).")]
    public float planningZoomSize = 18.0f;

    [Header("Aim Offset")]
    [Tooltip("Max world-unit offset toward the cursor when the mouse is at the screen edge.")]
    public float aimOffsetMaxDistance = 4.0f;

    [Header("Lerp Speeds")]
    [Tooltip("Rig tracking speed toward the player (high = snappy).")]
    public float playerFollowSpeed = 25.0f;

    [Tooltip("Smoothing speed for the aim offset.")]
    public float aimOffsetSpeed = 12.0f;

    [Tooltip("Smoothing speed for orthographic size transitions.")]
    public float zoomSpeed = 6.0f;

    [Tooltip("Rig position shift speed when moving to the castle for planning mode.")]
    public float planningTransitionSpeed = 5.0f;

    [Header("Isometric Rotation (locked at runtime)")]
    [Tooltip("X rotation of the camera rig (isometric elevation).")]
    public float cameraElevationAngle = 30.0f;

    [Tooltip("Y rotation of the camera rig (isometric diagonal).")]
    public float cameraYawAngle = 45.0f;
}
