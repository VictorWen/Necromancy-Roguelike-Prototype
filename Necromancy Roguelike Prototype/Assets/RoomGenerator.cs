using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private int roomSize = 5;
    [SerializeField] private int columns = 5;
    [SerializeField] private int rows = 4;
    [SerializeField] private int spacing = 1;

    public int RoomSize { get { return roomSize; } }

    private HashSet<int>  openUp = new HashSet<int>() { 6, 7, 8, 9, 13, 14, 15, 16, 17, 18, 19 };
    private HashSet<int> openRight = new HashSet<int>() { 2, 4, 5, 7, 10, 12, 14, 15, 17, 18, 19 };
    private HashSet<int> openDown = new HashSet<int>() { 0, 2, 3, 9, 10, 11, 12, 13, 14, 17, 19 };
    private HashSet<int> openLeft = new HashSet<int>() { 1, 3, 4, 8, 11, 12, 13, 14, 16, 18, 19 };

    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public int GetNextRoom(int up, int right, int down, int left)
    {
        HashSet<int> valid = new HashSet<int>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                valid.Add(i * columns + j);
            }
        }
        if (openDown.Contains(up)) valid.IntersectWith(openUp);
        if (openLeft.Contains(right)) valid.IntersectWith(openRight);
        if (openUp.Contains(down)) valid.IntersectWith(openDown);
        if (openRight.Contains(left)) valid.IntersectWith(openLeft);

        return valid.ElementAt<int>(Random.Range(0, valid.Count));
    }

    public TileBase[,] GetRoom(int index)
    {
        if (index == -1)
            return new TileBase[roomSize, roomSize];

        tilemap = GetComponent<Tilemap>();
        TileBase[,] output = new TileBase[roomSize, roomSize];
        int col = index % columns;
        int row = index / columns;
        col = (roomSize + spacing) * col;
        row = (roomSize + spacing) * row;
        for (int i = 0; i < roomSize; i++)
        {
            for (int j = 0; j < roomSize; j++)
            {
                output[i, j] = tilemap.GetTile(new Vector3Int(col + j - 2, 20 -(row + i), 0));
            }
        }
        return output;
    }

    public List<Vector2Int> GetAdjacent(int id)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        if (openUp.Contains(id)) list.Add(new Vector2Int(0, -1));
        if (openRight.Contains(id)) list.Add(new Vector2Int(1, 0));
        if (openDown.Contains(id)) list.Add(new Vector2Int(0, 1));
        if (openLeft.Contains(id)) list.Add(new Vector2Int(-1, 0));
        return list;
    }
}
