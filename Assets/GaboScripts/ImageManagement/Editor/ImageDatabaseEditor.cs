using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;


[CustomEditor(typeof(ImageDatabase))]
[System.Serializable]
public class ImageDatabaseEditor : Editor
{
    private SerializedProperty categoriesProperty;
    private SerializedProperty selectedCategoryProperty;

    //AUX
    DropdownField dropdownCategoryField;
    ObjectField spritesheetObjectField;

    private void OnEnable()
    {
        categoriesProperty = serializedObject.FindProperty("categories");
    }

    public override VisualElement CreateInspectorGUI()
    {

        VisualElement root = new VisualElement();

        // "Categories" List property
        PropertyField categoriesField = new(categoriesProperty);
        categoriesField.Bind(serializedObject);
        root.Add(categoriesField);

        //---------------CUSTOM-----------------------
        // "Spritesheet" object field
        spritesheetObjectField = new("New Spritesheet");
        spritesheetObjectField.objectType = typeof(Texture2D);
        root.Add(spritesheetObjectField);

        // "Category" dropdown field
        dropdownCategoryField = new("Category", GetCategoryNames(), 0);
        root.Add(dropdownCategoryField);

        // "Add Spritesheet" button
        Button addSpritesheetButton = new Button(AddSpritesheet);
        addSpritesheetButton.text = "Add spritesheet to selected category";
        root.Add(addSpritesheetButton);

        // Track changes and update when change is made
        root.TrackSerializedObjectValue(serializedObject, OnInspectorChange);
        return root;
    }

    private void OnInspectorChange(SerializedObject serialized)
    {
        // Update dropdownCategoryField
        dropdownCategoryField.choices = GetCategoryNames();
    }
    private List<String> GetCategoryNames()
    {
        List<String> categoryNameList = new();
        foreach (ImageCategory cat in ((ImageDatabase)target).categories)
        {
            categoryNameList.Add(cat.categoryName);
        }
        return categoryNameList;
    }

    private void AddSpritesheet()
    {
        Texture2D curr = (Texture2D)spritesheetObjectField.value;
        String path = AssetDatabase.GetAssetPath(curr);
        UnityEngine.Object[] sprites = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        foreach (UnityEngine.Object sprite in sprites)
        {
            Sprite newSprite = (Sprite)sprite;
            ((ImageDatabase)target).AddImage(newSprite, dropdownCategoryField.text);
            Debug.Log(sprite);
        }
        
    }

}
