using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic.Networks;
using Models.Contexts;
using Networks;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequester : Singleton<WebRequester>
{
    private static string urlRequest;
    private static GamepadClient client;
    private static readonly Dictionary<string, object> requests = new Dictionary<string, object>();

    private void Awake()
    {
        client = new GamepadClient();
    }

    public static void SendRequest(RequestContext requestContext, Action<ResponseContext, object> onResponseReceived = null, object data = null)
    {
        Instance.StartCoroutine(Upload(requestContext.request, onResponseReceived, data));
    }

    private static IEnumerator Upload(string url, Action<ResponseContext, object> onResponseReceived, object data)
    {
        var prefix = "http://";
        if (!url.Contains(prefix))
            url = prefix + url;
        using var www = UnityWebRequest.Get(url);

        string userName;
        if (PlayerPrefs.HasKey("Content-UserName"))
            userName = PlayerPrefs.GetString("Content-UserName");
        else
        {
            userName = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("Content-UserName", userName);
        }

        www.SetRequestHeader("Content-UserName", userName);

        var id = Guid.NewGuid().ToString();
        requests.Add(id, data);
        www.SetRequestHeader("RequestGuid", id);
        www.method = UnityWebRequest.kHttpVerbGET;
        www.url = url;
        Debug.Log($"Send {url}");

        yield return www.SendWebRequest();

        var responseCtx = new ResponseContext
        {
            result = www.result,
            data = www.downloadHandler.data,
        };

        var answer = EventSender.RestoreData(responseCtx);
        if (requests.TryGetValue(answer.id, out data))
            requests.Remove(answer.id);


        onResponseReceived?.Invoke(responseCtx, data);
    }
}