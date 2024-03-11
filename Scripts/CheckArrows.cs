using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckArrows : MonoBehaviour
{
    public static CheckArrows instance; // Adiciona uma instância estática para o script

    private QuestManager questManager;
    private bool up = true;
    private bool down = true;
    private bool left = true;
    private bool right = true;

    void Awake()
    {
        // Verifica se já existe uma instância. Se não, esta se torna a instância e não será destruída entre cenas.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Evita que o GameObject seja destruído ao carregar uma nova cena.
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Se uma outra instância já existir, destrua esta.
        }
    }

    void Start()
    {
        questManager = QuestManager.instance;
    }

    void Update()
    {
        if (questManager != null)
        {
            if ((Input.GetKeyDown(KeyCode.S) && down) || (Input.GetKeyDown(KeyCode.DownArrow) && down))
            {
                questManager.UpdateQuests("1", 1);
                down = false;
            }

            else if ((Input.GetKeyDown(KeyCode.W) && up) || (Input.GetKeyDown(KeyCode.UpArrow) && up))
            {
                questManager.UpdateQuests("1", 1);
                up = false;
            }

            else if ((Input.GetKeyDown(KeyCode.A) && left) || (Input.GetKeyDown(KeyCode.LeftArrow) && left))
            {
                questManager.UpdateQuests("1", 1);
                left = false;
            }

            else if ((Input.GetKeyDown(KeyCode.D) && right) || (Input.GetKeyDown(KeyCode.RightArrow) && right))
            {
                questManager.UpdateQuests("1", 1);
                right = false;
            }
        }
    }
}
