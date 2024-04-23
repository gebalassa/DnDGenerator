using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUtilities
{
    // Create unique Hash128 string based on a sprite's Texture2D byte data.
    public static string CreateUniqueId(Sprite sprite)
    {
        byte[] rawImage = sprite.texture.EncodeToPNG();
        Hash128 newId = Hash128.Compute(rawImage);
        //Debug.Log("ImageUtilities: - id: " + newId);
        return newId.ToString();
    }
}
