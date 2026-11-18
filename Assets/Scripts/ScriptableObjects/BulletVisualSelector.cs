using UnityEngine;

public abstract class BulletVisualSelector : ScriptableObject
{
    public abstract IBulletVisualSelector CreateRuntime();
}