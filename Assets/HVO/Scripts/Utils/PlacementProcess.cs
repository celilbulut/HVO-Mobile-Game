
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess
{
    private GameObject m_PlacementOutline;
    private BuildActionSO m_BuildAction;
    private Vector3Int[] m_HighlightPositions;
    private Tilemap m_WalkableTilemap;
    private Tilemap m_OverlayTileMap;
    private Tilemap[] m_UnreachableTileMaps;
    private Sprite m_PlaceholderTileSprite;
    private Color m_HighlightColor = new Color(0, 0.8f, 1, 0.4f);
    private Color m_BlockedColor = new Color(1f, 0.2f, 0, 0.8f);

    public BuildActionSO BuildAction => m_BuildAction;
    public int GoldCost => m_BuildAction.GoldCost;
    public int WoodCost => m_BuildAction.WoodCost;


    public PlacementProcess(BuildActionSO buildActionSO, 
                            Tilemap walkableTilemap, 
                            Tilemap overlayTilemap,
                            Tilemap[] unreachableTileMaps)                            
    {
        m_PlaceholderTileSprite = Resources.Load<Sprite>("Images/PlaceholderTileSprite"); //Yazim onemli.
        m_BuildAction = buildActionSO;
        m_WalkableTilemap = walkableTilemap;
        m_OverlayTileMap = overlayTilemap;
        m_UnreachableTileMaps = unreachableTileMaps;
    }

    public void Update() 
    {
       
        if (m_PlacementOutline != null)
        {
            HighlightTiles(m_PlacementOutline.transform.position);
        }
        
        if (HvoUtils.IsPointerOverUIElement()) return;

        if (HvoUtils.TryGetHoldPosition(out Vector3 worldPosition))
        {
            m_PlacementOutline.transform.position = SnapToGrid(worldPosition);
        }
    }

    public void ShowPlacementOutline()
    {
        m_PlacementOutline = new GameObject("PlacementOutline");
        var renderer = m_PlacementOutline.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 999; // Order in the layer
        renderer.color = new Color(1, 1, 1, 0.5f); // Yari transparent yapiyoruz bu sayede.
        renderer.sprite = m_BuildAction.PlacementSprite; // Towerin seklini cikartmak icin.
    }

    public void Cleanup() //UI da kalan placementlari kaldiriyor.
    {
        Object.Destroy(m_PlacementOutline);
        ClearHighlights();        
    }

    public bool TryFinalizePlacement(out Vector3 buildPosition)
    {
        if(IsPlacementAreaValid())
        {
            ClearHighlights();
            buildPosition = m_PlacementOutline.transform.position;
            Object.Destroy(m_PlacementOutline);
            return true;
        }

        Debug.Log("Invalid Placement Area");
        buildPosition = Vector3.zero;
        return false;
    }

    bool IsPlacementAreaValid()
    {
        foreach(var tilePosition in m_HighlightPositions)
        {
            if(!CanPlaceTile(tilePosition)) return false;
        }

        return true;
    }

    Vector3 SnapToGrid(Vector3 worldPosition)
    {
        return new Vector3(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), 0);
    }
    void HighlightTiles(Vector3 outlinePosition)
    {
        Vector3Int buildingSize = m_BuildAction.BuildingSize;
        Vector3 pivotPosition = outlinePosition + m_BuildAction.OriginOffset;

        ClearHighlights();

        m_HighlightPositions = new Vector3Int[buildingSize.x * buildingSize.y];

        for(int x= 0; x < buildingSize.x; x++)
        {
            for(int y = 0; y < buildingSize.y; y++)
            {
                m_HighlightPositions[x + y * buildingSize.x] = new Vector3Int((int)pivotPosition.x + x, (int)pivotPosition.y + y, 0);
            }
        }

        foreach (var tilePosition in m_HighlightPositions)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = m_PlaceholderTileSprite;

            if(CanPlaceTile(tilePosition))
            {
                tile.color = m_HighlightColor;
            }
            else
            {
                tile.color = m_BlockedColor;                
            }

            m_OverlayTileMap.SetTile(tilePosition, tile);            
        }
    }

    void ClearHighlights()
    {
        if(m_HighlightPositions == null) return;

        foreach(var tilePosition in m_HighlightPositions)
        {
            m_OverlayTileMap.SetTile(tilePosition, null);
        }
        m_HighlightPositions = null;
    }

    bool CanPlaceTile(Vector3Int tilePosition)
    {
        return m_WalkableTilemap.HasTile(tilePosition) && !IsInUnreachableTilemap(tilePosition) && !IsBlockedByGameObject(tilePosition);
    }

    bool IsInUnreachableTilemap(Vector3Int tilePosition)
    {
        foreach(var tilemap in m_UnreachableTileMaps)
        {
            if(tilemap.HasTile(tilePosition)) return true;
        }

        return false;
    }

    bool IsBlockedByGameObject(Vector3Int tilePosition)
    {
        Vector3 tileSize = m_WalkableTilemap.cellSize;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize / 2, tileSize * 0.5f, 0);

        foreach(var collider in colliders)
        {
            var layer = collider.gameObject.layer;
            if(layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }
        
        return false;
    }
}
