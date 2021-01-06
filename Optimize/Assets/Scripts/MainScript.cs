using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
// using Firebase.Storage;
using System.Threading;

public class MainScript : MonoBehaviour
{
    int numberOfCaptures = 10;

    bool status = true;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startCapturing());
    }

    // Update is called once per frame
    void Update()
    {
        // start fetching data as soon as application opens
        Debug.Log("update");

        if(status)  StartCoroutine(captureImage());
        
    }

    IEnumerator captureImage()
    {
        Debug.Log("enum");

        // imageNumber++;

        // clickStatus.text = "button clicked and went into enum";
        yield return new WaitForEndOfFrame();
        // clickStatus.text = "..";

        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();

        byte[] bytes = tex.EncodeToPNG();
        Object.Destroy(tex);

        string path = Application.persistentDataPath + "/Image" + System.DateTime.Now.ToString("HHmmss") + ".png";
        // string path = "C:/Users/YasHarsh/Desktop/Imagenow.png";
        Debug.Log(path + " captured");
        // int byteToInt = System.BitConverter.ToInt32(bytes, 0);
        // Debug.Log(byteToInt);

        // createImageArray(byteToInt, imageNumber);

        // afterMath.text = path + " captured";
        File.WriteAllBytes(path, bytes);
        status = false;
    }
}
