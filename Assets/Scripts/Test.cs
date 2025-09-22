using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _holder;

    private void Update()
    {
        Vector3 _worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.rigidbody)
            {
                Vector3 hitPoint = hit.point;
                Vector3 hitNormal = hit.normal;

                _holder = hit.rigidbody;
                _holder.useGravity = false;

                Vector3 newPosition = new Vector3 (hitPoint.x, hitNormal.y, hitPoint.z);
                _holder.MovePosition(_worldPos * _speed * Time.deltaTime);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_holder == false) return;
            _holder.useGravity = true;
            _holder = null;
        }
    }

    private void FixedUpdate()
    {
        if (!_holder) return;
    }

    //void FixedUpdate()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if(Physics.Raycast(cameraRay, out RaycastHit hitInfo))
    //        {
    //            Debug.Log(hitInfo.collider.name);
    //            hitInfo.collider.gameObject.GetComponent<Rigidbody>();

    //            if (hitInfo.rigidbody != null)
    //            {
    //                Vector3 currentPosition = hitInfo.rigidbody.position;
    //                hitInfo.rigidbody.MovePosition(currentPosition + Vector3.up * _speed * Time.deltaTime);
    //            }
    //        }
    //    }
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    if (Application.isPlaying)
    //    {
    //        Gizmos.color = Color.red;

    //        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        Gizmos.DrawRay(cameraRay.origin, cameraRay.direction * 100);
    //    }
    //}
}
