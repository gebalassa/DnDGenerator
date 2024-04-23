using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDatabase", menuName = "ImageDatabase")]
public class ImageDatabase : ScriptableObject
{
    public List<ImageCategory> categories;

    // Initializes images.
    // Helps initialize images which were added through Inspector and didn't run their constructor.
    public void Initialize()
    {
        foreach (ImageCategory category in categories)
        {
            foreach (ImageDnd image in category.images)
            {
                image.Initialize();
            }
        }
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
    }

    public ImageDnd GetImage(string categoryName, string Id)
    {
        foreach (ImageCategory category in categories)
        {
            if (category.categoryName == categoryName)
            {
                ImageDnd img = category.GetImage(Id);
                if (img != null)
                {
                    return img;
                }
                else
                {
                    Debug.LogError("ImageDatabase: Image not found!");
                    return null;
                }
            }
        }
        Debug.LogError("ImageDatabase: Category not found!");
        return null;
    }
    public ImageDnd GetImage(string Id)
    {
        foreach (ImageCategory category in categories)
        {
            ImageDnd img = category.GetImage(Id);
            if (img != null)
            {
                return img;
            }
        }
        Debug.LogError("ImageDatabase: Image not found!");
        return null;
    }
    public ImageDnd GetImage(Sprite sprite)
    {
        foreach (ImageCategory category in categories)
        {
            ImageDnd img = category.GetImage(sprite);
            if (img != null)
            {
                return img;
            }
        }
        Debug.LogError("ImageDatabase: Image not found!");
        return null;
    }
}
