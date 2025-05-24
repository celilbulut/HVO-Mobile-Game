using UnityEngine;

public class Node
{
    public int x;
    public int y;
    public float centerX;
    public float centerY;
    public bool isWalkable;

    public Node(Vector3Int leftBottomPositon, Vector3 cellSize, bool isWalkable)
    {
        x = leftBottomPositon.x;
        y = leftBottomPositon.y;
        Vector3 halfCellSize = cellSize / 2;
        var nodeCenterPosition = leftBottomPositon + halfCellSize;
        centerX = nodeCenterPosition.x;
        centerY = nodeCenterPosition.y;

        this.isWalkable = isWalkable;
    }

    public override string ToString()
    {
        return $"({x}, {y})"; //Debug.Log un baska turu.
    }
}
