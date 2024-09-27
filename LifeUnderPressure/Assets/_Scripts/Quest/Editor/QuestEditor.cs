using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    private SerializedProperty questType;
    private SerializedProperty fishes;
    private SerializedProperty rewards;

    private SerializedProperty audioOnAssign;
    private SerializedProperty audioOnProgress;
    private SerializedProperty audioOnEnd;
    private void OnEnable()
    {
        questType = serializedObject.FindProperty("type");
        fishes = serializedObject.FindProperty("fishes");
        rewards = serializedObject.FindProperty("rewards");

        audioOnAssign = serializedObject.FindProperty("audioOnAssign");
        audioOnProgress = serializedObject.FindProperty("audioOnProgress");
        audioOnEnd = serializedObject.FindProperty("audioOnEnd");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(questType, new GUIContent("Quest Type"));

        if(questType.enumValueFlag == (uint)Quest.QuestType.None)
        {
            return;
        }
        else if (questType.enumValueFlag == ((uint)Quest.QuestType.Scan))
        {
            EditorGUILayout.PropertyField(fishes, new GUIContent("Fishes to Scan"));
        }
        else if(questType.enumValueFlag == ((uint)Quest.QuestType.Capture))
        {
            EditorGUILayout.PropertyField(fishes, new GUIContent("Fishes to Capture"));
        }
        EditorGUILayout.LabelField("Rewards", EditorStyles.boldLabel);
        if (rewards.arraySize < 5 && GUILayout.Button("Add Reward"))
        {
            rewards.InsertArrayElementAtIndex(rewards.arraySize);
        }
        for (int i = 0; i < rewards.arraySize; i++)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            SerializedProperty reward = rewards.GetArrayElementAtIndex(i);

            SerializedProperty rewardType = reward.FindPropertyRelative("type");
            EditorGUILayout.PropertyField(rewardType, new GUIContent("Reward Type"));

            if (rewardType.enumValueFlag == ((uint)Quest.RewardType.Money))
            {
                SerializedProperty value = reward.FindPropertyRelative("value");
                EditorGUILayout.PropertyField(value, new GUIContent("Money"));
                value.intValue = Mathf.Clamp(value.intValue, 0, 1000);
            }
            else
            {
                SerializedProperty boolName = reward.FindPropertyRelative("boolName");
                EditorGUILayout.PropertyField(boolName, new GUIContent("Bool Name"));

                SerializedProperty value = reward.FindPropertyRelative("value");
                EditorGUILayout.PropertyField(value, new GUIContent("Value"));
                value.intValue = Mathf.Clamp(value.intValue, 0, 1);
            }

            if (GUILayout.Button("Remove"))
            {
                rewards.DeleteArrayElementAtIndex(i);
            }
        }

        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(audioOnAssign, new GUIContent("On Quest Assign"));
        EditorGUILayout.PropertyField(audioOnProgress, new GUIContent("On Quest Progress"));
        EditorGUILayout.PropertyField(audioOnEnd, new GUIContent("On Quest End"));

        serializedObject.ApplyModifiedProperties();
    }
}
