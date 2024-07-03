using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{
    public ImageDatabase db;
    public Sprite debugSprite = null;

    private void Start()
    {
        // Initialize database
        db.Initialize();

        // DEBUG
        if(debugSprite != null)
        {
            ImageDnd img = db.GetImage(debugSprite);
            Debug.Log(img.sprite);
        }
    }
}
