using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WFCTesting : MonoBehaviour
{
    //public WFCTrainer wfcTrainer;
    public List<Sprite> testSprites = new();

    private void OnValidate()
    {
        if (testSprites.Count > 0)
        {
            if (testSprites[testSprites.Count - 1] != null)
            {
                Debug.Log($"Hash128: {ImageUtilities.GetUniqueId(testSprites[testSprites.Count - 1])} Name: {testSprites[testSprites.Count - 1].name}"); 
            }
            //foreach (var sprite in testSprites)
            //{
            //    if (sprite != null) { Debug.Log($"Hash128: {ImageUtilities.GetUniqueId(sprite)} Name: {sprite.name}"); }
            //}
        }
    }
}
