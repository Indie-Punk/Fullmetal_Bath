using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace _CODE.WorldGeneration
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] float scale =.2f;
        public BlockType[,,] GenerateCave(float offsetX, float offsetZ)
        {
            var result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];
            for (int x = 0; x < ChunkRenderer.ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkRenderer.ChunkWidth; z++)
                {
                    float height  = Mathf.PerlinNoise((x/8f+offsetX) * scale, (z/8f+offsetZ) * scale) * 10 +15;
                    
                    for (int y = 0; y < height && y < ChunkRenderer.ChunkHeight; y++)
                    {
                        result[x, y, z] = BlockType.Rock;
                    }
                    // for (int y = 0; y < ChunkRenderer.ChunkWidth; y++)
                    // {
                    //     float perlin = PerlinNoise3DAlt((x * scale), 
                    //         (y * scale), (z * scale) );
                    //     // Debug.Log(perlin);
                    //     if (perlin > .1f)
                    //     {
                    //         result[x,y,z] = 1;
                    //     }
                    // }
                }
            }

            return result;
        }
        #region Noises
        
        public float perlinMax;
        public float perlinMin;
        public float twist = 1;
        public float turbulence = 1;
        
        public static float PerlinNoise3DAlt(float x, float y, float z)
        {
            y += 1;
            z += 2;
            float xy = _perlin3DFixed(x, y);
            float xz = _perlin3DFixed(x, z);
            float yz = _perlin3DFixed(y, z);
            float yx = _perlin3DFixed(y, x);
            float zx = _perlin3DFixed(z, x);
            float zy = _perlin3DFixed(z, y);
        
            return xy * xz * yz * yx * zx * zy;
        }
        
        static float _perlin3DFixed(float a, float b)
        {
            return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
        }
         float PerlinNoise3D(float x, float y, float z)
        {
            float xy = Mathf.PerlinNoise(x, y);
            float yz = Mathf.PerlinNoise(y, z);
            float xz = Mathf.PerlinNoise(x, z);
            float xyz = xy * yz * xz;
            return xyz;
        }
        
         float SpaghettiPerlinNoise(float x, float y, float z)
        {
            float twistX = Mathf.Sin(y * twist) * turbulence;
            float twistY = Mathf.Cos(z * twist) * turbulence;
            float twistZ = Mathf.Sin(x * twist) * turbulence;
            return PerlinNoise3D(x + twistX, y + twistY, z + twistZ);
        }
        #endregion
    }
}