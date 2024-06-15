using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewDatabase", menuName = "ImageDatabase")]
public class ImageDatabase : ScriptableObject
{
    // IMAGE DATABASE CONSTANTS
    public const int DEFAULT_PIXEL_HEIGHT = 32;
    public const int DEFAULT_PIXEL_WIDTH = 32;
    //
    public List<ImageCategory> categories;
    [NonSerialized]
    public Dictionary<string, ImageDnd> imageDictionary; // For quick lookups.

    // Initializes images.
    // Helps initialize images which were added through Inspector, which doesn't run their constructor.
    public void Initialize()
    {
        foreach (ImageCategory category in categories)
        {
            foreach (ImageDnd image in category.images)
            {
                image.Initialize();
            }
        }
        UpdateDictionary();
    }

    public void AddImage(Sprite sprite, string categoryName)
    {
        foreach (ImageCategory category in categories)
        {
            if (category.categoryName == categoryName)
            {
                ImageDnd resultingImage = category.AddImage(sprite);
                UpdateDictionary(resultingImage);
                return;
            }
        }
    }

    public ImageDnd GetImage(string Id)
    {
        ImageDnd image;
        imageDictionary.TryGetValue(Id, out image);
        if (image != null)
        {
            return image;
        }
        else
        {
            Debug.LogError("ImageDatabase: Image with id " + Id + "not found!");
            return null;
        }
    }
    public ImageDnd GetImage(Sprite sprite)
    {
        string currId = ImageUtilities.GetUniqueId(sprite);
        return GetImage(currId);
    }

    public ImageDnd[,] GetImageArray(string Id)
    {
        ImageDnd currImage = imageDictionary[Id];

        // Get sub-images
        List<ImageDnd> subImages = new();
        foreach (string currId in currImage.subImageIds)
        {
            subImages.Add(imageDictionary[currId]);
        }

        // Length check
        if (subImages.Count != currImage.rows * currImage.columns) { Debug.LogError("ImageDatabase.GetImageArray(): GetSubImages().Count != rows*columns"); }

        // Generate array
        ImageDnd[,] newArray = new ImageDnd[currImage.rows, currImage.columns];
        for (int i = 0; i < subImages.Count; i++)
        {
            newArray[i / currImage.columns, i % currImage.columns] = subImages[i];
        }
        return newArray;
    }
    public ImageDnd[,] GetImageArray(Sprite sprite)
    {
        string currId = ImageUtilities.GetUniqueId(sprite);
        return GetImageArray(currId);
    }



    private void UpdateDictionary()
    {
        // Initialize dictionry
        if (imageDictionary == null)
        {
            imageDictionary = new();
        }

        foreach (ImageCategory category in categories)
        {
            foreach (ImageDnd image in category.images)
            {
                // Null id check
                if (image.Id == null) { Debug.LogError("ImageDatabase: Image with null id found while updating dictionary!"); continue; }

                // Add image (if repeated, ignore)
                bool result = imageDictionary.TryAdd(image.Id, image);
                //if (!result)
                //{
                //    Debug.LogError("ImageDatabase: Image with id " + image.Id + " already exists in dictionary!");
                //}

                // Add sub-images if not single image
                if (image.rows == 1 && image.columns == 1) { continue; }
                
                List<ImageDnd> subImages = image.GetSubImagesFromScratch();
                foreach (ImageDnd subImage in subImages)
                {
                    // Ignore self (Note: Throw error, this should be done inside ImageDnd, not here)
                    if (image.Id == subImage.Id) { Debug.LogError("FIX!!! ImageDatabase:UpdateDictionary(): Ignoring self should be done in ImageDnd!"); continue; }
                    // Add sub-image (if repeated, ignore)
                    bool subResult = imageDictionary.TryAdd(subImage.Id, subImage);
                    //if (!subResult)
                    //{
                    //    Debug.LogError("ImageDatabase: Image id " + image.Id + " has a sub-image (" + subImage.Id + ") already in dictionary!");
                    //}
                }
            }
        }
    }
    // If image given, only updates that image.
    private void UpdateDictionary(ImageDnd newImage)
    {
        // Null image check
        if (newImage == null) { Debug.LogError("ImageDatabase: Null image"); return; }
        // Null Id check
        else if (newImage.Id == null) { Debug.LogError("ImageDatabase: Image with null id given to update dictionary!"); return; }

        // Add to dictionary (if repeated, ignore)
        bool result = imageDictionary.TryAdd(newImage.Id, newImage);
        //if (!result)
        //{
        //    Debug.LogError("ImageDatabase: Given image with id " + newImage.Id + " already exists in dictionary!");
        //}

        // Add sub-images if not single image
        if (newImage.rows == 1 && newImage.columns == 1) { return; }

        List<ImageDnd> subImages = newImage.GetSubImagesFromScratch();
        foreach (ImageDnd subImage in subImages)
        {
            // Ignore self (Note: Throw error, this should be done inside ImageDnd, not here)
            if (newImage.Id == subImage.Id) { Debug.LogError("FIX!!! ImageDatabase:UpdateDictionary(): Ignoring self should be done in ImageDnd!"); continue; }
            // Add sub-image (if repeated, ignore)
            bool subResult = imageDictionary.TryAdd(subImage.Id, subImage);
            //if (!subResult)
            //{
            //    Debug.LogError("ImageDatabase: Image id " + newImage.Id + " has a sub-image (" + subImage.Id + ") already in dictionary!");
            //}
        }
    }
}
