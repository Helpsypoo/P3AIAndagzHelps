using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour {
    public static TransitionManager Instance;
    [SerializeField] private Transform _transitionBarContainer;

    private Tween _transitionFX;
    private Coroutine _transitionCoroutine;

    private RectTransform _canvas;
    private List<RectTransform> _transitionBars = new List<RectTransform>();

    public bool Transitioning { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _canvas = GetComponent<RectTransform>();

        foreach (RectTransform _child in _transitionBarContainer) {
            _transitionBars.Add(_child);
            //Debug.Log($"found {_transitionBars.Count}");
        }
    }

    public void TransitionToScene(string _scene) {
        if (_transitionCoroutine != null) {
            StopCoroutine(_transitionCoroutine);
        }

        _transitionCoroutine = StartCoroutine(ChangeSceneAfterTransition(_scene));
    }

    IEnumerator ChangeSceneAfterTransition(string _sceneName) {
        Transitioning = true;
        TransitionOnEffect();
        yield return new WaitUntil(() => !_transitionFX.IsActive());
        yield return SceneManager.LoadSceneAsync(_sceneName);
        yield return new WaitForSeconds(1f);
        TransitionOffEffect();
        // Dirty hack.
        yield return new WaitForSeconds(2f);
        Transitioning = false;
    }
    
    private void TransitionOnEffect() {

        
        float _delay = .3f;
        
        if (_transitionFX != null) {
            _transitionFX.Kill();
        }
        
        float initialWidth = 0f;
        float targetWidth = _canvas.rect.width;
        
        AudioManager.Instance.Play(AudioManager.Instance.TransitionOn, MixerGroups.SFX, default, .25f);
        
        foreach (RectTransform bar in _transitionBars) {
            bar.sizeDelta = new Vector2(initialWidth, bar.sizeDelta.y);
            
            _transitionFX = bar.DOSizeDelta(new Vector2(targetWidth, bar.sizeDelta.y), _delay)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);

            _delay += 0.2f;
        }
    }
    
    private void TransitionOffEffect() {
        float _delay = .3f;
        
        if (_transitionFX != null) {
            _transitionFX.Kill();
        }
    
        float initialWidth = _canvas.rect.width;
        float targetWidth = 0;
        
        AudioManager.Instance.Play(AudioManager.Instance.TransitionOn, MixerGroups.SFX, default, .2f);
        
        foreach (RectTransform bar in _transitionBars) {
            bar.sizeDelta = new Vector2(initialWidth, bar.sizeDelta.y);
        
            _transitionFX = bar.DOSizeDelta(new Vector2(targetWidth, bar.sizeDelta.y), _delay)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);

            _delay += 0.2f;
        }
    }
}
