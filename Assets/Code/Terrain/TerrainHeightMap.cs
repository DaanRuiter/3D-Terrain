using UnityEngine;

namespace IslandBuilder
{
    namespace Terrain
    {
        /// <summary>
        /// Heightmap for terrain in floats
        /// </summary>
        public class TerrainHeightMap
        {
            private float[,] m_heightFloats;
            private float m_lowestFloat;
            private float m_highestFloat;
            private int m_size;
            private float m_totalStrength;

            public TerrainHeightMap(int size)
            {
                m_heightFloats = new float[size + 1, size + 1];
                m_size = size;
            }

            public bool SetHeightMap(float[,] heightMap, float highest, float lowest, float totalStrength)
            {
                if (m_heightFloats != null)
                    if (m_heightFloats == heightMap)
                        return false;

                m_heightFloats = heightMap;
                m_highestFloat = highest + lowest;
                m_lowestFloat = lowest;
                m_totalStrength = totalStrength;

                return true;
            }

            public float[,] GetPortion(int startX, int startY, int size)
            {
                float[,] heightMap = new float[size + 1, size + 1];

                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        heightMap[x, y] = m_heightFloats[x + startX, y + startY];
                    }
                }

                return heightMap;
            }

            public float GetFloat(int x, int y)
            {
                return m_heightFloats[x, y];
            }

            public void AddToFloat(int x, int y, float addition)
            {
                m_heightFloats[x, y] += addition;
            }
            
            public void RecalculateHighestPoint()
            {
                float highest = 0f;
                for (int x = 0; x < TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE; x++)
                {
                    for (int y = 0; y < TerrainChunkObject.TERRAIN_CHUNK_TILE_SIZE; y++)
                    {
                        if (m_heightFloats[x, y] > highest)
                        {
                            highest = m_heightFloats[x, y];
                        }
                    }
                }
                m_highestFloat = highest;
            }

            public float highestFloat
            {
                get
                {
                    return m_highestFloat;
                }
            }

            public float lowestFloat
            {
                get
                {
                    return m_lowestFloat;
                }
            }

            public int size
            {
                get
                {
                    return m_size;
                }
            }

            public float totalStrength
            {
                get
                {
                    return m_totalStrength;
                }
                set
                {
                    m_totalStrength = value;
                }
            }
        }
    }
}
