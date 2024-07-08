using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class OutputCameraScript : MonoBehaviour
{
    public Vector2Int tileResolution;
    Camera cam;
    RenderTexture outputTexture;

    bool exportSignal = false;
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

        cam.transform.position = new Vector3(gm.GetDimensions().x / 2, gm.GetDimensions().y / 2, transform.position.z);
        cam.orthographicSize = gm.GetDimensions().x / 2;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(exportSignal)
        {
            exportSignal = false;
            exportImage = true;
        }
    }

    void OnPostRenderCallback(Camera cam)
    {
        if (exportImage)
        {
            if(cam == this.cam)
            {
                Texture2D outputImage = new Texture2D(outputTexture.width, outputTexture.height, GraphicsFormat.B8G8R8A8_SRGB, TextureCreationFlags.None);
                outputImage.ReadPixels(new Rect(0, 0, outputTexture.width, outputTexture.height), 0, 0, true);
                outputImage.Apply();
                outputImage.name = "Output Image";

                string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "map", "png");
                if (!string.IsNullOrEmpty(path))
                {
                    Debug.Log(path);
                    File.WriteAllBytes(path, outputImage.EncodeToPNG());
                }
            }
            exportImage = false;
        }
    }

    public void ExportImage()
    {
        exportSignal = true;
    }
}
