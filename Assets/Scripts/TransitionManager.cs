using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour {
    public static TransitionManager Instance;
    [SerializeField] private Animator _anim;

    private Coroutine transitionCoroutine;
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void TransitionToScene(string _scene) {
        if (transitionCoroutine != null) {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(ChangeSceneAfterTransition(_scene));
    }

    IEnumerator ChangeSceneAfterTransition(string _sceneName) {
        _anim.Play("TransitionOn");
        yield return new WaitUntil(() => IsAnimationFinished(_anim, "TransitionOn"));
        yield return SceneManager.LoadSceneAsync(_sceneName);
        yield return new WaitForSeconds(1f);
        _anim.Play("TransitionOff");
    }

    private bool IsAnimationFinished(Animator animator, string animationName) {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1;
    }
}
