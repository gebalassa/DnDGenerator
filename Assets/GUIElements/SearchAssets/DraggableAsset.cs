using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DraggableAsset : MonoBehaviour
{
    //PROBABLEMENTE SEA NECESARIO CAMBIARLO LUEGO QUE SE CAMBIE LA CLASE IMAGEDND O ALGO NOSE, MUCHO CUIDADO
    [SerializeField] Sprite thumbnail;
    [SerializeField] new string name;

    [SerializeField] List<Sprite> images;
    [SerializeField] int width;
    [SerializeField] int height;



    public void UpdateObject()
    {
        GetComponent<Image>().sprite = thumbnail;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name + " " + width + "x" + height;
    }

    private void Awake()
    {
        UpdateObject();
    }

    public int Width()  { return width; }
    public int Height() { return height; }
    public Sprite Thumbnail() { return thumbnail; }
}
