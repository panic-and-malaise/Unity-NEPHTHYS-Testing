using UnityEngine;

public class Familiar : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Transform _rotationPivot;
    [Range(-256.0f, 256.0f)]
    [SerializeField] private float _rotationSpeed = 1.0f;
    [Range(-256.0f, 256.0f)]
    [SerializeField] private float _spriteRotationSpeed = 1.0f;

    [Header("Bullets")]
    [SerializeField] private float _shootTime = 1.0f;
    [SerializeField] private float _timeBetweenBursts = 1.0f;

    private BulletSpawner _spawner;

    private float _timerRotation;
    private float _timerShot;
    private float _timerBurst;


    void Awake()
    {
        _spawner = GetComponent<BulletSpawner>();
    }

    void Update()
    {
        _timerRotation += Time.deltaTime;

        if (_rotationSpeed != 0.0f)
            _rotationPivot.rotation = Quaternion.Euler(0f, 0f, _rotationSpeed * _timerRotation);
        // transform.RotateAround(_rotationPivot.position, new Vector3(0, 0, 1), _rotationSpeed * Time.deltaTime);
        if (_spriteRotationSpeed != 0.0f)
            transform.rotation = Quaternion.Euler(0f, 0f, _spriteRotationSpeed * _timerRotation);

        if (_spawner == null) return;

        _timerShot += Time.deltaTime;
        if (_timerShot >= _shootTime)
        {
            _spawner.enabled = false;

            _timerBurst += Time.deltaTime;

            if (_timerBurst >= _timeBetweenBursts)
            {
                _spawner.enabled = true;

                _timerShot = 0.0f;
                _timerBurst = 0.0f;
            }
        }
    }
}
