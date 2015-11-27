using UnityEngine;

using IslandBuilder.Terrain;

namespace IslandBuilder
{
    namespace Main
    {
        internal enum GameState
        {
            Menu,
            LoadingNew,
            LoadingExisting,
            Playing
        }

        /// <summary>
        /// The main class where you can start a new game or load existing ones.
        /// </summary>
        public class Main : MonoBehaviour
        {
            private static GameState m_gameState = GameState.Menu;
            private static WorldInfoContainer m_worldInfo;

            private TerrainChunksWrapper m_terrainChunkWrapper;

            private void Start()
            {
                m_terrainChunkWrapper = new TerrainChunksWrapper();
                m_terrainChunkWrapper.Awake();
                m_terrainChunkWrapper.BuildTerrainChunks();
            }

            public void CreateNewWorld()
            {
                m_gameState = GameState.LoadingNew;
            }

            public void LoadExistingWorld()
            {
                m_gameState = GameState.LoadingExisting;
            }


        }
    }
}
