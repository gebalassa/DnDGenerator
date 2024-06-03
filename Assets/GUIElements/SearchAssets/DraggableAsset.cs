using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DraggableAsset : MonoBehaviour
{
    //PROBABLEMENTE SEA NECESARIO CAMBIARLO LUEGO QUE SE CAMBIE LA CLASE IMAGEDND O ALGO NOSE, MUCHO CUIDADO
    [SerializeField] ImageDnd imageClass;
    [SerializeField] string category;
    [SerializeField] string assetName;

    Sprite thumbnail;

    public void SetValues(ImageDnd imageClass, string category, string assetName)
    {
        this.imageClass = imageClass;
        this.category = category;
        this.assetName = assetName;
    }

    private void Awake()
    {
        thumbnail = imageClass.sprite;
        UpdateObject();
    }

    public void UpdateObject()
    {
        GetComponent<Image>().sprite = thumbnail;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = assetName + " " + imageClass.columns + "x" + imageClass.rows;
    }

    public int Columns()  { return imageClass.columns; }
    public int Rows() { return imageClass.rows; }
    public ImageDnd ImageClass() { return imageClass; }
    public Sprite Thumbnail() { return thumbnail; }

    public List<ImageDnd> GetSubImages(ImageDatabase database)
    {
        List<ImageDnd> images = new List<ImageDnd>();
        foreach(string id in imageClass.subImageIds)
        {
            images.Add(database.GetImage(id));
        }
        return images;
    }
}
