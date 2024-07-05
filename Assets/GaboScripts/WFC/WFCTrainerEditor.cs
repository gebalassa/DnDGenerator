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
    private SerializedProperty debugTrainingMapsProperty;
    private void OnEnable()
    {
        debugTrainingMapsProperty = serializedObject.FindProperty("debugTrainingMaps");
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        // "debugTrainingMaps" property
        PropertyField debugTrainingMapsField = new(debugTrainingMapsProperty);
        debugTrainingMapsField.Bind(serializedObject);
        root.Add(debugTrainingMapsField);

        //------------------CUSTOM----------------
        // "Train" Button
        Button trainButton = new Button(TrainTarget);
        trainButton.text = "Train";
        root.Add(trainButton);

        return root;
    }

    private void TrainTarget()
    {
        ((WFCTrainer)target).Train();
    }

}
