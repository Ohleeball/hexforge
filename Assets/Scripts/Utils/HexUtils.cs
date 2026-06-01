using UnityEngine;

public static class HexUtils
{
    // Flat-top hex, even-q offset (Unity Tilemap: Hexagonal Flat-Top, Stagger Index Even)
    // Axial (cube) directions for flat-top hex
    static readonly (int dq, int dr)[] CubeDirections =
    {
        ( 1,  0), ( 1, -1), ( 0, -1),
        (-1,  0), (-1,  1), ( 0,  1)
    };

    /// <summary>Returns the 6 flat-top edge-sharing neighbours of <paramref name="cell"/>.</summary>
    public static Vector3Int[] GetNeighbors(Vector3Int cell)
    {
        OffsetToCube(cell.x, cell.y, out int q, out int r);
        var result = new Vector3Int[6];
        for (int i = 0; i < 6; i++)
        {
            CubeToOffset(q + CubeDirections[i].dq, r + CubeDirections[i].dr, out int nx, out int ny);
            result[i] = new Vector3Int(nx, ny, 0);
        }
        return result;
    }

    public static int HexDistance(Vector3Int a, Vector3Int b)
    {
        OffsetToCube(a.x, a.y, out int aq, out int ar);
        OffsetToCube(b.x, b.y, out int bq, out int br);
        int dq = aq - bq, dr = ar - br;
        return (Mathf.Abs(dq) + Mathf.Abs(dq + dr) + Mathf.Abs(dr)) / 2;
    }

    // even-q offset ↔ axial (cube) — matches Unity Flat-Top / Stagger Index Even
    static void OffsetToCube(int col, int row, out int q, out int r)
    {
        q = col;
        r = row - (col + (col & 1)) / 2;
    }

    static void CubeToOffset(int q, int r, out int col, out int row)
    {
        col = q;
        row = r + (q + (q & 1)) / 2;
    }
}
