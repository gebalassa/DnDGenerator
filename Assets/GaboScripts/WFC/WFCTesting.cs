using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCTesting : MonoBehaviour
{
    public WFCTrainer wfcTrainer;
    private int a = 6;
    public int A
    {
        get
        {
            return a;
        }
        set
        {
            a = value;
        }
    }
}
