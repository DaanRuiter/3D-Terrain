using UnityEngine;

namespace IslandBuilder
{
    namespace Terrain
    {
        /// <summary>
        /// A seed that can be used to generate perlin noise maps
        /// </summary>
        [RequireComponent(typeof(MeshFilter))]
        [RequireComponent(typeof(MeshRenderer))]
        [RequireComponent(typeof(MeshCollider))]
        public class TerrainChunkObject : MonoBehaviour
        {
            public const int TERRAIN_CHUNK_TILE_SIZE = 128;
            public const int TERRAIN_CHUNK_TEXTURE_RESOLUTION_SIZE = 128;

            private TerrainHeightMap m_heightMap;

            [HideInInspector]
            public int worldX, worldY;

            public TerrainChunkObject Build(int x, int y, TerrainHeightMap heightMap)
            {
                worldX = x;
                worldY = y;

                transform.position = new Vector3(x * (TERRAIN_CHUNK_TILE_SIZE - 1), 0, y * (TERRAIN_CHUNK_TILE_SIZE - 1));

                MeshFilter meshFilter = GetComponent<MeshFilter>();
                if (meshFilter.sharedMesh == null)
                    meshFilter.sharedMesh = new Mesh();
                Mesh mesh = meshFilter.sharedMesh;

                mesh.name = "Terrain Chunk";
                mesh.Clear();

                m_heightMap = heightMap;

                mesh.vertices = MeshBuilder.BuildVerticesFromHeightMap(m_heightMap);
                mesh.triangles = MeshBuilder.triangles;
                mesh.uv = MeshBuilder.uv;

                mesh.RecalculateNormals();

                Material mat = Resources.Load("Materials/Terrain") as Material;
                GetComponent<Renderer>().material = mat;
                GetComponent<Renderer>().material.mainTexture = MeshBuilder.GenerateTextureFromHeightMap(m_heightMap);
                GetComponent<Renderer>().material.SetFloat("_SPECCGLOSSMAP", 0.15f);
                GetComponent<MeshCollider>().sharedMesh = mesh;

                return this;
            }

            private void Update()
            {
                //force rebuild of mesh
                if (Input.GetKeyUp(KeyCode.B))
                    RebuildObject();
            }

            /// <summary>
            /// rebuilds the mesh and texture, use this after altering the heightmap to make the changes appear in-game
            /// </summary>
            public void RebuildObject()
            {
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                Mesh mesh = meshFilter.sharedMesh;
                mesh.vertices = MeshBuilder.BuildVerticesFromHeightMap(m_heightMap);
                mesh.RecalculateNormals();

                //m_heightMap.RecalculateHighestPoint();

                GetComponent<Renderer>().material.mainTexture = MeshBuilder.GenerateTextureFromHeightMap(m_heightMap);
                GetComponent<Renderer>().material.SetFloat("_Smoothness", 0.15f);
                GetComponent<MeshCollider>().sharedMesh = mesh;
            }

            public TerrainHeightMap heightMap
            {
                get
                {
                    return m_heightMap;
                }
            }
        }

        /// <summary>
        /// A class that contains methods to build the basic terrain mesh.
        /// </summary>
        public class MeshBuilder
        {

            public static Vector3[] vertices
            {
                get
                {
                    int size = TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE;
                    Vector3[] vertices = new Vector3[((int)Mathf.Pow(size + 1, 2))];

                    for (int i = 0, z = 0; z <= size; z++)
                    {
                        for (int x = 0; x <= size; x++, i++)
                        {
                            vertices[i] = new Vector3(x, 0, z);
                        }
                    }
                    return vertices;
                }
            }

            public static Vector2[] uv
            {
                get
                {
                    int size = TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE;
                    Vector2[] uv = new Vector2[(size + 1) * (size + 1)];
                    for (int i = 0, y = 0; y <= size; y++)
                    {
                        for (int x = 0; x <= size; x++, i++)
                        {
                            uv[i] = new Vector2((float)x / size, (float)y / size);
                        }
                    }
                    return uv;
                }
            }

            public static int[] triangles
            {
                get
                {
                    int size = TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE;
                    int[] triangles = new int[size * size * 6];
                    for (int ti = 0, vi = 0, y = 0; y < size; y++, vi++)
                    {
                        for (int x = 0; x < size; x++, ti += 6, vi++)
                        {
                            triangles[ti] = vi;
                            triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                            triangles[ti + 4] = triangles[ti + 1] = vi + size + 1;
                            triangles[ti + 5] = vi + size + 2;
                        }
                    }
                    return triangles;
                }
            }

            public static Vector3[] BuildVerticesFromHeightMap(TerrainHeightMap heightMap)
            {
                int size = TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE;
                Vector3[] vertices = new Vector3[((int)Mathf.Pow(size + 1, 2))];

                for (int i = 0, z = 0; z <= size; z++)
                {
                    for (int x = 0; x <= size; x++, i++)
                    {
                        float y = heightMap.GetFloat(x, z);
                        vertices[i] = new Vector3(x, y, z);
                    }
                }
                return vertices;
            }

            public static Texture2D GenerateTextureFromHeightMap(TerrainHeightMap heightMap)
            {
                Texture2D tex = new Texture2D(TerrainChunkObject.TERRAIN_CHUNK_TEXTURE_RESOLUTION_SIZE, TerrainChunkObject.TERRAIN_CHUNK_TEXTURE_RESOLUTION_SIZE);
                tex.filterMode = FilterMode.Point;
                for (int x = 0; x < tex.width; x++)
                {
                    for (int y = 0; y < tex.height; y++)
                    {
                        Color c = new Color();
                        if (heightMap.GetFloat(x, y) > heightMap.highestFloat * 0.8f)
                        {
                            c = Color.white;
                        }
                        else if (heightMap.GetFloat(x, y) > heightMap.highestFloat * 0.65f)
                        {
                            c = new Color(0.65f, 0.65f, 0.65f);
                        }
                        else if (heightMap.GetFloat(x, y) < heightMap.highestFloat * 0.45f)
                        {
                            c = Color.blue;
                        }
                        else
                        {
                            c = Color.green;
                        }
                        c *= heightMap.GetFloat(x, y) / heightMap.totalStrength;
                        tex.SetPixel(x, y, c);
                    }
                }
                tex.Apply();
                return tex;
            }

            public static float[,] GeneratePerlinMap(int worldX, int worldY, TerrainHeightMap heightMap, float[] scales, float[] additions, float[] strengths, decimal[] limits)
            {
                int size = heightMap.size;
                float[,] noise = new float[size, size];

                float top = 0;
                float bottom = Mathf.Infinity;
                float seed = 1500;
                float totalStrength = 0f;

                for (int i = 0; i < scales.Length; i++)
                {

                    float y = 0.0F;
                    while (y < size)
                    {
                        float x = 0.0F;
                        while (x < size)
                        {
                            float xCoord = (seed + x / size * scales[i]) * worldX;
                            float yCoord = (seed + y / size * scales[i]) * worldY;
                            float sample = Mathf.PerlinNoise(xCoord, yCoord);
                            noise[(int)x, (int)y] += (sample * strengths[i]) + additions[i];

                            //top and bottom check
                            if(noise[(int)x, (int)y] > top)
                                top = noise[(int)x, (int)y];
                            else if(noise[(int)x, (int)y] < bottom)
                                bottom = noise[(int)x, (int)y];
                            x++;
                        }
                        y++;
                    }
                    totalStrength += strengths[i];
                }
                heightMap.SetHeightMap(noise, top, bottom, totalStrength);
                return noise;
            }
        }
    }
}
