using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    private void Start()
    {
        // Verifica se as chaves existem antes de tentar ler (opcional)
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        {
            // Lê a posição salva
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");

            // Define a posição do personagem com a nova localização
            transform.position = new Vector3(x, y, z);
        }
    }
}