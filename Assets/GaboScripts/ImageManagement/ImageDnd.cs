using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[System.Serializable]
public class ImageDnd
{
    public Sprite sprite;
    public string Id
    {
        get
        {
            return _id;
        }
        set
        {
            if (idAlreadySet)
            {
                Debug.LogError("ImageDnd: Trying to change already set id! Currently " + _id);
            }
            else
            {
                _id = value;
                idAlreadySet = true;
            }
        }
    }
    private string _id;
    private string Name;
    private bool idAlreadySet = false;

    public ImageDnd(Sprite sprite)
    {
        this.sprite = sprite;
        this.Name = sprite.name;
        this._id = ImageUtilities.CreateUniqueId(sprite);
    }

    // To replace the constructor when instanced through Inspector, which can't call constructors by default.
    // Assumes sprite was already assigned in Inspector
    public void Initialize()
    {
        if (sprite == null) { Debug.LogError("ImageDnd: Initialize(): Sprite wasn't set beforehand in Inspector!"); return; }
        else
        {
            this.Name = sprite.name;
            this._id = ImageUtilities.CreateUniqueId(sprite);
            Debug.Log("ImageDnd: - Initializing Image: " + Id);
        }
    }

}
