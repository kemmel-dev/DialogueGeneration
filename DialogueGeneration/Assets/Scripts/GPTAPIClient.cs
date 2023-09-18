using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class GPTAPICLient : MonoBehaviour
{

    private string _key;
    private string _url;

    public static GPTAPICLient Instance { get; private set; }

    private void Awake()
    {
        // Open the .env file and read the key and url
        // C:\Users\Kamiel\Workspace\DialogueGeneration\DialogueGeneration

        var lines =
            System.IO.File.ReadAllLines(@"C:\Users\Kamiel\Workspace\DialogueGeneration\DialogueGeneration\.env");
        _key = lines[0].Split('=')[1];
        _url = lines[1].Split('=')[1];
        Instance = this;
    }

    public void SendMessageToChatGPT(string message, Action<GPTResponse> onResponseReceived)
    {
        List<Message> messages = new List<Message>
        {
            new Message { role = "user", content = message }
        };

        StartCoroutine(PostMessage(messages, onResponseReceived));
    }

    private IEnumerator PostMessage(List<Message> messages, Action<GPTResponse> onResponseReceived)
    {
        
        // Construct the message payload
        var requestPayload = new
            GPTRequest()
            {
                messages = messages.ToArray(),
                model = "gpt-4"
            };

        string jsonPayload = JsonUtility.ToJson(requestPayload);

        UnityWebRequest request = new UnityWebRequest(_url, "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + _key);
        
        Debug.Log(requestPayload);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        
        Debug.Log(request.downloadHandler.text);
        // Handle the response here (e.g., display it in your UI)
        var response = JsonUtility.FromJson<GPTResponse>(request.downloadHandler.text);
        onResponseReceived.Invoke(response);
    }
    
    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
    
    [Serializable]
    public class GPTRequest
    {
        public string model;
        public Message[] messages;
    }
    
    [Serializable]
    public class GPTResponse
    {
        public string id;
        public string @object;
        public long created;
        public string model;
        public Usage usage;
        public Choice[] choices;
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

    [Serializable]
    public class Choice
    {
        public Message message;
        public string finish_reason;
        public int index;
    }
}
