using UnityEngine;

namespace _CODE.WorldGeneration
{
    public class ChunkData
    {
        public ChunkDataState State;
        
        public Vector2Int ChunkPosition;
        public ChunkRenderer Renderer;
        public BlockType[] Blocks;
        
        public ChunkData LeftChunk;
        public ChunkData RightChunk;
        public ChunkData FwdChunk;
        public ChunkData BackChunk;
        public ChunkData DownChunk;
        public ChunkData UpChunk;
    }

    public enum ChunkDataState
    {
        StartedLoading,
        Loaded,
        StartedMeshing,
        SpawnedInWorld,
        Unloaded
    }
}