using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableAssetGenerator : MonoBehaviour
{
    [SerializeField] ImageDatabase database;
    [SerializeField] GameObject draggableAssetPrefab;

    [SerializeField] int columns;
    [SerializeField] Vector2 offset;    //from up-left corner
    [SerializeField] RectTransform panel;
    [SerializeField] RectTransform panelContent;

    List<DraggableAsset> assets;

    void Start()
    {
        //GET ASSETS FROM DATABASE
        assets = new List<DraggableAsset>();

        foreach (ImageCategory category in database.categories)
        {
            foreach (ImageDnd image in category.images)
            {
                //Create new object
                GameObject newDragAsset = Instantiate(draggableAssetPrefab, panelContent);
                //Get DraggableAsset script
                DraggableAsset dragScript = newDragAsset.GetComponent<DraggableAsset>();
                //Rewrite values 
                dragScript.SetValues(image, category.categoryName, image.sprite.name);
                dragScript.UpdateObject();
                //Add object to the list
                assets.Add(dragScript);
            }
        }

        //REARRANGE ASSETS IN PANEL
        //Get pixel width of assetPanel
        float assetPanelWidth = (panel.anchorMax.x - panel.anchorMin.x) * Camera.main.pixelWidth;
        //Take away offset size
        float size = assetPanelWidth * (1 - offset.x);
        //Divide remaining size between columns
        size = size / columns;
        //Take 20% away
        size -= size * 0.25f;

        panelContent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(size, size);
        panelContent.GetComponent<GridLayoutGroup>().spacing = new Vector2(size * 0.2f, size * 0.2f);
        panelContent.GetComponent<GridLayoutGroup>().padding = new RectOffset( (int) (offset.x * assetPanelWidth), 0, (int)(offset.y * assetPanelWidth), (int)(offset.y * assetPanelWidth));

        foreach (DraggableAsset da in assets)
        {
            da.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);

            da.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(size - 20, size - 20);
            da.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(size/2, -(size-20)/2);
        }
    }

    private void Update()
    {
    }
}
