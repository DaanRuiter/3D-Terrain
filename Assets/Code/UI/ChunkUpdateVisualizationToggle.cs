using UnityEngine;
using UnityEngine.UI;

using IslandBuilder.Terrain;

public class ChunkUpdateVisualizationToggle : MonoBehaviour {

    public Toggle m_toggle;

	public void OnValueChanged()
    {
        TerrainChunksWrapper.Singleton.renderChunkUpdates = m_toggle.isOn;
    }
}
