using UnityEngine;

using System.Collections.Generic;

namespace IslandBuilder
{
    namespace Terrain
    {
        /// <summary>
        /// A class that allows operations to be used as if all the chunks are a combined single object
        /// </summary>
        public class TerrainChunksWrapper
        {
            public const int WORLD_CHUNK_SIZE = 4;

            //Singleton
            private static TerrainChunksWrapper instance;
            public void Awake() { instance = this; }
            public static TerrainChunksWrapper Singleton { get { return instance; } }

            public bool renderChunkUpdates = false;

            private TerrainChunkObject[,] m_terrainChunks;
            private TerrainHeightMap m_globalheightMap;
            private TerrainHeightMap[,] m_chunkHeigtmaps;

            public void BuildTerrainChunks()
            {
                m_terrainChunks = new TerrainChunkObject[WORLD_CHUNK_SIZE, WORLD_CHUNK_SIZE];

                m_globalheightMap = new TerrainHeightMap(WORLD_CHUNK_SIZE * TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE);
                m_chunkHeigtmaps = new TerrainHeightMap[WORLD_CHUNK_SIZE, WORLD_CHUNK_SIZE];

                for (int x = 0; x < WORLD_CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < WORLD_CHUNK_SIZE; y++)
                    {
                        GameObject terrainObject = new GameObject("Terrain - " + x + ", " + y);

                        MeshBuilder.GeneratePerlinMap(x, y, m_globalheightMap,
                            new float[] { 1.25f, 3f, 10f, 25, 0.5f }, //scale
                            new float[] { 1f, 4.15f, 0f, 0, 0f}, //additions
                            new float[] { 28, 5f, 2f, 0.5f, 55f }, //strength
                            new decimal[] { 0.55m, 1m, 0.25m, 0.2m, 55m }); //limits)

                        m_chunkHeigtmaps[x, y] = new TerrainHeightMap(TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE);
                        m_terrainChunks[x, y] = terrainObject.AddComponent<TerrainChunkObject>();
                    }
                }

                for (int x = 0; x < WORLD_CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < WORLD_CHUNK_SIZE; y++)
                    {
                        m_chunkHeigtmaps[x, y].SetHeightMap(m_globalheightMap.GetPortion(x * TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE, 
                            y * TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE, TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE), m_globalheightMap.highestFloat, m_globalheightMap.lowestFloat, m_globalheightMap.totalStrength);

                        m_terrainChunks[x, y].Build(x, y, m_chunkHeigtmaps[x, y]);
                    }
                }
            }

            /// <summary>
            /// raise the heightmap of the terrain and rebuild the mesh afterwards
            /// uses the BrushSizeSlider value for it's size
            /// </summary>
            /// <param name="centerX">x position of the center point</param>
            /// <param name="centerY">y position of the center point</param>
            public void RaiseTerrain(int centerX, int centerY, TerrainChunkObject targetChunk)
            {
                int r = TerrainTerraforming.Singleton.brushSize; //brush radius
                float s = TerrainTerraforming.Singleton.brushStrength; //brush strength

                int ox = centerX, oy = centerY; // origin
                int worldSize = WORLD_CHUNK_SIZE * TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE; //total world size
                List<TerrainChunkObject> affectedChunks = new List<TerrainChunkObject>(); //the chunks that will need to be rebuilt

                //raise the heightmap floats
                for (int x = -r; x < r; x++)
                {
                    int height = (int)Mathf.Sqrt(r * r - x * x);
                
                    for (int y = -height; y < height; y++)
                    {
                        if (x + ox >= 0 && x + ox < worldSize && y + oy >= 0 && y + oy < worldSize)
                        {
                            m_globalheightMap.AddToFloat(x + ox, y + oy, s);
                        }
                    }
                }
                
                int globalX =  ox; //global x of the circle's center
                int globalY =  oy; //global y of the circle's center

                //calculate affected chunks
                for (int width = 0; width < WORLD_CHUNK_SIZE; width++)
                {
                    for (int length = 0; length < WORLD_CHUNK_SIZE; length++)
                    {
                        int currentGlobalX = m_terrainChunks[width, length].worldX * TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE;
                        int currentGlobalY = m_terrainChunks[width, length].worldY * TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE;
                        
                        if (globalX + r > currentGlobalX && globalY + r > currentGlobalY)
                            if (globalX - r < (currentGlobalX + TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE) && globalY - r < (currentGlobalY + TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE))
                                affectedChunks.Add(m_terrainChunks[width, length]);

                        //reset the highlighing color
                        m_terrainChunks[width, length].GetComponent<Renderer>().material.color = Color.white;
                    }
                }

                //apply changed height floats
                for (int x = 0; x < WORLD_CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < WORLD_CHUNK_SIZE; y++)
                    {

                        m_chunkHeigtmaps[x, y].SetHeightMap(m_globalheightMap.GetPortion(x * TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE,
                            y * TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE, TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE), m_globalheightMap.highestFloat, m_globalheightMap.lowestFloat, m_globalheightMap.totalStrength);
                    }
                }

                //rebuild affected chunks
                for (int i = 0; i < affectedChunks.Count; i++)
                {
                    affectedChunks[i].RebuildObject();
                    if(renderChunkUpdates)
                        affectedChunks[i].GetComponent<Renderer>().material.color = Color.blue;
                }
            }
        }
    }
}
