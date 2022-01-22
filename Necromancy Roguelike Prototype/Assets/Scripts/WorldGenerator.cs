using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private RoomGenerator roomGen;
    [SerializeField] private TileBase wallTile;

    private int[,] roomMap;
    private Tilemap tilemap;

    private void Start()
    {
        roomMap = new int[height, width];
        bool[,] visited = new bool[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                roomMap[i, j] = -1;
                visited[i, j] = false;
            }
        }

        roomMap[0, 0] = 5;
        visited[0, 0] = true;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        
        queue.Enqueue(new Vector2Int(1, 0));
        queue.Enqueue(new Vector2Int(width - 1, 0));
        queue.Enqueue(new Vector2Int(0, height - 1));
        queue.Enqueue(new Vector2Int(width - 1, height - 1));
        while (queue.Count > 0)
        {
            Vector2Int p = queue.Dequeue();

            int up = isValidPoint(p.x, p.y - 1) ? roomMap[p.y - 1, p.x] : -1;
            int right = isValidPoint(p.x + 1, p.y) ? roomMap[p.y, p.x + 1] : -1;
            int down = isValidPoint(p.x, p.y + 1) ? roomMap[p.y + 1, p.x] : -1;
            int left = isValidPoint(p.x - 1, p.y) ? roomMap[p.y, p.x - 1] : -1;

            roomMap[p.y, p.x] = roomGen.GetNextRoom(up, right, down, left);
            foreach (Vector2Int v in roomGen.GetAdjacent(roomMap[p.y, p.x]))
            {
                Vector2Int next = p + v;
                if (isValidPoint(next.x, next.y) && !visited[next.y, next.x])
                {
                    queue.Enqueue(p + v);
                    visited[next.y, next.x] = true;
                }
            }

            Debug.Log(p + " " + roomMap[p.y, p.x]);
        }
        tilemap = GetComponent<Tilemap>();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int roomId = roomMap[i, j];
                //Debug.Log(roomId);
                TileBase[,] room = roomGen.GetRoom(roomId);
                for (int y = 0; y < roomGen.RoomSize; y++)
                {
                    for (int x = 0; x < roomGen.RoomSize; x++)
                    {
                        tilemap.SetTile(new Vector3Int(j * roomGen.RoomSize + x, 49 - (i * roomGen.RoomSize + y), 0), room[y, x]);
                    }
                }
            }
        }

        for (int i = 0; i < height * roomGen.RoomSize; i++)
        {
            tilemap.SetTile(new Vector3Int(0, i, 0), wallTile);
            tilemap.SetTile(new Vector3Int(width * roomGen.RoomSize - 1, i, 0), wallTile);
        } 

        for (int i = 0; i < width * roomGen.RoomSize; i++)
        {
            tilemap.SetTile(new Vector3Int(i, 0, 0), wallTile);
            tilemap.SetTile(new Vector3Int(i, height * roomGen.RoomSize - 1, 0), wallTile);
        }
    }

    private bool isValidPoint(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
}
