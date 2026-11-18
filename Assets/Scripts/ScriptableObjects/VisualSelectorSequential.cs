using UnityEngine;

[CreateAssetMenu(menuName = "NEPHTHYS/Visual Selectors/Sequential")]
public class VisualSelectorSequential : BulletVisualSelector
{
    [SerializeField] private BulletVisual[] _visuals;

    public override IBulletVisualSelector CreateRuntime()
        => new VisualSelectorSequentialRuntime(_visuals);
}