using UnityEngine;
using Cinemachine;

public class MobaCam : MonoBehaviour {
    private CinemachineVirtualCamera vcam;

    [SerializeField] private Vector3 followOffset;
    [SerializeField] private Vector3 aimOffset;
    
    [SerializeField] private float positionSmoothTime = 0.3f;
    [SerializeField] private float rotationSmoothTime = 0.3f;
    
    private Vector3 velocity = Vector3.zero;
    void Start() {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    private void LateUpdate() {
        if (!vcam) {
            enabled = false;
            return;
        }
        
        Transform followTarget = vcam.Follow;
        if (!followTarget) {
            return;
        }

        // Smoothly set position of transform.position to vcam.follow + followOffset
        Vector3 targetPosition = followTarget.position + followOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, positionSmoothTime);

        // Smoothly set rotation of transform.rotation to look at vcam.follow + aimOffset
        Vector3 lookAtPosition = followTarget.position + aimOffset;
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / rotationSmoothTime);
    }
}
