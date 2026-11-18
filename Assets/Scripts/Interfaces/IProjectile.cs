using UnityEngine;

public interface IProjectile
{
    void MoveTick(float deltaTime);
    void OnHit(Collider2D collider);

    int Damage { get; }
}