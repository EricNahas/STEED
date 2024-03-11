using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System.Data;
using Unity.Burst.Intrinsics;
using UnityEditor.VersionControl;
using UnityEngine.Rendering.VirtualTexturing;
using Unity.VisualScripting;

public class ChatGPT : MonoBehaviour
{
    public TMP_InputField userInputField;
    public TextMeshProUGUI responseText;
    public TextMeshProUGUI PlayerText;
    private string apiURL = "https://api.openai.com/v1/chat/completions";
    private string apiKey = "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";
    private string promptPrefix;
    private AntagonistaScript antagonistaPrincipal = new AntagonistaScript();
    public PlayerController movimentoplayer;

    private WaitForSeconds messageDuration = new WaitForSeconds(4f); // Tempo em segundos para a caixa de texto permanecer ativa
    private WaitForSeconds messageDuration2 = new WaitForSeconds(6f);
    private WaitForSeconds messageDuration3 = new WaitForSeconds(10f);

    // Fun��o para enviar mensagem e receber resposta
    public void SendMessageToChatGPT(string message)
    {
        StartCoroutine(SendAndReceiveMessage(message));
    }

    public void Speak()
    {
        if (userInputField.gameObject.activeInHierarchy && userInputField.text != "")
        {
            SendMessageToChatGPT(userInputField.text);
            antagonistaPrincipal.addMessageLog(userInputField.text);
            PlayerText.text = userInputField.text;
            userInputField.text = ""; // Limpa o campo de entrada ap�s o envio
            movimentoplayer.enabled = true;
            PlayerText.gameObject.SetActive(true);
            StartCoroutine(DisablePlayerText());
            StartCoroutine(JustWait());
            StartCoroutine(DisableResponseText());
        }
        else
        {
            userInputField.gameObject.SetActive(true); // Ativa o campo de entrada
            userInputField.ActivateInputField(); // Foca no campo de entrada
            movimentoplayer.enabled = false;
            PlayerText.text = "";
        }
    }

    IEnumerator JustWait()
    {
        yield return messageDuration2;
        responseText.gameObject.SetActive(true);
    }
    IEnumerator SendAndReceiveMessage(string message)
    {
        promptPrefix = antagonistaPrincipal.returnPromptPrefix();
        string fullMessage = promptPrefix + message; // Combina script pr�vio com a mensagem do usu�rio

        // Construa o JSON com os par�metros desejados
        string json = "{\"model\": \"gpt-3.5-turbo\", \"messages\": [{\"role\": \"user\", \"content\": \"" + EscapeJson(fullMessage) + "\"}]}";
        Debug.Log("Request JSON: " + json);

        var postData = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiURL, "POST"))
        {
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                responseText.text = "Erro ao enviar a mensagem."; // Adicione uma mensagem de erro
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("Received: " + request.downloadHandler.text);
                string response = ProcessResponse(request.downloadHandler.text);
                responseText.text = response; // Atualize o campo de texto de resposta
                antagonistaPrincipal.addMessageLog(response);
            }
        }
    }

    private string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }

    IEnumerator DisablePlayerText()
    {
        yield return messageDuration; // Aguarda o tempo especificado
        PlayerText.gameObject.SetActive(false); // Desativa a caixa de texto do jogador
    }

    IEnumerator DisableResponseText()
    {
        yield return messageDuration3; // Aguarda o tempo especificado
        responseText.gameObject.SetActive(false); // Desativa a caixa de resposta do ChatGPT
        responseText.text = "";
    }

    // Processa a resposta da API
    string ProcessResponse(string jsonResponse)
    {
        if (string.IsNullOrEmpty(jsonResponse))
        {
            return "A resposta JSON est� vazia.";
        }

        ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);
        if (responseData == null || responseData.choices == null || responseData.choices.Length == 0)
        {
            return "Resposta inv�lida ou vazia recebida da API.";
        }

        // Como agora h� apenas uma mensagem por escolha, simplificamos o loop
        foreach (var choice in responseData.choices)
        {
            if (choice != null && choice.message != null && choice.message.role == "assistant")
            {
                return choice.message.content; // Retorna o conte�do da mensagem do assistente
            }
        }

        return "Nenhuma resposta encontrada.";
    }

// Add these classes outside of your ChatGPT class
[System.Serializable]
    public class ResponseData
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message; // Agora � um �nico objeto Message, n�o um array
        public int index;
        public string finish_reason;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
}
//sk - lDo8r3ACUK690Qf8VxIqT3BlbkFJNX8wPtLMlJV26glGkkiZ