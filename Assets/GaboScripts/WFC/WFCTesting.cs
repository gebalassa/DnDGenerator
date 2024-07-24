using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

#if UNITY_EDITOR
public class WFCTesting : MonoBehaviour
{
    //public WFCTrainer wfcTrainer;
    public List<Sprite> testSprites = new();
    private int oldCount = 0;

    private void OnValidate()
    {
        if (testSprites.Count > oldCount)
        {
            if (testSprites[testSprites.Count - 1] != null)
            {
                Debug.Log($"Hash128: {ImageUtilities.GetUniqueId(testSprites[testSprites.Count - 1])} Name: {testSprites[testSprites.Count - 1].name}");
                oldCount = testSprites.Count;
            }
            //foreach (var sprite in testSprites)
            //{
            //    if (sprite != null) { Debug.Log($"Hash128: {ImageUtilities.GetUniqueId(sprite)} Name: {sprite.name}"); }
            //}
        }
    }
}
#endif
