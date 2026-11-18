using UnityEngine;

[CreateAssetMenu(menuName = "NEPHTHYS/Bullet Visual")]
public class BulletVisual : ScriptableObject
{
    public Sprite Sprite;
    public Color Color = Color.white;
    public Vector3 Scale;
}