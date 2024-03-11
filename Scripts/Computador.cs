using UnityEngine;
using UnityEngine.SceneManagement;

public class Computador : MonoBehaviour
{
    public string sceneToLoad;
    private bool isPlayerNearby = false;

    // Detecta se o jogador entrou na área de interação
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Certifique-se de que seu personagem tem a tag "Player"
        {
            Debug.Log("Dentro do trigger");
            isPlayerNearby = true;
        }
    }

    // Detecta se o jogador saiu da área de interação
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Fora do trigger");
            isPlayerNearby = false;
        }
    }

    // Atualiza a cada frame
    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}