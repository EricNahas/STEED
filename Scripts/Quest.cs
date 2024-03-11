using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string id;
    public string title;
    public string description;
    public bool isActive;
    public bool isCompleted;
    public int currentAmount;
    public int requiredAmount;

    public Quest(string id, string title, string description, int requiredAmount)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.isActive = false;
        this.isCompleted = false;
        this.currentAmount = 0;
        this.requiredAmount = requiredAmount;
    }

    public void CheckProgress()
    {
        if (currentAmount >= requiredAmount)
        {
            isCompleted = true;
            isActive = false;
            // Aqui você pode adicionar lógica adicional para quando a quest for completada.
            Debug.Log(title + " has been completed!");
        }
    }

    public void ActivateQuest()
    {
        isActive = true;
    }
}