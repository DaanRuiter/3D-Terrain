using UnityEngine;

namespace IslandBuilder
{
    namespace Terrain
    {
        /// <summary>
        /// A seed that can be used to generate perlin noise maps
        /// </summary>
        public class WorldSeed
        {
            //the min and max value the seed can have
            private const int MIN_SEED_VALUE = -25000;
            private const int MAX_SEED_VALUE = 25000;

            //the value of the seed
            private int m_seedValue = 0;

            public WorldSeed(bool randomizeOnInit = false)
            {
                if (randomizeOnInit)
                    Randomize();
            }

            /// <summary>
            /// Randomize the float
            /// </summary>
            /// <returns>the randomized seed</returns>
            public int Randomize()
            {
                m_seedValue = Random.Range(MIN_SEED_VALUE, MAX_SEED_VALUE);
                return m_seedValue;
            }

            /// <summary>
            /// Set the seed
            /// </summary>
            public int set
            {
                set
                {
                    m_seedValue = value;
                }
            }

            /// <summary>
            /// Get the seed
            /// </summary>
            public int get
            {
                get
                {
                    return m_seedValue;
                }
            }
        }
    }
}
