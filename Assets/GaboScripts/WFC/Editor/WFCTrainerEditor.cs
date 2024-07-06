using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(WFCTrainer))]
[System.Serializable]
public class WFCTrainerEditor : Editor
{
    private SerializedProperty tileAssociationsProperty;
    private SerializedProperty tileFrequenciesProperty;
    private SerializedProperty trainingMapsProperty;
    private void OnEnable()
    {
        tileAssociationsProperty = serializedObject.FindProperty(nameof(WFCTrainer.tileAssociations));
        tileFrequenciesProperty = serializedObject.FindProperty(nameof(WFCTrainer.tileFrequencies));
        trainingMapsProperty = serializedObject.FindProperty(nameof(WFCTrainer.trainingMaps));
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        // "tileAssociations" property
        PropertyField tileAssociationsField = new(tileAssociationsProperty);
        tileAssociationsField.Bind(serializedObject);
        root.Add(tileAssociationsField);

        // "tileFrequencies" property
        PropertyField tileFrequenciesField = new(tileFrequenciesProperty);
        tileFrequenciesField.Bind(serializedObject);
        root.Add(tileFrequenciesField);

        // "trainingMaps" property
        PropertyField trainingMapsField = new(trainingMapsProperty);
        trainingMapsField.Bind(serializedObject);
        root.Add(trainingMapsField);

        //------------------CUSTOM----------------
        // "Train" Button
        Button trainButton = new Button(TrainTarget);
        trainButton.text = "Train";
        root.Add(trainButton);

        return root;
    }

    private void TrainTarget()
    {
        serializedObject.Update();
        ((WFCTrainer)target).Train();
        serializedObject.ApplyModifiedProperties();
    }

}
