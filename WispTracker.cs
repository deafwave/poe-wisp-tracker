using System;
using System.Collections.Generic;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using SharpDX;
using Vector2 = System.Numerics.Vector2;

namespace WispTracker;

public class WispTracker : BaseSettingsPlugin<WispTrackerSettings>
{
    private readonly List<TrackedMonster> _tracked = [];

    public WispTracker()
    {
        Name = "Wisp Tracker";
    }

    public override bool Initialise() => true;

    public override void AreaChange(AreaInstance area) => _tracked.Clear();

    public override Job Tick()
    {
        _tracked.Clear();

        if (!Settings.Enable || !GameController.InGame)
            return null;

        var showPurple = Settings.Purple.Show.Value;
        var showYellow = Settings.Yellow.Show.Value;
        if (!showPurple && !showYellow)
            return null;

        var mapMonsterMs = 0;
        try
        {
            GameController.IngameState.Data.MapStats?.TryGetValue(GameStat.MapMonstersMovementSpeedPct, out mapMonsterMs);
        }
        catch
        {
            mapMonsterMs = 0;
        }

        IReadOnlyCollection<Entity> monsters;
        try
        {
            monsters = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Monster];
        }
        catch
        {
            return null;
        }

        foreach (var entity in monsters)
        {
            if (entity is not { IsValid: true, IsAlive: true, IsHostile: true })
                continue;

            IReadOnlyDictionary<GameStat, int> stats;
            try
            {
                stats = entity.Stats;
                if (stats == null || stats.Count == 0)
                {
                    if (!entity.TryGetComponent<Stats>(out var statsComp) || statsComp.StatDictionary == null)
                        continue;
                    stats = statsComp.StatDictionary;
                }
            }
            catch
            {
                continue;
            }

            var purpleStat = 0;
            var purpleJuice = 0;
            if (showPurple && stats.TryGetValue(GameStat.SkillAreaOfEffectPctFinal, out purpleStat))
                purpleJuice = WispJuice.GuessPurple(purpleStat);

            var yellowStat = 0;
            var yellowJuice = 0;
            if (showYellow && stats.TryGetValue(GameStat.MovementVelocityPct, out var rawYellow))
            {
                yellowStat = WispJuice.AdjustYellowVelocity(rawYellow, HasHasteMod(entity), mapMonsterMs);
                yellowJuice = WispJuice.GuessYellow(yellowStat);
            }

            if (purpleJuice <= 0 && yellowJuice <= 0)
                continue;

            _tracked.Add(new TrackedMonster(entity, purpleJuice, purpleStat, yellowJuice, yellowStat));
        }

        return null;
    }

    public override void Render()
    {
        if (!Settings.Enable || !GameController.InGame)
            return;
        if (!ShouldDraw())
            return;
        if (_tracked.Count == 0)
            return;

        var drawWorld = Settings.Display.DrawWorld.Value;
        var drawMap = Settings.Display.DrawMap.Value;
        if (!drawWorld && !drawMap)
            return;

        var largeMapVisible = drawMap && GameController.IngameState.IngameUi.Map.LargeMap.IsVisibleLocal;
        var scale = Settings.Display.FontSize.Value / 16f;
        using var _ = Graphics.SetTextScale(scale);

        var worldOffset = new Vector2(Settings.Display.WorldOffsetX.Value, Settings.Display.WorldOffsetY.Value);
        var background = Settings.Display.BackgroundColor.Value;
        var purpleColor = Settings.Purple.TextColor.Value;
        var yellowColor = Settings.Yellow.TextColor.Value;
        var camera = GameController.IngameState.Camera;
        var maxDistance = Settings.Display.MaxDistance.Value;

        foreach (var tracked in _tracked)
        {
            var entity = tracked.Entity;
            if (entity is not { IsValid: true, IsAlive: true })
                continue;

            if (drawWorld && entity.DistancePlayer <= maxDistance)
            {
                var screen = camera.WorldToScreen(entity.PosNum) + worldOffset;
                DrawStacked(screen, tracked, purpleColor, yellowColor, background);
            }

            if (largeMapVisible)
            {
                var mapPos = GameController.IngameState.Data.GetGridMapScreenPosition(entity.PosNum.WorldToGrid());
                DrawStacked(mapPos, tracked, purpleColor, yellowColor, background);
            }
        }
    }

    private void DrawStacked(Vector2 origin, TrackedMonster tracked, Color purpleColor, Color yellowColor, Color background)
    {
        var lineHeight = 0f;
        if (tracked.PurpleJuice > 0)
        {
            var text = WispJuice.Format(tracked.PurpleJuice, tracked.PurpleStat);
            Graphics.DrawTextWithBackground(text, origin, purpleColor, FontAlign.Center, background);
            lineHeight = Graphics.MeasureText(text).Y;
        }

        if (tracked.YellowJuice > 0)
        {
            var text = WispJuice.Format(tracked.YellowJuice, tracked.YellowStat);
            var pos = tracked.PurpleJuice > 0 ? origin + new Vector2(0, lineHeight) : origin;
            Graphics.DrawTextWithBackground(text, pos, yellowColor, FontAlign.Center, background);
        }
    }

    private static bool HasHasteMod(Entity entity)
    {
        try
        {
            if (!entity.TryGetComponent<ObjectMagicProperties>(out var props) || props.Mods == null)
                return false;

            foreach (var mod in props.Mods)
            {
                if (mod == WispJuice.HasteModName)
                    return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private bool ShouldDraw()
    {
        var ui = GameController.IngameState.IngameUi;
        if (ui == null)
            return false;
        if (!Settings.Panels.IgnoreFullscreenPanels && ui.FullscreenPanels.Exists(x => x.IsVisible))
            return false;
        if (!Settings.Panels.IgnoreLargePanels && ui.LargePanels.Exists(x => x.IsVisible))
            return false;
        return true;
    }

    private readonly record struct TrackedMonster(
        Entity Entity,
        int PurpleJuice,
        int PurpleStat,
        int YellowJuice,
        int YellowStat);
}
