using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour
{
    public Light targetLight;
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.2f;
    public float minLightDuration = 0.5f;
    public float maxLightDuration = 1.5f;
    private bool isFlickering = false;

    void OnEnable()
    {
        // Start flickering when activated
        if (!isFlickering)
        {
            StartCoroutine(FlickerRoutine());
        }
    }

    void OnDisable()
    {
        // Stop the flicker when deactivated
        StopAllCoroutines();
        isFlickering = false;
        SetLightState(true); 
    }

    private void SetLightState(bool state)
    {
        if (targetLight != null)
        {
            targetLight.enabled = state;
        }
    }

    private IEnumerator FlickerRoutine()
    {
        isFlickering = true;
        
        while (true)
        {
            // Light On Phase
            SetLightState(true);
            float lightDuration = Random.Range(minLightDuration, maxLightDuration);
            yield return new WaitForSeconds(lightDuration);

            // Light Flicker Phase
            int flickerCount = Random.Range(1, 4); 
            for (int i = 0; i < flickerCount; i++)
            {
                SetLightState(false);
                float offTime = Random.Range(minFlickerInterval, maxFlickerInterval);
                yield return new WaitForSeconds(offTime);

                SetLightState(true);
                float onTime = Random.Range(minFlickerInterval, maxFlickerInterval);
                yield return new WaitForSeconds(onTime);
            }
        }
    }
}