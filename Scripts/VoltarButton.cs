using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoltarButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Back()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "Menu");
        SceneManager.LoadScene(lastScene);
    }
}
