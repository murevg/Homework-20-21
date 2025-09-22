using UnityEngine;

public class ExplosionSpawner : MonoBehaviour
{
    [SerializeField] private float _radius = 2f;
    [SerializeField] private float _force = 10f;
    [SerializeField] private float _upwardsModifier = 0.2f;

    [SerializeField] private GameObject _explosionVfxPrefab;

    private Collider[] _hits = new Collider[128];

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (TryGetPointOnGround(out Vector3 point))
            {
                SpawnExplosion(point);
            }
        }
    }

    private bool TryGetPointOnGround(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 5000f))
        {
            point = hit.point;
            return true;
        }
        point = default;
        return false;
    }

    private void SpawnExplosion(Vector3 center)
    {
        if (_explosionVfxPrefab != null)
        {
            Instantiate(_explosionVfxPrefab, center, Quaternion.identity);
        }

        int count = Physics.OverlapSphereNonAlloc(center, _radius, _hits);

        for (int i = 0; i < count; i++)
        {
            Collider collider = _hits[i];

            if (collider.attachedRigidbody != null)
            {
                collider.attachedRigidbody.AddExplosionForce(_force, center, _radius, _upwardsModifier, ForceMode.Impulse);
            }

            _hits[i] = null;
        }
    }
}
