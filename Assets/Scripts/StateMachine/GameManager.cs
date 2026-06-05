using UnityEngine;

/// <summary>
/// Top-level game state. The game is always in exactly one of two mutually exclusive
/// states: COMBAT or PLANNING (see docs/architecture.md — Game State Machine).
/// </summary>
public enum GameState
{
    COMBAT,
    PLANNING
}

/// <summary>
/// Minimal Phase 1 game state machine. Exposes the current <see cref="GameState"/> via a
/// singleton <see cref="Instance"/> so systems (e.g. PlayerController) can gate behaviour
/// on it. Starts in COMBAT. The base-perimeter trigger that flips to PLANNING and the
/// associated Time.timeScale handling are wired in a later phase — this only owns and
/// exposes the state.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameState state = GameState.COMBAT;

    /// <summary>Current top-level game state. Read-only to other systems.</summary>
    public GameState State => state;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
