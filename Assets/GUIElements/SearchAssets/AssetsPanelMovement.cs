using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssetsPanelMovement : MonoBehaviour
{
    [SerializeField] bool display = false;
    [SerializeField] float speedFactor = 1f;

    [SerializeField] RectTransform panel;
    [SerializeField] TMP_InputField searchBar;

    Vector2 visibleAnchorMin = new Vector2(0.75f, 0);
    Vector2 visibleAnchorMax = new Vector2(1, 0.95f);
    Vector2 hiddenAnchorMin = new Vector2(1f, 0);
    Vector2 hiddenAnchorMax = new Vector2(1.25f, 0.95f);

    // Start is called before the first frame update
    void Start()
    {
        if (display)
        {
            panel.anchorMin = visibleAnchorMin;
            panel.anchorMax = visibleAnchorMax;
        }
        else
        {
            panel.anchorMin = hiddenAnchorMin;
            panel.anchorMax = hiddenAnchorMax;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*SearchBarAtributes sba = searchBar.GetComponent<SearchBarAtributes>();
        
        //CHANGE STATE WETHER THEY ARE LOOKING FOR SOMETHING OR NOT
        if (!display && (searchBar.text != string.Empty || sba.IsSelected()))
        {
            display = true;
        }
        else if(display && searchBar.text == string.Empty && !sba.IsSelected())
        {
            display = false;
        }//*/

        //MOVE PANEL
        if (display && panel.anchorMin.x != visibleAnchorMin.x)
        {
            Vector2 speedVector = new Vector2(-0.1f * speedFactor * Time.deltaTime, 0);

            panel.anchorMin += speedVector;
            panel.anchorMax += speedVector;

            if (panel.anchorMin.x < visibleAnchorMin.x)
            {
                panel.anchorMin = visibleAnchorMin;
                panel.anchorMax = visibleAnchorMax;
            }
        }
        else if (!display && panel.anchorMin.x != hiddenAnchorMin.x)
        {
            Vector2 speedVector = new Vector2(0.1f * speedFactor * Time.deltaTime, 0);

            panel.anchorMin += speedVector;
            panel.anchorMax += speedVector;

            if (panel.anchorMin.x > hiddenAnchorMin.x)
            {
                panel.anchorMin = hiddenAnchorMin;
                panel.anchorMax = hiddenAnchorMax;
            }
        }
    }

    public void SwitchDisplayValue()
    {
        display = !display;
    }
}
