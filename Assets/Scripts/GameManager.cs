using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public float SunDamagePerSecond = 2.5f;
    
    public LayerMask ShadeLayerMask;

    public Camera MainCamera { get; private set; }

    private void Awake() {
        if (Instance) {
            Destroy(this);
        } else {
            Instance = this;
        }

        MainCamera = Camera.main;
    }

}
