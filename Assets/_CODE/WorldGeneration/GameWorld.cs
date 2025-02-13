using System;
using System.Collections.Generic;
using UnityEngine;

namespace _CODE.WorldGeneration
{
    public class GameWorld : MonoBehaviour
    {
        [SerializeField] private ChunkRenderer chunkRenderer;
        public Dictionary<Vector2Int,ChunkData> ChunkDatas = new Dictionary<Vector2Int,ChunkData>();

        private void Start()
        {
            Debug.Log("hello game world");
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    float xPos = x * ChunkRenderer.ChunkWidth*ChunkRenderer.BlockScale;
                    float zPos = y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                    
                    ChunkData chunkData = new ChunkData();
                    chunkData.ChunkPosition = new Vector2Int(x,y);
                    chunkData.Blocks = TerrainGenerator.GenerateCave(xPos, zPos);
                    ChunkDatas.Add(new Vector2Int(x,y), chunkData);
                    var chunk = Instantiate(chunkRenderer, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
                    chunk.ChunkData = chunkData;
                    chunk.ParentWorld = this;
                }
            }
        }
    }
}