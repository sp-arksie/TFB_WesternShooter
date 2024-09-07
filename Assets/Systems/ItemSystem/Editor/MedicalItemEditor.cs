using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MedicalItem))]
public class MedicalItemEditor : Editor
{
    #region Serialized properties
    SerializedProperty GrabPointsParent;
    SerializedProperty RightArmOnly;

    SerializedProperty hipPositionTransform;
    SerializedProperty hiddenTransform;

    SerializedProperty timeToUseItem;
    SerializedProperty isQuickAction;

    SerializedProperty doesRestoreHealth;
    SerializedProperty healthRestoreAmount;

    SerializedProperty doesRestoreBurnedHealth;
    SerializedProperty burnedHealthRestoreAmount;

    SerializedProperty doesDamageAbsorb;
    SerializedProperty damageAbsorbPercent;
    SerializedProperty damageAbsorbDuration;

    SerializedProperty doesHealthRegeneration;
    SerializedProperty regenerationPerSecond;
    SerializedProperty regenerationDuration;

    SerializedProperty doesCureDebuffs;
    SerializedProperty debuffCures;
    #endregion

    bool itemBaseGroup;
    bool generalMedicalItemGroup;
    bool medicalItemEffectsGroup;

    private void OnEnable()
    {
        GrabPointsParent = serializedObject.FindProperty("GrabPointsParent");
        RightArmOnly = serializedObject.FindProperty("RightArmOnly");

        hipPositionTransform = serializedObject.FindProperty("hipPositionTransform");
        hiddenTransform = serializedObject.FindProperty("hiddenTransform");

        timeToUseItem = serializedObject.FindProperty("timeToUseItem");
        isQuickAction = serializedObject.FindProperty("isQuickAction");

        doesRestoreHealth = serializedObject.FindProperty("doesRestoreHealth");
        healthRestoreAmount = serializedObject.FindProperty("healthRestoreAmount");

        doesRestoreBurnedHealth = serializedObject.FindProperty("doesRestoreBurnedHealth");
        burnedHealthRestoreAmount = serializedObject.FindProperty("burnedHealthRestoreAmount");

        doesDamageAbsorb = serializedObject.FindProperty("doesDamageAbsorb");
        damageAbsorbPercent = serializedObject.FindProperty("damageAbsorbPercent");
        damageAbsorbDuration = serializedObject.FindProperty("damageAbsorbDuration");

        doesHealthRegeneration = serializedObject.FindProperty("doesHealthRegeneration");
        regenerationPerSecond = serializedObject.FindProperty("regenerationPerSecond");
        regenerationDuration = serializedObject.FindProperty("regenerationDuration");

        doesCureDebuffs = serializedObject.FindProperty("doesCureDebuffs");
        debuffCures = serializedObject.FindProperty("debuffCures");
    }

    public override void OnInspectorGUI()
    {
        MedicalItem medicalItem = (MedicalItem)target;

        serializedObject.Update();

        // ItemBase
        itemBaseGroup = EditorGUILayout.BeginFoldoutHeaderGroup(itemBaseGroup, "ItemBase properties");
        if (itemBaseGroup)
        {
            EditorGUILayout.PropertyField(GrabPointsParent);
            EditorGUILayout.PropertyField(RightArmOnly);

            EditorGUILayout.PropertyField(hipPositionTransform);
            EditorGUILayout.PropertyField(hiddenTransform);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(10);


        // General
        generalMedicalItemGroup = EditorGUILayout.BeginFoldoutHeaderGroup(generalMedicalItemGroup, "General functionality");
        if (generalMedicalItemGroup)
        {
            EditorGUILayout.PropertyField(timeToUseItem);
            EditorGUILayout.PropertyField(isQuickAction);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(10);


        medicalItemEffectsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(medicalItemEffectsGroup, "Item Effects");
        if (medicalItemEffectsGroup)
        {
            // Health Restoring
            EditorGUILayout.PropertyField(doesRestoreHealth);
            if (medicalItem.DoesRestoreHealth)
            {
                EditorGUILayout.PropertyField(healthRestoreAmount);
            }

            EditorGUILayout.Space(10);

            // Burned Health Restoring
            EditorGUILayout.PropertyField(doesRestoreBurnedHealth);
            if (medicalItem.DoesRestoreBurnedHealth)
            {
                EditorGUILayout.PropertyField(burnedHealthRestoreAmount);
            }

            EditorGUILayout.Space(10);

            // Damage Absorb
            EditorGUILayout.PropertyField(doesDamageAbsorb);
            if (medicalItem.DoesDamageAbsorb)
            {
                EditorGUILayout.PropertyField(damageAbsorbPercent);
                EditorGUILayout.PropertyField(damageAbsorbDuration);
            }

            EditorGUILayout.Space(10);

            // Health Regeneration
            EditorGUILayout.PropertyField(doesHealthRegeneration);
            if (medicalItem.DoesHealthRegeneration)
            {
                EditorGUILayout.PropertyField(regenerationPerSecond);
                EditorGUILayout.PropertyField(regenerationDuration);
            }

            EditorGUILayout.Space(10);

            // Debuff Curing
            EditorGUILayout.PropertyField(doesCureDebuffs);
            if (medicalItem.DoesCureDebuffs)
            {
                EditorGUILayout.PropertyField(debuffCures);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
