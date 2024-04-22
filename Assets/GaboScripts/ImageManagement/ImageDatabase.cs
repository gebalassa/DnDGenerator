using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDatabase", menuName ="ImageDatabase")]
public class ImageDatabase : ScriptableObject
{
    public List<ImageCategory> categories;

    //TODO
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

    public Image GetImage(string categoryName, int Id)
    {
        foreach (ImageCategory category in categories)
        {
            if (category.categoryName == categoryName)
            {
                Image img = category.GetImage(Id);
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
}
