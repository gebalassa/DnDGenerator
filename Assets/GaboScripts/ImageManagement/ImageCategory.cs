using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImageCategory
{
    public string categoryName;
    public List<ImageDnd> images;

    public ImageDnd GetImage(string Id)
    {
        foreach (ImageDnd image in images)
        {
            if (image.Id == Id)
            {
                return image;
            }
        }
        Debug.LogError("ImageCategory: Image with id " + Id + " in category " +
            categoryName + " not found!");
        return null;
    }
    public ImageDnd GetImage(Sprite sprite)
    {
        string spriteId = ImageUtilities.CreateUniqueId(sprite);
        foreach (ImageDnd image in images)
        {
            if (image.Id == spriteId)
            {
                return image;
            }
        }
        Debug.LogError("ImageCategory: Image with id " + spriteId + " in category " +
            categoryName + " not found!");
        return null;
    }

    public void AddImage(Sprite sprite)
    {
        ImageDnd newImage = new(sprite);
        images.Add(newImage);
    }
}
