using UnityEngine;

public class SessionManager : MonoBehaviour {
    public static SessionManager Instance;
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void NewGame() {
        TransitionManager.Instance.TransitionToScene("Game");
    }
}
