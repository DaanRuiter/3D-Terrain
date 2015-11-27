using UnityEngine;
using UnityEngine.UI;

public enum BrushSliderType
{
    Size,
    Strength
}

public class BrushSlider : MonoBehaviour
{
    public Text currentSizeText;
    public Slider slider;
    public BrushSliderType type;

    private void Start()
    {
        currentSizeText.text = slider.value + "/" + slider.maxValue;
    }

    public void OnValueChange()
    {
        currentSizeText.text = slider.value + "/" + slider.maxValue;
        switch (type)
        {
            case BrushSliderType.Size:
                TerrainTerraforming.Singleton.brushSize = (int)slider.value;
                break;
            case BrushSliderType.Strength:
                TerrainTerraforming.Singleton.brushStrength = slider.value;
                break;
        }
    }

    public int currentSize
    {
        get
        {
            return (int)slider.value;
        }
    }
}
