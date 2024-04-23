using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{
    //TODO:
    // - Funcion para recortar spritesheets recibidos
    // - Inicializador

    public ImageDatabase db;
    public Sprite debugSprite;

    private void Start()
    {
        // Initialize database
        db.Initialize();

        // DEBUG
        ImageDnd img = db.GetImage(debugSprite);
        Debug.Log(img.sprite);
    }
}
