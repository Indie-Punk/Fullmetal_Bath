using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
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
        ProfilerMarker MeshingMarker = new ProfilerMarker(ProfilerCategory.Loading, "Meshing");

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct GeneratedMeshVertex
        {
            public Vector3 pos;
            public sbyte normalX, normalY, normalZ, normalW;
            public ushort uvX, uvY;
        }

        public class GeneratedMeshData
        {
            public GeneratedMeshVertex[] Vertices;
            public Bounds Bounds;
        }
        
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
            float xPos = chunkPosition.x * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
            float zPos = chunkPosition.y * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
                    
            ChunkData chunkData = new ChunkData();
            chunkData.ChunkPosition = chunkPosition;
            chunkData.Blocks = Generator.GenerateCave(xPos, zPos);
            ChunkDatas.Add(chunkPosition, chunkData);
        }

        void SpawnChunkRenderer(ChunkData chunkData)
        {
            ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.left, out chunkData.LeftChunk);
            ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.right, out chunkData.RightChunk);
            ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.up, out chunkData.FwdChunk);
            ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.down, out chunkData.BackChunk);
            
            float xPos = chunkData.ChunkPosition.x * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
            float zPos = chunkData.ChunkPosition.y * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
            
            var chunk = Instantiate(chunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
            chunk.ChunkData = chunkData;
            chunk.ParentWorld = this;

            MeshingMarker.Begin();
            GeneratedMeshData meshData = MeshBuilder.GenerateMesh(chunkData);
            chunk.SetMesh(meshData);
            MeshingMarker.End();
            
            chunkData.Renderer = chunk;
        }
        // bool 
        void Update()
        {
            Vector3Int playerWorldPos = Vector3Int.FloorToInt(drill.transform.position / MeshBuilder.BlockScale);
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
                        blockCenter = hitInfo.point - hitInfo.normal * MeshBuilder.BlockScale / 2;
                    else
                        blockCenter = hitInfo.point + hitInfo.normal * MeshBuilder.BlockScale / 2;
                    Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter / MeshBuilder.BlockScale);
                    Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
                    if (ChunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                    {
                        var chunkOrigin = new Vector3Int(chunkPos.x, 0, chunkPos.y) * MeshBuilder.ChunkWidth;
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
            Vector2Int chunkPosition =  new Vector2Int(blockWorldPos.x / MeshBuilder.ChunkWidth, blockWorldPos.z / MeshBuilder.ChunkWidth);

            if (blockWorldPos.x < 0) chunkPosition.x--;
            if (blockWorldPos.z < 0) chunkPosition.y--;
            
            return chunkPosition;
        }
    }
}