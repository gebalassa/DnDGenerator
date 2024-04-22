using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImageCategory
{
    public string categoryName;
    public List<Image> images;
    private int idCounter;

    public Image GetImage(int Id)
    {
        foreach (Image image in images)
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

    public void AddImage(Sprite sprite)
    {
        Image newImage = new(sprite, idCounter);
        images.Add(newImage);
        idCounter++;
    }
}
