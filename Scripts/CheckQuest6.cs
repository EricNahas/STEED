using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckQuest6 : MonoBehaviour
{
    private QuestManager questManager = QuestManager.instance;

    void Update()
    {
        if (AntagonistaScript.messageCounter >= 11)
        {
            questManager.UpdateQuests("6", 1);
            Destroy(this);
        }
    }
}
