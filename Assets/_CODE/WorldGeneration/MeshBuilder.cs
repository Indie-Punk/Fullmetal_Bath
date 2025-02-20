using System.Collections.Generic;
using UnityEngine;

namespace _CODE.WorldGeneration
{

    public static class MeshBuilder
    {
        public const int ChunkWidth = 8;
        public const int ChunkWidthSQ = ChunkWidth * ChunkWidth;
        public const int ChunkHeight = 128;
        public const float BlockScale =  .2f;
        
        public static GameWorld.GeneratedMeshData GenerateMesh(ChunkData chunkData)
        {
            List<GameWorld.GeneratedMeshVertex> vertices = new List<GameWorld.GeneratedMeshVertex>();
            //TO DO в будущем высоты не будет
            int maxY = 0;
            for (int y = 0; y < ChunkHeight; y++)
            {
                for (int x = 0; x < ChunkWidth; x++)
                {
                    for (int z = 0; z < ChunkWidth; z++)
                    {
                        if (GenerateBlock(x, y, z, vertices, chunkData))
                        {
                            if (maxY < y)
                                maxY = y;
                        }
                    }
                }
            }
            var mesh = new GameWorld.GeneratedMeshData();
            mesh.Vertices = vertices.ToArray();
            
            Vector3 boundsSize = new Vector3(ChunkWidth, ChunkHeight, ChunkWidth) * BlockScale;
            mesh.Bounds = new Bounds(boundsSize/2, boundsSize);
            
            mesh.Data = chunkData;

            return mesh;
        }

        private static bool GenerateBlock(int x, int y, int z, List<GameWorld.GeneratedMeshVertex> verticies, ChunkData chunkData)
        {
            var blockPosition = new Vector3Int(x, y, z);

            if (GetBlockAtPosition(blockPosition, chunkData) == 0) return false;

            if (GetBlockAtPosition(blockPosition + Vector3Int.right,chunkData) == 0) GenerateRightSide(blockPosition, verticies);
            if (GetBlockAtPosition(blockPosition + Vector3Int.left,chunkData) == 0) GenerateLeftSide(blockPosition, verticies);
            if (GetBlockAtPosition(blockPosition + Vector3Int.forward, chunkData) == 0) GenerateFrontSide(blockPosition, verticies);
            if (GetBlockAtPosition(blockPosition + Vector3Int.back,chunkData) == 0) GenerateBackSide(blockPosition, verticies);
            if (GetBlockAtPosition(blockPosition + Vector3Int.up, chunkData) == 0) GenerateTopSide(blockPosition, verticies);
            if (blockPosition.y > 0 && GetBlockAtPosition(blockPosition + Vector3Int.down, chunkData) == 0)
                GenerateBottomSide(blockPosition, verticies);
            return true;
        }

        private static BlockType GetBlockAtPosition(Vector3Int blockPosition, ChunkData chunkData)
        {
            if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
                blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
                blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
            {
                int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                return chunkData.Blocks[index];
            }
            else
            {
                if (blockPosition.y < 0 || blockPosition.y >= ChunkHeight)
                    return BlockType.Air;

                if (blockPosition.x < 0)
                {
                    if (chunkData.LeftChunk == null) return BlockType.Air;
                
                    blockPosition.x += ChunkWidth;
                    int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                    return chunkData.LeftChunk.Blocks[index];
                }
                
                if (blockPosition.x >= ChunkWidth)
                {
                    if (chunkData.RightChunk == null) return BlockType.Air;
                
                    blockPosition.x -= ChunkWidth;
                    int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                    return chunkData.RightChunk.Blocks[index];
                }
                
                if (blockPosition.z < 0)
                {
                    if (chunkData.BackChunk == null) return BlockType.Air;
                
                    blockPosition.z += ChunkWidth;
                    int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                    return chunkData.BackChunk.Blocks[index];
                }
                
                if (blockPosition.z >= ChunkWidth)
                {
                    if (chunkData.FwdChunk == null) return BlockType.Air;
                    blockPosition.z -= ChunkWidth;
                    int index = blockPosition.x + blockPosition.y * ChunkWidthSQ + blockPosition.z * ChunkWidth;
                    return chunkData.FwdChunk.Blocks[index];
                }


                return BlockType.Air;
            }
        }

        private static void GenerateRightSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies)
        {
            GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

            vertex.normalX = sbyte.MaxValue;
            vertex.normalY = 0;
            vertex.normalZ = 0;
            vertex.normalW = 1;
            GetUvs(out vertex.uvX, out vertex.uvY);

            vertex.pos = (new Vector3(1, 0, 0) + blockPosition) * BlockScale;
            verticies.Add(vertex);
            vertex.pos = (new Vector3(1, 1, 0) + blockPosition) * BlockScale;
            verticies.Add(vertex);
            vertex.pos = (new Vector3(1, 0, 1) + blockPosition) * BlockScale;
            verticies.Add(vertex);
            vertex.pos = (new Vector3(1, 1, 1) + blockPosition) * BlockScale;
            verticies.Add(vertex);
        }

        private static void GenerateLeftSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies)
        {
            GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();
            
            vertex.normalX = sbyte.MinValue;
            vertex.normalY = 0;
            vertex.normalZ = 0;
            vertex.normalW = 1;
            GetUvs(out vertex.uvX, out vertex.uvY);
            
            vertex.pos = ((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
        }

        private static void GenerateFrontSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies)
        {
            GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

            vertex.normalX = 0;
            vertex.normalY = 0;
            vertex.normalZ = sbyte.MaxValue;
            vertex.normalW = 1;
            GetUvs(out vertex.uvX, out vertex.uvY);
            
            vertex.pos = ((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(1, 1, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
        }

        private static void GenerateBackSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies)
        {
            GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

            vertex.normalX = 0;
            vertex.normalY = 0;
            vertex.normalZ = sbyte.MinValue;
            vertex.normalW = 1;
            GetUvs(out vertex.uvX, out vertex.uvY);
            
            vertex.pos = ((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
        }

        private static void GenerateTopSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies)
        {
            GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

            vertex.normalX = 0;
            vertex.normalY = sbyte.MaxValue;
            vertex.normalZ = 0;
            vertex.normalW = 1;
            GetUvs(out vertex.uvX, out vertex.uvY);
            
            vertex.pos = ((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(1, 1, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
        }

        private static void GenerateBottomSide(Vector3Int blockPosition, List<GameWorld.GeneratedMeshVertex> verticies)
        {
            GameWorld.GeneratedMeshVertex vertex = new GameWorld.GeneratedMeshVertex();

            vertex.normalX = 0;
            vertex.normalY = sbyte.MinValue;
            vertex.normalZ = 0;
            vertex.normalW = 1;
            GetUvs(out vertex.uvX, out vertex.uvY);
            
            vertex.pos = ((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
            vertex.pos = ((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
            verticies.Add(vertex);
        }

        public static void GetUvs(out ushort x, out ushort y)
        {
            x = 16 * 256;
            y = 240*256;
        }
    }
}