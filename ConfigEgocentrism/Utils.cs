using BepInEx.Configuration;
using RoR2;
using System;
using UnityEngine;

namespace ConfigEgocentrism
{
    public static class Utils
    {
        public enum ItemTierLookup
        {
            white = ItemTier.Tier1,
            green = ItemTier.Tier2,
            red = ItemTier.Tier3,
            blue = ItemTier.Lunar,
            yellow = ItemTier.Boss,
            untiered = ItemTier.NoTier,
            voidwhite = ItemTier.VoidTier1,
            voidgreen = ItemTier.VoidTier2,
            voidred = ItemTier.VoidTier3,
            voidyellow = ItemTier.VoidBoss
        }

        public enum RoundingMode
        {
            AlwaysDown,
            AlwaysUp,
            Closest
        }

        public static int Round(float f, string roundingModeStr, int defaultVal = 0)
        {
            RoundingMode mode = RoundingMode.AlwaysDown;
            if (Enum.TryParse(roundingModeStr, out RoundingMode parsedMode))
                mode = parsedMode;

            switch(mode)
            {
                case RoundingMode.AlwaysDown:
                    return Mathf.FloorToInt(f);
                case RoundingMode.AlwaysUp:
                    return Mathf.CeilToInt(f);
                case RoundingMode.Closest:
                    return Mathf.RoundToInt(f);
            }

            Log.LogError($"Rounding mode \"{roundingModeStr}\" not implemented. Returning default value ({defaultVal.ToString()})");
            return defaultVal;
        }

        public static float GetStrictlyPositiveConfigFloat(ConfigEntry<float> config)
        {
            if (config.Value > 0.0f)
                return config.Value;

            return (float)config.DefaultValue;
        }

        //Formula: baseVal + (stack * stackMult)^stackExponent
        public static float GetAdditionalFormulaValue(float baseVal, int stack, float stackMult, float stackExponent)
        {
            return baseVal + Mathf.Pow(stack * stackMult, stackExponent);
        }

        //Formula: baseVal / (stack * stackMult)^stackExponent
        public static float GetDividingFormulaValue(float baseVal, int stack, float stackMult, float stackExponent)
        {
            return baseVal / Mathf.Pow(stack * stackMult, stackExponent);
        }
    }
}
