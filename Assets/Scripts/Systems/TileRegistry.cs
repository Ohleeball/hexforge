using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks per-cell buildability. BuildingManager reads this before allowing placement.
/// stone-tile cells are registered as buildable; tile-castle and tile-walls are not.
/// </summary>
public class TileRegistry : MonoBehaviour
{
    public static TileRegistry Instance { get; private set; }

    readonly HashSet<Vector3Int> _buildable = new HashSet<Vector3Int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>Registers a cell as buildable or unbuildable.</summary>
    public void Register(Vector3Int cell, bool buildable)
    {
        if (buildable)
            _buildable.Add(cell);
        else
            _buildable.Remove(cell);
    }

    public bool IsBuildable(Vector3Int cell) => _buildable.Contains(cell);
}
