using UnityEngine;

public class Pathfinding
{
    private int m_Width;
    private int m_Height;
    private Node[,] m_Grid;
    private TilemapManager m_TilemapManager;
    public Node[,] Grid => m_Grid;
    public Pathfinding(TilemapManager tilemapManager)
    {
        m_TilemapManager = tilemapManager;
        tilemapManager.PathfindingTilemap.CompressBounds();
        var bounds = tilemapManager.PathfindingTilemap.cellBounds;
        m_Width = bounds.size.x;
        m_Height = bounds.size.y;
        m_Grid = new Node[m_Width, m_Height];
        InitializeGrid();
    }

    void InitializeGrid() //Node tabanlı grid yapısı kurmak icin.
    {
        //Node in orta noktasini ayarliyor
        Vector3 halfCellSize = m_TilemapManager.PathfindingTilemap.cellSize / 2;
        Vector3Int offset = m_TilemapManager.PathfindingTilemap.cellBounds.min;

        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                Vector3Int nodeLeftButtomPosition = new Vector3Int(x + offset.x, y + offset.y);
                var nodeCenterPosition = nodeLeftButtomPosition + halfCellSize;

                bool isWalkable = m_TilemapManager.CanWalkAtTile(nodeLeftButtomPosition);

                var node = new Node(nodeCenterPosition.x, nodeCenterPosition.y, isWalkable);
                m_Grid[x, y] = node;

                if (!isWalkable)
                {
                    Debug.Log($"Node x: {x}, y: {y} | Position: Vector2({node.x}, {node.y}) | W: {node.isWalkable}");                    
                }
            }
        }
    }
}
