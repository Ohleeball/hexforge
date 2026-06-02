using UnityEngine;

public static class HexUtils
{
    // Unity Tilemap Grid: Cell Layout = Hexagon, Cell Swizzle = XZY.
    // Unity's Hexagon layout is point-top with row-staggered "odd-r" offset
    // coordinates (odd rows shifted +half a cell in X). The XZY swizzle only lays
    // the grid flat on the world XZ plane — it does NOT change which cells are
    // adjacent, so neighbour math is a pure cell-space (odd-r) property.
    // The 6 axial/cube unit directions below are layout-independent.
    static readonly (int dq, int dr)[] CubeDirections =
    {
        ( 1,  0), ( 1, -1), ( 0, -1),
        (-1,  0), (-1,  1), ( 0,  1)
    };

    /// <summary>Returns the 6 edge-sharing neighbours of <paramref name="cell"/>.</summary>
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

    // odd-r offset ↔ axial (cube) — matches Unity's Hexagon cell layout
    // (point-top, odd rows shifted right). x = column, y = row.
    // (row - (row & 1)) is always even, so the integer division is exact and
    // behaves correctly for negative rows.
    static void OffsetToCube(int col, int row, out int q, out int r)
    {
        q = col - (row - (row & 1)) / 2;
        r = row;
    }

    static void CubeToOffset(int q, int r, out int col, out int row)
    {
        col = q + (r - (r & 1)) / 2;
        row = r;
    }
}
