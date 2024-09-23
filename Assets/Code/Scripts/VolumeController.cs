using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VolumeController : MonoBehaviour
{
    private Vignette vignette;
    private Bloom bloom;
    private ChromaticAberration chromaticAberration;
    private Volume globalVolume;

    private Coroutine currentVignetteCoroutine;
    private Coroutine currentBloomCoroutine;
    private Coroutine currentChromaticAberrationCoroutine;

    public void SetGlobalVolume(Volume volume)
    {
        globalVolume = volume;

        if (globalVolume != null)
        {
            if (globalVolume.profile.TryGet<Vignette>(out vignette))
            {
                Debug.Log("Vignette trovata nel Volume Profile!");
                vignette.active = true;
            }

            if (globalVolume.profile.TryGet<Bloom>(out bloom))
            {
                Debug.Log("Bloom trovato nel Volume Profile!");
                bloom.active = true;
            }

            if (globalVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
            {
                Debug.Log("Chromatic Aberration trovata nel Volume Profile!");
                chromaticAberration.active = true;
            }
        }
    }

    // Metodo per modificare la Vignette
    public void ModifyVignette(Color newColor, float newIntensity, float transitionInTime, float duration, float transitionOutTime)
    {
        if (vignette != null)
        {
            if (currentVignetteCoroutine != null) StopCoroutine(currentVignetteCoroutine);
            currentVignetteCoroutine = StartCoroutine(ApplyVignetteEffect(newColor, newIntensity, transitionInTime, duration, transitionOutTime));
        }
    }

    private IEnumerator ApplyVignetteEffect(Color newColor, float newIntensity, float transitionInTime, float duration, float transitionOutTime)
    {
        Color originalColor = vignette.color.value;
        float originalIntensity = vignette.intensity.value;

        yield return StartCoroutine(TransitionVignette(originalColor, newColor, originalIntensity, newIntensity, transitionInTime));

        yield return new WaitForSeconds(duration);

        yield return StartCoroutine(TransitionVignette(newColor, originalColor, newIntensity, originalIntensity, transitionOutTime));

        currentVignetteCoroutine = null;
    }

    private IEnumerator TransitionVignette(Color fromColor, Color toColor, float fromIntensity, float toIntensity, float transitionTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            vignette.color.value = Color.Lerp(fromColor, toColor, elapsedTime / transitionTime);
            vignette.intensity.value = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / transitionTime);
            yield return null;
        }
        vignette.color.value = toColor;
        vignette.intensity.value = toIntensity;
    }

    // Metodo per modificare il Bloom
    public void ModifyBloom(float newIntensity, float transitionInTime, float duration, float transitionOutTime)
    {
        if (bloom != null)
        {
            if (currentBloomCoroutine != null) StopCoroutine(currentBloomCoroutine);
            currentBloomCoroutine = StartCoroutine(ApplyBloomEffect(newIntensity, transitionInTime, duration, transitionOutTime));
        }
    }

    private IEnumerator ApplyBloomEffect(float newIntensity, float transitionInTime, float duration, float transitionOutTime)
    {
        float originalIntensity = bloom.intensity.value;

        yield return StartCoroutine(TransitionBloom(originalIntensity, newIntensity, transitionInTime));

        yield return new WaitForSeconds(duration);

        yield return StartCoroutine(TransitionBloom(newIntensity, originalIntensity, transitionOutTime));

        currentBloomCoroutine = null;
    }

    private IEnumerator TransitionBloom(float fromIntensity, float toIntensity, float transitionTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            bloom.intensity.value = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / transitionTime);
            yield return null;
        }
        bloom.intensity.value = toIntensity;
    }

    // Metodo per modificare Chromatic Aberration
    public void ModifyChromaticAberration(float newIntensity, float transitionInTime, float duration, float transitionOutTime)
    {
        if (chromaticAberration != null)
        {
            if (currentChromaticAberrationCoroutine != null) StopCoroutine(currentChromaticAberrationCoroutine);
            currentChromaticAberrationCoroutine = StartCoroutine(ApplyChromaticAberrationEffect(newIntensity, transitionInTime, duration, transitionOutTime));
        }
    }

    private IEnumerator ApplyChromaticAberrationEffect(float newIntensity, float transitionInTime, float duration, float transitionOutTime)
    {
        float originalIntensity = chromaticAberration.intensity.value;

        yield return StartCoroutine(TransitionChromaticAberration(originalIntensity, newIntensity, transitionInTime));

        yield return new WaitForSeconds(duration);

        yield return StartCoroutine(TransitionChromaticAberration(newIntensity, originalIntensity, transitionOutTime));

        currentChromaticAberrationCoroutine = null;
    }

    private IEnumerator TransitionChromaticAberration(float fromIntensity, float toIntensity, float transitionTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            chromaticAberration.intensity.value = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / transitionTime);
            yield return null;
        }
        chromaticAberration.intensity.value = toIntensity;
    }
}
