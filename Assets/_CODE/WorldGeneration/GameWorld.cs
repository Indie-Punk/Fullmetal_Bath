using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _CODE.WorldGeneration
{
    public class GameWorld : MonoBehaviour
    {
        private const int ViewRadius = 2;
        [SerializeField] private ChunkRenderer chunkPrefab;
        [SerializeField] private TerrainGenerator Generator;
        [SerializeField] private Transform drill;
        public Dictionary<Vector2Int,ChunkData> ChunkDatas = new Dictionary<Vector2Int,ChunkData>();
        private Camera mainCamera;
        private Vector2Int currentPlayerChunk;

        private void Start()
        {
            mainCamera = Camera.main;
            Debug.Log("hello game world");
            
            ChunkRenderer.InitTriangles();
            
            Generator.Init();
            StartCoroutine(Generate(false));
        }

        private IEnumerator Generate(bool wait)
        {
            int loadRadius = ViewRadius + 1;
            
            for (int x = currentPlayerChunk.x - loadRadius; x <= currentPlayerChunk.x + loadRadius; x++)
            {
                for (int y = currentPlayerChunk.y - loadRadius; y <= currentPlayerChunk.y + loadRadius; y++)
                {
                    Vector2Int chunkPosition = new Vector2Int(x,y);
                    if (ChunkDatas.ContainsKey(chunkPosition)) continue;
                        
                    LoadChunkAt(chunkPosition);

                    if (wait) yield return null;
                }
            }
            
            for (int x = currentPlayerChunk.x - ViewRadius; x <= currentPlayerChunk.x + ViewRadius; x++)
            {
                for (int y = currentPlayerChunk.y - ViewRadius; y <= currentPlayerChunk.y + ViewRadius; y++)
                {
                    Vector2Int chunkPosition = new Vector2Int(x,y);
                    
                    ChunkData chunkData = ChunkDatas[chunkPosition];
                    
                    if(chunkData.Renderer != null) continue;
                    
                    SpawnChunkRenderer(chunkData);

                    if (wait) yield return new WaitForSecondsRealtime(0.1f);
                }
            }
        }

        [ContextMenu("Regenerate world")]
        public void Regenerate()
        {
            Generator.Init();
            foreach (var chunkData in ChunkDatas)
            {
                Destroy(chunkData.Value.Renderer.gameObject);
            }
            
            ChunkDatas.Clear();

            StartCoroutine(Generate(false));
        }
        private void LoadChunkAt(Vector2Int chunkPosition)
        {
            float xPos = chunkPosition.x * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
            float zPos = chunkPosition.y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                    
            ChunkData chunkData = new ChunkData();
            chunkData.ChunkPosition = chunkPosition;
            chunkData.Blocks = Generator.GenerateCave(xPos, zPos);
            ChunkDatas.Add(chunkPosition, chunkData);
        }

        void SpawnChunkRenderer(ChunkData chunkData)
        {
            
            float xPos = chunkData.ChunkPosition.x * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
            float zPos = chunkData.ChunkPosition.y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
            
            var chunk = Instantiate(chunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
            chunk.ChunkData = chunkData;
            chunk.ParentWorld = this;
            
            chunkData.Renderer = chunk;
        }
        // bool 
        void Update()
        {
            Vector3Int playerWorldPos = Vector3Int.FloorToInt(drill.transform.position / ChunkRenderer.BlockScale);
            Vector2Int playerChunk = GetChunkContainingBlock(playerWorldPos);
            if (playerChunk != currentPlayerChunk)
            {
                currentPlayerChunk = playerChunk;
                StartCoroutine(Generate(true));
            }
            
            CheckInput();
        }

        private float interactionTimer;
        private void CheckInput()
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                bool isDestroying = Input.GetMouseButton(0);
                interactionTimer -= Time.deltaTime;
                if (interactionTimer > 0)
                    return;
                Ray ray = mainCamera.ViewportPointToRay(new Vector3(.5f, .5f));

                if (Physics.Raycast(ray, out var hitInfo))
                {
                    Vector3 blockCenter;
                    if (isDestroying)
                        blockCenter = hitInfo.point - hitInfo.normal * ChunkRenderer.BlockScale / 2;
                    else
                        blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.BlockScale / 2;
                    Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.BlockScale);
                    Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
                    if (ChunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                    {
                        var chunkOrigin = new Vector3Int(chunkPos.x, 0, chunkPos.y) * ChunkRenderer.ChunkWidth;
                        if (isDestroying)
                        {
                            chunkData.Renderer.DestroySphere(blockWorldPos - chunkOrigin,5);
                        }
                        else
                        {
                            chunkData.Renderer.SpawnBlock(blockWorldPos - chunkOrigin);
                        }
                    }
                }

                interactionTimer = .05f;
            }
            else
            {
                interactionTimer = .05f;
            }
        }

        public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
        {
            Vector2Int chunkPosition =  new Vector2Int(blockWorldPos.x / ChunkRenderer.ChunkWidth, blockWorldPos.z / ChunkRenderer.ChunkWidth);

            if (blockWorldPos.x < 0) chunkPosition.x--;
            if (blockWorldPos.z < 0) chunkPosition.y--;
            
            return chunkPosition;
        }
    }
}