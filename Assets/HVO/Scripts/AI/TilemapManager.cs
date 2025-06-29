using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SingletonManager<TilemapManager>
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_OverlayTileMap;
    [SerializeField] private Tilemap[] m_UnreachableTilemaps;

    private Pathfinding m_Pathfinding;
    public Tilemap PathfindingTilemap => m_WalkableTilemap;

    void Start()
    {
        m_Pathfinding = new Pathfinding(this);
    }

    public List<Node> FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        return m_Pathfinding.FindPath(startPosition, endPosition);
    }

    public bool CanWalkAtTile(Vector3Int tilePosition)
    {
        return m_WalkableTilemap.HasTile(tilePosition) && !IsInUnreachableTilemap(tilePosition);
    }

    public bool CanPlaceTile(Vector3Int tilePosition)
    {
        return m_WalkableTilemap.HasTile(tilePosition) && !IsInUnreachableTilemap(tilePosition) && !IsBlockedByGameObject(tilePosition);
    }

    public bool IsInUnreachableTilemap(Vector3Int tilePosition)
    {
        foreach (var tilemap in m_UnreachableTilemaps)
        {
            if (tilemap.HasTile(tilePosition)) return true;
        }

        return false;
    }

    public bool IsBlockedByGameObject(Vector3Int tilePosition)
    {
        Vector3 tileSize = m_WalkableTilemap.cellSize;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize / 2, tileSize * 0.5f, 0);

        foreach (var collider in colliders)
        {
            var layer = collider.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public void SetTileOverlay(Vector3Int tilePosition, Tile tile)
    {
        m_OverlayTileMap.SetTile(tilePosition, tile);
    }
}
