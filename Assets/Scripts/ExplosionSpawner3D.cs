using UnityEngine;

public class ExplosionSpawner3D : MonoBehaviour
{
    [SerializeField] private float _radius = 2f;            // Радиус взрыва
    [SerializeField] private float _force = 10f;            // Сила взрыва
    [SerializeField] private float _upwardsModifier = 0.2f; // «Подброс» (0 — без подброса)

    [SerializeField] private GameObject _explosionVfxPrefab;

    // Для OverlapSphereNonAlloc, чтобы не аллокировать каждый раз
    private readonly Collider[] _hits = new Collider[128]; // что если коллайдеров будет больше 128?????

    void Update()
    {
        // ПКМ — создаём взрыв в точке попадания луча
        if (Input.GetMouseButtonDown(1))
        {
            if (TryGetPointOnGround(out Vector3 point))
            {
                SpawnExplosion(point);
            }
        }
    }

    bool TryGetPointOnGround(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 5000f))
        {
            point = hit.point; // Берём точку попадания луча по поверхности коллайдера
            return true;       // Сообщаем, что точка найдена
        }
        point = default;       // Если промах — отдаём (0,0,0)
        return false;          // И сообщаем, что ничего не найдено
    }

    void SpawnExplosion(Vector3 center) //Создает взрыв в заданной позиции 
    {
        if (_explosionVfxPrefab != null)
        {
            Instantiate(_explosionVfxPrefab, center, Quaternion.identity);
        }

        // Находим коллайдеры в радиусе (без аллокаций)
        int count = Physics.OverlapSphereNonAlloc(center, _radius, _hits);

        //Debug.Log("Количество коллайдеров в радусе взрыва: " + count);

        // Заполняем массив _hits всеми коллайдерами внутри сферы (center, radius)
        // Возвращает фактическое число найденных коллайдеров

        for (int i = 0; i < count; i++)
        {
            Collider collider = _hits[i];

            if (collider.attachedRigidbody != null)
            {
                collider.attachedRigidbody.AddExplosionForce(_force, center, _radius, _upwardsModifier, ForceMode.Impulse);
            }

            _hits[i] = null; // чистим ссылку для следующего кадра
        }
    }
}
