using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    private void Start()
    {
        // Verifica se as chaves existem antes de tentar ler (opcional)
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        {
            // L� a posi��o salva
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");

            // Define a posi��o do personagem com a nova localiza��o
            transform.position = new Vector3(x, y, z);
        }
    }
}