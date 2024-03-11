using UnityEngine;
using UnityEngine.SceneManagement;

public class EscButtonOpcoes : MonoBehaviour
{
    private const string TestSceneName = "Opcoes";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key was pressed");
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene != TestSceneName)
            {
                SavePlayerPosition();
                SceneManager.LoadScene(TestSceneName);
            }
            else
            {
                LoadPlayerPosition();
                SceneManager.LoadScene(PlayerPrefs.GetString("LastScene"));
            }
        }
    }

    private void SavePlayerPosition()
    {
        Vector3 currentPosition = transform.position;
        Debug.Log($"Current Position before saving: {currentPosition}");

        PlayerPrefs.SetFloat("PlayerPosX", currentPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", currentPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", currentPosition.z);
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    private void LoadPlayerPosition()
    {
        Vector3 savedPosition;

        if (PlayerPrefs.GetString("LastScene") == "MenuJogo")
        {
            {
                PlayerPrefs.SetFloat("PlayerPosX", (float)3.5);
                PlayerPrefs.SetFloat("PlayerPosY", (float)-0.5);
                PlayerPrefs.SetFloat("PlayerPosZ", 0);

                PlayerPrefs.SetString("LastScene", "JogoQuarto");
            }
        }

        savedPosition = new Vector3(
        PlayerPrefs.GetFloat("PlayerPosX"),
        PlayerPrefs.GetFloat("PlayerPosY"),
        PlayerPrefs.GetFloat("PlayerPosZ"));


        transform.position = savedPosition;
        Debug.Log($"Position loaded: {savedPosition}");
    }

    void Start()
    {
        LoadPlayerPosition();
    }
}