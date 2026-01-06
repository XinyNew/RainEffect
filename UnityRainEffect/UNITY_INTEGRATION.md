# Unity Integration Guide

This guide provides detailed instructions for integrating the Rain Effect into your Unity project.

## Table of Contents

1. [Quick Integration](#quick-integration)
2. [Manual Setup](#manual-setup)
3. [Configuration](#configuration)
4. [Advanced Customization](#advanced-customization)
5. [Troubleshooting](#troubleshooting)

## Quick Integration

### Step 1: Import Assets

Copy the following folders from `UnityRainEffect/Assets` to your project's Assets folder:

```
YourProject/Assets/
  ├── Scripts/
  │   ├── Raindrops.cs
  │   ├── RainRenderer.cs
  │   ├── RainEffectController.cs
  │   ├── WeatherSystem.cs
  │   └── RainEffectSetup.cs
  ├── Shaders/
  │   └── RainWaterEffect.shader
  └── Textures/
      ├── Drops/
      └── Weather/
```

### Step 2: Setup Camera

1. Select your Main Camera in the Hierarchy
2. Add the `RainEffectSetup` component (Component → Add → Rain Effect Setup)
3. Assign the required textures in the Inspector:
   - **Drop Alpha**: `Textures/Drops/drop-alpha.png`
   - **Drop Color**: `Textures/Drops/drop-color.png`
   - **Rain Foreground**: `Textures/Weather/texture-rain-fg.png`
   - **Rain Background**: `Textures/Weather/texture-rain-bg.png`
4. Press Play!

The `RainEffectSetup` component will automatically configure all necessary components.

## Manual Setup

If you prefer manual control, follow these steps:

### Step 1: Add Raindrops Component

1. Select your Main Camera
2. Add Component → Raindrops
3. Configure properties:
   ```
   Drop Alpha Texture: drop-alpha.png
   Drop Color Texture: drop-color.png
   Options:
     Min R: 10
     Max R: 40
     Rain Chance: 0.3
     Rain Limit: 3
     Raining: true
   ```

### Step 2: Add RainRenderer Component

1. With Main Camera still selected
2. Add Component → Rain Renderer
3. Configure properties:
   ```
   Texture Foreground: texture-rain-fg.png
   Texture Background: texture-rain-bg.png
   Parallax Bg: 5
   Parallax Fg: 20
   Brightness: 1.04
   Alpha Multiply: 6
   Alpha Subtract: 3
   Min Refraction: 256
   Max Refraction: 512
   Raindrops Component: (drag Raindrops component here)
   ```

### Step 3: Add RainEffectController Component

1. With Main Camera still selected
2. Add Component → Rain Effect Controller
3. Configure properties:
   ```
   Rain Renderer: (drag RainRenderer component here)
   Enable Parallax: true
   Parallax Smooth Time: 1
   ```

### Step 4: (Optional) Add WeatherSystem Component

For multiple weather presets:

1. Add Component → Weather System
2. Configure properties:
   ```
   Rain Renderer: (drag RainRenderer component here)
   Raindrops: (drag Raindrops component here)
   Weather Presets: (see Configuration section)
   ```

## Configuration

### Raindrops Options

| Property | Description | Default | Range |
|----------|-------------|---------|-------|
| Min R | Minimum raindrop radius | 10 | 5-20 |
| Max R | Maximum raindrop radius | 40 | 20-100 |
| Max Drops | Maximum number of drops | 900 | 100-2000 |
| Rain Chance | Probability of new drops spawning | 0.3 | 0-1 |
| Rain Limit | Max drops spawning simultaneously | 3 | 1-10 |
| Raining | Enable/disable rain | true | bool |
| Drop Fall Multiplier | Speed of falling | 1 | 0.5-3 |

### RainRenderer Settings

| Property | Description | Default | Range |
|----------|-------------|---------|-------|
| Parallax Bg | Background parallax depth | 5 | 0-20 |
| Parallax Fg | Foreground parallax depth | 20 | 0-50 |
| Min Refraction | Minimum distortion | 256 | 100-500 |
| Max Refraction | Maximum distortion | 512 | 200-1000 |
| Brightness | Overall brightness | 1 | 0.5-2 |
| Alpha Multiply | Drop visibility | 6-20 | 1-50 |
| Alpha Subtract | Drop transparency | 3-5 | 0-10 |
| Render Shine | Enable highlights | false | bool |
| Render Shadow | Enable shadows | false | bool |

### Weather Preset Configuration

Example weather preset setup:

```csharp
Weather Presets:
  - Name: "Heavy Rain"
    Foreground Texture: texture-rain-fg
    Background Texture: texture-rain-bg
    Raindrops Options:
      Rain Chance: 0.4
      Rain Limit: 5
      Min R: 12
      Max R: 45

  - Name: "Light Drizzle"
    Foreground Texture: texture-drizzle-fg
    Background Texture: texture-drizzle-bg
    Raindrops Options:
      Rain Chance: 0.15
      Rain Limit: 2
      Min R: 8
      Max R: 25
```

## Advanced Customization

### Custom Shader Modifications

The shader is located at `Assets/Shaders/RainWaterEffect.shader`. You can modify:

#### 1. Refraction Strength

Find the refraction calculation in the fragment shader:
```hlsl
float2 refractionPos = scaledTexCoord(tc)
    + (pixel() * refraction * (_MinRefraction + (d * _RefractionDelta)))
    + refractionParallax;
```

Multiply the refraction vector by a custom factor:
```hlsl
float2 refractionPos = scaledTexCoord(tc)
    + (pixel() * refraction * (_MinRefraction + (d * _RefractionDelta)) * 2.0) // 2x stronger
    + refractionParallax;
```

#### 2. Color Tinting

Add color tinting to the final output:
```hlsl
float4 tint = float4(0.8, 0.9, 1.0, 1.0); // Blueish tint
return blend(bg, fg) * tint;
```

#### 3. Custom Parallax Calculation

Modify the parallax function:
```hlsl
float2 parallax(float v)
{
    // Custom non-linear parallax
    float factor = v * v; // Square for exponential effect
    return float2(_ParallaxX, _ParallaxY) * pixel() * factor;
}
```

### Creating Custom Drop Textures

1. Create a 64x64 PNG image
2. Draw your drop shape (white = opaque, black = transparent)
3. For alpha texture: Use grayscale for transparency
4. For color texture: Use colors for the drop appearance
5. Import to Unity and assign to Raindrops component

### Performance Optimization

#### For Mobile Devices

```csharp
// Reduce drop count
raindrops.options.maxDrops = 300;
raindrops.options.rainLimit = 2;

// Simplify rendering
rainRenderer.renderShine = false;
rainRenderer.renderShadow = false;

// Lower refraction quality
rainRenderer.minRefraction = 128;
rainRenderer.maxRefraction = 256;
```

#### For High-End PCs

```csharp
// Increase drop count
raindrops.options.maxDrops = 1500;
raindrops.options.rainLimit = 5;

// Enable all effects
rainRenderer.renderShine = true;
rainRenderer.renderShadow = true;

// Higher refraction quality
rainRenderer.minRefraction = 384;
rainRenderer.maxRefraction = 768;
```

### Scripting API

#### Controlling Rain at Runtime

```csharp
// Start/stop rain
Raindrops raindrops = GetComponent<Raindrops>();
raindrops.options.raining = true;

// Change intensity
raindrops.options.rainChance = 0.5f; // More rain
raindrops.options.rainLimit = 5; // More simultaneous drops

// Change drop size
raindrops.options.minR = 15f;
raindrops.options.maxR = 50f;
```

#### Changing Textures Dynamically

```csharp
RainRenderer renderer = GetComponent<RainRenderer>();
renderer.SetForegroundTexture(newForegroundTexture);
renderer.SetBackgroundTexture(newBackgroundTexture);
```

#### Animating Parallax

```csharp
RainRenderer renderer = GetComponent<RainRenderer>();

void Update() {
    float time = Time.time;
    float x = Mathf.Sin(time * 0.5f);
    float y = Mathf.Cos(time * 0.3f);
    renderer.SetParallax(x, y);
}
```

## Troubleshooting

### Problem: No rain visible

**Solution:**
- Check that textures are assigned in Inspector
- Verify `Raining` is set to `true` in Raindrops options
- Increase `Rain Chance` value
- Check that drop textures are imported correctly

### Problem: Black screen

**Solution:**
- Verify shader compiled without errors (check Console)
- Check that background/foreground textures are assigned
- Ensure textures are set to Texture2D type (not Sprite)

### Problem: Poor performance

**Solution:**
- Reduce `Max Drops` value
- Disable `Render Shine` and `Render Shadow`
- Lower screen resolution
- Use smaller texture sizes

### Problem: Refraction not working

**Solution:**
- Check that `Water Map Texture` is assigned (should be automatic)
- Verify `Min Refraction` and `Max Refraction` values are different
- Ensure `Alpha Multiply` is high enough to make drops visible

### Problem: Parallax not responding

**Solution:**
- Check that `RainEffectController` component is attached
- Verify `Enable Parallax` is checked
- Test mouse movement (try moving cursor across the screen)

### Problem: Textures appear stretched

**Solution:**
- Adjust `Texture Ratio` in shader properties
- Use textures with matching aspect ratios
- Modify the `scaledTexCoord()` function in the shader

## Examples

### Example 1: Weather Transition System

```csharp
using UnityEngine;
using System.Collections;

public class WeatherTransition : MonoBehaviour
{
    public WeatherSystem weatherSystem;
    public float transitionInterval = 10f;

    void Start()
    {
        StartCoroutine(CycleWeather());
    }

    IEnumerator CycleWeather()
    {
        while (true)
        {
            yield return new WaitForSeconds(transitionInterval);
            weatherSystem.NextWeatherPreset();
        }
    }
}
```

### Example 2: Rain Intensity Controller

```csharp
using UnityEngine;

public class RainIntensityController : MonoBehaviour
{
    public Raindrops raindrops;
    public AnimationCurve intensityCurve;
    public float cycleDuration = 30f;

    void Update()
    {
        float t = (Time.time % cycleDuration) / cycleDuration;
        float intensity = intensityCurve.Evaluate(t);
        
        raindrops.options.rainChance = intensity;
        raindrops.options.rainLimit = Mathf.RoundToInt(intensity * 5f);
    }
}
```

### Example 3: Interactive Parallax

```csharp
using UnityEngine;

public class InteractiveParallax : MonoBehaviour
{
    public RainRenderer rainRenderer;
    public float parallaxStrength = 1f;
    
    void Update()
    {
        // Get device orientation (mobile)
        Vector3 acceleration = Input.acceleration;
        float x = acceleration.x * parallaxStrength;
        float y = acceleration.y * parallaxStrength;
        
        rainRenderer.SetParallax(x, y);
    }
}
```

## Best Practices

1. **Always test on target hardware** - Performance varies significantly between platforms
2. **Use appropriate texture sizes** - Don't use 4K textures on mobile
3. **Profile your build** - Use Unity Profiler to identify bottlenecks
4. **Cache component references** - Don't use GetComponent in Update()
5. **Batch texture changes** - Update multiple properties before applying
6. **Consider LOD system** - Reduce quality based on distance or platform

## Further Reading

- [Unity Shader Documentation](https://docs.unity3d.com/Manual/ShadersOverview.html)
- [Post Processing Effects](https://docs.unity3d.com/Manual/PostProcessingOverview.html)
- [Render Textures](https://docs.unity3d.com/Manual/class-RenderTexture.html)
- [Original Codrops Article](http://tympanus.net/codrops/?p=25417)
