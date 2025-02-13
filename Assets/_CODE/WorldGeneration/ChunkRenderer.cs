using System.Collections;
using System.Collections.Generic;
using _CODE.WorldGeneration;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 48;
    public const int ChunkHeight = 48;
    public const float BlockScale = .25f;

    public ChunkData ChunkData;
    public GameWorld ParentWorld;
    // public BlockType[,,] Blocks = new BlockType[ChunkWidth, ChunkHeight, ChunkWidth];

    private List<Vector3> verticies = new List<Vector3>();
    private List<int> triangles = new List<int>();

    void Start()
    {
        Mesh chunkMesh = new Mesh();


        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();
        
        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    private void GenerateBlock(int x, int y, int z)
    {
        var blockPosition = new Vector3Int(x, y, z);

        if (GetBlockAtPosition(blockPosition) == 0) return;

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0) GenerateRightSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0) GenerateLeftSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0) GenerateFrontSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0) GenerateBackSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0) GenerateTopSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0) GenerateBottomSide(blockPosition);
    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y >= ChunkHeight)
                return BlockType.Air;
            Vector2Int adjucentChunkPosition = ChunkData.ChunkPosition;
            
            if (blockPosition.x < 0)
            {
                adjucentChunkPosition.x--;
                blockPosition.x += ChunkWidth;
            }
            else if (blockPosition.x >= ChunkWidth)
            {
                adjucentChunkPosition.x++;
                blockPosition.x -= ChunkWidth;
            }


            if (blockPosition.z < 0)
            {
                adjucentChunkPosition.y--;
                blockPosition.z += ChunkWidth;
            }
            else if (blockPosition.z >= ChunkWidth)
            {
                adjucentChunkPosition.y++;
                blockPosition.z -= ChunkWidth;
            }

            if (ParentWorld.ChunkDatas.TryGetValue(adjucentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
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
        triangles.Add(verticies.Count - 4);
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 2);

        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 1);
        triangles.Add(verticies.Count - 2);
    }
}