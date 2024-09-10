using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageModifierDefinitions
{
    public enum DamageModifier
    {
        None,
        Head,
        Torso,
        Legs,
        Arms
    }

    public static float GetDamageModifierValue(DamageModifier modifier)
    {
        switch (modifier)
        {
            case DamageModifier.None:
                return 1f;
            case DamageModifier.Head:
                return 2.5f;
            case DamageModifier.Torso:
                return 1f;
            case DamageModifier.Legs:
                return 0.6f;
            case DamageModifier.Arms:
                return 0.6f;
            default:
                Debug.LogWarning($"{modifier} modifier value in DamageModifierDefinitions has not been set.");
                return -1f;
        }
    }
}
