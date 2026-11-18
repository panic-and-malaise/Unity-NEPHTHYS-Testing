using UnityEngine;

public interface IMovementPattern
{
    void Enter(GameObject actor);
    void Tick(GameObject actor, float deltaTime);

    bool IsFinished { get; }
}
