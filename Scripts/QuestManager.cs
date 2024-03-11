using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Importar a namespace UI

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public GameObject questPanelPrefab; // Prefab do painel da quest
    public Transform questPanelParent; // O pai no UI onde os painéis das quests serão instanciados
    private Dictionary<Quest, GameObject> questPanels = new Dictionary<Quest, GameObject>();
    public static int questCounter = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Para o QuestManager

            // Obtém o Canvas, que é o pai raiz do questPanelParent
            Canvas canvas = questPanelParent.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                DontDestroyOnLoad(canvas.gameObject); // Aplica a todo o Canvas
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    public static int currentQuest()
    {
        return questCounter;
    }

    public void nextQuest()
    {
        questCounter++;
    }



    public List<Quest> quests = new List<Quest>();

    private void Start()
    {

        Quest tutorialQuest = new Quest("1", "tutorial", "Use as setas ou WASD para andar pelo quarto", 4); // ID, Descrição, Progresso necessário
        AddQuest(tutorialQuest);
        tutorialQuest.ActivateQuest();

        UpdateQuestPanelUI(); // Atualize o painel de UI para mostrar a nova quest
    }

    public void AddQuest(Quest newQuest)
    {
        quests.Add(newQuest);
        UpdateQuestPanelUI(); // Atualiza a UI sempre que uma nova quest é adicionada
    }

    public Quest GetQuestById(string id)
    {
        return quests.Find(quest => quest.id == id);
    }

    public void UpdateQuests(string id, int amount)
    {
        Quest questToUpdate = GetQuestById(id);
        if (questToUpdate != null && questToUpdate.isActive && !questToUpdate.isCompleted)
        {
            questToUpdate.currentAmount += amount;
            questToUpdate.CheckProgress();
            UpdateQuestPanelUI(); // Atualiza a UI sempre que uma quest é atualizada
        }
    }

    // Método para atualizar o painel de quests na UI
    public void UpdateQuestPanelUI()
    {
        // Verifica se questPanelPrefab e questPanelParent são nulos
        if (questPanelPrefab == null)
        {
            Debug.LogError("QuestPanelPrefab é nulo!");
            return;
        }
        if (questPanelParent == null)
        {
            Debug.LogError("QuestPanelParent é nulo!");
            return;
        }

        // Atualize ou crie painéis para cada quest ativa
        foreach (Quest quest in quests)
        {
            if (quest.isActive)
            {
                GameObject questPanel;
                // Verifique se um painel já existe para esta quest
                if (questPanels.ContainsKey(quest))
                {
                    // Use o painel existente
                    questPanel = questPanels[quest];
                }
                else
                {
                    // Crie um novo painel e adicione ao dicionário
                    questPanel = Instantiate(questPanelPrefab, questPanelParent);
                    questPanels[quest] = questPanel;
                }

                // Atualize os textos do painel
                TextMeshProUGUI titleText = questPanel.transform.Find("TITLE").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI descriptionText = questPanel.transform.Find("DESCRIPTION").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI progressText = questPanel.transform.Find("PROGRESS").GetComponent<TextMeshProUGUI>();

                if (titleText != null) titleText.text = quest.title;
                if (descriptionText != null) descriptionText.text = quest.description;
                if (progressText != null) progressText.text = quest.currentAmount + "/" + quest.requiredAmount;
            }
        }

        // Opcional: Remova painéis para quests inativas ou concluídas
        List<Quest> questsToRemove = new List<Quest>();
        foreach (var pair in questPanels)
        {
            if (!pair.Key.isActive || pair.Key.isCompleted)
            {
                questsToRemove.Add(pair.Key); // Marque para remoção do dicionário
            }
        }
        foreach (Quest quest in questsToRemove)
        {
           CompleteQuest(quest.id);
           questPanels.Remove(quest); // Remova do dicionário
        }
    }




    public void CompleteQuest(string id)
    {
        Quest questToComplete = GetQuestById(id);
        if (questToComplete != null)
        {
            // Marque a quest como concluída
            questToComplete.isCompleted = true;

            // Atualize a UI para mostrar que a quest foi concluída
            if (questPanels.ContainsKey(questToComplete))
            {
                GameObject questPanel = questPanels[questToComplete];
                TextMeshProUGUI progressText = questPanel.transform.Find("PROGRESS").GetComponent<TextMeshProUGUI>();
                if (progressText != null)
                {
                    progressText.text = "CONCLUÍDA!"; // Atualize o texto para mostrar que a quest foi concluída
                }

                // Inicie uma corrotina para lidar com o fade out e depois adicionar a nova quest
                StartCoroutine(CompleteAndSetupNextQuest(questPanel, questToComplete.id));
            }
        }
    }

    private IEnumerator CompleteAndSetupNextQuest(GameObject questPanel, string questId)
    {
        // Executa o fade out
        yield return StartCoroutine(FadeOutQuestPanel(questPanel));

        // Após o fade, prossiga para a próxima quest
        nextQuest(); // Incrementa o contador de quest

        // Verifica qual é a próxima quest com base no contador atualizado e adiciona ela
        Quest newQuest;
        switch (currentQuest())
        {
            case 1:
                newQuest = new Quest("2", "Primeira vez lá fora", "Saia do seu quarto clicando E perto da porta", 1);
                AddQuest(newQuest);
                newQuest.ActivateQuest();
                break;
            case 2:
                // Adicione aqui a lógica para outras quests
                break;
                // Adicione mais cases conforme necessário
        }

        UpdateQuestPanelUI(); // Atualize o painel de UI para mostrar a nova quest
    }


    private IEnumerator FadeOutQuestPanel(GameObject questPanel)
    {
        // Supondo que você tem um CanvasGroup no seu painel para controlar a opacidade
        CanvasGroup canvasGroup = questPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = questPanel.AddComponent<CanvasGroup>(); // Se não existir, adicione um
        }

        float duration = 2.0f; // Duração do fade
        float startTime = Time.time; // Tempo inicial

        while (Time.time < startTime + duration)
        {
            // Atualiza a opacidade do painel com o tempo
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = 1 - t; // Faz um fade out (de opaco para transparente)
            yield return null; // Aguarde até o próximo frame
        }

        // Após o fade, desative o painel
        questPanel.SetActive(false);
        Destroy(questPanel);
    }
}
