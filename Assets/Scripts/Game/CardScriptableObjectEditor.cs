using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardScriptableObject))]
public class CardScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CardScriptableObject card = (CardScriptableObject)target;

        // === COMMON PROPERTIES (always visible) ===
        EditorGUILayout.LabelField("Common Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cardSprite"));
        EditorGUILayout.Space(10);

        // === CARD TYPE ===
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cardType"));
        EditorGUILayout.Space(10);

        // === CONDITIONAL FIELDS BASED ON CARD TYPE ===
        switch (card.cardType)
        {
            case CardType.HeroCard:
                EditorGUILayout.LabelField("Hero Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("heroCardType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("heroClassType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("heroAge"));
                break;

            case CardType.MoveCard:
                EditorGUILayout.LabelField("Move Card Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("moveCardType"));
                break;

            case CardType.SpecialCard:
                EditorGUILayout.LabelField("Special Card Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("specialCardType"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
