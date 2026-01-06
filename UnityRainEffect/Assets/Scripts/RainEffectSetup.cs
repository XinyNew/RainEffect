using UnityEngine;

/// <summary>
/// Simple setup script to initialize the rain effect scene
/// Place this on the Main Camera
/// </summary>
public class RainEffectSetup : MonoBehaviour
{
    [Header("Drop Textures")]
    public Texture2D dropAlpha;
    public Texture2D dropColor;
    
    [Header("Weather Textures - Rain")]
    public Texture2D rainForeground;
    public Texture2D rainBackground;
    
    [Header("Weather Textures - Drizzle")]
    public Texture2D drizzleForeground;
    public Texture2D drizzleBackground;
    
    [Header("Weather Textures - Storm")]
    public Texture2D stormForeground;
    public Texture2D stormBackground;
    
    [Header("Weather Textures - Sun")]
    public Texture2D sunForeground;
    public Texture2D sunBackground;
    
    [Header("Weather Textures - Fallout")]
    public Texture2D falloutForeground;
    public Texture2D falloutBackground;
    
    private void Start()
    {
        SetupComponents();
    }
    
    private void SetupComponents()
    {
        // Add Raindrops component if not present
        Raindrops raindrops = gameObject.GetComponent<Raindrops>();
        if (raindrops == null)
        {
            raindrops = gameObject.AddComponent<Raindrops>();
        }
        
        // Set textures
        if (dropAlpha != null) raindrops.dropAlphaTexture = dropAlpha;
        if (dropColor != null) raindrops.dropColorTexture = dropColor;
        
        // Configure rain options
        raindrops.options = new Raindrops.RaindropsOptions
        {
            minR = 10f,
            maxR = 40f,
            maxDrops = 900,
            rainChance = 0.3f,
            rainLimit = 3,
            dropletsRate = 50,
            trailRate = 1f,
            trailScaleRange = new Vector2(0.2f, 0.45f),
            collisionRadius = 0.45f,
            dropletsCleaningRadiusMultiplier = 0.28f,
            raining = true
        };
        
        // Add RainRenderer component if not present
        RainRenderer renderer = gameObject.GetComponent<RainRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<RainRenderer>();
        }
        
        // Set textures for rain weather
        if (rainForeground != null) renderer.textureForeground = rainForeground;
        if (rainBackground != null) renderer.textureBackground = rainBackground;
        
        // Configure renderer settings
        renderer.brightness = 1.04f;
        renderer.alphaMultiply = 6f;
        renderer.alphaSubtract = 3f;
        renderer.parallaxBg = 5f;
        renderer.parallaxFg = 20f;
        renderer.minRefraction = 256f;
        renderer.maxRefraction = 512f;
        renderer.raindropsComponent = raindrops;
        
        // Add controller component if not present
        RainEffectController controller = gameObject.GetComponent<RainEffectController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<RainEffectController>();
        }
        controller.rainRenderer = renderer;
        controller.enableParallax = true;
        controller.parallaxSmoothTime = 1f;
        
        // Add weather system if not present
        WeatherSystem weatherSystem = gameObject.GetComponent<WeatherSystem>();
        if (weatherSystem == null)
        {
            weatherSystem = gameObject.AddComponent<WeatherSystem>();
        }
        weatherSystem.rainRenderer = renderer;
        weatherSystem.raindrops = raindrops;
        
        // Setup weather presets
        SetupWeatherPresets(weatherSystem);
        
        Debug.Log("Rain Effect Setup Complete!");
    }
    
    private void SetupWeatherPresets(WeatherSystem weatherSystem)
    {
        var presets = new System.Collections.Generic.List<WeatherSystem.WeatherPreset>();
        
        // Rain preset
        if (rainForeground != null && rainBackground != null)
        {
            presets.Add(new WeatherSystem.WeatherPreset
            {
                name = "Rain",
                foregroundTexture = rainForeground,
                backgroundTexture = rainBackground,
                raindropsOptions = new Raindrops.RaindropsOptions
                {
                    rainChance = 0.3f,
                    rainLimit = 3,
                    raining = true,
                    minR = 10f,
                    maxR = 40f
                }
            });
        }
        
        // Drizzle preset
        if (drizzleForeground != null && drizzleBackground != null)
        {
            presets.Add(new WeatherSystem.WeatherPreset
            {
                name = "Drizzle",
                foregroundTexture = drizzleForeground,
                backgroundTexture = drizzleBackground,
                raindropsOptions = new Raindrops.RaindropsOptions
                {
                    rainChance = 0.15f,
                    rainLimit = 2,
                    raining = true,
                    minR = 8f,
                    maxR = 30f
                }
            });
        }
        
        // Storm preset
        if (stormForeground != null && stormBackground != null)
        {
            presets.Add(new WeatherSystem.WeatherPreset
            {
                name = "Storm",
                foregroundTexture = stormForeground,
                backgroundTexture = stormBackground,
                raindropsOptions = new Raindrops.RaindropsOptions
                {
                    rainChance = 0.5f,
                    rainLimit = 5,
                    raining = true,
                    minR = 12f,
                    maxR = 50f,
                    dropFallMultiplier = 1.5f
                }
            });
        }
        
        // Sun preset (no rain)
        if (sunForeground != null && sunBackground != null)
        {
            presets.Add(new WeatherSystem.WeatherPreset
            {
                name = "Sunny",
                foregroundTexture = sunForeground,
                backgroundTexture = sunBackground,
                raindropsOptions = new Raindrops.RaindropsOptions
                {
                    rainChance = 0f,
                    rainLimit = 0,
                    raining = false
                }
            });
        }
        
        // Fallout preset
        if (falloutForeground != null && falloutBackground != null)
        {
            presets.Add(new WeatherSystem.WeatherPreset
            {
                name = "Fallout",
                foregroundTexture = falloutForeground,
                backgroundTexture = falloutBackground,
                raindropsOptions = new Raindrops.RaindropsOptions
                {
                    rainChance = 0.4f,
                    rainLimit = 4,
                    raining = true,
                    minR = 10f,
                    maxR = 45f
                }
            });
        }
        
        weatherSystem.weatherPresets = presets.ToArray();
    }
}
