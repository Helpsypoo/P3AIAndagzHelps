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

        // Smoothly set position
        Vector3 targetPosition = followTarget.position + followOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, positionSmoothTime);

        // Calculate the look at position and the desired rotation
        Vector3 lookAtPosition = followTarget.position + aimOffset;
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);

        Vector3 currentEuler = transform.rotation.eulerAngles;
        Vector3 targetEuler = targetRotation.eulerAngles;

        // Smoothly interpolate the X axis rotation
        float smoothEulerX = Mathf.LerpAngle(currentEuler.x, targetEuler.x, Time.deltaTime / rotationSmoothTime);

        // Apply the interpolated X rotation while keeping the current Y and Z rotations
        transform.rotation = Quaternion.Euler(smoothEulerX, currentEuler.y, currentEuler.z);
    }
}
