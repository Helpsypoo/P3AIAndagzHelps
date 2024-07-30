using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChonkCube : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 120;
    
    void Update()
    {
        gameObject.transform.eulerAngles = new Vector3(
            gameObject.transform.eulerAngles.x,
            gameObject.transform.eulerAngles.y + rotationSpeed * Time.deltaTime,
            gameObject.transform.eulerAngles.z
        );
    }
}
