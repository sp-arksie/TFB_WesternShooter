using UnityEngine;

public static class StatusEffectsDefinitions
{
    // Bleeding
    public const float lightBleedingDamageRate = 2f;
    public const float heavyBleedingDamageRate = 5f;
    public const float lightToHeavyBleedingTransitionTime = 10f;

    // Burning
    public const float lightBurningDamageRate = 1f;
    public const float heavyBurningDamageRate = 3f;
    public const float lightToHeavyBurningTransitionTime = 10f;

    //Poison
    public const float poisonEffectDuration = 10f;


    public static float GetStatusEffectDamage(StatusEffect damageType, float timeSinceEffectStart)
    {
        float damage = 0f;

        switch (damageType)
        {
            case StatusEffect.None:
                return 0f;
            case StatusEffect.Bleeding:
                if (timeSinceEffectStart < lightToHeavyBleedingTransitionTime) damage = lightBleedingDamageRate;
                else damage = heavyBleedingDamageRate;
                return damage;
            case StatusEffect.Burning:
                if (timeSinceEffectStart < lightToHeavyBurningTransitionTime) damage = lightBurningDamageRate;
                else damage = heavyBurningDamageRate;
                return damage;
            default:
                Debug.LogWarning($"{damageType} definition has not been set in StatusEffectsDefinitions");
                return 0f;
        }
    }
}
