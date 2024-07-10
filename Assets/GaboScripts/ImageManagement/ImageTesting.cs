#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static WFCTrainer;


// Tests for ImageDatabase family of classes
public class ImageTesting : MonoBehaviour
{
    public ImageDatabase imageDatabase;
    public ImageDnd testImage;

    private void Start()
    {
        //TestGetSubSpriteIds();
        //TestGet2DArray();
        TestGridClass();
    }

    // Obtain and instantiate all the sub-sprites of an image on the scene.
    public void TestGetSubSpriteIds()
    {
        // Initialize test image and db
        //testImage.Initialize();
        //imageDatabase.Initialize();

        // Instantiate sub-images
        int counter = 0;
        foreach (string currId in testImage.subImageIds)
        {
            // Create new scene object
            GameObject newImage = new GameObject();
            SpriteRenderer newSpriteRenderer = newImage.AddComponent<SpriteRenderer>();
            newSpriteRenderer.sprite = imageDatabase.GetImage(currId).sprite;

            // Set position
            float x = (float)(counter % testImage.rows * (newSpriteRenderer.sprite.rect.width / newSpriteRenderer.sprite.pixelsPerUnit));
            float y = -1 * (float)(counter / testImage.rows * (newSpriteRenderer.sprite.rect.height / newSpriteRenderer.sprite.pixelsPerUnit));
            newImage.transform.position = new Vector3(x, y, 0);
            counter++;
        }
    }
    public void TestGet2DArray()
    {
        // Initialize test image and db
        testImage.Initialize();
        imageDatabase.Initialize();

        // Get array
        //ImageDnd[,] currArray = testImage.Get2DArrayFromScratch();
        //ImageDnd[,] currArray = testImage.Get2DArray(imageDatabase.imageDictionary);
        ImageDnd[,] currArray = imageDatabase.GetImageArray(testImage.Id);

        for(int i = 0; i < currArray.GetLength(0); i++)
        {
            for(int j = 0; j < currArray.GetLength(1); j++)
            {
                // Create new scene object
                GameObject newImage = new GameObject();
                SpriteRenderer newSpriteRenderer = newImage.AddComponent<SpriteRenderer>();
                newSpriteRenderer.sprite = currArray[i, j].sprite;

                // Set position
                float x = (float) (j * (newSpriteRenderer.sprite.rect.width / newSpriteRenderer.sprite.pixelsPerUnit));
                float y = (float) (-1 * i * (newSpriteRenderer.sprite.rect.height / newSpriteRenderer.sprite.pixelsPerUnit));
                newImage.transform.position = new Vector3(x, y, 0);
            }
        }

    }
    public void TestGridClass()
    {
        // Initialize test image and db
        testImage.Initialize();
        imageDatabase.Initialize();

        string mapsPath = "Assets/Maps";
        string[] mapGuids = AssetDatabase.FindAssets("t:TextAsset", new string[] { mapsPath });
        if (mapGuids.Length == 0) { Debug.LogWarning("Couldn't find any maps!"); }

        List<GridClassNameWrapper> maps = new List<GridClassNameWrapper>();
        foreach (string mapGuid in mapGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(mapGuid);
            TextAsset currTextAssetMap = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (currTextAssetMap != null)
            {
                string json = currTextAssetMap.text;
                GridHelper gh = JsonUtility.FromJson<GridHelper>(json);
                GridClass currMap = gh.ConvertToGridClass();
                maps.Add(new GridClassNameWrapper(currMap, currTextAssetMap.name));
            }
            else
            {
                Debug.LogWarning("Couldn't cast object " + path + " as Text Asset!");
            }
        }

        // Instantiate grid class
        // NOTA****Por la forma rara en que instancia
        // (hacia arriba primero en la columna, y asi hacia la derecha)
        // entonces i depende del width (x), y j del height (y)
        GridClassNameWrapper chosenMap = maps[0];
        for (int i = 0; i < chosenMap.gc.width; i++)
        {
            for (int j = 0; j < chosenMap.gc.height; j++)
            {
                // Create new scene object
                GameObject newImage = new GameObject();
                SpriteRenderer newSpriteRenderer = newImage.AddComponent<SpriteRenderer>();
                if (chosenMap.gc.Grid[i, j].Id != "none" && chosenMap.gc.Grid[i, j].Id != "wall")
                {
                    newSpriteRenderer.sprite = imageDatabase.GetImage(chosenMap.gc.Grid[i, j].Id).sprite;
                    // Set position
                    float x = (float)(i * (newSpriteRenderer.sprite.rect.width / newSpriteRenderer.sprite.pixelsPerUnit));
                    float y = (float)(j * (newSpriteRenderer.sprite.rect.height / newSpriteRenderer.sprite.pixelsPerUnit));
                    newImage.transform.position = new Vector3(x, y, 0);
                }
            }
        }
    }
}
#endif
