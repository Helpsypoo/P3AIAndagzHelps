using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
    [FormerlySerializedAs("fill")] [SerializeField] private Image healthFill;
    [SerializeField] private Image chaseFill;
    private CanvasGroup canvasGroup;
    private Coroutine updateDisplay;

    private float fadeOutDuration = 1f;
    private float fillAdjustDuration = .3f;
    private float chaseFillAdjustDuration = .5f;
    private float displayDuration = .5f;
    private float chaseFillAdjustDelay = 1f;
    
    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start() {
        canvasGroup.alpha = 0;
    }

    public void HiddenHealthDisplayUpdate(float _health, float _maxHealth) {
        float _pct = Mathf.Clamp01(_health / _maxHealth);
        healthFill.fillAmount = _pct;
        chaseFill.fillAmount = _pct;
    }
    
    public void UpdateHealthDisplay(float _health, float _maxHealth) {
        canvasGroup.alpha = 1f;
        
        float targetFillAmount = Mathf.Clamp01(_health / _maxHealth);
        
        healthFill.fillAmount = targetFillAmount;
        
        if (updateDisplay != null) {
            StopCoroutine(updateDisplay);
        }

        updateDisplay = StartCoroutine(UpdateDisplayCoroutine(targetFillAmount));
    }

    public void ForceHealthDisplay(bool _enable) {
        if (updateDisplay != null) {
            StopCoroutine(updateDisplay);
        }
        canvasGroup.alpha = _enable ? 1f : 0f;
    }

    IEnumerator UpdateDisplayCoroutine(float _healthPct) {
        yield return new WaitForSeconds(chaseFillAdjustDelay);

        // Smoothly change chaseFill.fillAmount
        float elapsedTime = 0f;
        float initialChaseFillAmount = chaseFill.fillAmount;

        while (elapsedTime < chaseFillAdjustDuration) {
            chaseFill.fillAmount = Mathf.Lerp(initialChaseFillAmount, _healthPct, elapsedTime / chaseFillAdjustDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        chaseFill.fillAmount = _healthPct;

        // Wait before starting the fade out
        yield return new WaitForSeconds(displayDuration);
        // Smoothly change canvasGroup.alpha to 0 over fadeOutDuration
        elapsedTime = 0f;
        float initialAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeOutDuration) {
            canvasGroup.alpha = Mathf.Lerp(initialAlpha, 0f, elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
