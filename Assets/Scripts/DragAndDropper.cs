using UnityEngine;

public class DragAndDropper : MonoBehaviour
{
    [SerializeField] private float _baseY = 1f;
    [SerializeField] private float _radius = 1.5f;
    [SerializeField] private float _step = 0.05f;
    [SerializeField] private int _maxStepsUp = 200;
    [SerializeField] private int _maxStepsDown = 60;

    private Rigidbody _selectedRigidbody;
    private Vector3 _cachedTarget;
    private Collider[] _hits = new Collider[128];
    private Collider _selfCollider;

    private void Awake()
    {
        _selfCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (Camera.main == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.rigidbody != null)
            {
                _selectedRigidbody = hit.rigidbody;
                _selectedRigidbody.useGravity = false;
                _selfCollider = _selectedRigidbody.GetComponent<Collider>();
            }
        }

        if (_selectedRigidbody != null && Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, new Vector3(0f, _baseY, 0f));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 position = ray.GetPoint(enter);
                _cachedTarget = new Vector3(position.x, _baseY, position.z);

                _cachedTarget.y = ToDown(_cachedTarget, _radius, _step, _maxStepsDown);

                _cachedTarget.y = ToUp(_cachedTarget, _radius, _step, _maxStepsUp);
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
        if (_selectedRigidbody != null)
        {
            _selectedRigidbody.MovePosition(_cachedTarget);
        }
    }

    private float ToUp(Vector3 start, float radius, float step, int maxSteps)
    {
        Vector3 position = start;
        for (int i = 0; i < maxSteps; i++)
        {
            int count = Physics.OverlapSphereNonAlloc(position, radius, _hits);
            bool hasOthers = HasOtherColliders(count);
            if (!hasOthers) break;
            position.y += step;
        }
        return position.y;
    }

    private float ToDown(Vector3 start, float radius, float step, int maxSteps)
    {
        Vector3 position = start;
        for (int i = 0; i < maxSteps; i++)
        {
            int count = Physics.OverlapSphereNonAlloc(position, radius, _hits);
            bool hasOthers = HasOtherColliders(count);
            if (hasOthers)
                return position.y;
            position.y -= step;
        }
        return position.y;
    }

    private bool HasOtherColliders(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Collider nearbyCollider = _hits[i];
            if (nearbyCollider == null) continue;
            if (_selfCollider != null && nearbyCollider == _selfCollider) continue;
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (_selectedRigidbody != null)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawWireSphere(_cachedTarget, _radius);
        }
    }
}
