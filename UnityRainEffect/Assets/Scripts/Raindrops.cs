using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages raindrop simulation and rendering
/// </summary>
public class Raindrops : MonoBehaviour
{
    [System.Serializable]
    public class Drop
    {
        public float x;
        public float y;
        public float r;
        public float spreadX;
        public float spreadY;
        public float momentum;
        public float momentumX;
        public float lastSpawn;
        public float nextSpawn;
        public Drop parent;
        public bool isNew = true;
        public bool killed = false;
        public float shrink;
    }
    
    [System.Serializable]
    public class RaindropsOptions
    {
        public float minR = 10f;
        public float maxR = 40f;
        public int maxDrops = 900;
        public float rainChance = 0.3f;
        public int rainLimit = 3;
        public int dropletsRate = 50;
        public Vector2 dropletsSize = new Vector2(2f, 4f);
        public float dropletsCleaningRadiusMultiplier = 0.43f;
        public bool raining = true;
        public float globalTimeScale = 1f;
        public float trailRate = 1f;
        public bool autoShrink = true;
        public Vector2 spawnArea = new Vector2(-0.1f, 0.95f);
        public Vector2 trailScaleRange = new Vector2(0.2f, 0.5f);
        public float collisionRadius = 0.65f;
        public float collisionRadiusIncrease = 0.01f;
        public float dropFallMultiplier = 1f;
        public float collisionBoostMultiplier = 0.05f;
        public float collisionBoost = 1f;
    }
    
    public RenderTexture raindropsTexture;
    public Texture2D dropAlphaTexture;
    public Texture2D dropColorTexture;
    public RaindropsOptions options = new RaindropsOptions();
    
    private int width;
    private int height;
    private float scale;
    private RenderTexture dropletsTexture;
    private List<Drop> drops = new List<Drop>();
    private List<Texture2D> dropsGfx = new List<Texture2D>();
    private int dropSize = 64;
    private float dropletsPixelDensity = 1f;
    private int dropletsCounter = 0;
    private float lastRender;
    
    private void Start()
    {
        width = Screen.width;
        height = Screen.height;
        scale = 1f;
        
        InitializeTextures();
        RenderDropsGfx();
    }
    
    private void InitializeTextures()
    {
        // Create render textures
        raindropsTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        raindropsTexture.filterMode = FilterMode.Bilinear;
        raindropsTexture.Create();
        
        dropletsTexture = new RenderTexture(
            Mathf.RoundToInt(width * dropletsPixelDensity),
            Mathf.RoundToInt(height * dropletsPixelDensity),
            0,
            RenderTextureFormat.ARGB32
        );
        dropletsTexture.filterMode = FilterMode.Bilinear;
        dropletsTexture.Create();
        
        // Clear textures
        RenderTexture.active = raindropsTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = dropletsTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }
    
    private void RenderDropsGfx()
    {
        int dropBuffer = 1;
        
        for (int i = 0; i < options.maxR; i++)
        {
            float r = Mathf.Lerp(options.minR, options.maxR, (float)i / options.maxR);
            Texture2D canvas = new Texture2D(dropSize, dropSize, TextureFormat.ARGB32, false);
            Color[] pixels = new Color[dropSize * dropSize];
            
            for (int y = 0; y < dropSize; y++)
            {
                for (int x = 0; x < dropSize; x++)
                {
                    pixels[y * dropSize + x] = Color.clear;
                }
            }
            
            canvas.SetPixels(pixels);
            canvas.Apply();
            dropsGfx.Add(canvas);
        }
    }
    
    private void Update()
    {
        float deltaTime = Time.deltaTime * options.globalTimeScale;
        UpdateDrops(deltaTime);
        RenderDroplets(deltaTime);
        Render();
    }
    
    private void UpdateDrops(float deltaTime)
    {
        // Spawn new drops
        if (options.raining)
        {
            int rainDropsCount = 0;
            for (int i = 0; i < drops.Count && rainDropsCount < options.rainLimit; i++)
            {
                if (drops[i].y > 0 && drops[i].y < options.spawnArea.y)
                {
                    rainDropsCount++;
                }
            }
            
            if (rainDropsCount < options.rainLimit && Random.value < options.rainChance)
            {
                SpawnDrop();
            }
        }
        
        // Update existing drops
        for (int i = drops.Count - 1; i >= 0; i--)
        {
            Drop drop = drops[i];
            
            if (drop.killed)
            {
                drops.RemoveAt(i);
                continue;
            }
            
            // Simple gravity physics
            drop.momentum += deltaTime * 0.05f * options.dropFallMultiplier;
            drop.y += drop.momentum;
            
            // Remove drops that fall off screen
            if (drop.y > height + drop.r)
            {
                drops.RemoveAt(i);
            }
        }
    }
    
    private void SpawnDrop()
    {
        Drop drop = new Drop
        {
            x = Random.Range(0, width),
            y = Random.Range(options.spawnArea.x * height, options.spawnArea.y * height),
            r = Random.Range(options.minR, options.maxR),
            momentum = Random.Range(1f, 3f),
            spreadX = Random.Range(-0.5f, 0.5f),
            spreadY = Random.Range(-0.5f, 0.5f)
        };
        
        drops.Add(drop);
    }
    
    private void RenderDroplets(float deltaTime)
    {
        // Simplified droplets rendering
        dropletsCounter += deltaTime * 1000f;
    }
    
    private void Render()
    {
        RenderTexture.active = raindropsTexture;
        
        // Clear with transparent
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        
        // Render drops
        foreach (Drop drop in drops)
        {
            DrawDrop(drop);
        }
        
        RenderTexture.active = null;
    }
    
    private void DrawDrop(Drop drop)
    {
        // Simplified drop rendering
        float normalizedR = Mathf.InverseLerp(options.minR, options.maxR, drop.r);
        
        // Create simple circle texture on GPU
        Graphics.DrawTexture(
            new Rect(drop.x - drop.r, drop.y - drop.r, drop.r * 2, drop.r * 2),
            dropAlphaTexture
        );
    }
    
    private void OnDestroy()
    {
        if (raindropsTexture != null)
        {
            raindropsTexture.Release();
            Destroy(raindropsTexture);
        }
        
        if (dropletsTexture != null)
        {
            dropletsTexture.Release();
            Destroy(dropletsTexture);
        }
    }
    
    public float GetDeltaR()
    {
        return options.maxR - options.minR;
    }
    
    public float GetArea()
    {
        return (width * height) / scale;
    }
    
    public float GetAreaMultiplier()
    {
        return Mathf.Sqrt(GetArea() / (1024f * 768f));
    }
}
