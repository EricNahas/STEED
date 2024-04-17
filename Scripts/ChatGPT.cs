using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System.Data;
using Unity.Burst.Intrinsics;
using UnityEngine.Rendering.VirtualTexturing;
using Unity.VisualScripting;
using static ChatGPT;
using System.Linq;
using MiniJSON;
using System;

public class ChatGPT : MonoBehaviour
{
    public TMP_InputField userInputField;
    public TextMeshProUGUI responseText;
    public TextMeshProUGUI PlayerText;
    private string apiURL = "https://api.openai.com/v1/chat/completions";
    private string apiKey = "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";
    private string promptPrefix;
    public bool isAntagonist = false;
    private static AntagonistaScript antagonistaPrincipal = new AntagonistaScript();
    public PersonagemNPCScript personagemNPCScript;
    public int messageCounter = 0;

    public PlayerController movimentoplayer;
    public NPCWalk movimentoNPC;

    private WaitForSeconds messageDuration = new WaitForSeconds(4f); // Tempo em segundos para a caixa de texto permanecer ativa
    private WaitForSeconds messageDuration2 = new WaitForSeconds(6f);
    private WaitForSeconds messageDuration3 = new WaitForSeconds(10f);

    // Função para enviar mensagem e receber resposta
    public void SendMessageToChatGPT(string message)
    {
        StartCoroutine(SendAndReceiveMessage(message, true));
        messageCounter++;
    }

    public void Speak()
    {
        if (userInputField.gameObject.activeInHierarchy && userInputField.text != "")
        {
            SendMessageToChatGPT(userInputField.text);
            PlayerText.text = userInputField.text;
            userInputField.text = ""; // Limpa o campo de entrada após o envio
            movimentoplayer.enabled = true;
            if (movimentoNPC != null)
                movimentoNPC.enabled = true;
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
            if (movimentoNPC != null)
                movimentoNPC.enabled = false;
            PlayerText.text = "";
        }
    }

    IEnumerator JustWait()
    {
        yield return messageDuration2;
        responseText.gameObject.SetActive(true);
    }
    IEnumerator SendAndReceiveMessage(string message, bool aparecer, Action<string> callback = null)
    {
        string json = "";

        if (aparecer)
        {
            if (isAntagonist)
            {
                promptPrefix = antagonistaPrincipal.returnPromptPrefix();
                antagonistaPrincipal.addMessageLog(message);

                Debug.Log(antagonistaPrincipal.getMesssageLog().Count(c => c == '\n'));

                if (antagonistaPrincipal.getCounter() == 11)
                {
                    VamosSeuQuarto(message, (resultado) =>
                    {
                        if (resultado)
                        {
                            PlayerPrefs.SetString("AntagonistaFollow", "true");
                            Debug.Log("Resposta é true");
                        }
                        else
                        {
                            PlayerPrefs.SetString("AntagonistaFollow", "false");
                            Debug.Log("Resposta é false");
                        }

                        antagonistaPrincipal.increaseScene();
                    });
                }
            }

            else
            {
                promptPrefix = personagemNPCScript.returnPromptPrefix();
                personagemNPCScript.addMessageLog(message);
            }

            string fullMessage = promptPrefix + message; // Combina script prévio com a mensagem do usuário
                                     
            json = "{\"model\": \"gpt-3.5-turbo\", \"messages\": [{\"role\": \"user\", \"content\": \"" + EscapeJson(fullMessage) + "\"}]}";

        }
        else
        {
            json = "{\"model\": \"gpt-3.5-turbo\", \"messages\": [{\"role\": \"user\", \"content\": \"" + EscapeJson(message) + "\"}]}";
        }


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

                if (aparecer)
                {
                    responseText.text = response; // Atualize o campo de texto de resposta

                    Debug.Log(response);

                    if (isAntagonist)
                    {
                        antagonistaPrincipal.addMessageLog(response);
                        
                    }
                    else
                    {
                        personagemNPCScript.addMessageLog(response);

                    }
                }
                else
                {
                    callback?.Invoke(response);
                }
                
            }
        }
    }

    private void VamosSeuQuarto(string message, Action<bool> callback)
    {
        string prompt = "Sua resposta deve ser apenas 'true' ou 'false'. Caso a mensagem contenha uma resposta afirmativa ('sim', 'claro', 'vamos', 'com certeza', 'bora', 'tá bom', 'tudo bem', 'simbora' e outros), ou uma solicitação para ir para seu quarto ('Vamos pro meu quarto' ou algo assim) true. Caso haja uma resposta negativa ou algo que não se" +
            " encaixe, false. Segue a mensagem a ser analisada: ";
        prompt += message;

        StartCoroutine(SendAndReceiveMessage(prompt, false, (response) =>
        {
            bool resultado = response.Trim().ToLower().Contains("true");
            callback.Invoke(resultado);
        }));
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
            return "A resposta JSON está vazia.";
        }

        ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);
        if (responseData == null || responseData.choices == null || responseData.choices.Length == 0)
        {
            return "Resposta inválida ou vazia recebida da API.";
        }

        // Como agora há apenas uma mensagem por escolha, simplificamos o loop
        foreach (var choice in responseData.choices)
        {
            if (choice != null && choice.message != null && choice.message.role == "assistant")
            {

                return choice.message.content; // Retorna o conteúdo da mensagem do assistente
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
        public Message message; // Agora é um único objeto Message, não um array
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