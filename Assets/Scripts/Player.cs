using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable
{
    public static Player Instance { get; private set; }
    
    [Header("Input")]
    [SerializeField] private InputActionReference _actionMove;
    [SerializeField] private InputActionReference _actionFocus;
    [SerializeField] private InputActionReference _actionShootPrimary;

    [Header("Movement")]
    [SerializeField] private float _speedNormal;
    [SerializeField] private float _speedFocus;
    
    [Header("Combat")]
    [SerializeField] private BulletSpawner _shooterNormal;
    [SerializeField] private BulletSpawner _shooterFocus;
    [SerializeField] private int _health = 3;
    [SerializeField] private float _invincibilityTime = 1.0f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer _hitboxVisual;

    private Rigidbody2D _body;
    private SpriteRenderer _sprite;
    private Animator _animator;

    private BulletSpawner _shooter;
    private Vector2 _moveInput;
    private bool _isFocused;
    private float _invincibilityTimer;

    private bool IsInvincible => _invincibilityTimer > 0.0f;
    public bool IsFocused
    {
        get =>_isFocused; private set
        {
            _isFocused = value;
            // _hitboxVisual.enabled = _isFocused;
            _shooter = _isFocused ? _shooterFocus : _shooterNormal;
        }
    }

    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _shooter = _shooterNormal;

        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void OnEnable()
    {
        _actionMove.action.Enable();
        _actionFocus.action.Enable();
        _actionShootPrimary.action.Enable();
    }

    void OnDisable()
    {
        _actionMove.action.Disable();
        _actionFocus.action.Disable();
        _actionShootPrimary.action.Disable();
    }

    void Update()
    {
        _moveInput = _actionMove.action.ReadValue<Vector2>();
        _moveInput = Vector2.ClampMagnitude(_moveInput, 1.0f);

        IsFocused = _actionFocus.action.IsPressed();

        _animator.SetBool("Moving", _moveInput.x != 0.0f);
        _animator.SetBool("Focused", _isFocused);
        _sprite.flipX = _moveInput.x > 0.0f;


        if (_actionShootPrimary.action.IsPressed())
        {
            if (_shooter.TryShoot())
                AudioManager.Instance.PlayEffect(AudioManager.Instance.PlayerShoot);
        }

        if (IsInvincible) _invincibilityTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        float speed = _isFocused ? _speedFocus : _speedNormal;
        Vector2 movement = _moveInput * speed;

        _body.MovePosition(_body.position + movement * Time.fixedDeltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.violetRed;
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0, 0), 0.1f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHazard _))
        {
            OnHit(1);
        }
    }

    public void OnHit(int amount)
    {
        if (!IsInvincible)
        {
            Debug.Log("HIT BY HAZARD!");
            _health -= amount;
            _invincibilityTimer = _invincibilityTime;
        }
        if (_health <= 0) Die();
    }

    public void Die()
    {
        AudioManager.Instance.PlayEffect(AudioManager.Instance.PlayerDead, 0.5f);
        Destroy(this);
    }
}
