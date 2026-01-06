using UnityEngine;

/// <summary>
/// Example script demonstrating programmatic control of the rain effect
/// Attach this to any GameObject to see various usage examples
/// </summary>
public class RainEffectExamples : MonoBehaviour
{
    [Header("References")]
    public Raindrops raindrops;
    public RainRenderer rainRenderer;
    public WeatherSystem weatherSystem;
    
    [Header("Example Settings")]
    public bool enableExample1_CyclicIntensity = false;
    public bool enableExample2_TimeBasedWeather = false;
    public bool enableExample3_AutoParallax = false;
    public bool enableExample4_InteractiveRain = false;
    
    [Header("Example 1: Cyclic Intensity")]
    public AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 0.2f, 1, 0.6f);
    public float intensityCycleDuration = 30f;
    
    [Header("Example 2: Time-Based Weather")]
    public float weatherChangeDuration = 15f;
    
    [Header("Example 3: Auto Parallax")]
    public float parallaxSpeed = 0.5f;
    public float parallaxAmount = 0.8f;
    
    private float timer = 0f;
    
    private void Start()
    {
        // Auto-find components if not assigned
        if (raindrops == null)
            raindrops = FindObjectOfType<Raindrops>();
        
        if (rainRenderer == null)
            rainRenderer = FindObjectOfType<RainRenderer>();
        
        if (weatherSystem == null)
            weatherSystem = FindObjectOfType<WeatherSystem>();
        
        // Verify components exist
        if (raindrops == null || rainRenderer == null)
        {
            Debug.LogWarning("RainEffectExamples: Required components not found!");
            enabled = false;
            return;
        }
        
        Debug.Log("RainEffectExamples: Initialized. Check Inspector to enable examples.");
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        // Example 1: Vary rain intensity over time
        if (enableExample1_CyclicIntensity)
        {
            Example1_CyclicIntensity();
        }
        
        // Example 2: Automatically change weather presets
        if (enableExample2_TimeBasedWeather && weatherSystem != null)
        {
            Example2_TimeBasedWeather();
        }
        
        // Example 3: Automatic parallax animation
        if (enableExample3_AutoParallax)
        {
            Example3_AutoParallax();
        }
        
        // Example 4: Interactive rain (rain only when mouse is pressed)
        if (enableExample4_InteractiveRain)
        {
            Example4_InteractiveRain();
        }
    }
    
    /// <summary>
    /// Example 1: Smoothly vary rain intensity over time using an animation curve
    /// </summary>
    private void Example1_CyclicIntensity()
    {
        float normalizedTime = (timer % intensityCycleDuration) / intensityCycleDuration;
        float intensity = intensityCurve.Evaluate(normalizedTime);
        
        // Apply intensity to rain parameters
        raindrops.options.rainChance = Mathf.Lerp(0.1f, 0.6f, intensity);
        raindrops.options.rainLimit = Mathf.RoundToInt(Mathf.Lerp(1, 6, intensity));
        raindrops.options.dropFallMultiplier = Mathf.Lerp(0.8f, 1.5f, intensity);
    }
    
    /// <summary>
    /// Example 2: Automatically cycle through weather presets
    /// </summary>
    private void Example2_TimeBasedWeather()
    {
        if (timer % weatherChangeDuration < Time.deltaTime)
        {
            weatherSystem.NextWeatherPreset();
            Debug.Log($"Weather changed to: {weatherSystem.weatherPresets[weatherSystem.currentPresetIndex].name}");
        }
    }
    
    /// <summary>
    /// Example 3: Automatic parallax animation without mouse input
    /// </summary>
    private void Example3_AutoParallax()
    {
        float x = Mathf.Sin(timer * parallaxSpeed) * parallaxAmount;
        float y = Mathf.Cos(timer * parallaxSpeed * 0.7f) * parallaxAmount;
        
        rainRenderer.SetParallax(x, y);
    }
    
    /// <summary>
    /// Example 4: Rain only appears when mouse button is held
    /// </summary>
    private void Example4_InteractiveRain()
    {
        raindrops.options.raining = Input.GetMouseButton(0);
        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Rain started (mouse pressed)");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Rain stopped (mouse released)");
        }
    }
    
    // ===== Additional Example Methods (can be called from other scripts) =====
    
    /// <summary>
    /// Example: Smoothly transition rain intensity to a target value
    /// </summary>
    public void TransitionRainIntensity(float targetIntensity, float duration)
    {
        StartCoroutine(TransitionRainIntensityCoroutine(targetIntensity, duration));
    }
    
    private System.Collections.IEnumerator TransitionRainIntensityCoroutine(float targetIntensity, float duration)
    {
        float startChance = raindrops.options.rainChance;
        float targetChance = Mathf.Lerp(0.1f, 0.6f, targetIntensity);
        
        int startLimit = raindrops.options.rainLimit;
        int targetLimit = Mathf.RoundToInt(Mathf.Lerp(1, 6, targetIntensity));
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            raindrops.options.rainChance = Mathf.Lerp(startChance, targetChance, t);
            raindrops.options.rainLimit = Mathf.RoundToInt(Mathf.Lerp(startLimit, targetLimit, t));
            
            yield return null;
        }
        
        raindrops.options.rainChance = targetChance;
        raindrops.options.rainLimit = targetLimit;
    }
    
    /// <summary>
    /// Example: Gradually fade rain in or out
    /// </summary>
    public void FadeRain(bool fadeIn, float duration)
    {
        StartCoroutine(FadeRainCoroutine(fadeIn, duration));
    }
    
    private System.Collections.IEnumerator FadeRainCoroutine(bool fadeIn, float duration)
    {
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        
        float originalAlphaMultiply = rainRenderer.alphaMultiply;
        float originalAlphaSubtract = rainRenderer.alphaSubtract;
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            
            // Adjust renderer alpha parameters
            rainRenderer.alphaMultiply = originalAlphaMultiply * alpha;
            rainRenderer.alphaSubtract = Mathf.Lerp(20f, originalAlphaSubtract, alpha);
            
            yield return null;
        }
        
        if (!fadeIn)
        {
            raindrops.options.raining = false;
        }
    }
    
    /// <summary>
    /// Example: Create a storm effect (increasing intensity)
    /// </summary>
    public void StartStorm()
    {
        Debug.Log("Storm starting!");
        
        // Increase rain parameters
        raindrops.options.rainChance = 0.6f;
        raindrops.options.rainLimit = 6;
        raindrops.options.minR = 15f;
        raindrops.options.maxR = 50f;
        raindrops.options.dropFallMultiplier = 1.8f;
        
        // Adjust visual settings for dramatic effect
        rainRenderer.brightness = 0.9f;
        rainRenderer.alphaMultiply = 8f;
    }
    
    /// <summary>
    /// Example: Return to calm rain
    /// </summary>
    public void StopStorm()
    {
        Debug.Log("Storm ending, returning to calm rain");
        
        // Reset to gentle rain
        raindrops.options.rainChance = 0.25f;
        raindrops.options.rainLimit = 3;
        raindrops.options.minR = 10f;
        raindrops.options.maxR = 35f;
        raindrops.options.dropFallMultiplier = 1f;
        
        // Reset visual settings
        rainRenderer.brightness = 1.04f;
        rainRenderer.alphaMultiply = 6f;
    }
    
    /// <summary>
    /// Example: Respond to game events (e.g., player enters building)
    /// </summary>
    public void OnPlayerEnterBuilding()
    {
        Debug.Log("Player entered building - stopping rain");
        FadeRain(false, 2f);
    }
    
    /// <summary>
    /// Example: Respond to game events (e.g., player exits building)
    /// </summary>
    public void OnPlayerExitBuilding()
    {
        Debug.Log("Player exited building - starting rain");
        raindrops.options.raining = true;
        FadeRain(true, 2f);
    }
    
    // ===== Unity Editor Helper Methods =====
    
    private void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUILayout.BeginArea(new Rect(10, 100, 300, 400));
        GUILayout.Label("Rain Effect Examples", GUI.skin.box);
        
        if (GUILayout.Button("Start Storm"))
            StartStorm();
        
        if (GUILayout.Button("Stop Storm"))
            StopStorm();
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Fade Rain In (2s)"))
            FadeRain(true, 2f);
        
        if (GUILayout.Button("Fade Rain Out (2s)"))
            FadeRain(false, 2f);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Light Rain"))
            TransitionRainIntensity(0.2f, 2f);
        
        if (GUILayout.Button("Medium Rain"))
            TransitionRainIntensity(0.5f, 2f);
        
        if (GUILayout.Button("Heavy Rain"))
            TransitionRainIntensity(0.9f, 2f);
        
        GUILayout.EndArea();
    }
}
