using UnityEngine;

public class HighlightLightSync : MonoBehaviour
{
    private float frequency;
    private float magnitude;
    private float offset;

    private Light lightSource;
    private Transform parentTransform;
    private Renderer parentRenderer;
    private Material parentMaterial;

    private float lightIntensity;
    private float lightRange;

    void Start()
    {
        lightSource = GetComponent<Light>();
        lightIntensity = lightSource.intensity;
        lightRange = lightSource.range;

        parentTransform = transform.parent;
        if (parentTransform != null)
        {
            parentRenderer = parentTransform.GetComponent<Renderer>();

            if (parentRenderer != null)
            {
                parentMaterial = parentRenderer.material;

                if (parentMaterial.HasProperty("_HighlightColFreq") &&
                    parentMaterial.HasProperty("_HighlightColMag") &&
                    parentMaterial.HasProperty("_HighlightSinOffset"))
                {
                    frequency = parentMaterial.GetFloat("_HighlightColFreq");
                    magnitude = parentMaterial.GetFloat("_HighlightColMag");
                    offset = parentMaterial.GetFloat("_HighlightSinOffset");
                }
                else
                {
                    Debug.LogError("Shader properties not found on Material");
                }
            }
            else
            {
                Debug.LogError("Renderer not found on parent object");
            }
        }
        else
        {
            Debug.LogError("Parent Transform not found");
        }

    }

    void Update()
    {
        PulsingBloomSync(lightIntensity, lightRange);
    }

    private void PulsingBloomSync(float intensity, float range)
    {
        float t = Time.time;

        float result = (magnitude * Mathf.Sin(t * frequency) * 0.5f + offset);

        lightSource.intensity = intensity * result;
        lightSource.range = range * result;        
    }
}
