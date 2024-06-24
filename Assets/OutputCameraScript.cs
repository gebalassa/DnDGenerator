using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class OutputCameraScript : MonoBehaviour
{
    public Vector2Int tileResolution;
    Camera cam;
    RenderTexture outputTexture;

    bool exportImage = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        outputTexture = cam.targetTexture;
        Camera.onPostRender += OnPostRenderCallback;
    }

    public void ResizeCamera(GridManager gm)
    {
        outputTexture.width = gm.GetDimensions().x * (int)gm.GetAssetMap().cellSize.x * tileResolution.x;
        outputTexture.height = gm.GetDimensions().y * (int)gm.GetAssetMap().cellSize.y * tileResolution.y;

        cam.transform.position = new Vector3(gm.GetDimensions().x / 2, gm.GetDimensions().y / 2, -10);
        cam.orthographicSize = gm.GetDimensions().x / 2;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.O))
        {
            exportImage = true;
        }
    }

    void OnPostRenderCallback(Camera cam)
    {
        if (exportImage)
        {
            if(cam == this.cam)
            {
                Texture2D outputImage = new Texture2D(outputTexture.width, outputTexture.height);
                outputImage.ReadPixels(new Rect(0, 0, outputTexture.width, outputTexture.height), 0, 0);
                outputImage.Apply();
                outputImage.name = "Output Image";

                Debug.Log(Application.dataPath);
                File.WriteAllBytes(Application.dataPath + "/" + outputImage.name + ".png", outputImage.EncodeToPNG());
                exportImage = false;
            }
        }
    }
}
