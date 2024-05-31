using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ImageUtilities
{
    // Create unique Hash128 string based on a sprite's Texture2D byte data.
    public static string GetUniqueId(Sprite sprite)
    {
        // Create new texture based only on the rectangle used by the sprite
        Texture2D newTexture = new((int)sprite.rect.width, (int)sprite.rect.height);
        Graphics.CopyTexture(sprite.texture, 0, 0, (int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height, newTexture, 0, 0, 0, 0);
        //Color[] pixels = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
        //newTexture.SetPixels(pixels);

        byte[] rawImage = newTexture.EncodeToPNG();
        Hash128 newId = Hash128.Compute(rawImage);
        //Debug.Log("ImageUtilities: - id: " + newId);
        return newId.ToString();
    }

    //// Divide ImageDnd into sub-tiles and return their Hash128 Ids.
    //public static List<string> GetSubImageIds(ImageDnd img)
    //{
    //    // Variables
    //    int rows = img.rows;
    //    int columns = img.columns;
    //    Sprite sprite = img.sprite;

    //    // Null check
    //    if (sprite == null)
    //    {
    //        Debug.LogError("ImageUtilities: SetSubSprites(): Null sprite.");
    //    }

    //    // Correct proportions check
    //    if (sprite.texture.height % rows != 0 || sprite.texture.width % columns != 0)
    //    {
    //        Debug.LogError(string.Format("ImageUtilities: SetSubSprites(): Can't divide {0}x{1} texture by {2}x{3}.",
    //            sprite.texture.width, sprite.texture.height, columns, rows));
    //    }

    //    // Create image sub-tiles
    //    List<Sprite> subSpriteList = new List<Sprite>();
    //    // Loop
    //    for (int i = 0; i < rows; i++)
    //    {
    //        for (int j = 0; j < columns; j++)
    //        {
    //            int resultingHeight = sprite.texture.height / rows;
    //            int resultingWidth = sprite.texture.width / columns;
    //            Rect newRect = new Rect(i*resultingWidth, j*resultingHeight, resultingWidth, resultingHeight);
    //            Sprite newSubSprite = Sprite.Create(sprite.texture, newRect, new Vector2(0.5f, 0.5f));
    //            subSpriteList.Add(newSubSprite);
    //        }
    //    }

    //    // Obtain references from obtained sub-tiles
    //    List<string> subImageIds = new List<string>();
    //    foreach (Sprite subSprite in subSpriteList)
    //    {
    //        string newId = ImageUtilities.GetUniqueId(subSprite);
    //        subImageIds.Add(newId);
    //    }

    //    return subImageIds;
    //}
}
