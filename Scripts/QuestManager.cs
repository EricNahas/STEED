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
    public static int questCounter = 5;
    public static List<Quest> quests = new List<Quest>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Para o QuestManager

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

    private void Start()
    {
        InitializeAllQuests();
        //LoadAllQuestsProgress();
        setNewQuest();
        UpdateQuestPanelUI();
    }

    public static void SaveAllQuestsProgress()
    {
        foreach (var quest in quests)
        {
            PlayerPrefs.SetInt($"Quest_{quest.id}_Progress", quest.currentAmount);
            PlayerPrefs.SetInt($"Quest_{quest.id}_Completed", quest.isCompleted ? 1 : 0);
        }
        PlayerPrefs.SetInt("SaveState", questCounter);
        PlayerPrefs.Save();
    }


    private void LoadAllQuestsProgress()
    {
        foreach (var quest in quests)
        {
            int progress = PlayerPrefs.GetInt($"Quest_{quest.id}_Progress", 0);
            bool isCompleted = PlayerPrefs.GetInt($"Quest_{quest.id}_Completed", 0) == 1;
            questCounter = PlayerPrefs.GetInt("SaveState");

            quest.currentAmount = progress;
            quest.isCompleted = isCompleted;

            // Reative a quest somente se ela não estiver concluída.
            if (!isCompleted)
            {
                quest.isActive = progress > 0; // Ativa a quest se houve progresso.
            }
        }
    }

    public void nextQuest()
    {
        questCounter++;
        SaveAllQuestsProgress(); // Salva o progresso sempre que avançar para a próxima quest.
    }

    public void AddQuest(Quest newQuest)
    {
        quests.Add(newQuest);
        UpdateQuestPanelUI(); // Atualiza a UI sempre que uma nova quest é adicionada.
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
            UpdateQuestPanelUI(); // Atualiza a UI sempre que uma quest é atualizada.
        }
    }

    public void UpdateQuestPanelUI()
    {
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

        foreach (Quest quest in quests)
        {
            if (quest.isActive)
            {
                GameObject questPanel;
                if (questPanels.ContainsKey(quest))
                {
                    questPanel = questPanels[quest];
                }
                else
                {
                    questPanel = Instantiate(questPanelPrefab, questPanelParent);
                    questPanels[quest] = questPanel;
                }

                TextMeshProUGUI titleText = questPanel.transform.Find("TITLE").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI descriptionText = questPanel.transform.Find("DESCRIPTION").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI progressText = questPanel.transform.Find("PROGRESS").GetComponent<TextMeshProUGUI>();

                if (titleText != null) titleText.text = quest.title;
                if (descriptionText != null) descriptionText.text = quest.description;
                if (progressText != null) progressText.text = $"{quest.currentAmount}/{quest.requiredAmount}";
            }
        }

        List<Quest> questsToRemove = new List<Quest>();
        foreach (var pair in questPanels)
        {
            if (!pair.Key.isActive || pair.Key.isCompleted)
            {
                questsToRemove.Add(pair.Key);
            }
        }
        foreach (Quest quest in questsToRemove)
        {
            CompleteQuest(quest.id);
            questPanels.Remove(quest);
        }
    }

    public void CompleteQuest(string id)
    {
        Quest questToComplete = GetQuestById(id);
        if (questToComplete != null)
        {
            questToComplete.isCompleted = true;

            if (questPanels.ContainsKey(questToComplete))
            {
                GameObject questPanel = questPanels[questToComplete];
                TextMeshProUGUI progressText = questPanel.transform.Find("PROGRESS").GetComponent<TextMeshProUGUI>();
                if (progressText != null)
                {
                    progressText.text = "CONCLUÍDA!";
                }
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
        setNewQuest();

        UpdateQuestPanelUI(); // Atualize o painel de UI para mostrar a nova quest
    }

    private void InitializeAllQuests()
    {
        quests.Clear(); // Certifique-se de que a lista está vazia antes de adicionar novas quests
        quests.Add(new Quest("1", "Bem-vindo!", "Use as setas ou AWSD para andar pelo quarto", 4));
        quests.Add(new Quest("2", "Primeira vez lá fora", "Saia do seu quarto clicando E perto da porta", 1));
        quests.Add(new Quest("3", "Uma caminhada", "Conheça os corredores e vá para o lobby", 1));
        quests.Add(new Quest("4", "Seu primeiro amigo", "Volte para seu quarto, clique E perto do computador e veja a aba Amigos", 1));
        quests.Add(new Quest("5", "Já sabemos o básico", "Vá para o lobby e fale com o Atendente Ocupado e o Atendente Legal", 2));
        quests.Add(new Quest("6", "Vá conversar", "No lobby, fale com algum vizinho até fazer amizade", 1));
    }

    private void setNewQuest()
    {
        Quest newQuest;
        switch (questCounter)
        {

            case 0:
                newQuest = quests.Find(quest => quest.id == "1");
                if (!newQuest.isCompleted) // Verifica se a quest já foi concluída
                {
                    newQuest.ActivateQuest();

                    GameObject portaObj = GameObject.FindGameObjectWithTag("Porta");
                    GameObject esc = GameObject.FindGameObjectWithTag("Player");
                    GameObject pc = GameObject.FindGameObjectWithTag("Computador");

                    Debug.Log(portaObj + "" + esc + "" + pc);

                    if (portaObj != null && esc != null && pc != null) // Checa se o objeto foi encontrado
                    {
                        Porta portaScript = portaObj.GetComponent<Porta>(); // Obtém a referência ao script Porta
                        EscButtonOpcoes escBtn = esc.GetComponent<EscButtonOpcoes>();
                        Computador pcClick = pc.GetComponent<Computador>();

                        Debug.Log(portaScript + "" + escBtn + "" + pcClick);

                        if (portaScript != null && escBtn != null && pcClick != null) // Checa se o script Porta está anexado ao objeto
                        {
                            portaScript.enabled = false; // Desativa o script Porta
                            escBtn.enabled = false;
                            pcClick.enabled = false;    
                        }
                    }
                    else
                    {
                        Debug.LogError("null");
                    }

                }
                break;

            case 1:
                newQuest = quests.Find(quest => quest.id == "2");
                if (!newQuest.isCompleted) // Verifica se a quest já foi concluída
                {

                    GameObject portaObj = GameObject.FindGameObjectWithTag("Porta");
                    GameObject esc = GameObject.FindGameObjectWithTag("Player");
                    GameObject pc = GameObject.FindGameObjectWithTag("Computador");

                    Debug.Log(portaObj + "" + esc + "" + pc);

                    if (portaObj != null && esc != null && pc != null) // Checa se o objeto foi encontrado
                    {
                        Porta portaScript = portaObj.GetComponent<Porta>(); // Obtém a referência ao script Porta
                        EscButtonOpcoes escBtn = esc.GetComponent<EscButtonOpcoes>();
                        Computador pcClick = pc.GetComponent<Computador>();

                        Debug.Log(portaScript + "" + escBtn + "" + pcClick);

                        if (portaScript != null && escBtn != null && pcClick != null) // Checa se o script Porta está anexado ao objeto
                        {
                            portaScript.enabled = true; // Desativa o script Porta
                            escBtn.enabled = true;
                            pcClick.enabled = true;
                        }
                    }

                    newQuest.ActivateQuest();
                }
                break;

            case 2:
                newQuest = quests.Find(quest => quest.id == "3");
                if (!newQuest.isCompleted) // Verifica se a quest já foi concluída
                {
                    newQuest.ActivateQuest();
                }

                break;

            case 3:
                newQuest = quests.Find(quest => quest.id == "4");
                if (!newQuest.isCompleted) // Verifica se a quest já foi concluída
                {
                    newQuest.ActivateQuest();

                }
                break;

            case 4:
                newQuest = quests.Find(quest => quest.id == "5");
                if (!newQuest.isCompleted) // Verifica se a quest já foi concluída
                {
                    newQuest.ActivateQuest();

                }
                break;

            case 5:
                newQuest = quests.Find(quest => quest.id == "6");
                if (!newQuest.isCompleted) // Verifica se a quest já foi concluída
                {
                    newQuest.ActivateQuest();

                }
                break;
        }
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