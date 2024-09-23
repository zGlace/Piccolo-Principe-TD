using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignetteController : MonoBehaviour
{
    private Vignette vignette;
    private Volume globalVolume;
    private Coroutine currentTransitionCoroutine;

    public void SetGlobalVolume(Volume volume)
    {
        globalVolume = volume;

        if (globalVolume != null && globalVolume.profile.TryGet<Vignette>(out vignette))
        {
            Debug.Log("Vignette trovata nel Volume Profile!");
            vignette.active = true;
        }
        else
        {
            Debug.LogError("Vignette non trovata nel Volume Profile!");
        }
    }

    public void ModifyVignette(Color newColor, float newIntensity, float transitionInTime, float duration, float transitionOutTime)
    {
        if (vignette != null)
        {
            Debug.Log("Modificando la vignette...");

            // Annulla la transizione precedente, se esiste
            if (currentTransitionCoroutine != null)
            {
                StopCoroutine(currentTransitionCoroutine);
            }

            // Avvia una nuova coroutine
            currentTransitionCoroutine = StartCoroutine(ApplyVignetteEffect(newColor, newIntensity, transitionInTime, duration, transitionOutTime));
        }
        else
        {
            Debug.LogError("Vignette non assegnata correttamente!");
        }
    }

    private IEnumerator ApplyVignetteEffect(Color newColor, float newIntensity, float transitionInTime, float duration, float transitionOutTime)
    {
        Color originalColor = vignette.color.value;
        float originalIntensity = vignette.intensity.value;

        // Transizione in entrata verso il nuovo stato
        yield return StartCoroutine(TransitionVignette(originalColor, newColor, originalIntensity, newIntensity, transitionInTime));

        // Attendi il tempo specificato con la nuova impostazione
        yield return new WaitForSeconds(duration);

        // Transizione in uscita tornando ai valori originali
        yield return StartCoroutine(TransitionVignette(newColor, originalColor, newIntensity, originalIntensity, transitionOutTime));

        // Reset della coroutine attuale
        currentTransitionCoroutine = null;
    }

    private IEnumerator TransitionVignette(Color fromColor, Color toColor, float fromIntensity, float toIntensity, float transitionTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;

            // Interpoliamo il colore e l'intensità della vignette
            vignette.color.value = Color.Lerp(fromColor, toColor, elapsedTime / transitionTime);
            vignette.intensity.value = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / transitionTime);

            yield return null; // Aspettiamo un frame
        }

        // Assicuriamoci che i valori finali siano correttamente impostati
        vignette.color.value = toColor;
        vignette.intensity.value = toIntensity;
    }
}
