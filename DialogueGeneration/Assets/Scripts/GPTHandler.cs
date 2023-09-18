using System.Linq;
using System.Net.Mime;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class GPTHandler : MonoBehaviour
    {
        
        public TextMeshProUGUI responseText;
        public TextMeshProUGUI promptText;
        
        private void SendMessageToChatGPT(string message)
        {
            GPTAPICLient.Instance.SendMessageToChatGPT(message, OnResponseReceived);
        }

        public void GetResponse()
        {
            SendMessageToChatGPT(promptText.text);
        }

        private void OnResponseReceived(GPTAPICLient.GPTResponse response)
        {
            responseText.text = response.choices.FirstOrDefault().message.content;
        }
    }
}