using UnityEngine;

/// <summary>
/// Renders the rain water effect using the custom shader
/// </summary>
[RequireComponent(typeof(Camera))]
public class RainRenderer : MonoBehaviour
{
    [Header("Textures")]
    public Texture2D textureForeground;
    public Texture2D textureBackground;
    public Texture2D textureShine;
    public RenderTexture waterMapTexture;
    
    [Header("Parallax Settings")]
    [Range(-1f, 1f)]
    public float parallaxX = 0f;
    [Range(-1f, 1f)]
    public float parallaxY = 0f;
    public float parallaxBg = 5f;
    public float parallaxFg = 20f;
    
    [Header("Refraction Settings")]
    public float minRefraction = 256f;
    public float maxRefraction = 512f;
    
    [Header("Rendering Settings")]
    public float brightness = 1f;
    public float alphaMultiply = 20f;
    public float alphaSubtract = 5f;
    public bool renderShine = false;
    public bool renderShadow = false;
    
    [Header("References")]
    public Raindrops raindropsComponent;
    
    private Material rainMaterial;
    private Shader rainShader;
    
    private void Start()
    {
        InitializeMaterial();
    }
    
    private void InitializeMaterial()
    {
        // Load the shader
        rainShader = Shader.Find("Custom/RainWaterEffect");
        if (rainShader == null)
        {
            Debug.LogError("RainWaterEffect shader not found!");
            return;
        }
        
        // Create material
        rainMaterial = new Material(rainShader);
        
        // Set textures
        if (textureForeground != null)
            rainMaterial.SetTexture("_TextureFg", textureForeground);
        if (textureBackground != null)
            rainMaterial.SetTexture("_TextureBg", textureBackground);
        if (textureShine != null)
            rainMaterial.SetTexture("_TextureShine", textureShine);
        
        // Set initial parameters
        UpdateMaterialProperties();
    }
    
    private void Update()
    {
        if (rainMaterial == null)
            return;
            
        // Update water map from raindrops component
        if (raindropsComponent != null && raindropsComponent.raindropsTexture != null)
        {
            rainMaterial.SetTexture("_WaterMap", raindropsComponent.raindropsTexture);
        }
        else if (waterMapTexture != null)
        {
            rainMaterial.SetTexture("_WaterMap", waterMapTexture);
        }
        
        UpdateMaterialProperties();
    }
    
    private void UpdateMaterialProperties()
    {
        if (rainMaterial == null)
            return;
        
        // Update parallax
        rainMaterial.SetFloat("_ParallaxX", parallaxX);
        rainMaterial.SetFloat("_ParallaxY", parallaxY);
        rainMaterial.SetFloat("_ParallaxBg", parallaxBg);
        rainMaterial.SetFloat("_ParallaxFg", parallaxFg);
        
        // Update refraction
        rainMaterial.SetFloat("_MinRefraction", minRefraction);
        rainMaterial.SetFloat("_RefractionDelta", maxRefraction - minRefraction);
        
        // Update rendering settings
        rainMaterial.SetFloat("_Brightness", brightness);
        rainMaterial.SetFloat("_AlphaMultiply", alphaMultiply);
        rainMaterial.SetFloat("_AlphaSubtract", alphaSubtract);
        rainMaterial.SetFloat("_RenderShine", renderShine ? 1f : 0f);
        rainMaterial.SetFloat("_RenderShadow", renderShadow ? 1f : 0f);
        
        // Update texture ratio
        if (textureBackground != null)
        {
            float textureRatio = (float)textureBackground.width / textureBackground.height;
            rainMaterial.SetTexture("_TextureBg", textureBackground);
            rainMaterial.SetFloat("_TextureRatio", textureRatio);
        }
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (rainMaterial != null)
        {
            // Apply the rain effect as a post-processing effect
            Graphics.Blit(source, destination, rainMaterial);
        }
        else
        {
            // Pass through if material is not ready
            Graphics.Blit(source, destination);
        }
    }
    
    private void OnDestroy()
    {
        if (rainMaterial != null)
        {
            Destroy(rainMaterial);
        }
    }
    
    // Public method to update parallax from input
    public void SetParallax(float x, float y)
    {
        parallaxX = Mathf.Clamp(x, -1f, 1f);
        parallaxY = Mathf.Clamp(y, -1f, 1f);
    }
    
    // Public method to update textures at runtime
    public void SetForegroundTexture(Texture2D texture)
    {
        textureForeground = texture;
        if (rainMaterial != null && texture != null)
        {
            rainMaterial.SetTexture("_TextureFg", texture);
        }
    }
    
    public void SetBackgroundTexture(Texture2D texture)
    {
        textureBackground = texture;
        if (rainMaterial != null && texture != null)
        {
            rainMaterial.SetTexture("_TextureBg", texture);
            float textureRatio = (float)texture.width / texture.height;
            rainMaterial.SetFloat("_TextureRatio", textureRatio);
        }
    }
}
