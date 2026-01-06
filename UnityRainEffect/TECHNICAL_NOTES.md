# Technical Conversion Notes

This document provides technical details about the conversion from WebGL to Unity.

## Overview

The original rain effect was built using:
- **Rendering**: WebGL with GLSL shaders
- **Logic**: JavaScript ES6
- **Graphics API**: Canvas 2D for drops, WebGL for refraction
- **Build System**: Gulp + Browserify + Babel

The Unity port uses:
- **Rendering**: Unity render pipeline with HLSL shaders
- **Logic**: C# MonoBehaviour components
- **Graphics API**: Unity Graphics class and RenderTextures
- **Build System**: Unity's built-in build system

## Shader Conversion Details

### Vertex Shader (simple.vert → RainWaterEffect.shader)

**Original GLSL:**
```glsl
precision mediump float;
attribute vec2 a_position;

void main() {
   gl_Position = vec4(a_position, 0.0, 1.0);
}
```

**Unity HLSL:**
```hlsl
struct appdata {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f {
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float4 screenPos : TEXCOORD1;
};

v2f vert (appdata v) {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.screenPos = ComputeScreenPos(o.vertex);
    return o;
}
```

**Key Changes:**
- `attribute` → struct members with semantic annotations
- `gl_Position` → returned struct with `SV_POSITION`
- Added Unity helper functions: `UnityObjectToClipPos`, `ComputeScreenPos`
- Added screen position calculation for fragment shader

### Fragment Shader (water.frag → RainWaterEffect.shader)

**Coordinate System:**
- GLSL: `gl_FragCoord` (bottom-left origin)
- Unity: `screenPos` from vertex shader (top-left origin)
- Solution: Flip Y coordinate in texCoord calculation

**Texture Sampling:**
- GLSL: `texture2D(sampler, coords)`
- Unity: `tex2D(sampler, coords)`
- Both use same UV coordinates (0-1 range)

**Uniforms:**
- GLSL: `uniform type name;`
- Unity: Properties block + shader variables
```hlsl
Properties {
    _WaterMap ("Water Map", 2D) = "white" {}
}
// In CGPROGRAM:
sampler2D _WaterMap;
```

**Built-in Variables:**
- GLSL: `gl_FragCoord` for pixel position
- Unity: Custom calculation using `screenPos`

**Precision Qualifiers:**
- GLSL: `precision mediump float;`
- Unity: Default precision is sufficient, removed

**Output:**
- GLSL: `gl_FragColor = color;`
- Unity: `return color;` with `SV_Target` semantic

## Script Conversion Details

### Raindrops.js → Raindrops.cs

**Class Structure:**
```javascript
// JavaScript
function Raindrops(width, height, scale, dropAlpha, dropColor, options) {
    this.width = width;
    this.height = height;
    // ...
}
Raindrops.prototype = {
    init() { /* ... */ },
    update() { /* ... */ }
}
```

```csharp
// C#
public class Raindrops : MonoBehaviour {
    private int width;
    private int height;
    
    private void Start() { /* init */ }
    private void Update() { /* update */ }
}
```

**Canvas 2D → RenderTexture:**
```javascript
// JavaScript
this.canvas = createCanvas(width, height);
this.ctx = this.canvas.getContext('2d');
```

```csharp
// C#
raindropsTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
RenderTexture.active = raindropsTexture;
// Draw using Graphics class
```

**Animation Loop:**
```javascript
// JavaScript - runs continuously via RAF
function update() {
    // update logic
    requestAnimationFrame(update);
}
```

```csharp
// C# - Unity's Update is called automatically
void Update() {
    // update logic
}
```

### rain-renderer.js → RainRenderer.cs

**WebGL Context → Unity Material:**
```javascript
// JavaScript
this.gl = new GL(canvas, {alpha: false}, vertShader, fragShader);
this.gl.createUniform("2f", "resolution", width, height);
```

```csharp
// C#
rainMaterial = new Material(rainShader);
rainMaterial.SetVector("_Resolution", new Vector2(width, height));
```

**Render Loop:**
```javascript
// JavaScript
draw() {
    this.gl.useProgram(this.programWater);
    this.gl.draw();
    requestAnimationFrame(this.draw.bind(this));
}
```

```csharp
// C# - Post-processing effect
void OnRenderImage(RenderTexture source, RenderTexture destination) {
    Graphics.Blit(source, destination, rainMaterial);
}
```

**Texture Updates:**
```javascript
// JavaScript
this.gl.updateTexture(this.canvasLiquid);
```

```csharp
// C#
rainMaterial.SetTexture("_WaterMap", raindropsComponent.raindropsTexture);
```

### Event Handling

**Mouse Input:**
```javascript
// JavaScript
document.addEventListener('mousemove', (event) => {
    let x = event.pageX;
    let y = event.pageY;
    // update parallax
});
```

```csharp
// C#
void Update() {
    Vector2 mousePos = Input.mousePosition;
    float x = mousePos.x;
    float y = mousePos.y;
    // update parallax
}
```

## Architecture Differences

### Original WebGL Architecture
```
index.html
  └─ index.js (main logic)
      ├─ rain-renderer.js (WebGL rendering)
      │   ├─ gl-obj.js (WebGL wrapper)
      │   ├─ webgl.js (WebGL utilities)
      │   └─ shaders/ (GLSL)
      ├─ raindrops.js (drop simulation)
      ├─ image-loader.js (async loading)
      └─ utilities (random, times, etc.)
```

### Unity Architecture
```
Unity Scene
  └─ Main Camera (GameObject)
      ├─ Raindrops (Component)
      ├─ RainRenderer (Component)
      ├─ RainEffectController (Component)
      ├─ WeatherSystem (Component)
      └─ RainEffectSetup (Component)

Assets/
  ├─ Scripts/ (C# components)
  ├─ Shaders/ (ShaderLab/HLSL)
  ├─ Textures/ (imported assets)
  └─ Scenes/ (Unity scenes)
```

## Data Flow

### Original WebGL
```
User Input → JavaScript Event Handler
    ↓
Update Parallax Values
    ↓
Raindrops Canvas Update (CPU)
    ↓
WebGL Shader Parameters Update
    ↓
WebGL Render (GPU)
    ↓
Screen Output
```

### Unity
```
User Input → Unity Input System
    ↓
RainEffectController.Update()
    ↓
RainRenderer.SetParallax()
    ↓
Raindrops.Update() → RenderTexture (GPU)
    ↓
RainRenderer.OnRenderImage() (GPU)
    ↓
Screen Output
```

## Performance Considerations

### CPU Workload
**WebGL:**
- Drop simulation: JavaScript (slower)
- Canvas drawing: 2D context (CPU-based)
- Texture upload: CPU → GPU transfer

**Unity:**
- Drop simulation: C# (faster than JS)
- RenderTexture: GPU-based
- Texture update: Already on GPU

**Result:** Unity version generally has better CPU performance.

### GPU Workload
**WebGL:**
- Single shader pass
- Basic texture sampling
- Screen-space quad

**Unity:**
- Single shader pass (OnRenderImage)
- Similar texture sampling
- Screen-space post-processing

**Result:** Similar GPU load, minor Unity overhead from post-processing stack.

### Memory Usage
**WebGL:**
- Canvas elements: ~8-16MB
- Textures: ~20MB
- JavaScript heap: variable

**Unity:**
- RenderTextures: ~8-16MB
- Textures: ~20MB
- Managed heap: ~5-10MB

**Result:** Similar memory footprint.

## Platform-Specific Adaptations

### WebGL to Unity WebGL
When building Unity project to WebGL:
- Shaders compile to WebGL-compatible code
- C# compiles to WebAssembly via IL2CPP
- Generally performs similarly to original
- May have slightly higher overhead from Unity runtime

### Mobile Platforms
Unity advantages for mobile:
- IL2CPP provides native performance
- Better memory management
- Built-in profiling tools
- Platform-specific optimizations

Adjustments needed:
- Reduce `maxDrops` count
- Lower texture resolutions
- Disable optional effects (shine, shadow)

### Desktop Platforms
Unity advantages:
- Native execution (no browser overhead)
- Better GPU driver access
- Multi-threading support
- Higher performance ceiling

## Feature Additions in Unity Port

### Not in Original
1. **WeatherSystem** - Multiple preset management
2. **RainEffectSetup** - Quick setup helper
3. **Component-based** - Unity's modular system
4. **Inspector integration** - Visual editing
5. **Serialization** - Save/load configurations
6. **Package system** - Unity Package Manager compatible

### Retained Features
1. **Rain physics** - Same drop behavior
2. **Refraction shader** - Same visual quality
3. **Parallax effect** - Same depth feel
4. **Multiple textures** - Same weather variety
5. **Real-time rendering** - Same responsiveness

### Simplified/Removed
1. **Build system** - Use Unity's built-in
2. **Module loading** - Unity handles imports
3. **Image loader** - Unity's asset system
4. **Browser compatibility** - Unity handles platforms
5. **Polyfills** - Not needed in Unity/C#

## Testing Notes

### What to Test
1. **Visual Quality**
   - Does rain look correct?
   - Is refraction working?
   - Are textures displaying properly?

2. **Performance**
   - Frame rate with max drops
   - Memory usage over time
   - Startup time

3. **Controls**
   - Mouse parallax response
   - Touch input (if applicable)
   - Keyboard weather switching

4. **Platform-Specific**
   - Windows standalone
   - Mac standalone
   - WebGL build
   - Mobile builds (iOS/Android)

### Known Limitations
1. **VR/AR** - Not tested, may need stereo rendering adjustments
2. **URP/HDRP** - Built for Built-in RP, may need shader updates
3. **Very old GPUs** - Shader Model 2.0 not supported
4. **Console platforms** - Not tested on PS/Xbox/Switch

## Future Improvement Opportunities

### Optimization
- Compute shader for drop simulation (GPU acceleration)
- Instanced rendering for drops
- LOD system for distance-based quality
- Occlusion culling for drops

### Features
- Wind effects on rain direction
- Drop splatter effects
- Wiper blade simulation
- Dynamic weather transitions with blending
- Sound effects integration
- Reflection probe support

### Compatibility
- URP shader variant
- HDRP shader variant
- Mobile-optimized shaders
- VR stereo rendering support
- Shader Graph version

## Lessons Learned

1. **Coordinate Systems Matter** - WebGL and Unity have different origins (bottom-left vs top-left)
2. **Shader Semantics Critical** - Proper use of `SV_POSITION`, `TEXCOORD`, etc.
3. **Component Modularity** - Unity's component system naturally separates concerns
4. **Inspector Integration** - Making properties editable greatly improves usability
5. **Platform Abstraction** - Unity handles many cross-platform details automatically

## References

- Original WebGL source code
- Unity shader documentation
- Unity C# scripting reference
- WebGL to Unity conversion guides
- GLSL to HLSL conversion tables
