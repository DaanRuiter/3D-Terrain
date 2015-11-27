using UnityEngine;
using IslandBuilder.Terrain;

public class TerrainTerraforming : MonoBehaviour {

    //Singleton
    private static TerrainTerraforming instance;
    private void Awake() { instance = this; }
    public static TerrainTerraforming Singleton { get { return instance; } }

    public int brushSize = 3;
    public float brushStrength = 0.5f;

    private Ray m_ray;

    private void Update()
    {
        m_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(m_ray, out hit))
        {
            if(hit.transform != null)
            {
                TerrainChunkObject chunk = hit.transform.GetComponent<TerrainChunkObject>();
                if (chunk != null)
                {
                    Vector3 mouseHit = hit.transform.InverseTransformPoint(hit.point);
                    int x = (int)hit.point.x;
                    int y = (int)hit.point.z;

                    if (Input.GetMouseButton(0))
                    {
                        TerrainChunksWrapper.Singleton.RaiseTerrain(x, y, chunk);
                    }

                }
            }
        }
    }
}
