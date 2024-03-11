using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Importar a namespace UI

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public GameObject questPanelPrefab; // Prefab do painel da quest
    public Transform questPanelParent; // O pai no UI onde os pain�is das quests ser�o instanciados
    private Dictionary<Quest, GameObject> questPanels = new Dictionary<Quest, GameObject>();
    public static int questCounter = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Para o QuestManager

            // Obt�m o Canvas, que � o pai raiz do questPanelParent
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

        Quest tutorialQuest = new Quest("1", "tutorial", "Use as setas ou WASD para andar pelo quarto", 4); // ID, Descri��o, Progresso necess�rio
        AddQuest(tutorialQuest);
        tutorialQuest.ActivateQuest();

        UpdateQuestPanelUI(); // Atualize o painel de UI para mostrar a nova quest
    }

    public void AddQuest(Quest newQuest)
    {
        quests.Add(newQuest);
        UpdateQuestPanelUI(); // Atualiza a UI sempre que uma nova quest � adicionada
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
            UpdateQuestPanelUI(); // Atualiza a UI sempre que uma quest � atualizada
        }
    }

    // M�todo para atualizar o painel de quests na UI
    public void UpdateQuestPanelUI()
    {
        // Verifica se questPanelPrefab e questPanelParent s�o nulos
        if (questPanelPrefab == null)
        {
            Debug.LogError("QuestPanelPrefab � nulo!");
            return;
        }
        if (questPanelParent == null)
        {
            Debug.LogError("QuestPanelParent � nulo!");
            return;
        }

        // Atualize ou crie pain�is para cada quest ativa
        foreach (Quest quest in quests)
        {
            if (quest.isActive)
            {
                GameObject questPanel;
                // Verifique se um painel j� existe para esta quest
                if (questPanels.ContainsKey(quest))
                {
                    // Use o painel existente
                    questPanel = questPanels[quest];
                }
                else
                {
                    // Crie um novo painel e adicione ao dicion�rio
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

        // Opcional: Remova pain�is para quests inativas ou conclu�das
        List<Quest> questsToRemove = new List<Quest>();
        foreach (var pair in questPanels)
        {
            if (!pair.Key.isActive || pair.Key.isCompleted)
            {
                questsToRemove.Add(pair.Key); // Marque para remo��o do dicion�rio
            }
        }
        foreach (Quest quest in questsToRemove)
        {
           CompleteQuest(quest.id);
           questPanels.Remove(quest); // Remova do dicion�rio
        }
    }




    public void CompleteQuest(string id)
    {
        Quest questToComplete = GetQuestById(id);
        if (questToComplete != null)
        {
            // Marque a quest como conclu�da
            questToComplete.isCompleted = true;

            // Atualize a UI para mostrar que a quest foi conclu�da
            if (questPanels.ContainsKey(questToComplete))
            {
                GameObject questPanel = questPanels[questToComplete];
                TextMeshProUGUI progressText = questPanel.transform.Find("PROGRESS").GetComponent<TextMeshProUGUI>();
                if (progressText != null)
                {
                    progressText.text = "CONCLU�DA!"; // Atualize o texto para mostrar que a quest foi conclu�da
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

        // Ap�s o fade, prossiga para a pr�xima quest
        nextQuest(); // Incrementa o contador de quest

        // Verifica qual � a pr�xima quest com base no contador atualizado e adiciona ela
        Quest newQuest;
        switch (currentQuest())
        {
            case 1:
                newQuest = new Quest("2", "Primeira vez l� fora", "Saia do seu quarto clicando E perto da porta", 1);
                AddQuest(newQuest);
                newQuest.ActivateQuest();
                break;
            case 2:
                // Adicione aqui a l�gica para outras quests
                break;
                // Adicione mais cases conforme necess�rio
        }

        UpdateQuestPanelUI(); // Atualize o painel de UI para mostrar a nova quest
    }


    private IEnumerator FadeOutQuestPanel(GameObject questPanel)
    {
        // Supondo que voc� tem um CanvasGroup no seu painel para controlar a opacidade
        CanvasGroup canvasGroup = questPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = questPanel.AddComponent<CanvasGroup>(); // Se n�o existir, adicione um
        }

        float duration = 2.0f; // Dura��o do fade
        float startTime = Time.time; // Tempo inicial

        while (Time.time < startTime + duration)
        {
            // Atualiza a opacidade do painel com o tempo
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = 1 - t; // Faz um fade out (de opaco para transparente)
            yield return null; // Aguarde at� o pr�ximo frame
        }

        // Ap�s o fade, desative o painel
        questPanel.SetActive(false);
        Destroy(questPanel);
    }
}
