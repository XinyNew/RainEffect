# Quick Start Guide

Get the rain effect running in your Unity project in 5 minutes!

## 🚀 Super Quick Start (Recommended)

### 1. Open Unity
- Unity 2021.3 LTS or higher required
- Open the `UnityRainEffect` folder as a project

### 2. Open Demo Scene
- Navigate to `Assets/Scenes/RainEffectDemo.unity`
- Double-click to open

### 3. Assign Textures
Select the Main Camera in Hierarchy, then in the Inspector:

**Drop Textures:**
- Drop Alpha: Drag `Assets/Textures/Drops/drop-alpha.png`
- Drop Color: Drag `Assets/Textures/Drops/drop-color.png`

**Rain Weather:**
- Rain Foreground: Drag `Assets/Textures/Weather/texture-rain-fg.png`
- Rain Background: Drag `Assets/Textures/Weather/texture-rain-bg.png`

**Optional - Other Weather:**
- Drizzle/Storm/Sun/Fallout textures can be assigned for multiple presets

### 4. Press Play ▶️
Done! You should see rain drops with refraction effects.

---

## 🎮 Controls

- **Mouse Movement**: Creates parallax depth effect
- **Keys 1-5**: Switch weather presets
- **Arrow Keys**: Cycle through weather presets

---

## 📦 Add to Existing Project

### Quick Method (Using Setup Script)

1. Copy these folders to your project:
   ```
   Assets/Scripts/
   Assets/Shaders/
   Assets/Textures/
   ```

2. Select your Main Camera

3. Add Component → `Rain Effect Setup`

4. Assign textures (same as step 3 above)

5. Press Play!

### Manual Method (More Control)

1. Copy folders (same as above)

2. Add components to Main Camera in this order:
   - `Raindrops`
   - `Rain Renderer`
   - `Rain Effect Controller`

3. Configure each component:

**Raindrops Component:**
```
Drop Alpha Texture: drop-alpha.png
Drop Color Texture: drop-color.png
Options → Raining: ✓ (checked)
```

**Rain Renderer Component:**
```
Texture Foreground: texture-rain-fg.png
Texture Background: texture-rain-bg.png
Raindrops Component: (drag Raindrops component)
```

**Rain Effect Controller:**
```
Rain Renderer: (drag Rain Renderer component)
Enable Parallax: ✓ (checked)
```

4. Press Play!

---

## 🔧 Common Tweaks

### Make Rain Heavier
```
Raindrops → Options:
  Rain Chance: 0.5 (higher = more rain)
  Rain Limit: 5 (more simultaneous drops)
```

### Make Rain Lighter
```
Raindrops → Options:
  Rain Chance: 0.15
  Rain Limit: 2
  Min R: 8
  Max R: 25
```

### Adjust Visual Quality
```
Rain Renderer:
  Brightness: 1.2 (brighter)
  Alpha Multiply: 10 (more visible drops)
  Alpha Subtract: 2 (more transparent)
```

### Better Performance
```
Raindrops → Options:
  Max Drops: 300 (fewer drops)
  
Rain Renderer:
  Render Shine: ✗ (unchecked)
  Render Shadow: ✗ (unchecked)
```

---

## ❓ Troubleshooting

### No Rain Visible?
- Check `Raindrops → Options → Raining` is checked
- Increase `Rain Chance` to 0.5
- Check textures are assigned

### Black Screen?
- Verify foreground/background textures are assigned
- Check Console for shader errors

### Poor Performance?
- Reduce `Max Drops` to 300
- Uncheck `Render Shine` and `Render Shadow`

### Parallax Not Working?
- Move your mouse across the screen
- Check `Enable Parallax` is checked in Rain Effect Controller

---

## 📚 Next Steps

Once you have the basic effect working:

1. **Try Different Weather Presets**
   - Add `Weather System` component
   - Configure multiple weather presets
   - Press number keys 1-5 to switch

2. **Customize the Look**
   - Adjust `Parallax Bg/Fg` for depth
   - Change `Min/Max Refraction` for distortion strength
   - Modify `Brightness` for different moods

3. **Read Full Documentation**
   - `README.md` - Complete feature guide
   - `UNITY_INTEGRATION.md` - Advanced integration
   - `CHANGELOG.md` - Version history

4. **Create Your Own Weather**
   - Use your own background images
   - Adjust rain parameters to match
   - Save as weather presets

---

## 🎨 Example Configurations

### Heavy Storm
```
Rain Chance: 0.6
Rain Limit: 6
Min R: 15
Max R: 55
Drop Fall Multiplier: 1.8
```

### Light Drizzle
```
Rain Chance: 0.12
Rain Limit: 2
Min R: 6
Max R: 20
Drop Fall Multiplier: 0.8
```

### Gentle Rain
```
Rain Chance: 0.25
Rain Limit: 3
Min R: 10
Max R: 35
Drop Fall Multiplier: 1.0
```

---

## 💡 Tips

1. **Start with default settings** - They're tuned for good quality
2. **Test on your target platform** - Mobile needs different settings than PC
3. **Use the demo scene as reference** - Copy settings from working examples
4. **Adjust one parameter at a time** - Easier to see what each does
5. **Save working configurations** - Use Weather System presets

---

## 🆘 Need Help?

- Check `README.md` for detailed information
- See `UNITY_INTEGRATION.md` for advanced topics
- Look at the demo scene for working examples
- Review original WebGL version: http://tympanus.net/codrops/?p=25417

---

**Enjoy your rain effect! 🌧️**
