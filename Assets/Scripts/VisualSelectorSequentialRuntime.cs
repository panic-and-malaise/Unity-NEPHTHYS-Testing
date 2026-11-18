public class VisualSelectorSequentialRuntime : IBulletVisualSelector
{
    private readonly BulletVisual[] _visuals;
    private int _index;

    public VisualSelectorSequentialRuntime(BulletVisual[] visuals)
    {
        _visuals = visuals;
    }

    public BulletVisual GetNextVisual()
    {
        if (_visuals == null || _visuals.Length == 0)
            return null;

        BulletVisual visual = _visuals[_index];

        _index = (_index + 1) % _visuals.Length;

        return visual;
    }
}