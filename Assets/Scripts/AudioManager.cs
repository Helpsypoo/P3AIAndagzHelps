using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    private void Awake() {
        if (Instance) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
}
