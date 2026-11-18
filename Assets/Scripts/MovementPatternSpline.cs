using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.U2D;

public class MovementPatternSpline : MonoBehaviour, IMovementPattern
{
    [SerializeField] private SplineContainer _spline;
    [SerializeField] private float _duration;
    [SerializeField] private bool _rotateToPath;

    private float _elapsed;
    public bool IsFinished => _elapsed >= _duration;

    public MovementPatternSpline(SplineContainer spline, float duration, bool rotateToPath = false)
    {
        _spline = spline;
        _duration = Mathf.Max(0.001f, duration);
        _rotateToPath = rotateToPath;
    }

    public void Enter(GameObject actor)
    {
        _elapsed = 0.0f;

        actor.transform.position = _spline.EvaluatePosition(0.0f);
    }

    public void Tick(GameObject actor, float deltaTime)
    {
        if (IsFinished) return;

        _elapsed += deltaTime;

        float t = Mathf.Clamp01(_elapsed / _duration);
        Vector3 position = _spline.EvaluatePosition(t);

        actor.transform.position = position;

        if (_rotateToPath)
        {
            Vector3 tanget = _spline.EvaluateTangent(t);

            if (tanget.sqrMagnitude > 0.001f)
                actor.transform.up = tanget;
        }
    }
}
