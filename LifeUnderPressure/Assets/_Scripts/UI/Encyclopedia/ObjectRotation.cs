using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectRotation : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    void Update()
    {
        if (!IsPointerOverUI())
        {
            RotateObject();
        }
    }

    void RotateObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;


            float rotationSpeed = 0.2f; // Adjust rotation speed as needed
            transform.Rotate(Vector3.up, -mouseDelta.x * rotationSpeed, Space.World); 
            transform.Rotate(Vector3.right, mouseDelta.y * rotationSpeed, Space.World); 

            lastMousePosition = Input.mousePosition;
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
