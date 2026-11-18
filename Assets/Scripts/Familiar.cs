using UnityEngine;

public class Familiar : MonoBehaviour
{
    [SerializeField] private Transform _rotationPivot;
    [Range(-256.0f, 256.0f)]
    [SerializeField] private float _rotationSpeed = 1.0f;
    [Range(-256.0f, 256.0f)]
    [SerializeField] private float _spriteRotationSpeed = 1.0f;

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;

        if (_rotationSpeed != 0.0f)
            _rotationPivot.rotation = Quaternion.Euler(0f, 0f, _rotationSpeed * _timer);
        // transform.RotateAround(_rotationPivot.position, new Vector3(0, 0, 1), _rotationSpeed * Time.deltaTime);
        if (_spriteRotationSpeed != 0.0f)
            transform.rotation = Quaternion.Euler(0f, 0f, _spriteRotationSpeed * _timer);
    }
}
