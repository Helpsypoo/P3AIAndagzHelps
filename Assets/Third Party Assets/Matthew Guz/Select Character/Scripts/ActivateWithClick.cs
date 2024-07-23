using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToToggleObject : MonoBehaviour
{
    public GameObject objectToToggle;
    public GameObject objectP; // Object that is considered "active"

    private bool isVisible = false; // Initially we set it to false

    void Start()
    {
        // Make sure the object is initially disabled
        objectToToggle.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == objectP)
                {
                    ToggleVisibility();
                }
                else
                {
                    // If you click anywhere other than the P object, we deactivate the object
                    objectToToggle.SetActive(false);
                    isVisible = false;
                }
            }
        }
    }

    void ToggleVisibility()
    {
        isVisible = !isVisible;
        objectToToggle.SetActive(isVisible);
    }
}
