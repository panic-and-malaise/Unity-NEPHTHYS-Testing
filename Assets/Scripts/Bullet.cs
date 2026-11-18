using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour, IProjectile
{
    private IObjectPool<Bullet> _pool;

    private IMovementPattern _pattern;
    private float _lifetime;
    private float _timer;

    [SerializeField] public int _damage;
    public int Damage { get => _damage; set => _damage = value; }
    [SerializeField] private float _rotationSpeed = 0.0f;

    private SpriteRenderer _sprite;

    public void Initialize(
        IObjectPool<Bullet> pool,
        IMovementPattern pattern,
        float lifetime = 3.0f
    )
    {
        _pool = pool;
        _pattern = pattern;
        _lifetime = lifetime;
        _timer = 0.0f;
    }

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    public void MoveTick(float deltaTime)
    {
        _pattern.Tick(gameObject, deltaTime);
    }

    public void OnHit(Collider2D collider)
    {
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

        if (_rotationSpeed > 0.0f)
            transform.rotation = Quaternion.Euler(0f, 0f, _rotationSpeed * _timer * 100);
    }

    public void SetVisual(BulletVisual visual)
    {
        _sprite.sprite = visual.Sprite;
        _sprite.color = visual.Color;
        if (visual.Scale != Vector3.zero)
            transform.localScale = visual.Scale;
    }
}
