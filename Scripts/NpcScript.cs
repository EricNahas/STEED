using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcScript : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isPlayerNearby = false;
    public ChatGPT npc;
    public TMP_InputField userInputField;

    // Detecta se o jogador entrou na área de interação
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Certifique-se de que seu personagem tem a tag "Player"
        {
            Debug.Log("Dentro do trigger");
            isPlayerNearby = true;
            userInputField.gameObject.SetActive(true);
        }
    }

    // Detecta se o jogador saiu da área de interação
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Fora do trigger");
            isPlayerNearby = false;
            userInputField.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && isPlayerNearby || Input.GetKeyDown(KeyCode.KeypadEnter) && isPlayerNearby)
        {
            if (npc != null)
            {
                npc.Speak();
            }
            else
            {
                Debug.LogError("NPCScript: O componente ChatGPT não foi atribuído.");
            }
        }
    }
}