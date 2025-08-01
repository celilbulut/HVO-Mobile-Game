using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SingletonManager<TilemapManager>
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_OverlayTileMap;
    [SerializeField] private Tilemap m_UnbuildableTileMap;

    [SerializeField] private Tilemap[] m_UnreachableTilemaps;

    private Pathfinding m_Pathfinding;
    public Tilemap PathfindingTilemap => m_WalkableTilemap;

    void Start()
    {
        m_Pathfinding = new Pathfinding(this);
    }

    public List<Vector3> FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        return m_Pathfinding.FindPath(startPosition, endPosition);
    }

    public Node FindNode(Vector3 position)
    {
        return m_Pathfinding.FindNode(position);
    }

    public void UpdateNodesInArea(Vector3Int startPosition, int width, int height)
    {
        m_Pathfinding.UpdateNodesInArea(startPosition, width, height);
    }

    public bool CanWalkAtTile(Vector3Int tilePosition)
    {
        return m_WalkableTilemap.HasTile(tilePosition)
               && !IsInUnreachableTilemap(tilePosition)
               && !IsBlockedByBuilding(tilePosition);
    }

    public bool CanPlaceTile(Vector3Int tilePosition)
    {
        return m_WalkableTilemap.HasTile(tilePosition)
               && !IsInUnreachableTilemap(tilePosition)
               && !IsBlockedByGameObject(tilePosition)
               && !m_UnbuildableTileMap.HasTile(tilePosition);
    }

    public bool IsInUnreachableTilemap(Vector3Int tilePosition)
    {
        foreach (var tilemap in m_UnreachableTilemaps)
        {
            if (tilemap.HasTile(tilePosition)) return true;
        }

        return false;
    }

    public bool IsBlockedByBuilding(Vector3Int tilePosition)
    {
        Vector3 worldPosition = m_WalkableTilemap.CellToWorld(tilePosition) + m_WalkableTilemap.cellSize / 2;
        int unitMask = 1 << LayerMask.NameToLayer("Unit"); // Unit layerina kadar kaydirma yapmak.

        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition, unitMask);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Building"))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBlockedByGameObject(Vector3Int tilePosition)
    {
        Vector3 tileSize = m_WalkableTilemap.cellSize;
        int unitMask = 1 << LayerMask.NameToLayer("Unit");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize / 2, tileSize * 0.5f, 0, unitMask);

        return colliders.Length > 0;
    }

    public void SetTileOverlay(Vector3Int tilePosition, Tile tile)
    {
        m_OverlayTileMap.SetTile(tilePosition, tile);
    }
}
