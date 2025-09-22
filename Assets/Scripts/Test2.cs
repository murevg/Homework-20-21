using UnityEngine;

public class Test2 : MonoBehaviour
{
    [SerializeField] private Rigidbody _selectedRigidbody;
    [SerializeField] private float _baseY = 2f;
    [SerializeField] private float _radius = 1f;
    [SerializeField] private bool _isAnotherOneNearby;

    [SerializeField] Vector3 _currentPosition;
    [SerializeField] Collider[] _hits;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(cameraRay, out RaycastHit hit) && hit.rigidbody)
            {
                Debug.Log(hit.collider.name);
                _selectedRigidbody = hit.rigidbody;

                _selectedRigidbody.useGravity = false;

                _hits = Physics.OverlapSphere(_selectedRigidbody.position, _radius);

                //int count = Physics.OverlapSphereNonAlloc(_selectedRigidbody.position, _radius, _hits);

                if (_hits.Length > 2)
                {
                    _isAnotherOneNearby = true;
                }
                else
                {
                    _isAnotherOneNearby = false;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && _selectedRigidbody != null)
        {
            _selectedRigidbody.useGravity = true;
            _selectedRigidbody = null;
        }
    }

    private void FixedUpdate()
    {
        if(_selectedRigidbody != null)
        {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(_selectedRigidbody.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            Vector3 targetPosition = new Vector3(worldPosition.x, _currentPosition.y, worldPosition.z); 
            _currentPosition = targetPosition;

            if (_isAnotherOneNearby)
            {
                targetPosition = new Vector3(worldPosition.x, _baseY+1, worldPosition.z);
            }
            else
            {
                targetPosition = new Vector3(worldPosition.x, _baseY, worldPosition.z);
            }

            _selectedRigidbody.MovePosition(targetPosition);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;

            if (Input.GetMouseButton(0))
            {
                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(cameraRay, out RaycastHit hit) && hit.rigidbody)
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawRay(cameraRay.origin, cameraRay.direction * 100);
            }
        }
    }
}
