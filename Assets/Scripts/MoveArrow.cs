using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArrow : MonoBehaviour {
    private Material arrow;
    private GameObject circle;
    private Material circleMat;

    [Header("Settings")] 
    [SerializeField] private Color _color;
    [SerializeField] private float _startingCircleSize;
    [SerializeField] private float _circleTweenDuration;
    [SerializeField] private float _arrowTweenStartDelay;
    [SerializeField] private float _arrowTweenDuration;

    private Vector2 _arrowStart = new Vector2(0f, -1f);
    private Vector2 _arrowEnd = new Vector2(0f, 1f);

    private Coroutine _arrowTween;
    private Coroutine _circleTween;

    private void Awake() {
        arrow = GetComponent<Renderer>().material;
        circle = transform.GetChild(0).gameObject;
        circleMat = circle.GetComponent<Renderer>().material;
    }

    public void Play(Vector3 _postion, Vector3 _up) {

        if (_arrowTween != null) {
            StopCoroutine(_arrowTween);
        }
        
        if (_circleTween != null) {
            StopCoroutine(_circleTween);
        }

        //ApplyColorToMaterials();
        
        transform.position = _postion;
        transform.up = _up;
        
        _circleTween = StartCoroutine(CircleAnimation());
        _arrowTween = StartCoroutine(ArrowAnimation());
    }
    
    private void ApplyColorToMaterials() {
        if (arrow != null) {
            arrow.SetColor("_BaseColor", _color);
        }
        if (circleMat != null) {
            circleMat.SetColor("_BaseColor", _color);
        }
    }

    IEnumerator CircleAnimation() {
        float elapsedTime = 0f;
        circle.transform.localScale = Vector3.one * _startingCircleSize;
        Vector3 initialScale = circle.transform.localScale;

        while (elapsedTime < _circleTweenDuration) {
            circle.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsedTime / _circleTweenDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        circle.transform.localScale = Vector3.zero;
    }
    
    IEnumerator ArrowAnimation() {
        yield return new WaitForSeconds(_arrowTweenStartDelay);

        float elapsedTime = 0f;
        arrow.SetTextureOffset("_BaseMap", _arrowStart);

        while (elapsedTime < _arrowTweenDuration)
        {
            Vector2 newOffset = Vector2.Lerp(_arrowStart, _arrowEnd, elapsedTime / _arrowTweenDuration);
            arrow.SetTextureOffset("_BaseMap", newOffset);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        arrow.SetTextureOffset("_BaseMap", _arrowEnd);
    }

    private void OnDisable() {
        StopAllCoroutines();
        Reset();
    }

    private void Reset() {
        circle.transform.localScale = Vector3.zero;
        arrow.SetTextureOffset("_BaseMap", new Vector2(0f, -1f));
    }
}
