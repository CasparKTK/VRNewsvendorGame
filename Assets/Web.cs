using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Web : MonoBehaviour
{
    public string user;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetUser());
    }

    void WebData(string input)
    {
        user = input;
    }

    // Update is called once per frame
    public IEnumerator GetUser()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://newsvendortum.atwebpages.com/GetUser.php"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            
            if (webRequest.isNetworkError)
            {
                Debug.Log( webRequest.error);
            }
            else
            {
                WebData(webRequest.downloadHandler.text);
            }
            
        }
    }
    public IEnumerator ReturnUser(string userid)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);
        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://newsvendortum.atwebpages.com/ReturnUser.php", form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            
            if (webRequest.isNetworkError)
            {
                Debug.Log( webRequest.error);
            }
            else
            {
                Debug.Log( webRequest.downloadHandler.text);
            }
            
        }
    }
    public IEnumerator SaveData( string userid, int excon, int round, int demand, int order)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);
        form.AddField("excon", excon);
        form.AddField("round", round);
        form.AddField("demand", demand);
        form.AddField("order", order);
        using (UnityWebRequest www = UnityWebRequest.Post("http://newsvendortum.atwebpages.com/SaveData.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}
