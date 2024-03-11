using UnityEngine.SceneManagement;
using UnityEngine;

public class Porta : MonoBehaviour
{
    public string sceneToLoad; // Nome da cena para carregar
    public Vector3 newLocation; // A nova localiza��o para onde o personagem deve ir
    public bool nearByDoor = false; // Booleano para verificar se o personagem est� perto da porta

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            nearByDoor = true;
            Debug.Log("Encostou");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            nearByDoor = false;
            Debug.Log("Saiu");
        }
    }

    void Update()
    {
        // Verifique se o usu�rio pressionou a tecla E e est� perto da porta
        if (Input.GetKeyDown(KeyCode.E) && nearByDoor)
        {
            // Salva a nova localiza��o usando PlayerPrefs
            PlayerPrefs.SetFloat("PlayerPosX", newLocation.x);
            PlayerPrefs.SetFloat("PlayerPosY", newLocation.y);
            PlayerPrefs.SetFloat("PlayerPosZ", newLocation.z);
            PlayerPrefs.Save(); // Garante que a prefer�ncia seja salva

            // Carrega a nova cena
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}