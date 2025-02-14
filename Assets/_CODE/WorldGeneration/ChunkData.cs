using UnityEngine;

namespace _CODE.WorldGeneration
{
    public class ChunkData : MonoBehaviour
    {
        public Vector2Int ChunkPosition;
        public ChunkRenderer Renderer;
        public BlockType[,,] Blocks;
    }
}