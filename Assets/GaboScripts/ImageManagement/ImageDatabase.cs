using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDatabase", menuName = "ImageDatabase")]
public class ImageDatabase : ScriptableObject
{
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
                return;
            }
        }
        UpdateDictionary();
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
        string currId = ImageUtilities.CreateUniqueId(sprite);
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
                }
                else
                {
                    Debug.LogError("ImageDatabase: Image with null id found while updating dictionary!");
                }
            }
        }
    }
}
