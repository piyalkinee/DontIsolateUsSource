using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    #region Perfabs
    //Menus
    private GameObject uiblock_MainMeny;
    private GameObject uiblock_HowPlay;
    private GameObject uiblock_GameUI;
    //Dialogs
    private GameObject dialog_ToMeny;
    private GameObject dialog_SettlerPanel;
    private GameObject dialog_SettlersTask;
    private GameObject item_Settler;
    private GameObject uiblock_Message;
    private GameObject uiblock_EndGame;
    #endregion
    #region UI
    //Scripts
    private Game Game;
    private World World;
    //Methods
    private void Awake()
    {
        //Scripts
        Game = GameObject.Find("-Main-").GetComponent<Game>();
        World = GameObject.Find("-World-").GetComponent<World>();

        //Menus
        uiblock_MainMeny = Resources.Load<GameObject>("uiblock_MainMeny");
        uiblock_HowPlay = Resources.Load<GameObject>("uiblock_HowPlay");
        uiblock_GameUI = Resources.Load<GameObject>("uiblock_GameUI");

        //Dialogs
        dialog_ToMeny = Resources.Load<GameObject>("dialog_ToMeny");
        dialog_SettlerPanel = Resources.Load<GameObject>("dialog_SettlerPanel");
        dialog_SettlersTask = Resources.Load<GameObject>("dialog_SettlersTask");
        item_Settler = Resources.Load<GameObject>("item_Settler");
        uiblock_Message = Resources.Load<GameObject>("uiblock_Message");
        uiblock_EndGame = Resources.Load<GameObject>("uiblock_EndGame");

        MainMenyOpen();
    }
    public void CloseAllUIMenys()
    {
        foreach (Transform item in transform)
        {
            if (item.tag != "Message")
                Destroy(item.gameObject);
        }
    }
    public void CloseAllUIMessages()
    {
        foreach (Transform item in transform)
        {
            if (item.tag == "Message")
                Destroy(item.gameObject);
        }
    }
    #endregion
    #region MainMeny
    public void MainMenyOpen()
    {
        CloseAllUIMenys();
        CloseAllUIMessages();

        GameObject currentUIBlock = Instantiate(uiblock_MainMeny, transform);

        currentUIBlock.transform.Find("button_PlayGameJamMode").GetComponent<Button>().onClick.AddListener(delegate { ButtonMainMenyStartGameJamMode(); });
        currentUIBlock.transform.Find("button_GameInfo").GetComponent<Button>().onClick.AddListener(delegate { ButtonMainMenyGameInfo(); });
        currentUIBlock.transform.Find("button_Quit").GetComponent<Button>().onClick.AddListener(delegate { ButtonMainMenyQuit(); });
    }
    //Buttons
    public void ButtonMainMenyStartGameJamMode()
    {
        SoundMaster.CreateSound(Sound.ClickButton);

        CloseAllUIMenys();

        GameObject.Find("-Main-").GetComponent<Game>().StartGameMode(GameMode.GameJam);

        GameUIOpen();
    }
    public void ButtonMainMenyGameInfo()
    {
        SoundMaster.CreateSound(Sound.ClickButton);

        CloseAllUIMenys();
        CloseAllUIMessages();

        GameObject currentUIBlock = Instantiate(uiblock_HowPlay, transform);

        currentUIBlock.transform.Find("button_ToMeny").GetComponent<Button>().onClick.AddListener(delegate { ButtonDialogAgree(DialogType.ToMeny); });
    }
    public void ButtonMainMenyQuit()
    {
        SoundMaster.CreateSound(Sound.ClickButton);

        GameObject.Find("-Main-").GetComponent<Game>().Quit();
    }
    #endregion
    #region GameUI
    //Fields
    private GameObject CurrentUIBlock;
    private ToolType ToolSelect;
    private BuildingType BuildingSelect;
    private List<string> MessageQueue;
    private bool MessagesLock;
    //Methods
    private void Update()
    {
        if (CurrentUIBlock != null)
        {
            //Mouse dawn or esc
            if (ToolSelect == ToolType.Dig)
            {
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    GameUIOpen();
                }
            }
            else if (ToolSelect == ToolType.Build)
            {
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    GameUIOpen();
                    World.BuildBlockShowRemove();
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    World.BuildBlockShowRotate();
                }
            }
            //Resources
            if (CurrentUIBlock.name == "GameUI")
            {
                CurrentUIBlock.transform.Find("uiblock_ResourceWood").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = World.CurrentLevel.ResourceWoodCurrent.ToString();
                CurrentUIBlock.transform.Find("uiblock_ResourceMeals").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = World.CurrentLevel.ResourceMealCurrent.ToString();
            }
            //SettlerPanel
            if (CurrentDialog == DialogType.SettlerPanel)
            {
                GameObject stats = CurrentUIBlock.transform.Find("uiblock_Dialogs").Find("dialog_SettlerPanel").Find("uiblock_Stats").gameObject;

                stats.transform.Find("item_Health").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.HealthCurrent + "/" + SelectedSettler.HealthMax;
                stats.transform.Find("item_Health").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.HealthMax * SelectedSettler.HealthCurrent);
                stats.transform.Find("item_Energy").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.EnergyCurrent + "/" + SelectedSettler.EnergyMax;
                stats.transform.Find("item_Energy").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.EnergyMax * SelectedSettler.EnergyCurrent);
                stats.transform.Find("item_Hunger").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.HungerCurrent + "/" + SelectedSettler.HungerMax;
                stats.transform.Find("item_Hunger").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.HungerMax * SelectedSettler.HungerCurrent);
                stats.transform.Find("item_Happiness").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.HappinessCurrent + "/" + SelectedSettler.HappinessMax;
                stats.transform.Find("item_Happiness").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.HappinessMax * SelectedSettler.HappinessCurrent);
                stats.transform.Find("item_Heat").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.HeatCurrent + "/" + SelectedSettler.HeatMax;
                stats.transform.Find("item_Heat").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.HeatMax * SelectedSettler.HeatCurrent);

                GameObject inventory = CurrentUIBlock.transform.Find("uiblock_Dialogs").Find("dialog_SettlerPanel").Find("uiblock_Inventory").gameObject;

                for (int i = 1; i <= SelectedSettler.Inventory.Size; i++)
                {
                    if (SelectedSettler.Inventory.CheckOne(i - 1))
                    {
                        inventory.transform.Find("slots").Find("image_Slot" + i).Find("image_Item").GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite_" + SelectedSettler.Inventory.ShowOne(i - 1));
                        inventory.transform.Find("slots").Find("image_Slot" + i).Find("image_Item").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    }
                    else
                    {
                        inventory.transform.Find("slots").Find("image_Slot" + i).Find("image_Item").GetComponent<Image>().sprite = null;
                        inventory.transform.Find("slots").Find("image_Slot" + i).Find("image_Item").GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                    }
                }
            }
        }
        //Skybox
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.4f);
    }
    public void GameUIOpen()
    {
        World.BuildBlockShowRemove();
        ToolSelect = ToolType.Null;
        CurrentDialog = DialogType.Null;

        CloseAllUIMenys();

        if (CurrentUIBlock != null)
            Destroy(CurrentUIBlock);

        if (World.CurrentLevel.Day < 21 && World.CurrentLevel.Settlers.Count > 0)
        {
            CurrentUIBlock = Instantiate(uiblock_GameUI, transform);
            CurrentUIBlock.name = "GameUI";

            //Panel clock and thermometer
            EventTemperatureChanged();
            EventDayChanged();

            //Additional buttons
            CurrentUIBlock.transform.Find("button_ToMeny").GetComponent<Button>().onClick.AddListener(delegate { DialogOpen(DialogType.ToMeny); });
            CurrentUIBlock.transform.Find("button_Sound").GetComponent<Button>().onClick.AddListener(delegate { ButtonSounds(); });

            if (SoundMaster.SoundIsUsedCheck()) CurrentUIBlock.transform.Find("button_Sound").Find("image_Info").GetComponent<Image>().sprite = Resources.Load<Sprite>("gui_SoundOn");
            else CurrentUIBlock.transform.Find("button_Sound").Find("image_Info").GetComponent<Image>().sprite = Resources.Load<Sprite>("gui_SoundOff");
           

            //Panel tools
            CurrentUIBlock.transform.Find("uiblock_PanelTool").Find("button_ToolDig").GetComponent<Button>().onClick.AddListener(delegate { ButtonToolDig(); });
            CurrentUIBlock.transform.Find("uiblock_PanelTool").Find("button_ToolSettlersTask").GetComponent<Button>().onClick.AddListener(delegate { ButtonToolSettlersTask(); });

            //Panel build
            Transform panelBuild = CurrentUIBlock.transform.Find("uiblock_PanelBuild");
            GameObject button_80 = Resources.Load<GameObject>("button_80");
            foreach (Building building in Game.Buildings.Select(kvp => kvp.Value).ToList())
            {
                GameObject newButton = Instantiate(button_80, panelBuild);
                newButton.name = "button_Build" + building.Type;

                if (World.CurrentLevel.Chests.Count < 1 && building.Type != BuildingType.Chest ||
                    Game.Buildings[building.Type].CostWood > World.CurrentLevel.ResourceWoodCurrent)
                {
                    newButton.GetComponent<Image>().color = new Color32(164, 14, 14, 180);
                }

                newButton.transform.Find("image_Info").GetComponent<Image>().sprite = Resources.Load<Sprite>("gui_Build" + building.Type);
                newButton.transform.Find("text_Cost").GetComponent<TextMeshProUGUI>().text = Game.Buildings[building.Type].CostWood.ToString();

                newButton.GetComponent<Button>().onClick.AddListener(delegate { ButtonBuild(building.Type); });
            }
        }
        else if (World.CurrentLevel.Day > 1 && World.CurrentLevel.Day < 21 && World.CurrentLevel.Settlers.Count < 1)
        {
            MessageQueue = new List<string>();
            CloseAllUIMessages();
            CurrentUIBlock = Instantiate(uiblock_EndGame, transform);
            CurrentUIBlock.transform.Find("text_DayInfo").GetComponent<TextMeshProUGUI>().text = "День " + World.CurrentLevel.Day;
            CurrentUIBlock.transform.Find("text_Stat").GetComponent<TextMeshProUGUI>().text = "Все поселенцы погибли...";
            CurrentUIBlock.transform.Find("button_ToMeny").GetComponent<Button>().onClick.AddListener(delegate { ButtonDialogAgree(DialogType.ToMeny); });
        }
        else
        {
            MessageQueue = new List<string>();
            CloseAllUIMessages();
            CurrentUIBlock = Instantiate(uiblock_EndGame, transform);
            CurrentUIBlock.transform.Find("text_Stat").GetComponent<TextMeshProUGUI>().text = World.CurrentLevel.Settlers.Count + " Поселенцев выжило";
            CurrentUIBlock.transform.Find("button_ToMeny").GetComponent<Button>().onClick.AddListener(delegate { ButtonDialogAgree(DialogType.ToMeny); });
        }
    }
    public void ClickToObjectInWorld(GameObject currentObject)
    {
        SoundMaster.CreateSound(Sound.Build);

        if (currentObject.name == "Wall")
        {
            switch (ToolSelect)
            {
                case ToolType.Dig:
                    World.DigBlock(currentObject.transform);
                    CurrentUIBlock.transform.Find("uiblock_DigTool").Find("text_Data").GetComponent<TextMeshProUGUI>().text = World.CurrentLevel.NumberOfPicksCurrent + "/" + World.CurrentLevel.NumberOfPicksMax;
                    break;
            }
        }
        else if (currentObject.name == "Floor")
        {
            switch (ToolSelect)
            {
                case ToolType.Build:
                    World.BuildBlock(BuildingSelect, currentObject.transform);
                    GameUIOpen();
                    break;
            }
        }
        else if (currentObject.tag == "Settler")
        {
            SelectedSettler = currentObject.GetComponent<Creature>();
            DialogOpen(DialogType.SettlerPanel);
        }
    }
    public void EnterToObjectInWorld(GameObject currentObject)
    {
        if (currentObject.name == "Floor")
        {
            switch (ToolSelect)
            {
                case ToolType.Build:
                    World.BuildBlockShow(BuildingSelect, currentObject.transform);
                    break;
            }
        }
    }
    //Buttons
    public void ButtonSounds()
    {
        if (SoundMaster.SoundIsUsedCheck())
        {
            CurrentUIBlock.transform.Find("button_Sound").Find("image_Info").GetComponent<Image>().sprite = Resources.Load<Sprite>("gui_SoundOff");
            SoundMaster.SoundOff();
        }
        else
        {
            CurrentUIBlock.transform.Find("button_Sound").Find("image_Info").GetComponent<Image>().sprite = Resources.Load<Sprite>("gui_SoundOn");
            SoundMaster.SoundOn();
        }
    }
    public void ButtonToolDig()
    {
        SoundMaster.CreateSound(Sound.ClickButton);

        GameUIOpen();

        ToolSelect = ToolType.Dig;

        CurrentUIBlock.transform.Find("uiblock_DigTool").gameObject.SetActive(true);
        CurrentUIBlock.transform.Find("uiblock_DigTool").Find("text_Data").GetComponent<TextMeshProUGUI>().text = World.CurrentLevel.NumberOfPicksCurrent + "/" + World.CurrentLevel.NumberOfPicksMax;

        World.BuildBlockShowRemove();
    }
    public void ButtonToolSettlersTask()
    {
        SoundMaster.CreateSound(Sound.ClickButton);

        GameUIOpen();

        ToolSelect = ToolType.Null;

        DialogOpen(DialogType.SettlersTask);

        World.BuildBlockShowRemove();
    }
    public void ButtonBuild(BuildingType type)
    {
        SoundMaster.CreateSound(Sound.ClickButton);

        GameUIOpen();

        ToolSelect = ToolType.Build;
        BuildingSelect = type;

        CurrentUIBlock.transform.Find("uiblock_BuildTool").gameObject.SetActive(true);

        World.BuildBlockShowRemove();
    }
    //Events
    public void EventTemperatureChanged()
    {
        int currentTemperature = World.CurrentLevel.Temperature;

        CurrentUIBlock.transform.Find("uiblock_ClockAndThermometer").Find("text_Temperature").GetComponent<TextMeshProUGUI>().text = currentTemperature + "°C";

        if (currentTemperature > 25)
        {
            CurrentUIBlock.transform.Find("uiblock_ClockAndThermometer").Find("text_Temperature").GetComponent<TextMeshProUGUI>().color = new Color32(231, 76, 60, 255);
        }
        else if (currentTemperature < 5)
        {
            CurrentUIBlock.transform.Find("uiblock_ClockAndThermometer").Find("text_Temperature").GetComponent<TextMeshProUGUI>().color = new Color32(25, 181, 254, 255);
        }
        else
        {
            CurrentUIBlock.transform.Find("uiblock_ClockAndThermometer").Find("text_Temperature").GetComponent<TextMeshProUGUI>().color = new Color32(249, 179, 47, 255);
        }
    }
    public void EventDayChanged()
    {
        CurrentUIBlock.transform.Find("uiblock_ClockAndThermometer").Find("image_Info").Find("text_AmountOfDays").GetComponent<TextMeshProUGUI>().text = World.CurrentLevel.Day.ToString();
    }
    public void EventShowMessage(string message)
    {
        if (MessageQueue == null)
            MessageQueue = new List<string>();

        MessageQueue.Add(message);

        if (!MessagesLock)
            StartCoroutine(EventShowMessage());
    }
    public IEnumerator EventShowMessage()
    {
        MessagesLock = true;

        GameObject messageBox = Instantiate(uiblock_Message, transform);
        messageBox.tag = "Message";
        messageBox.transform.Find("text_Data").GetComponent<TextMeshProUGUI>().text = MessageQueue[0];

        MessageQueue.RemoveAt(0);

        if (MessageQueue.Count <= 3)
            yield return new WaitForSeconds(5f);
        else if (MessageQueue.Count <= 7)
            yield return new WaitForSeconds(3f);
        else
            yield return new WaitForSeconds(1f);

        CloseAllUIMessages();

        if (MessageQueue.Count > 0)
        {
            StartCoroutine(EventShowMessage());
        }
        else
        {
            MessagesLock = false;
        }
    }
    #endregion
    #region Dialogs
    //Fields
    private DialogType CurrentDialog = DialogType.Null;
    public Creature SelectedSettler { get; set; }
    //Methods
    public void DialogOpen(DialogType dialog)
    {
        GameUIOpen();

        CurrentDialog = dialog;

        GameObject dialogsWindow = CurrentUIBlock.transform.Find("uiblock_Dialogs").gameObject;
        GameObject currentDialogObject;

        switch (CurrentDialog)
        {
            case DialogType.ToMeny:
                currentDialogObject = Instantiate(dialog_ToMeny, dialogsWindow.transform);
                currentDialogObject.transform.Find("button_Yes").GetComponent<Button>().onClick.AddListener(delegate { ButtonDialogAgree(DialogType.ToMeny); });
                currentDialogObject.transform.Find("button_No").GetComponent<Button>().onClick.AddListener(delegate { ButtonDialogClose(); });
                break;
            case DialogType.SettlerPanel:
                currentDialogObject = Instantiate(dialog_SettlerPanel, dialogsWindow.transform);
                currentDialogObject.name = "dialog_SettlerPanel";
                currentDialogObject.transform.Find("button_Close").GetComponent<Button>().onClick.AddListener(delegate { ButtonDialogClose(); });
                currentDialogObject.transform.Find("button_Exile").GetComponent<Button>().onClick.AddListener(delegate { ButtonExileTheSettler(SelectedSettler); });

                currentDialogObject.transform.Find("uiblock_Names").Find("item_Name").Find("text_NameData").GetComponent<TextMeshProUGUI>().text = SelectedSettler.Name;
                currentDialogObject.transform.Find("uiblock_Names").Find("item_Surname").Find("text_SurnameData").GetComponent<TextMeshProUGUI>().text = SelectedSettler.Surname;

                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Health").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.HealthCurrent + "/" + SelectedSettler.HealthMax;
                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Health").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.HealthMax * SelectedSettler.HealthCurrent);

                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Energy").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.EnergyCurrent + "/" + SelectedSettler.EnergyMax;
                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Energy").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.EnergyMax * SelectedSettler.EnergyCurrent);

                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Hunger").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.HungerCurrent + "/" + SelectedSettler.HungerMax;
                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Hunger").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.HungerMax * SelectedSettler.HungerCurrent);

                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Happiness").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.HappinessCurrent + "/" + SelectedSettler.HappinessMax;
                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Happiness").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.HappinessMax * SelectedSettler.HappinessCurrent);

                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Heat").Find("text_Amount").GetComponent<TextMeshProUGUI>().text = SelectedSettler.HeatCurrent + "/" + SelectedSettler.HeatMax;
                currentDialogObject.transform.Find("uiblock_Stats").Find("item_Heat").Find("bar_Amount").GetChild(0).GetComponent<Image>().fillAmount = 0.01f * (100 / SelectedSettler.HeatMax * SelectedSettler.HeatCurrent);

                for (int i = 1; i <= SelectedSettler.Inventory.Size; i++)
                {
                    if (SelectedSettler.Inventory.CheckOne(i - 1))
                    {
                        currentDialogObject.transform.Find("uiblock_Inventory").Find("slots").Find("image_Slot" + i).Find("image_Item").GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite_" + SelectedSettler.Inventory.ShowOne(i - 1));
                        currentDialogObject.transform.Find("uiblock_Inventory").Find("slots").Find("image_Slot" + i).Find("image_Item").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    }
                }

                bool bedStatys = SelectedSettler.MyBed;
                if (bedStatys)
                {
                    currentDialogObject.transform.Find("uiblock_BedStatys").Find("text_Data").GetComponent<TextMeshProUGUI>().text = "ЕСТЬ";
                    currentDialogObject.transform.Find("uiblock_BedStatys").Find("text_Data").GetComponent<TextMeshProUGUI>().color = new Color32(30, 188, 97, 255);
                }
                else
                {
                    currentDialogObject.transform.Find("uiblock_BedStatys").Find("text_Data").GetComponent<TextMeshProUGUI>().text = "НЕТУ";
                    currentDialogObject.transform.Find("uiblock_BedStatys").Find("text_Data").GetComponent<TextMeshProUGUI>().color = new Color32(183, 28, 12, 255);
                }
                break;
            case DialogType.SettlersTask:
                currentDialogObject = Instantiate(dialog_SettlersTask, dialogsWindow.transform);
                currentDialogObject.transform.Find("text_SettlersCount").GetComponent<TextMeshProUGUI>().text = "Поселенцев: " + World.CurrentLevel.Settlers.Count;
                currentDialogObject.transform.Find("button_Close").GetComponent<Button>().onClick.AddListener(delegate { ButtonDialogClose(); });

                foreach (Creature settler in World.CurrentLevel.Settlers)
                {
                    GameObject content = currentDialogObject.transform.Find("scrollview_Settler").Find("Viewport").Find("Content").gameObject;
                    content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, content.GetComponent<RectTransform>().sizeDelta.y + item_Settler.GetComponent<RectTransform>().sizeDelta.y + 5);

                    GameObject item = Instantiate(item_Settler, content.transform);
                    item.transform.Find("Head").GetComponent<Image>().sprite = settler.Head;
                    item.transform.Find("Head").Find("Face").GetComponent<Image>().sprite = settler.Face;

                    item.transform.Find("NameSurname").GetComponent<TextMeshProUGUI>().text = settler.Name + " " + settler.Surname;

                    item.transform.Find("uiblock_Buttons").Find("button_Info").GetComponent<Button>().onClick.AddListener(delegate { ButtonOpenSettlerPanel(settler); });
                    item.transform.Find("uiblock_Buttons").Find("button_Rest").GetComponent<Button>().onClick.AddListener(delegate { ButtonChangeSettlerTask(settler, CreatureTaskType.DoingNothing, CreatureWorkType.Null); });
                    item.transform.Find("uiblock_Buttons").Find("button_HarvestWood").GetComponent<Button>().onClick.AddListener(delegate { ButtonChangeSettlerTask(settler, CreatureTaskType.Works, CreatureWorkType.Lumberjack); });
                    item.transform.Find("uiblock_Buttons").Find("button_HarvestMeal").GetComponent<Button>().onClick.AddListener(delegate { ButtonChangeSettlerTask(settler, CreatureTaskType.Works, CreatureWorkType.Collector); });
                    item.transform.Find("uiblock_Buttons").Find("button_Isolate").GetComponent<Button>().onClick.AddListener(delegate { ButtonChangeSettlerTask(settler, CreatureTaskType.Isolates, CreatureWorkType.Null); });

                    switch (settler.Task)
                    {
                        case CreatureTaskType.DoingNothing:
                            item.transform.Find("uiblock_Buttons").Find("button_Rest").GetComponent<Image>().color = new Color32(249, 179, 46, 255);
                            break;
                        case CreatureTaskType.Works:
                            switch (settler.Work)
                            {
                                case CreatureWorkType.Lumberjack:
                                    item.transform.Find("uiblock_Buttons").Find("button_HarvestWood").GetComponent<Image>().color = new Color32(249, 179, 46, 255);
                                    break;
                                case CreatureWorkType.Collector:
                                    item.transform.Find("uiblock_Buttons").Find("button_HarvestMeal").GetComponent<Image>().color = new Color32(249, 179, 46, 255);
                                    break;
                            }
                            break;
                        case CreatureTaskType.Isolates:
                            item.transform.Find("uiblock_Buttons").Find("button_Isolate").GetComponent<Image>().color = new Color32(249, 179, 46, 255);
                            break;
                    }
                }
                break;
        }
    }
    //Buttons
    public void ButtonDialogAgree(DialogType dialog)
    {
        SoundMaster.CreateSound(Sound.ClickButton);
        switch (dialog)
        {
            case DialogType.ToMeny:
                //World
                World.Clear();
                //UI
                CloseAllUIMenys();
                MainMenyOpen();
                break;
        }
    }
    public void ButtonDialogClose()
    {
        CloseAllUIMenys();

        SoundMaster.CreateSound(Sound.ClickButton);

        GameUIOpen();
    }
    public void ButtonChangeSettlerTask(Creature settler, CreatureTaskType taskType, CreatureWorkType workType)
    {
        SoundMaster.CreateSound(Sound.ClickButton);

        settler.ChangeTask(taskType, workType);

        GameUIOpen();

        DialogOpen(DialogType.SettlersTask);
    }
    public void ButtonOpenSettlerPanel(Creature settler)
    {
        SoundMaster.CreateSound(Sound.ClickButton);

        SelectedSettler = settler;

        GameUIOpen();

        DialogOpen(DialogType.SettlerPanel);
    }
    public void ButtonExileTheSettler(Creature settler)
    {
        SoundMaster.CreateSound(Sound.ClickButton);
        settler.Breakdown = true;
    }
    #endregion
}