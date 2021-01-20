using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.Networking;

public class MainScript : MonoBehaviour
{

    // string url = "http://ec2-13-59-115-87.us-east-2.compute.amazonaws.com:5000/getcoordinates";
    // string url = "http://127.0.0.1:8000/getcoordinates";
    string url = "https://ar-anatomy.herokuapp.com/getcoordinates";

    static WebCamTexture camera;

    public Camera cam;

    public GameObject skullPrefab;

    public TextMeshProUGUI apiData, countdown, status;

    private const float API_CHECK_MAXTIME = 60.0f / 60.0f; //3 second
    private float apiCheckCountdown = API_CHECK_MAXTIME;

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
        // Instantiate(skullPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    void Update()
    {
        apiCheckCountdown -= Time.deltaTime;
        countdown.text = apiCheckCountdown.ToString();
        if (apiCheckCountdown <= 0)
        {
            sendRequest();
            apiCheckCountdown = API_CHECK_MAXTIME;
        }
    }

    public void sendRequest()
    {
        Debug.Log("Analyzing Image");
        StartCoroutine(captureImage());
    }

    IEnumerator request(Texture2D tex)
    {

        Debug.Log("sending request");
        status.text = "sending request";
        byte[] bytes = tex.EncodeToJPG();
        Destroy(tex);

        var form = new WWWForm();
        form.AddBinaryData("image", bytes, "screenShot.jpg", "image/jpg");
        status.text = "binary data added";

        WWW w = new WWW(url, form);
        yield return w;
        status.text = "dataextracted";
        Debug.Log("data" + w.text);
        apiData.text = "API Data" + w.text;

        string[] separatedData = w.text.Split( new char [] {'[', '[', ']', ']'} );  
        // Debug.Log(separatedData[2] + "---" + separatedData[6]);

        string[] topLeftCorner = separatedData[2].Split(',');
        string[] bottomRightCorner = separatedData[6].Split(',');

        // Debug.Log(topLeftCorner[0] + " " + topLeftCorner[1] + " " + bottomRightCorner[0] + " " + bottomRightCorner[1]);

        float TLx = float.Parse(topLeftCorner[0]);// - 405f;
        float TLy = float.Parse(topLeftCorner[1]);// - 770f;

        float BRx = float.Parse(bottomRightCorner[0]);// - 667f;
        float BRy = float.Parse(bottomRightCorner[1]);// - 1287f;

        float cx = (TLx+BRx)/2;
        float cy = (TLy+BRy)/2;

        Vector3 point = new Vector3();

        point = cam.ScreenToWorldPoint(new Vector3(cx, -cy, 10f));
        point += new Vector3(0f, 12f, 0f);
        skullPrefab.transform.localPosition = point;
        // skullPrefab.transform.localScale = new Vector3(1,1,1);


        // topRightCorner --> TRx, TRy;
        // bottomLeftCorner --> BLx, BLy;

        // Debug.Log("request");
        // WWW request = new WWW(url);
        // StartCoroutine(onResponse(request));

        // Debug.Log(bytes.To);
        // Debug.Log(tex);

        // string path = Application.persistentDataPath + "/Image" + System.DateTime.Now.ToString("HHmmss") + ".jpg";
        // File.WriteAllBytes(path, bytes);
        // Debug.Log(path);
        

                // var uwr = new UnityWebRequest(url, "POST");
                // uwr.uploadHandler = new UploadHandlerRaw(bytes);
                // uwr.downloadHandler = new DownloadHandlerBuffer();

                // uwr.SetRequestHeader("Content-Type", "image/jpeg");
                // uwr.GetRequestHeader("image");

                    // List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
                    // formData.Add(new MultipartFormDataSection("image", "image.jpg"));
                // formData.AddBinaryData( new MultipartFormDataSection("image", bytes, "screenShot.png", "image/jpg") );

                    // UnityWebRequest uwr = UnityWebRequest.Post(url, formData); 

                    // yield return uwr.SendWebRequest();

                    // string response = uwr.downloadHandler.text;

                    // Debug.Log(response);
                // Debug.Log(uwr.downloadHandler);

                // yield return null;

            // Create a Web Form

        
        // form.AddField("frameCount", Time.frameCount.ToString());
        
        // Upload to a cgi script    
        
        // Debug.Log(w.bytes);
        // if (w.error != null)
        //     print(w.error);    
        // else
        //     print("Finished Uploading Screenshot"); 

        // yield return null;

    }

    IEnumerator captureImage()
    {
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
