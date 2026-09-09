public static class ChronoVisibilityFactory
{
    public static IChronoVisibility Create(VisibilityType visibilityType)
    {
        switch (visibilityType)
        {
            case VisibilityType.Full:
                return new FullVisibility();
            case VisibilityType.Half:
                return new HalfVisibility();
            case VisibilityType.Hidden:
                return new HiddenVisibility();
            default:
                return new FullVisibility();
        }
    }
}
