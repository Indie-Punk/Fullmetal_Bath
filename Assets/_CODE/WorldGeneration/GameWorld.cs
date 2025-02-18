using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

namespace _CODE.WorldGeneration
{
    public class GameWorld : MonoBehaviour
    {
        private int ViewRadius = 2;
        [SerializeField] private ChunkRenderer chunkPrefab;
        [SerializeField] private TerrainGenerator Generator;
        [SerializeField] private Transform drill;
        public Dictionary<Vector2Int,ChunkData> ChunkDatas = new Dictionary<Vector2Int,ChunkData>();
        private Camera mainCamera;
        private Vector2Int currentPlayerChunk;
        
        ConcurrentQueue<GeneratedMeshData> meshingResults = new ConcurrentQueue<GeneratedMeshData>();

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
            public ChunkData Data;
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

            Vector2Int center = currentPlayerChunk;

            List<ChunkData> loadingChunks = new List<ChunkData>(); 
            for (int x = center.x - loadRadius; x <= center.x + loadRadius; x++)
            {
                for (int y = center.y - loadRadius; y <= center.y + loadRadius; y++)
                {
                    Vector2Int chunkPosition = new Vector2Int(x,y);
                    if (ChunkDatas.ContainsKey(chunkPosition)) continue;

                    ChunkData loadingChunkData = LoadChunkAt(chunkPosition);
                    loadingChunks.Add(loadingChunkData);

                    if (wait) yield return null;
                }
            }

            while (loadingChunks.Any(c=>c.State == ChunkDataState.StartedLoading))
            {
                yield return null;
            }
            
            for (int x = center.x - ViewRadius; x <= center.x + ViewRadius; x++)
            {
                for (int y = center.y - ViewRadius; y <= center.y + ViewRadius; y++)
                {
                    Vector2Int chunkPosition = new Vector2Int(x,y);
                    
                    ChunkData chunkData = ChunkDatas[chunkPosition];
                    
                    if(chunkData.Renderer != null) continue;
                    
                    SpawnChunkRenderer(chunkData);

                    if (wait) yield return null;
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
        private ChunkData LoadChunkAt(Vector2Int chunkPosition)
        {
            float xPos = chunkPosition.x * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
            float zPos = chunkPosition.y * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
                    
            ChunkData chunkData = new ChunkData();
            chunkData.State = ChunkDataState.StartedLoading;
            chunkData.ChunkPosition = chunkPosition;
            ChunkDatas.Add(chunkPosition, chunkData);

            Task.Factory.StartNew(() =>
            {
                chunkData.Blocks = Generator.GenerateCave(xPos, zPos);
                chunkData.State = ChunkDataState.Loaded;
            });

            return chunkData;
        }

        void SpawnChunkRenderer(ChunkData chunkData)
        {
            ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.left, out chunkData.LeftChunk);
            ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.right, out chunkData.RightChunk);
            ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.up, out chunkData.FwdChunk);
            ChunkDatas.TryGetValue(chunkData.ChunkPosition + Vector2Int.down, out chunkData.BackChunk);

            chunkData.State = ChunkDataState.StartedMeshing;
            
            Task.Factory.StartNew(() =>
            {
                GeneratedMeshData meshData = MeshBuilder.GenerateMesh(chunkData);
                meshingResults.Enqueue(meshData);
            });

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

            if (meshingResults.TryDequeue(out GeneratedMeshData meshData))
            {
                float xPos = meshData.Data.ChunkPosition.x * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
                float zPos = meshData.Data.ChunkPosition.y * MeshBuilder.ChunkWidth * MeshBuilder.BlockScale;
                
                var chunk = Instantiate(chunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
                chunk.ChunkData = meshData.Data;
                chunk.ParentWorld = this;
                chunk.SetMesh(meshData);
                meshData.Data.Renderer = chunk;
                meshData.Data.State = ChunkDataState.SpawnedInWorld;
            }
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
                        Debug.Log("blockWorldPos " + blockWorldPos);
                        Debug.Log("chunkOrigin " + chunkOrigin);
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

                interactionTimer = .5f;
            }
            else
            {
                interactionTimer = .5f;
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