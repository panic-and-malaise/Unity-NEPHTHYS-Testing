using UnityEngine;
using UnityEngine.Pool;

public class PlayerBullet : MonoBehaviour, IProjectile
{
    private IObjectPool<PlayerBullet> _pool;

    private Vector2 _direction;
    private float _speed;
    private float _lifetime;
    private float _timer;

    [SerializeField] public int _damage;
    public int Damage { get => _damage; set => _damage = value; }

    public void Initialize(
        IObjectPool<PlayerBullet> pool,
        Vector2 direction,
        float speed,
        float lifetime = 3.0f
    )
    {
        _pool = pool;
        _direction = direction;
        _speed = speed;
        _lifetime = lifetime;
        _timer = 0.0f;
    }

    public void MoveTick(float deltaTime)
    {
        transform.position += (Vector3) _direction * _speed * deltaTime;
    }

    public void OnHit(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player _)) return;
        if (collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.OnHit(Damage);
            Release();
        }
    }

    public void Release()
    {
        _pool.Release(this);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        OnHit(collision);
    }

    // Update is called once per frame
    void Update()
    {
        MoveTick(Time.deltaTime);
        _timer += Time.deltaTime;
        if (_timer >= _lifetime) Release();
    }
}
