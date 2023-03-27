using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class basicMain : MonoBehaviour
{
    public Button Hello;
    public string host;
    public int port;

    private void Start()
    {
        this.Hello.onClick.AddListener(() =>
        {
            var url = string.Format("{0}:{1}/", host, port);
            Debug.Log(url);
            StartCoroutine(this.GetBasic(url, (raw) =>
            {
                Debug.LogFormat("{0}", raw);
            }));
        });
    }

    private IEnumerator GetBasic(string url, System.Action <string> callback)
    {
        var webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        Debug.Log("---->" + webRequest.downloadHandler.text);

        if(webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("��Ʈ��ũ ȯ���� ���� �ʾ� ��� �Ұ���");
        }
        else
        {
            callback(webRequest.downloadHandler.text);
        }
    }

}
