using UnityEngine;

public class Menu : MonoBehaviour {
    [SerializeField] private GameObject ResumeButton;
    public GameObject WarningMessage;
    private void Start() {
        WarningMessage.SetActive(false);
        ResumeButton.SetActive(SessionManager.Instance.Level0Status > 0);
    }

    public void Resume()
    {
        SessionManager.Instance.ResumeGame();
    }
}
