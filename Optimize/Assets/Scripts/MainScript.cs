using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.Networking;

public class MainScript : MonoBehaviour
{

    string url = "http://ec2-3-139-61-182.us-east-2.compute.amazonaws.com:5000/getcoordinates";

    static WebCamTexture camera;

    void Start()
    {
        if (camera == null)
        {
            camera = new WebCamTexture();
        }

        GetComponent<Renderer>().material.mainTexture = camera;

        if(!camera.isPlaying)
        {
            camera.Play();
        }
    }

    void Update()
    {
       
    }

    public void sendRequest()
    {
        StartCoroutine(captureImage());
    }

    IEnumerator request(Texture2D tex)
    {
        Debug.Log("request");
        // WWW request = new WWW(url);
        // StartCoroutine(onResponse(request));


        byte[] bytes = tex.EncodeToJPG();
        Destroy(tex);

        var uwr = new UnityWebRequest(url, "POST");
        uwr.uploadHandler = new UploadHandlerRaw(bytes);
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "image/jpeg");
        // uwr.GetRequestHeader("image");

        yield return uwr.SendWebRequest();

        string response = uwr.downloadHandler.text;

        Debug.Log(response);

        yield return null;

    }

    // private IEnumerator onResponse(WWW req)
    // {
    //     yield return req;

    //     Debug.Log(req.text);

    // }

    IEnumerator captureImage()
    {
        Debug.Log("enum");

        yield return new WaitForEndOfFrame();

        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();

        yield return StartCoroutine(request(tex));

        // byte[] bytes = tex.EncodeToPNG();
        // Object.Destroy(tex);

        // string path = Application.persistentDataPath + "/Image" + System.DateTime.Now.ToString("HHmmss") + ".png";
        // // string path = "C:/Users/YasHarsh/Desktop/Imagenow.png";
        // Debug.Log(path + " captured");
        // // int byteToInt = System.BitConverter.ToInt32(bytes, 0);
        // // Debug.Log(byteToInt);

        // // createImageArray(byteToInt, imageNumber);

        // // afterMath.text = path + " captured";
        // File.WriteAllBytes(path, bytes);
        // status = false;
    }
}
