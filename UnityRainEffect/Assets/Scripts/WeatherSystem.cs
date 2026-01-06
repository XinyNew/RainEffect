using UnityEngine;

/// <summary>
/// Manages different weather presets and transitions
/// </summary>
public class WeatherSystem : MonoBehaviour
{
    [System.Serializable]
    public class WeatherPreset
    {
        public string name;
        public Texture2D foregroundTexture;
        public Texture2D backgroundTexture;
        public Raindrops.RaindropsOptions raindropsOptions;
    }
    
    [Header("References")]
    public RainRenderer rainRenderer;
    public Raindrops raindrops;
    
    [Header("Weather Presets")]
    public WeatherPreset[] weatherPresets;
    public int currentPresetIndex = 0;
    
    [Header("Transition Settings")]
    public float transitionDuration = 2f;
    
    private bool isTransitioning = false;
    private float transitionTimer = 0f;
    
    private void Start()
    {
        if (rainRenderer == null)
            rainRenderer = FindObjectOfType<RainRenderer>();
        
        if (raindrops == null)
            raindrops = FindObjectOfType<Raindrops>();
        
        // Apply initial weather preset
        if (weatherPresets.Length > 0)
        {
            ApplyWeatherPreset(currentPresetIndex);
        }
    }
    
    private void Update()
    {
        // Allow cycling through presets with keyboard
        if (Input.GetKeyDown(KeyCode.Alpha1) && weatherPresets.Length > 0)
        {
            SetWeatherPreset(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && weatherPresets.Length > 1)
        {
            SetWeatherPreset(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && weatherPresets.Length > 2)
        {
            SetWeatherPreset(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && weatherPresets.Length > 3)
        {
            SetWeatherPreset(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && weatherPresets.Length > 4)
        {
            SetWeatherPreset(4);
        }
        
        // Cycle with arrow keys
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextWeatherPreset();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousWeatherPreset();
        }
    }
    
    public void SetWeatherPreset(int index)
    {
        if (index < 0 || index >= weatherPresets.Length)
        {
            Debug.LogWarning($"Weather preset index {index} out of range!");
            return;
        }
        
        currentPresetIndex = index;
        ApplyWeatherPreset(index);
    }
    
    public void NextWeatherPreset()
    {
        if (weatherPresets.Length == 0) return;
        currentPresetIndex = (currentPresetIndex + 1) % weatherPresets.Length;
        ApplyWeatherPreset(currentPresetIndex);
    }
    
    public void PreviousWeatherPreset()
    {
        if (weatherPresets.Length == 0) return;
        currentPresetIndex--;
        if (currentPresetIndex < 0)
            currentPresetIndex = weatherPresets.Length - 1;
        ApplyWeatherPreset(currentPresetIndex);
    }
    
    private void ApplyWeatherPreset(int index)
    {
        WeatherPreset preset = weatherPresets[index];
        
        Debug.Log($"Applying weather preset: {preset.name}");
        
        // Update renderer textures
        if (rainRenderer != null)
        {
            if (preset.foregroundTexture != null)
                rainRenderer.SetForegroundTexture(preset.foregroundTexture);
            
            if (preset.backgroundTexture != null)
                rainRenderer.SetBackgroundTexture(preset.backgroundTexture);
        }
        
        // Update raindrops settings
        if (raindrops != null && preset.raindropsOptions != null)
        {
            raindrops.options = preset.raindropsOptions;
        }
    }
    
    private void OnGUI()
    {
        // Display current weather preset name
        GUI.Label(new Rect(10, 10, 300, 30), $"Current Weather: {weatherPresets[currentPresetIndex].name}");
        GUI.Label(new Rect(10, 40, 400, 30), "Use Arrow Keys or Number Keys (1-5) to change weather");
    }
}
