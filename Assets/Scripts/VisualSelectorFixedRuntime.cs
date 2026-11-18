public class VisualSelectorFixedRuntime : IBulletVisualSelector
{
    private readonly BulletVisual _visual;

    public VisualSelectorFixedRuntime(BulletVisual visual)
    {
        _visual = visual;
    }

    public BulletVisual GetNextVisual()
    {
        return _visual;
    }
}