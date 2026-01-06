# Unity Rain Effect

This is a Unity port of the WebGL Rain & Water Effect originally created by Lucas Bebber for Codrops.

## Features

- Realistic rain drops with physics simulation
- Water refraction effects on background textures
- Multiple weather presets (Rain, Drizzle, Storm, Sunny, Fallout)
- Mouse/touch parallax effect
- Optimized rendering using Unity's shader system
- Easy to integrate into existing Unity projects

## Requirements

- Unity 2021.3 LTS or higher
- Shader Model 3.0+ support

## Installation

### Option 1: Copy to Existing Project

1. Copy the `UnityRainEffect/Assets` folder contents to your Unity project's `Assets` folder
2. Open your Unity project
3. The rain effect assets will be imported automatically

### Option 2: Open as Standalone Project

1. Open Unity Hub
2. Click "Add" and navigate to the `UnityRainEffect` folder
3. Select the folder and click "Open"
4. Unity will import all assets automatically

## Quick Start

### Using the Demo Scene

1. Open the scene: `Assets/Scenes/RainEffectDemo.unity`
2. Select the Main Camera in the hierarchy
3. Assign textures to the `RainEffectSetup` component:
   - Drop Textures: `Assets/Textures/Drops/drop-alpha.png`, `drop-color.png`
   - Rain Textures: `Assets/Textures/Weather/texture-rain-fg.png`, `texture-rain-bg.png`
   - (Optional) Assign other weather textures
4. Press Play to see the effect

### Adding to Your Scene

1. Select your Main Camera
2. Add the following components (in order):
   - `Raindrops` component
   - `RainRenderer` component
   - `RainEffectController` component
   - `RainEffectSetup` component (optional, for easy setup)

3. Configure the `RainRenderer`:
   - Assign foreground and background textures
   - Adjust parallax, refraction, and brightness settings

4. Configure the `Raindrops`:
   - Assign drop alpha and color textures
   - Adjust rain parameters (rain chance, drop size, etc.)

## Controls

- **Mouse Movement**: Creates parallax effect (move mouse to see the effect)
- **Number Keys (1-5)**: Switch between weather presets
- **Arrow Keys (Left/Right)**: Cycle through weather presets

## Components

### Raindrops
Manages the rain drop simulation and rendering to a texture.

**Properties:**
- `Drop Alpha Texture`: Texture for drop transparency
- `Drop Color Texture`: Texture for drop color/shape
- `Options`: Configure rain behavior (spawn rate, size, physics, etc.)

### RainRenderer
Applies the water refraction effect as a camera post-processing effect.

**Properties:**
- `Texture Foreground`: Foreground texture to display through water
- `Texture Background`: Background texture to display
- `Texture Shine`: Optional shine/highlight texture
- `Parallax Settings`: Control the parallax depth effect
- `Refraction Settings`: Control water refraction strength
- `Rendering Settings`: Brightness, alpha, and visual quality

### RainEffectController
Handles user input for the parallax effect.

**Properties:**
- `Enable Parallax`: Toggle parallax effect on/off
- `Parallax Smooth Time`: How quickly parallax responds to input

### WeatherSystem
Manages multiple weather presets and transitions between them.

**Properties:**
- `Weather Presets`: Array of weather configurations
- `Transition Duration`: Time to blend between weather states

### RainEffectSetup
Helper component that automatically configures all components with textures.

## Customization

### Creating Custom Weather Presets

1. Prepare two textures (foreground and background)
2. Add a new preset in the `WeatherSystem` component:
   - Name: Display name for the preset
   - Foreground/Background Textures: Your textures
   - Raindrops Options: Configure rain behavior

### Adjusting Visual Quality

**For better performance:**
- Reduce `Max Drops` in Raindrops options
- Lower texture resolutions
- Disable `Render Shine` and `Render Shadow`

**For higher quality:**
- Increase `Max Drops` for more rain
- Use higher resolution textures
- Enable `Render Shine` for highlights
- Increase `Alpha Multiply` for more visible drops

### Shader Customization

The rain effect shader is located at:
`Assets/Shaders/RainWaterEffect.shader`

You can modify it to:
- Change refraction behavior
- Add custom visual effects
- Optimize for specific platforms

## Technical Details

### Rendering Pipeline

1. `Raindrops` component simulates rain physics and renders drops to a RenderTexture
2. `RainRenderer` applies the water refraction shader as a post-processing effect
3. The shader uses the raindrops texture to distort and refract the scene

### Shader Implementation

The shader is written in Unity's ShaderLab/HLSL format and includes:
- Water refraction calculation
- Parallax scrolling for depth
- Alpha blending for transparency
- Optional shine and shadow effects

### Performance Considerations

- Rain simulation runs on CPU (may limit maximum drop count on mobile)
- Shader runs on GPU (generally performant on modern hardware)
- Render texture size matches screen resolution
- Consider reducing resolution on lower-end devices

## Troubleshooting

### Black Screen or No Effect
- Ensure textures are assigned to the RainRenderer component
- Check that the shader compiled successfully (no errors in Console)
- Verify the camera has the RainRenderer component

### Poor Performance
- Reduce `Max Drops` in Raindrops options
- Lower screen resolution
- Disable shine and shadow effects
- Use lower resolution textures

### Textures Not Appearing
- Check texture import settings (ensure they're set to Texture2D)
- Verify textures are not compressed excessively
- Make sure textures are assigned in the Inspector

## Credits

Original WebGL effect by Lucas Bebber for Codrops:
- [Codrops Article](http://tympanus.net/codrops/?p=25417)
- [Original Demo](http://tympanus.net/Development/RainEffect/)

Unity port: Converted from WebGL to Unity-compatible format

## License

This project maintains the same license as the original:
Integrate or build upon it for free in your personal or commercial projects. Don't republish, redistribute or sell "as-is".

Read more: [Codrops License](http://tympanus.net/codrops/licensing/)

## Changelog

### Version 1.0.0
- Initial Unity port from WebGL version
- GLSL to HLSL shader conversion
- JavaScript to C# script conversion
- Added Unity-specific features (Inspector integration, component system)
- Multiple weather preset system
- Mouse/touch parallax control
- Demo scene included

## Support

For issues related to the Unity port, please refer to the repository issues page.
For questions about the original effect, see the Codrops article linked above.
