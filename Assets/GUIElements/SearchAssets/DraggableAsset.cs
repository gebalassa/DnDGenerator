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

    private void Awake()
    {
        UpdateObject();
    }

    public void SetValues(ImageDnd imageClass, string category, string assetName)
    {
        this.imageClass = imageClass;
        this.category = category;
        this.assetName = assetName;
    }

    public void UpdateObject()
    {
        thumbnail = imageClass.sprite;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = assetName + " " + imageClass.columns + "x" + imageClass.rows;
        transform.GetChild(1).GetComponent<Image>().sprite = thumbnail;
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
