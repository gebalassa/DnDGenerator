using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Image
{
    private int _id;
    private bool idAlreadySet = false;
    public int Id
    {
        get
        {
            return _id;
        }
        set
        {
            if (idAlreadySet)
            {
                Debug.LogError("Image: Trying to change already set id! Currently " + _id);
            }
            else
            {
                _id = value;
                idAlreadySet = true;
            }
        }
    }
    public Sprite sprite;

    public Image(Sprite sprite, int Id)
    {
        this.sprite = sprite;
        this._id = Id;
    }
}
