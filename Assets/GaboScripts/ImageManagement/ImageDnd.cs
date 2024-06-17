using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ImageDnd
{
    public Sprite sprite;
    // TODO: Usar subspriterefs
    // Sub-sprites
    public int rows = 1;
    public int columns = 1;
    [NonSerialized]
    public List<string> subImageIds; // Holds Ids of sub-sprites of Sprite

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
    private bool idAlreadySet;

    public ImageDnd(Sprite sprite)
    {
        Initialize(sprite);
    }

    // Instance initializer.
    // Used in constructor but can also replace it when instanced through Inspector,
    // which apparently can't call constructors with parameters.
    public void Initialize(Sprite sprite = null)
    {
        idAlreadySet = false;
        if (this.sprite == null && sprite == null)
        {
            this.Name = null;
            this.Id = null;
            this.subImageIds = null;
            Debug.LogError("ImageDnd: Initialize(): Sprite wasn't given in constructor nor set beforehand in Inspector!");
        }
        else
        {
            // If sprite is given, set as sprite.
            if (sprite != null)
            {
                this.sprite = sprite;
            }
            //// If the set sprite is multi-tile, the whole texture is used instead as sprite.
            //// NOTE: This is an ugly bypass to the fact Unity doesn't allow passing the whole texture as sprite if its sliced,
            //// choosing the first sub-sprite automatically.
            //if (rows > 1 || columns > 1)
            //{
            //    Sprite fullTextureSprite = Sprite.Create(this.sprite.texture, new Rect(0, 0, this.sprite.texture.width, this.sprite.texture.height), new Vector2(0.5f, 0.5f));
            //    this.sprite = fullTextureSprite;
            //}

            this.Name = this.sprite.name;
            this.Id = ImageUtilities.GetUniqueId(this.sprite);
            SetSubImageIds();
            Debug.Log("ImageDnd: Initialize(): - Initializing Image: " + Id);
        }
    }

    // Get sub-images by creating fresh ImageDnd objects (instead of using a dictionary)
    public List<ImageDnd> GetSubImagesFromScratch()
    {
        // Get sub-sprites
        Texture2D currTexture = sprite.texture;
        UnityEngine.Object[] subUncastedSprites = Resources.LoadAll<Sprite>(currTexture.name);

        // Create ImageDnd objects for each subsprite
        List<ImageDnd> subImages = new();
        foreach (UnityEngine.Object subUncastedSprite in subUncastedSprites)
        {
            Sprite subSprite = (Sprite)subUncastedSprite;
            ImageDnd newImage = new ImageDnd(subSprite);
            //Ignore self
            if (Id == newImage.Id) { continue; }
            // Add
            subImages.Add(newImage);
        }
        return subImages;
    }

    // Return 2D array with the sub-images properly placed (from scratch, without using dictionary).
    public ImageDnd[,] Get2DArrayFromScratch()
    {
        // Null check
        if (subImageIds == null) { Debug.LogError("ImageDnd:Get2DArray(): Sub images have not been set!"); }

        // Get sub-images
        List<ImageDnd> subImages = GetSubImagesFromScratch();

        // Length check
        if (subImages.Count != rows * columns) { Debug.LogError("Get2DArray(): GetSubImages().Count != rows*columns"); }

        // Generate array
        ImageDnd[,] newArray = new ImageDnd[rows, columns];
        for (int i = 0; i < subImages.Count; i++)
        {
            newArray[i / columns, i % columns] = subImages[i];
        }
        return newArray;
    }
    
    // Get array using given dictionary. Should be faster than creating the objects from scratch.
    public ImageDnd[,] Get2DArray(Dictionary<string, ImageDnd> imageDictionary)
    {
        // Null check
        if (subImageIds == null) { Debug.LogError("ImageDnd:Get2DArray(): Sub images have not been set!"); }

        // Get sub-images (using dictionary)
        List<ImageDnd> subImages = new();
        foreach (string currId in subImageIds)
        {
            subImages.Add(imageDictionary[currId]);
        }

        // Length check
        if (subImages.Count != rows * columns) { Debug.LogError("Get2DArray(): GetSubImages().Count != rows*columns"); }

        // Generate array
        ImageDnd[,] newArray = new ImageDnd[rows, columns];
        for (int i = 0; i < subImages.Count; i++)
        {
            newArray[i / rows, i % rows] = subImages[i];
        }
        return newArray;
    }

    private void SetSubImageIds()
    {
        // Null checks
        // Null sprite check
        if (sprite == null) { Debug.LogError("ImageUtilities: SetSubImagesIds(): Null root sprite."); }
        // Single image check
        if (rows == 1 || columns == 1) { subImageIds = null; return; }

        // Initialize empty
        subImageIds = new();
        // Get sub-images
        List<ImageDnd> subImages = GetSubImagesFromScratch();
        // Rows & Columns check
        if ((rows * columns) != subImages.Count) { Debug.LogError(string.Format("Sub-image number is not equal to (Rows*Columns) == {0}!", rows * columns)); }

        // Obtain Ids and add to subImageIds
        foreach (ImageDnd subImage in subImages)
        {
            subImageIds.Add(subImage.Id);
        }
    }

}
