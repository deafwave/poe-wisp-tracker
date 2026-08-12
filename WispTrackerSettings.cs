using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace WispTracker;

public class WispTrackerSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new(false);

    public DisplaySettings Display { get; set; } = new();
    public PurpleSettings Purple { get; set; } = new();
    public YellowSettings Yellow { get; set; } = new();
    public UIPanelSettings Panels { get; set; } = new();
}

[Submenu]
public class DisplaySettings
{
    [Menu("Draw on world")]
    public ToggleNode DrawWorld { get; set; } = new(true);

    [Menu("Draw on map")]
    public ToggleNode DrawMap { get; set; } = new(true);

    [Menu("Max draw distance")]
    public RangeNode<int> MaxDistance { get; set; } = new(150, 1, 500);

    [Menu("Font size")]
    public RangeNode<int> FontSize { get; set; } = new(16, 8, 48);

    [Menu("World X offset")]
    public RangeNode<int> WorldOffsetX { get; set; } = new(0, -200, 200);

    [Menu("World Y offset", "Negative moves the label up.")]
    public RangeNode<int> WorldOffsetY { get; set; } = new(-20, -200, 200);

    public ColorNode BackgroundColor { get; set; } = new(Color.Black with { A = 165 });
}

[Submenu]
public class PurpleSettings
{
    [Menu("Show purple / Wild wisps")]
    public ToggleNode Show { get; set; } = new(true);

    public ColorNode TextColor { get; set; } = new(SharpDX.Color.MediumPurple);
}

[Submenu]
public class YellowSettings
{
    [Menu("Show yellow / Vivid wisps")]
    public ToggleNode Show { get; set; } = new(true);

    public ColorNode TextColor { get; set; } = new(SharpDX.Color.Yellow);
}

[Submenu(CollapsedByDefault = true)]
public class UIPanelSettings
{
    public ToggleNode IgnoreFullscreenPanels { get; set; } = new(false);
    public ToggleNode IgnoreLargePanels { get; set; } = new(false);
}
