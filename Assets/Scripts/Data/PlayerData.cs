using UnityEngine;

/// <summary>
/// Tunable values for the player character. All player balance numbers live here so they
/// can be adjusted in the Inspector without touching <see cref="PlayerController"/>.
/// Asset lives at Assets/Data/Player/PlayerData.asset.
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "HexForge/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("Top movement speed in world-units per second.")]
    public float moveSpeed = 4.0f;

    [Tooltip("How quickly the player ramps up toward moveSpeed, in units/second^2. Higher = snappier starts.")]
    public float acceleration = 30.0f;

    [Tooltip("How quickly the player slows to a stop when there is no input, in units/second^2. Higher = snappier stops.")]
    public float deceleration = 45.0f;
}
