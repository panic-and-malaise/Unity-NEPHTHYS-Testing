using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour
{
    enum SpawnerType
    {
        StraightVector2,
        RotatingVector2,
        
        AimedAtPlayer,
        AimedAheadOfPlayer,
    }

    enum FiringMode
    {
        ShotsPerSecond,
        TimeBetweenShots,
        Instant
    }

    [Header("Spawner Settings")]
    [SerializeField] private SpawnerType _spawnerType = SpawnerType.StraightVector2;
    [SerializeField] private FiringMode _firingMode = FiringMode.ShotsPerSecond;

    [SerializeField] private int _maxShots = 1000;
    [SerializeField] private float _maxLifetime = 1000.0f;
    [SerializeField] private float _rotationDegrees = 1.0f;
    [SerializeField] private float _rotationOffset = 0.0f;
    [SerializeField] private int _burstSize = 1;
    [SerializeField] private Vector2 _spread = Vector2.zero;
    [SerializeField] private bool _randomSpread = false;
    [SerializeField] private Vector2 _randomSpreadArea;
    [SerializeField] private bool _autoFire = true;

    [Range(0.001f, 256.0f)] 
    [SerializeField] private float _fireRate = 10.0f;
    [Range(0.001f, 10.0f)]
    [SerializeField] private float _timeBetweenShots = 0.5f;

    [Header("Bullet Settings")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _bulletSpeed = 1.0f;
    [SerializeField] private float _bulletLifetime = 3.0f;
    [SerializeField] private int _poolSize = 64;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Vector2 _fireDirection = Vector2.down;

    [Header("Visual Settings")]
    [SerializeField] private BulletVisualSelector _visualSelector;
    private IBulletVisualSelector _runtimeVisualSelector;

    private ObjectPool<Bullet> _pool;
    private float _fireTimer;
    private float _lifetime;
    private int _shotsFired;

    void Awake()
    {
        _pool = new ObjectPool<Bullet>(
            createFunc: () =>
            {
                Bullet bullet = Instantiate(_bulletPrefab);
                bullet.gameObject.SetActive(false);
                return bullet;
            },
            actionOnGet: bullet =>
            {
                bullet.gameObject.SetActive(true);
            },
            actionOnRelease: bullet =>
            {
                bullet.gameObject.SetActive(false);
            },
            actionOnDestroy: bullet =>
            {
                DestroyImmediate(bullet);
            },
            collectionCheck: false,
            defaultCapacity: _poolSize,
            maxSize: _poolSize
        );

        if (_visualSelector != null)
            _runtimeVisualSelector = _visualSelector.CreateRuntime();
    }

    void Update()
    {
        if (!_autoFire) return;

        _lifetime += Time.deltaTime;

        if (_lifetime > _maxLifetime) Despawn();

        TryShoot();
    }
    
    private void ShootXTimes(int amount)
    {
        for (int i = 0; i < amount; i++) Shoot();
    }

    private void Shoot()
    {
        _fireTimer = 0.0f; // Reset fire timer
        _shotsFired++;     // Register shot as fired

        float centeredIndex = _shotsFired % _burstSize - (_burstSize - 1) / 2.0f;
        Vector2 offset = _spread * centeredIndex;

        Bullet bullet = _pool.Get(); // Retrieve bullet from the pool

        bullet.transform.position = _firePoint.transform.position + (Vector3)offset;
        if (_randomSpread)
            bullet.transform.position += new Vector3(
                Random.Range(-_randomSpreadArea.x, _randomSpreadArea.x),
                Random.Range(-_randomSpreadArea.y, _randomSpreadArea.y),
                0.0f
            );
        bullet.transform.rotation = _firePoint.transform.rotation;
        if (_runtimeVisualSelector != null)
            bullet.SetVisual(_runtimeVisualSelector.GetNextVisual());

        IMovementPattern pattern = new MovementPatternStraight(_fireDirection, _bulletSpeed, _bulletLifetime);

        switch (_spawnerType)
        {
            case SpawnerType.StraightVector2:
                SpawnBullet(bullet, pattern);
                break;
            case SpawnerType.RotatingVector2:
                float angle = _shotsFired * _rotationDegrees + _rotationOffset;

                Vector2 direction = (Quaternion.Euler(0f, 0f, angle) * _fireDirection).normalized;
                bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                SpawnBullet(bullet, new MovementPatternStraight(direction, _bulletSpeed, _bulletLifetime));
                break;
            case SpawnerType.AimedAtPlayer:
                Vector3 aimedDirection = (Player.Instance.transform.position - _firePoint.transform.position).normalized;

                SpawnBullet(bullet, new MovementPatternStraight(aimedDirection, _bulletSpeed, _bulletLifetime));
                break;
            default:
                break;
        }

        if (_shotsFired >= _maxShots * _burstSize) Despawn();
    }

    private void SpawnBullet(Bullet bullet, IMovementPattern pattern)
    {
        bullet.Initialize(
            _pool,
            pattern,
            _bulletLifetime
        );
    }

    private float GetShotInterval()
    {
        return _firingMode switch {
            FiringMode.Instant =>
                float.MinValue,

            FiringMode.ShotsPerSecond =>
                1.0f / Mathf.Max(_fireRate, 0.001f),

            FiringMode.TimeBetweenShots =>
                _timeBetweenShots,

            _ => 1f
        };
    }

    public bool TryShoot()
    {
        _fireTimer += Time.deltaTime;
        bool shouldFire = _fireTimer >= GetShotInterval();

        if (shouldFire)
            ShootXTimes(_firingMode == FiringMode.Instant ? _maxShots * _burstSize : _burstSize);

        return shouldFire;
    }

    public void SetBulletPrefab(Bullet bullet)
    {
        _bulletPrefab = bullet;
    }

    private void Despawn()
    {
        _pool.Dispose();
        Destroy(gameObject);
    }
}