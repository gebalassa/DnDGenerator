using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDatabase", menuName = "ImageDatabase")]
public class ImageDatabase : ScriptableObject
{
    // IMAGE DATABASE CONSTANTS
    public const int DEFAULT_PIXEL_HEIGHT = 32;
    public const int DEFAULT_PIXEL_WIDTH = 32;
    //
    public List<ImageCategory> categories;
    private Dictionary<string, ImageDnd> imageDictionary; // For quick lookups.

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
                category.AddImage(sprite);
                UpdateDictionary();
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
        ImageDnd image;
        imageDictionary.TryGetValue(currId, out image);
        if (image != null)
        {
            return image;
        }
        else
        {
            Debug.LogError("ImageDatabase: Image with id " + currId + "not found!");
            return null;
        }
    }

    private void UpdateDictionary()
    {
        if (imageDictionary == null)
        {
            imageDictionary = new();
        }

        foreach (ImageCategory category in categories)
        {
            foreach (ImageDnd image in category.images)
            {
                if (image.Id != null)
                {
                    bool result = imageDictionary.TryAdd(image.Id, image);
                    if (!result)
                    {
                        Debug.LogError("ImageDatabase: Image with id " + image.Id + " already exists in dictionary!");
                    }
                    // Add sub-images if not single image
                    if (image.rows > 1 || image.columns > 1)
                    {
                        List<ImageDnd> subImages = image.GetSubImages();

                        foreach (ImageDnd subImage in subImages)
                        {
                            // Ignore self
                            if (image.Id == subImage.Id) { continue; }
                            // Add sub-image
                            bool subResult = imageDictionary.TryAdd(subImage.Id, subImage);
                            if (!subResult)
                            {
                                Debug.LogError("ImageDatabase: Image id " + image.Id + " has a sub-image (" + subImage.Id + ") already in dictionary!");
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("ImageDatabase: Image with null id found while updating dictionary!");
                }
            }
        }
    }
}
