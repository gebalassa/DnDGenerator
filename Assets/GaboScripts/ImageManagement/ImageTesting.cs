using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tests for ImageDatabase family of classes
public class ImageTesting : MonoBehaviour
{
    public ImageDatabase imageDatabase;
    public ImageDnd testImage;

    private void Start()
    {
        TestGetSubSpriteIds();
    }

    // Obtain and instantiate all the sub-sprites of an image on the scene.
    public void TestGetSubSpriteIds()
    {
        // Initialize test image and db
        testImage.Initialize();
        imageDatabase.Initialize();

        // Instantiate sub-images
        int counter = 0;
        foreach (string currId in testImage.subImageIds)
        {
            // Create new scene object
            GameObject newImage = new GameObject();
            SpriteRenderer newSpriteRenderer = newImage.AddComponent<SpriteRenderer>();
            newSpriteRenderer.sprite = imageDatabase.GetImage(currId).sprite;

            // Set position
            float x = (float)(counter % testImage.rows) * (newSpriteRenderer.sprite.rect.width / newSpriteRenderer.sprite.pixelsPerUnit);
            float y = -1 * (float)(counter / testImage.rows) * (newSpriteRenderer.sprite.rect.height / newSpriteRenderer.sprite.pixelsPerUnit);
            newImage.transform.position = new Vector3(x, y, 0);
            counter++;
        }
    }
}
