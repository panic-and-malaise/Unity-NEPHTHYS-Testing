
using UnityEngine;

[CreateAssetMenu(menuName = "NEPHTHYS/Visual Selectors/Fixed")]
public class VisualSelectorFixed : BulletVisualSelector
{
    [SerializeField] private BulletVisual _visual;

    public override IBulletVisualSelector CreateRuntime()
        => new VisualSelectorFixedRuntime(_visual);
}