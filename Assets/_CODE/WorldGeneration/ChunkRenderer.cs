using System;
using System.Collections;
using System.Collections.Generic;
using _CODE.WorldGeneration;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 32;
    public const int ChunkWidthSQ = ChunkWidth * ChunkWidth;
    public const int ChunkHeight = 128;
    public const float BlockScale = .25f;

    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private Mesh chunkMesh;

    public ChunkData leftChunk;
    public ChunkData rightChunk;
    public ChunkData fwdChunk;
    public ChunkData backChunk;
    public ChunkData downChunk;

    private static int[] triangles;
    
    ProfilerMarker MeshingMarker = new ProfilerMarker(ProfilerCategory.Loading, "Meshing");

    public static void InitTriangles()
    {
        triangles = new int[65536 * 6 / 4];

        int vertexNum = 4;
        for (int i = 0; i < triangles.Length; i += 6)
        {
            triangles[i] = vertexNum- 4;
            triangles[i + 1] = vertexNum- 3;
            triangles[i + 2] = vertexNum - 2;

            triangles[i + 3] = vertexNum - 3;
            triangles[i + 4] = vertexNum - 1;
            triangles[i + 5] = vertexNum - 2;
            vertexNum += 4;
        }
        
    }
    
    void Start()
    {
        // ParentWorld.ChunkDatas.TryGetValue(ChunkData.ChunkPosition, out downChunk);
        ParentWorld.ChunkDatas.TryGetValue(ChunkData.ChunkPosition + Vector2Int.left, out leftChunk);
        ParentWorld.ChunkDatas.TryGetValue(ChunkData.ChunkPosition + Vector2Int.right, out rightChunk);
        ParentWorld.ChunkDatas.TryGetValue(ChunkData.ChunkPosition + Vector2Int.up, out fwdChunk);
        ParentWorld.ChunkDatas.TryGetValue(ChunkData.ChunkPosition + Vector2Int.down, out backChunk);
        
        
        chunkMesh = new Mesh();
        
        RegenerateMesh();

        GetComponent<MeshFilter>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
        ChunkData.Blocks[index] = BlockType.Rock;
        RegenerateMesh();
    }
    public void DestroyBlock(Vector3Int blockPosition)
    {
        int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
        ChunkData.Blocks[index] = BlockType.Air;
        RegenerateMesh();
    }

    public void DestroySphere(Vector3Int blockPosition, int radius)
    {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int z = -radius; z <= radius; z++)
                    {
                        if (x * x + y * y + z * z <= radius * radius)
                        {
                            
                            int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                            ChunkData.Blocks[index] = BlockType.Air;
                            // DestroyBlock(blockPosition + new Vector3Int(x,y,z));
                            // Console.WriteLine($"Cube at ({centerX + x}, {centerY + y}, {centerZ + z})");
                        }
                    }
                }
            }
            RegenerateMesh();
    }

    private void RegenerateMesh()
    {
        MeshingMarker.Begin();
        verticies.Clear();
        uvs.Clear();

        //TO DO в будущем высоты не будет
        int maxY = 0;
        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    if (GenerateBlock(x, y, z))
                    {
                        if (maxY < y)
                            maxY = y;
                    }
                }
            }
        }

        chunkMesh.triangles = Array.Empty<int>();
        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.uv = uvs.ToArray();
        chunkMesh.SetTriangles(triangles, 0, verticies.Count *6/4,0, false);

        chunkMesh.Optimize();
        
        chunkMesh.RecalculateNormals();
        Vector3 boundsSize = new Vector3(ChunkWidth, maxY, ChunkWidth) * BlockScale;
        chunkMesh.bounds = new Bounds(boundsSize/2, boundsSize);
        
        
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
        MeshingMarker.End();
    }
    private bool GenerateBlock(int x, int y, int z)
    {
        var blockPosition = new Vector3Int(x, y, z);

        if (GetBlockAtPosition(blockPosition) == 0) return false;

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0) GenerateRightSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0) GenerateLeftSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0) GenerateFrontSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0) GenerateBackSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0) GenerateTopSide(blockPosition);
        if (blockPosition.y > 0 && GetBlockAtPosition(blockPosition + Vector3Int.down) == 0) GenerateBottomSide(blockPosition);
        return true;
    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
            return ChunkData.Blocks[index];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y >= ChunkHeight)
                return BlockType.Air;

            if (blockPosition.x < 0)
            {
                if (leftChunk == null) return BlockType.Air;
                
                blockPosition.x += ChunkWidth;
                int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                return leftChunk.Blocks[index];
            }
            if (blockPosition.x >= ChunkWidth)
            {
                if (rightChunk == null) return BlockType.Air;
                
                blockPosition.x -= ChunkWidth;
                int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                return rightChunk.Blocks[index];
            }
            if (blockPosition.z < 0)
            {
                if (backChunk == null) return BlockType.Air;
                
                blockPosition.z += ChunkWidth;
                int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                return backChunk.Blocks[index];
            }
            if (blockPosition.z >= ChunkWidth)
            {
                if (fwdChunk == null) return BlockType.Air;
                blockPosition.z -= ChunkWidth;
                int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                return fwdChunk.Blocks[index];
            }
            

            return BlockType.Air;
        }
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateBackSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateTopSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void AddLastVerticiesSquare()
    {
        uvs.Add(new Vector2(0,0));
        uvs.Add(new Vector2(0,1));
        uvs.Add(new Vector2(1,0));
        uvs.Add(new Vector2(1,1));
    }
}