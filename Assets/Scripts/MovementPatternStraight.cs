using UnityEngine;

public class MovementPatternStraight : IMovementPattern
{
    public bool IsFinished => _elapsed >= _duration;
    private float _elapsed;

    [SerializeField] private Vector2 _direction = Vector2.down;
    [SerializeField] private float _speed;
    [SerializeField] private float _duration;

    public MovementPatternStraight(Vector2 direction, float speed, float duration)
    {
        _direction = direction;
        _speed = speed;
        _duration = duration;
    }

    public void Enter(GameObject actor)
    {
        _elapsed = 0.0f;
    }

    public void Tick(GameObject actor, float deltaTime)
    {
        _elapsed += deltaTime;

        actor.transform.position += (Vector3) _direction * _speed * deltaTime;
    }
}
