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
    [SerializeField] private PlayerShooter _shooter;
    [SerializeField] private int _health = 3;
    [SerializeField] private float _invincibilityTime = 1.0f;

    private Rigidbody2D _body;

    private Vector2 _moveInput;
    private bool _isFocused;
    private float _invincibilityTimer;

    private bool IsInvincible => _invincibilityTimer > 0.0f;

    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
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

        _isFocused = _actionFocus.action.IsPressed();

        if (_actionShootPrimary.action.IsPressed())
            _shooter.TryShoot();

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
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.05f, 0), 0.15f);
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
        Destroy(this);
    }
}
