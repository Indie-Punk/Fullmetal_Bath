using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CODE.WorldGeneration
{
    public class WorldGeneration : MonoBehaviour
    {
        public ChunkRenderer chunkRenderer;
        // [SerializeField] private GameObject cube;
        // [SerializeField] private List<GameObject> spawnedCubes;


        [SerializeField] private int WIDTH = 32;
        [SerializeField] private int HEIGHT = 32;
        [SerializeField] private int DEPTH = 32;

        private float[,] map;
        private float[,,] map3D;

        private Vector2Int[] Rotators = new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1)
        };
        private Vector3Int[] Rotators3D = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, -1, 0),
            
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 1, -1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(1, 1, -1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(1, -1, -1),
            new Vector3Int(0, -1, -1),
            new Vector3Int(-1, -1, -1),
            
            new Vector3Int(0, 0, 1),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(-1, 1, 1),
            new Vector3Int(0, 1, 1),
            new Vector3Int(1, 1, 1),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, -1, 1),
            new Vector3Int(0, -1, 1),
            new Vector3Int(-1, -1, 1)
        };

        private void Awake()
        {
            map = new float[WIDTH, HEIGHT];
            map3D = new float[WIDTH, HEIGHT, DEPTH];
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                PerlinNoise3DGeneration();
                // chunkRenderer.Generate(map3D);
            }
            // if (Input.GetKeyDown(KeyCode.F))
            // {
            //     
            //     EmptyMap();
            //     Spagetti();
            //     chunkRenderer.Generate();
            // }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SpaghettiNoise3DGeneration();
                // chunkRenderer.Generate(map3D);
            }
            // if (Input.GetKeyDown(KeyCode.X))
            // {
            //     Interpolate3D();
            //     chunkRenderer.Generate();
            // }
            if (Input.GetKeyDown(KeyCode.C))
                EmptyMap();
        }

        
        
        private void Interpolate3D()
        {
            float[,,] array = new float[WIDTH, HEIGHT, DEPTH];
            for (int i = 1; i < WIDTH; i++)
            {
                for (int j = 1; j < HEIGHT; j++)
                {
                    for (int k = 1; k < DEPTH; k++)
                    {
                        array[i, j, k] = (this.map3D[i - 1, j, k - 1] + this.map3D[i - 1, j - 1, k - 1] + this.map3D[i, j - 1,k - 1] + this.map3D[i + 1, j - 1, k - 1] + this.map3D[i + 1, j, k - 1] + this.map3D[i + 1, j + 1, k - 1] + this.map3D[i, j + 1, k - 1] + this.map3D[i - 1, j + 1, k - 1] + this.map3D[i, j, k - 1]
                            + this.map3D[i - 1, j, k] + this.map3D[i - 1, j - 1, k] + this.map3D[i, j - 1,k] + this.map3D[i + 1, j - 1, k] + this.map3D[i + 1, j, k] + this.map3D[i + 1, j + 1, k] + this.map3D[i, j + 1, k] + this.map3D[i - 1, j + 1, k] + this.map3D[i, j, k]
                            + this.map3D[i - 1, j, k + 1] + this.map3D[i - 1, j - 1, k + 1] + this.map3D[i, j - 1,k + 1] + this.map3D[i + 1, j - 1, k + 1] + this.map3D[i + 1, j, k + 1] + this.map3D[i + 1, j + 1, k + 1] + this.map3D[i, j + 1, k + 1] + this.map3D[i - 1, j + 1, k + 1] + this.map3D[i, j, k + 1]) / 27f;

                    }
                }
            }
            this.map3D = array;
        }
        // private void ApplyMap(bool force3D)
        // {
        //     for (int i = 0; i < WIDTH; i++)
        //     {
        //         for (int j = 0; j < HEIGHT; j++)
        //         {
        //             if (!force3D)
        //             {
        //                 if (map[i, j] <= 0.5f)
        //                     SpawnCube(new Vector2(i, j));
        //             }
        //             else
        //             {
        //                 for (int k = 0; k < DEPTH; k++)
        //                 {
        //                     if (map3D[i, j, k] <= 0.5f)
        //                     {
        //                         SpawnCube(new Vector3(i, j, k));
        //                     }
        //                 }
        //             }
        //         }
        //     }
        // }

        private void EmptyMap()
        {
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    this.map[i, j] = 0f;
                    for (int k = 0; k < DEPTH; k++)
                    {
                        
                        this.map3D[i, j, k] = 0f;
                    }
                }
            }
            // chunkRenderer.Clear();
        }
        

        public float frequency = .5f;
        public float scale = 1;
        public int seed;
        private void Spagetti3D()
        {
            float width = (float)WIDTH / 10 * scale;
            float height = (float)HEIGHT / 10 * scale;
            float depth = (float)DEPTH / 10 * scale;
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    for (int k = 0; k < DEPTH; k++)
                    {

                        float perlinValue = PerlinNoise3D(seed + (float)i / width, seed + (float)j / height,
                            seed + (float)k / depth);
                        // Debug.Log(perlinValue);
                        if (perlinValue > 0.45f 
                            && perlinValue < 0.55f)
                        {
                            this.map3D[i, j, k] = 1f;
                        }
                        else
                        {
                            this.map3D[i, j, k] = 0f;
                        }
                    }
                }
            }
        }

        public float noiseMax;
        public float noiseMin;
        void PerlinNoise3DGeneration()
        {
            map3D = new float[WIDTH, HEIGHT, DEPTH];
            float width = (float)WIDTH / 10 * scale;
            float height = (float)HEIGHT / 10 * scale;
            float depth = (float)DEPTH / 10 * scale;
            for(int x = 0; x < WIDTH; x++)
            {
                for(int y = 0; y < HEIGHT; y++)
                {
                    for(int z = 0; z < DEPTH; z++)
                    {
                        float perlinValue = PerlinNoise3D(seed + (float)x / width, seed + (float)y / height,
                            seed + (float)z / depth);
                        // Debug.Log(perlinValue);
                        if(perlinValue<noiseMax && perlinValue>noiseMin)
                        {
                            this.map3D[x, y, z] = 1f;
                        }
                        else
                        {
                            this.map3D[x, y, z] = 0f;
                        }
                    }
                }
            }

        }
        void SpaghettiNoise3DGeneration()
        {
            map3D = new float[WIDTH, HEIGHT, DEPTH];
            float width = (float)WIDTH / 10 * scale;
            float height = (float)HEIGHT / 10 * scale;
            float depth = (float)DEPTH / 10 * scale;
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    for (int z = 0; z < DEPTH; z++)
                    {
                        float perlinValue = SpaghettiPerlinNoise(seed + (float)x / width, seed + (float)y / height,
                            seed + (float)z / depth);
                        Debug.Log(perlinValue);
                        this.map3D[x, y, z] = perlinValue;
                        // if (perlinValue < noiseMax && perlinValue > noiseMin)
                        // {
                        //     this.map3D[x, y, z] = 1f;
                        // }
                        // else
                        // {
                        //     this.map3D[x, y, z] = 0f;
                        // }
                    }
                }
            }
        }

        #region 2D

        
        private void Interpolate()
        {
            float[,] array = new float[256, 256];
            for (int i = 1; i < 255; i++)
            {
                for (int j = 1; j < 255; j++)
                {
                    array[i, j] = (this.map[i - 1, j] + this.map[i - 1, j - 1] + this.map[i, j - 1] + this.map[i + 1, j - 1] + this.map[i + 1, j] + this.map[i + 1, j + 1] + this.map[i, j + 1] + this.map[i - 1, j + 1] + this.map[i, j]) / 9f;
                }
            }
            this.map = array;
        }
        
        private void PerlinWorm()
        {
            float width = (float)WIDTH / 10 * scale;
            float height = (float)HEIGHT / 10 * scale;
            float num = Random.Range(0f, 5f);
            int num2 = Random.Range(0, WIDTH);
            List<Vector2Int> list = new List<Vector2Int>();
            for (int i = 0; i < WIDTH; i++)
            {
                list.Add(this.Rotators[(int)(Mathf.PerlinNoise(num + (float)i / width, num + (float)num2 / height) * (float)this.Rotators.Length)]);
            }
            Vector2Int a = new Vector2Int(Random.Range(0, WIDTH), Random.Range(0, WIDTH));
            for (int j = 0; j < list.Count; j++)
            {
                a += list[j];
                if (a.x >= WIDTH)
                {
                    a.x = 0;
                }
                if (a.y >= HEIGHT)
                {
                    a.y = 0;
                }
                if (a.x < 0)
                {
                    a.x = WIDTH;
                }
                if (a.y < 0)
                {
                    a.y = HEIGHT;
                }
                this.map[a.x, a.y] = 5f;
            }
        }

        private void PerlinMap()
        {
            float width = (float)WIDTH / 10;
            float height = (float)HEIGHT / 10;
            float num = Random.Range(0f, 5f);
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    this.map[i, j] = Mathf.PerlinNoise(num + (float)i / width, num + (float)j / height);
                }
            }
        }
        
        private void Cheese()
        {
            float width = (float)WIDTH / 10;
            float height = (float)HEIGHT / 10;
            float num = Random.Range(0f, 5f);
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (Mathf.PerlinNoise(num + (float)i / width, num + (float)j / height) > 0.7f)
                    {
                        this.map[i, j] = 1f;
                    }
                    else
                    {
                        this.map[i, j] = 0f;
                    }
                }
            }
        }
        private void Spagetti()
        {
            float width = (float)WIDTH / 10 * scale;
            float height = (float)HEIGHT / 10 * scale;
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (Mathf.PerlinNoise(seed + (float)i / width, seed + (float)j / height) > 0.45f 
                        && Mathf.PerlinNoise(seed + (float)i / width, seed + (float)j / height) < 0.55f)
                    {
                        this.map[i, j] = 1f;
                    }
                    else
                    {
                        this.map[i, j] = 0f;
                    }
                }
            }
        }
        #endregion

        #region Noises
        public float twist;
        public float turbulence;
        // public static float PerlinNoise3D(float x, float y, float z)
        // {
        //     y += 1;
        //     z += 2;
        //     float xy = _perlin3DFixed(x, y);
        //     float xz = _perlin3DFixed(x, z);
        //     float yz = _perlin3DFixed(y, z);
        //     float yx = _perlin3DFixed(y, x);
        //     float zx = _perlin3DFixed(z, x);
        //     float zy = _perlin3DFixed(z, y);
        //
        //     return xy * xz * yz * yx * zx * zy;
        // }
        //
        // static float _perlin3DFixed(float a, float b)
        // {
        //     return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
        // }

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