using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManagerReferences : MonoBehaviour
{
    public GraphicRaycaster gRaycaster;
    public EventSystem eventSystem;
    public WFCManager wfcManager;
    public ImageDatabase database;

    private void Awake()
    {
        database.Initialize();
    }
}
