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
    private SerializedProperty trainingMapsProperty;
    private void OnEnable()
    {
        trainingMapsProperty = serializedObject.FindProperty(nameof(WFCTrainer.trainingMaps));
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

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
        ((WFCTrainer)target).Train();
    }

}
