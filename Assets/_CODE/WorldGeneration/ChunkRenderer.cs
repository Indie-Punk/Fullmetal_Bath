using System;
using System.Collections;
using System.Collections.Generic;
using _CODE.WorldGeneration;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{

    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    private Mesh chunkMesh;


    ProfilerMarker meshMarker = new ProfilerMarker(ProfilerCategory.Loading, "Meshing");
    private static int[] triangles;
    

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
    
    void Awake()
    {
        chunkMesh = new Mesh();

        GetComponent<MeshFilter>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        int index = blockPosition.x + blockPosition.y * MeshBuilder.ChunkWidthSQ + blockPosition.z * MeshBuilder.ChunkWidth;
        ChunkData.Blocks[index] = BlockType.Rock;
        RegenerateMesh();
    }
    public void DestroyBlock(Vector3Int blockPosition)
    {
        int index = blockPosition.x + blockPosition.y * MeshBuilder.ChunkWidthSQ + blockPosition.z * MeshBuilder.ChunkWidth;
        ChunkData.Blocks[index] = BlockType.Air;
        RegenerateMesh();
    }

    void RegenerateMesh()
    {
        SetMesh(MeshBuilder.GenerateMesh(ChunkData));
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
                            
                            int index = blockPosition.x + blockPosition.y * MeshBuilder.ChunkWidthSQ + blockPosition.z * MeshBuilder.ChunkWidth;
                            ChunkData.Blocks[index] = BlockType.Air;
                            // DestroyBlock(blockPosition + new Vector3Int(x,y,z));
                            // Console.WriteLine($"Cube at ({centerX + x}, {centerY + y}, {centerZ + z})");
                        }
                    }
                }
            }
    }

    public void SetMesh(GameWorld.GeneratedMeshData meshData)
    {
        // meshMarker.Begin();
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.SNorm8, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.UNorm16, 2),
        };

        chunkMesh.SetVertexBufferParams(meshData.Vertices.Length, layout);
        chunkMesh.SetVertexBufferData(meshData.Vertices,0,0, meshData.Vertices.Length);
        
        int triangleCount = meshData.Vertices.Length / 4 * 6;
        
        chunkMesh.SetIndexBufferParams(triangleCount, IndexFormat.UInt32);
        chunkMesh.SetIndexBufferData(triangles,0,0, triangleCount);

        chunkMesh.subMeshCount = 1;
        chunkMesh.SetSubMesh(0, new SubMeshDescriptor(0, triangleCount));
        // chunkMesh.Optimize();

        chunkMesh.bounds = meshData.Bounds;
        
        
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
        
        // meshMarker.End();
    }
}