using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IHazard
{
    [SerializeField] private float _health = 1.0f;

    private IMovementPattern _movementPattern = null;//new MovementPatternStraight(Vector2.down, 5.0f, 3.0f);

    public void OnHit(int amount)
    {
        _health -= amount;
        if (_health <= 0) Die();
    }
    
    public void Die()
    {
        Destroy(gameObject);
    }

    void Awake()
    {
        if (TryGetComponent(out IMovementPattern pattern)) _movementPattern = pattern;
    }

    void Start()
    {
        _movementPattern?.Enter(gameObject);
    }

    void Update()
    {
        _movementPattern?.Tick(gameObject, Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
    }

    public void OnHazardCollision(Collider2D collider)
    {

    }
}
