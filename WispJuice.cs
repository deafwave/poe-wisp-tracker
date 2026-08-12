using System;

namespace WispTracker;

public static class WispJuice
{
    public const int PurpleBasePct = 30;
    public const int PurplePerThousand = 15;
    public const int PurpleHighBreakpointJuice = 5000;
    public const int PurpleHighBreakpointPct = 105;
    public const int PurpleHighBasePct = 55;
    public const int PurpleHighPerThousand = 10;
    public const int YellowBasePct = 25;
    public const int YellowPerThousand = 10;
    public const int HasteModVelocityPct = 50;
    public const string HasteModName = "MonsterModAttackCastMovementSpeed";

    public static int GuessPurple(int areaPct) =>
        areaPct <= PurpleHighBreakpointPct
            ? Guess(areaPct, PurpleBasePct, PurplePerThousand)
            : Guess(areaPct, PurpleHighBasePct, PurpleHighPerThousand);

    public static int ForwardPurple(int juice)
    {
        if (juice <= PurpleHighBreakpointJuice)
            return PurpleBasePct + PurplePerThousand * juice / 1000;
        return PurpleHighBasePct + PurpleHighPerThousand * juice / 1000;
    }

    public static int GuessYellow(int velocityPct) =>
        Guess(velocityPct, YellowBasePct, YellowPerThousand);

    public static int AdjustYellowVelocity(int velocityPct, bool hasHasteMod, int mapMonstersMovementSpeedPct = 0)
    {
        if (hasHasteMod)
            velocityPct -= HasteModVelocityPct;
        return velocityPct - mapMonstersMovementSpeedPct;
    }

    public static int Guess(int statPct, int basePct, int perThousand)
    {
        if (perThousand <= 0 || statPct <= basePct)
            return 0;

        return (int)Math.Round((statPct - basePct) * 1000.0 / perThousand);
    }

    public static string Format(int juice, int statPct) => $"{juice} ({statPct}%)";
}
