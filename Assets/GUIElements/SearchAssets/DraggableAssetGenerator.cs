using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableAssetGenerator : MonoBehaviour
{
    [SerializeField] ImageDatabase database;
    [SerializeField] GameObject draggableAssetPrefab;

    [SerializeField] Transform panel;
    [SerializeField] Transform panelContent;

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
                GameObject newDragAsset = Instantiate(draggableAssetPrefab, panel);
                //Get DraggableAsset script
                DraggableAsset dragScript = newDragAsset.GetComponent<DraggableAsset>();
                //Rewrite values 
                dragScript.SetValues(image, category.categoryName, image.sprite.name);
                //Add object to the list
                assets.Add(dragScript);
            }
        }

        //REARRANGE ASSETS IN PANEL


    }
}
