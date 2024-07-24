using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class DropdownManager : MonoBehaviour
{
    [SerializeField] ManagerReferences references;

    [SerializeField] TMP_Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {

        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
        foreach (ImageCategory cat in references.database.categories)
        {
            list.Add(new TMP_Dropdown.OptionData(cat.categoryName));
        }

        dropdown.options = list;
    }

}
