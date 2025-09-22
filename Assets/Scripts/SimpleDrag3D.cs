using UnityEngine;

public class SimpleDrag3D : MonoBehaviour
{
    private Vector3 _mousePosition;

    private void OnMouseDown()
    {
        _mousePosition = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - _mousePosition);
    }
}
