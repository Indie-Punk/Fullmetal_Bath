using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _CODE.WorldGeneration
{
    public class GameWorld : MonoBehaviour
    {
        private const int ViewRadius = 2;
        [SerializeField] private ChunkRenderer chunkRenderer;
        [SerializeField] private TerrainGenerator Generator;
        [SerializeField] private Transform drill;
        public Dictionary<Vector2Int,ChunkData> ChunkDatas = new Dictionary<Vector2Int,ChunkData>();
        private Camera mainCamera;
        private Vector2Int currentPlayerChunk;

        private void Start()
        {
            mainCamera = Camera.main;
            Debug.Log("hello game world");
            StartCoroutine(Generate(false));
        }

        private IEnumerator Generate(bool wait)
        {
            for (int x = currentPlayerChunk.x - ViewRadius; x < currentPlayerChunk.x + ViewRadius; x++)
            {
                for (int y = currentPlayerChunk.y - ViewRadius; y < currentPlayerChunk.y + ViewRadius; y++)
                {
                    Vector2Int chunkPosition = new Vector2Int(x,y);
                    if (ChunkDatas.ContainsKey(chunkPosition)) continue;
                        
                    LoadChunkAt(chunkPosition);

                    if (wait) yield return new WaitForSecondsRealtime(0.2f);
                }
            }
        }

        private void LoadChunkAt(Vector2Int chunkPosition)
        {
            float xPos = chunkPosition.x * ChunkRenderer.ChunkWidth*ChunkRenderer.BlockScale;
            float zPos = chunkPosition.y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                    
            ChunkData chunkData = new ChunkData();
            chunkData.ChunkPosition = chunkPosition;
            chunkData.Blocks = Generator.GenerateCave(xPos, zPos);
            ChunkDatas.Add(chunkPosition, chunkData);
            var chunk = Instantiate(chunkRenderer, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
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

        private float destroyingTimer;
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
                            chunkData.Renderer.DestroyBlock(blockWorldPos - chunkOrigin);
                        }
                        else
                        {
                            chunkData.Renderer.SpawnBlock(blockWorldPos - chunkOrigin);
                        }
                    }
                }

                interactionTimer = .1f;
            }
            else
            {
                interactionTimer = .1f;
            }
        }

        public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
        {
            return new Vector2Int(blockWorldPos.x / ChunkRenderer.ChunkWidth, blockWorldPos.z / ChunkRenderer.ChunkWidth);
        }
    }
}