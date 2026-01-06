using UnityEngine;

/// <summary>
/// Controls the rain effect with mouse/touch input for parallax
/// </summary>
public class RainEffectController : MonoBehaviour
{
    [Header("References")]
    public RainRenderer rainRenderer;
    
    [Header("Input Settings")]
    public bool enableParallax = true;
    public float parallaxSmoothTime = 1f;
    
    private Vector2 targetParallax;
    private Vector2 currentParallax;
    private Vector2 parallaxVelocity;
    
    private void Start()
    {
        if (rainRenderer == null)
        {
            rainRenderer = GetComponent<RainRenderer>();
        }
        
        if (rainRenderer == null)
        {
            Debug.LogWarning("RainRenderer component not found!");
        }
    }
    
    private void Update()
    {
        if (!enableParallax || rainRenderer == null)
            return;
        
        UpdateParallaxFromInput();
        SmoothParallax();
    }
    
    private void UpdateParallaxFromInput()
    {
        Vector2 inputPos = Vector2.zero;
        
        // Handle mouse input
        if (Input.mousePresent)
        {
            inputPos = Input.mousePosition;
        }
        // Handle touch input
        else if (Input.touchCount > 0)
        {
            inputPos = Input.GetTouch(0).position;
        }
        else
        {
            return;
        }
        
        // Convert screen position to normalized parallax values (-1 to 1)
        float normalizedX = (inputPos.x / Screen.width) * 2f - 1f;
        float normalizedY = (inputPos.y / Screen.height) * 2f - 1f;
        
        targetParallax = new Vector2(normalizedX, normalizedY);
    }
    
    private void SmoothParallax()
    {
        // Smooth damp towards target
        currentParallax.x = Mathf.SmoothDamp(
            currentParallax.x,
            targetParallax.x,
            ref parallaxVelocity.x,
            parallaxSmoothTime
        );
        
        currentParallax.y = Mathf.SmoothDamp(
            currentParallax.y,
            targetParallax.y,
            ref parallaxVelocity.y,
            parallaxSmoothTime
        );
        
        // Apply to renderer
        rainRenderer.SetParallax(currentParallax.x, currentParallax.y);
    }
}
