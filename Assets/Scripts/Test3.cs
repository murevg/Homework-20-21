using UnityEngine;

public class Test3 : MonoBehaviour
{
    [SerializeField] private float _baseY = 1f;
    [SerializeField] private float _radius = 1.5f;
    [SerializeField] private float _step = 0.05f; // шаг подъёма/опускания
    [SerializeField] private int _maxStepsUp = 200;  // максимум шагов вверх (10 м при step=0.05)
    [SerializeField] private int _maxStepsDown = 60; // максимум шагов вниз

    private Rigidbody _selectedRigidbody;
    private Camera _camera;
    private Vector3 _cachedTarget;
    private readonly Collider[] _hits = new Collider[128];
    private Collider _selfCollider;

    private void Awake()
    {
        _camera = Camera.main;
        _selfCollider = GetComponent<Collider>(); // если у объекта несколько — можно хранить массив
    }

    private void Update()
    {
        if (_camera == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.rigidbody != null)
            {
                _selectedRigidbody = hit.rigidbody;
                _selectedRigidbody.useGravity = false;
                // на случай если перетаскиваем не тот же объект, что этот скрипт
                _selfCollider = _selectedRigidbody.GetComponent<Collider>();
            }
        }

        if (_selectedRigidbody != null && Input.GetMouseButton(0))
        {
            // считаем XZ точку (вариант с плоскостью на _baseY; можно заменить на любой из прошлых вариантов)
            Plane plane = new Plane(Vector3.up, new Vector3(0f, _baseY, 0f));
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 position = ray.GetPoint(enter);
                _cachedTarget = new Vector3(position.x, _baseY, position.z);

                // 1) сначала опустимся, если над пустотой (чтобы не зависать зря)
                _cachedTarget.y = ToDown(_cachedTarget, _radius, _step, _maxStepsDown);

                // 2) затем поднимемся, пока вокруг есть коллайдеры (пока тесно) — ищем свободное место
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
        Vector3 pos = start;
        for (int i = 0; i < maxSteps; i++)
        {
            int count = Physics.OverlapSphereNonAlloc(pos, radius, _hits);
            bool hasOthers = HasOtherColliders(count);
            if (!hasOthers) break;           // чисто — хватит подниматься
            pos.y += step;                   // иначе поднимаемся ещё немного
        }
        return pos.y;
    }

    private float ToDown(Vector3 start, float radius, float step, int maxSteps)
    {
        Vector3 pos = start;
        for (int i = 0; i < maxSteps; i++)
        {
            int count = Physics.OverlapSphereNonAlloc(pos, radius, _hits);
            bool hasOthers = HasOtherColliders(count);
            if (hasOthers) // уже есть «соседи» — значит стоим на чём-то/рядом — можно остановиться
                return pos.y;
            pos.y -= step; // спускаемся
        }
        return pos.y;      // достигли лимита — оставляем как есть
    }

    // Проверяем, остались ли какие-то коллайдеры кроме своего
    private bool HasOtherColliders(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var c = _hits[i];
            if (c == null) continue;
            if (_selfCollider != null && c == _selfCollider) continue; // пропускаем свой
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
