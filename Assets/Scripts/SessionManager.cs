using UnityEngine;

public class SessionManager : MonoBehaviour {
    public static SessionManager Instance;

    public int BlueGoop;
    public int MaxBlueGoop = 1000;
    public int OrangeGoop;
    public int MaxOrangeGoop = 1000;

    
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
    
    public void ResumeGame() {
        TransitionManager.Instance.TransitionToScene("MissionSelect");
    }
}
