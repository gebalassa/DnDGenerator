using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchBarAtributes : MonoBehaviour
{
    bool isSelected = false;

    public void SelectBar()
    {
        isSelected = true;
    }
    public void UnSelectBar()
    {
        isSelected = false;
    }

    public bool IsSelected() {  return isSelected; }
}
