using UnityEngine;
using UnityEngine.Pool;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private PlayerBullet _bulletPrefab;
    [SerializeField] private float _bulletSpeed = 1.0f;
    [SerializeField] private float _bulletLifetime = 3.0f;
    [SerializeField] private float _fireRate = 10.0f;
    [SerializeField] private int _poolSize = 64;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Vector2 _fireDirection = Vector2.up;

    private ObjectPool<PlayerBullet> _pool;
    private float _fireTimer;

    void Awake()
    {
        _pool = new ObjectPool<PlayerBullet>(
            createFunc: () =>
            {
                PlayerBullet bullet = Instantiate(_bulletPrefab);
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
    }

    private void Shoot()
    {
        PlayerBullet bullet = _pool.Get();

        bullet.transform.position = _firePoint.transform.position;
        bullet.transform.rotation = _firePoint.transform.rotation;

        bullet.Initialize(
            _pool,
            _fireDirection,
            _bulletSpeed,
            _bulletLifetime
        );


        AudioManager.Instance.PlayEffect(AudioManager.Instance.PlayerShoot);
    }

    public void TryShoot()
    {
        _fireTimer += Time.deltaTime;
        
        if (_fireTimer >= 1.0f / _fireRate)
        {
            _fireTimer = 0.0f;
            Shoot();
        }
    }
}
