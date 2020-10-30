using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Creature : MonoBehaviour
{
    #region Fields
    //Attribute
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public Sprite Head { get; private set; }
    public Sprite Face { get; private set; }
    public Sprite Body { get; private set; }
    //Characteristic
    public int HealthMax { get; private set; }
    public int HealthCurrent;
    public int EnergyMax { get; private set; }
    public int EnergyCurrent;
    public int HungerMax { get; private set; }
    public int HungerCurrent;
    public int HappinessMax { get; private set; }
    public int HappinessCurrent;
    public int HeatMax { get; private set; }
    public int HeatCurrent;
    //Items
    public Inventory Inventory { get; private set; }
    //Tech
    public CreatureTaskType Task { get; private set; }
    public CreatureWorkType Work { get; private set; }
    private List<Vector3> Path;
    private Vector3 PositionOld;
    private Vector3 PositionNew;
    private float MovingStep;
    private float MovingProgress;
    private bool IsMoving;
    private bool IsSleep;
    public bool Breakdown;
    public bool IsSick;
    public int StageOfTheDisease = 0;
    //Scripts
    private Game Game;
    private World World;
    private UI UI;
    public BuildingObject MyBed { get; private set; }
    private ResourceSource SelectedTree;
    #endregion
    #region Constructor
    public void Constructor(string name, string surname, int healthMax, int energyMax, int hungerMax, int happinessMax, int heatMax)
    {
        Game = GameObject.Find("-Main-").GetComponent<Game>();
        World = GameObject.Find("-World-").GetComponent<World>();
        UI = GameObject.Find("-UI-").GetComponent<UI>();

        Name = name;
        Surname = surname;

        Head = Game.SkinsHead[Random.Range(0, Game.SkinsHead.Count)];
        Face = Game.SkinsFace[Random.Range(0, Game.SkinsFace.Count)];
        Body = Game.SkinsBody[Random.Range(0, Game.SkinsBody.Count)];

        transform.Find("text_Name").GetComponent<TextMeshPro>().text = Name;

        transform.Find("Skin").Find("Head").GetComponent<SpriteRenderer>().sprite = Head;
        transform.Find("Skin").Find("Face").GetComponent<SpriteRenderer>().sprite = Face;
        transform.Find("Skin").Find("Body").GetComponent<SpriteRenderer>().sprite = Body;

        //Health
        HealthMax = healthMax;
        HealthCurrent = HealthMax;
        //Energy
        EnergyMax = energyMax;
        EnergyCurrent = EnergyMax;
        //Hunger
        HungerMax = hungerMax;
        HungerCurrent = 0;
        //Happiness
        HappinessMax = happinessMax;
        HappinessCurrent = HappinessMax / 2;
        //Heat
        HeatMax = heatMax;
        HeatCurrent = HeatMax / 2;

        MovingStep = 0.04f;

        Inventory = new Inventory(5);

        StartCoroutine(WhatWillIDo());
        StartCoroutine(UpdateStats());
    }
    #endregion
    #region Tasks
    public void GoTo(Vector3 position)
    {
        PathFind(position);
    }
    public void ChangeTask(CreatureTaskType taskType, CreatureWorkType workType)
    {
        SelectedTree = null;

        Task = taskType;
        Work = workType;
    }
    #endregion
    #region Analysis
    private IEnumerator WhatWillIDo()
    {
        CreatureActionType decision = SituationAssessment();

        switch (decision)
        {
            case CreatureActionType.GoToARandomPoint:
                GoTo(new Vector3(Random.Range(0, World.CurrentLevel.Cells.GetLength(0)), 0.5f, Random.Range(0, World.CurrentLevel.Cells.GetLength(1))));
                break;
            case CreatureActionType.TakeTheBed:
                foreach (BuildingObject bed in World.CurrentLevel.Beds)
                {
                    if (!bed.TheBedIsBusy)
                    {
                        MyBed = bed.TakeTheBed();
                        break;
                    }
                }
                break;
            case CreatureActionType.GoToWork:
                //Tree find
                if (SelectedTree == null)
                {
                    bool treeIsSelect = false;
                    foreach (ResourceSource tree in World.CurrentLevel.Trees)
                    {
                        if (!treeIsSelect)
                        {
                            switch (Work)
                            {
                                case CreatureWorkType.Lumberjack:
                                    if (tree.StockOfWood > 0)
                                    {
                                        if (World.CurrentLevel.Trees.Count < 10)
                                        {
                                            SelectedTree = tree;
                                            treeIsSelect = true;
                                        }
                                        else if (Random.Range(0, 5) == 0)
                                        {
                                            SelectedTree = tree;
                                            treeIsSelect = true;
                                        }
                                        break;
                                    }
                                    break;
                                case CreatureWorkType.Collector:
                                    if (tree.StockOfMeal > 0)
                                    {
                                        if (World.CurrentLevel.Trees.Count < 10)
                                        {
                                            SelectedTree = tree;
                                            treeIsSelect = true;
                                        }
                                        else if (Random.Range(0, 3) == 0)
                                        {
                                            SelectedTree = tree;
                                            treeIsSelect = true;
                                        }
                                        break;
                                    }
                                    break;
                            }
                        }
                        else break;
                    }
                }
                //Move to tree
                if (transform.position.x != SelectedTree.transform.position.x &&
                   transform.position.z != SelectedTree.transform.position.z)
                {
                    GoTo(SelectedTree.transform.position);
                }
                else
                {
                    if (Inventory.HaveEmptySlots())
                    {
                        switch (Work)
                        {
                            case CreatureWorkType.Lumberjack:
                                if (SelectedTree.TakeOne(ResourceType.Wood) == 1)
                                {
                                    Inventory.Put(ItemType.Log);
                                }
                                else
                                {
                                    SelectedTree.Destroy();
                                }
                                break;
                            case CreatureWorkType.Collector:
                                if (SelectedTree.TakeOne(ResourceType.Meal) == 1)
                                {
                                    Inventory.Put(ItemType.Apple);
                                }
                                else
                                {
                                    SelectedTree = null;
                                }
                                break;
                        }

                    }
                }
                break;
            case CreatureActionType.GoToTheChest:
                if (World.CurrentLevel.Chests.Count > 0)
                {
                    if (World.CurrentLevel.Chests[0].transform.position.x != transform.position.x &&
                       World.CurrentLevel.Chests[0].transform.position.z != transform.position.z)
                    {
                        GoTo(World.CurrentLevel.Chests[0].transform.position);
                    }
                    else
                    {
                        for (int i = 0; i < Inventory.Size; i++)
                        {
                            if (Inventory.Get(ItemType.Log))
                            {
                                World.CurrentLevel.ResourceWoodCurrent += 1;
                            }
                            if (Inventory.Get(ItemType.Apple))
                            {
                                World.CurrentLevel.ResourceMealCurrent += 1;
                            }
                        }
                    }
                }
                else UI.EventShowMessage(Name + " " + Surname + " не может найти сундук");
                break;
            case CreatureActionType.GoEat:
                if (World.CurrentLevel.Chests.Count > 0)
                {
                    if (World.CurrentLevel.Chests[0].transform.position.x != transform.position.x &&
                       World.CurrentLevel.Chests[0].transform.position.z != transform.position.z)
                    {
                        GoTo(World.CurrentLevel.Chests[0].transform.position);
                    }
                    else
                    {
                        if (World.CurrentLevel.ResourceMealCurrent > 50)
                        {
                            HungerCurrent = 0;
                            World.CurrentLevel.ResourceMealCurrent -= 3;
                        }
                        else if (World.CurrentLevel.ResourceMealCurrent > 0)
                        {
                            HungerCurrent -= 8;
                            World.CurrentLevel.ResourceMealCurrent -= 1;
                        }
                        else
                        {
                            UI.EventShowMessage(Name + " " + Surname + " ЕДА КОНЧИЛАСЬ ЧЕ МЫ БУДЕМ ЖРАТЬ!!!!");
                        }
                    }
                }
                else UI.EventShowMessage(Name + " " + Surname + " хочет есть, а сундуков нету");
                break;
            case CreatureActionType.GoToBed:
                if (MyBed != null)
                {
                    if (MyBed.transform.position.x != transform.position.x &&
                        MyBed.transform.position.z != transform.position.z)
                    {
                        GoTo(MyBed.transform.position);
                    }
                    else
                    {
                        if (EnergyCurrent < 70)
                        {
                            EnergyCurrent += 10;
                            IsSleep = true;
                        }
                        else IsSleep = false;

                        if (!IsSleep)
                        {
                            HappinessCurrent -= 2;
                            EnergyCurrent += 4;
                        }

                        if (World.CurrentLevel.Temperature > 20) HeatCurrent += 4;
                        else if (World.CurrentLevel.Temperature > 10) HeatCurrent += 2;
                        else if (World.CurrentLevel.Temperature > 0) HeatCurrent += 1;
                        else if (World.CurrentLevel.Temperature > -10) HeatCurrent += 0;
                        else HeatCurrent -= 1;

                    }
                }
                else UI.EventShowMessage(Name + " " + Surname + " хочет кровать, у него нету кровати!");
                break;
            case CreatureActionType.TalkNonsense:
                switch (Random.Range(0, 12))
                {
                    case 0: UI.EventShowMessage(Name + " " + Surname + " какой чудесный день!"); break;
                    case 1: UI.EventShowMessage(Name + " " + Surname + " почему я не работаю"); break;
                    case 2: UI.EventShowMessage(Name + " " + Surname + " подозревает что " + NameGenerator.NewName() + " " + NameGenerator.NewSurname() + " болен"); break;
                    default:
                        break;
                }
                break;
            case CreatureActionType.Exile:
                if (transform.position.x < 1 ||
                    transform.position.z < 1 ||
                    transform.position.x > World.CurrentLevel.Cells.GetLength(0) - 2 ||
                    transform.position.z > World.CurrentLevel.Cells.GetLength(1) - 2)
                {
                    Death(Name + " " + Surname + " ушел из поселения");
                }

                if (!IsMoving)
                {
                    Vector3 exilePosition;

                    switch (Random.Range(0, 4))
                    {
                        default:
                        case 0: exilePosition = new Vector3(Random.Range(0, World.CurrentLevel.Cells.GetLength(0)), 0, 0); break;
                        case 1: exilePosition = new Vector3(0, 0, Random.Range(0, World.CurrentLevel.Cells.GetLength(1))); break;
                        case 2: exilePosition = new Vector3(Random.Range(0, World.CurrentLevel.Cells.GetLength(0)), 0, World.CurrentLevel.Cells.GetLength(1) - 1); break;
                        case 3: exilePosition = new Vector3(World.CurrentLevel.Cells.GetLength(0) - 1, 0, Random.Range(0, World.CurrentLevel.Cells.GetLength(1))); break;
                    }

                    GoTo(exilePosition);
                }
                break;
        }

        yield return new WaitForSeconds(3f);
        StartCoroutine(WhatWillIDo());
    }
    private CreatureActionType SituationAssessment()
    {
        if (!IsSleep)
        {
            if (Breakdown)
            {
                return CreatureActionType.Exile;
            }
            else if (MyBed == null && World.CurrentLevel.CheckAvailabilityOfBeds())
            {
                return CreatureActionType.TakeTheBed;
            }
            else if (HungerCurrent > 10 && Task != CreatureTaskType.Isolates && Task != CreatureTaskType.Works)
            {
                return CreatureActionType.GoEat;
            }
            else if (HungerCurrent > 15)
            {
                return CreatureActionType.GoEat;
            }
            else if (EnergyCurrent < 20)
            {
                return CreatureActionType.GoToBed;
            }
            else if (Task == CreatureTaskType.Works)
            {
                if (Inventory.HaveEmptySlots())
                    return CreatureActionType.GoToWork;
                else
                    return CreatureActionType.GoToTheChest;
            }
            else if (Task == CreatureTaskType.Isolates && HungerCurrent < 16)
            {
                return CreatureActionType.GoToBed;
            }
            else if (Task == CreatureTaskType.DoingNothing && !IsMoving)
            {
                if (Random.Range(0, 2) == 0)
                    return CreatureActionType.GoToARandomPoint;
                else return CreatureActionType.TalkNonsense;
            }
            else return CreatureActionType.Null;
        }
        else return CreatureActionType.GoToBed;
    }
    private void PathFind(Vector3 finish)
    {
        Vector3Int startInt = new Vector3Int((int)transform.position.x, 0, (int)transform.position.z);
        Vector3Int finishInt = new Vector3Int((int)finish.x, 0, (int)finish.z);

        int mapSizeX = World.CurrentLevel.Cells.GetLength(0);
        int mapSizeZ = World.CurrentLevel.Cells.GetLength(1);

        if (World.CurrentLevel.Cells[finishInt.x, finishInt.z].WallBlock == BlockType.Null)
        {
            int[,] pathMap = new int[mapSizeX, mapSizeZ];
            List<Vector3Int> positionsToCheck = new List<Vector3Int>();

            for (int x = 0; x < mapSizeX; x++)
            {
                for (int z = 0; z < mapSizeZ; z++)
                {
                    if (World.CurrentLevel.Cells[x, z].WallBlock != BlockType.Null)
                    {
                        pathMap[x, z] = -1;
                    }
                }
            }

            positionsToCheck.Add(new Vector3Int(startInt.x, startInt.y, startInt.z));

            int distance = 1;
            bool finishFound = false;

            while (!finishFound)
            {
                if (positionsToCheck.Count > 0)
                {
                    List<Vector3Int> addPositionsToCheck = new List<Vector3Int>();
                    foreach (Vector3Int currentPosition in positionsToCheck)
                    {
                        if (finishInt.x != currentPosition.x || finishInt.z != currentPosition.z)
                        {
                            if (currentPosition.x + 1 < pathMap.GetLength(0))
                            {
                                if (pathMap[currentPosition.x + 1, currentPosition.z] == 0)
                                {
                                    pathMap[currentPosition.x + 1, currentPosition.z] = distance;
                                    addPositionsToCheck.Add(new Vector3Int(currentPosition.x + 1, currentPosition.y, currentPosition.z));
                                }
                            }
                            if (currentPosition.x - 1 > 0)
                            {
                                if (pathMap[currentPosition.x - 1, currentPosition.z] == 0)
                                {
                                    pathMap[currentPosition.x - 1, currentPosition.z] = distance;
                                    addPositionsToCheck.Add(new Vector3Int(currentPosition.x - 1, currentPosition.y, currentPosition.z));
                                }
                            }
                            if (currentPosition.z + 1 < pathMap.GetLength(1))
                            {
                                if (pathMap[currentPosition.x, currentPosition.z + 1] == 0)
                                {
                                    pathMap[currentPosition.x, currentPosition.z + 1] = distance;
                                    addPositionsToCheck.Add(new Vector3Int(currentPosition.x, currentPosition.y, currentPosition.z + 1));
                                }
                            }
                            if (currentPosition.z - 1 > 0)
                            {
                                if (pathMap[currentPosition.x, currentPosition.z - 1] == 0)
                                {
                                    pathMap[currentPosition.x, currentPosition.z - 1] = distance;
                                    addPositionsToCheck.Add(new Vector3Int(currentPosition.x, currentPosition.y, currentPosition.z - 1));
                                }
                            }
                        }
                        else
                        {
                            pathMap[currentPosition.x, currentPosition.z] = distance;
                            finishFound = true;
                            break;
                        }
                    }
                    positionsToCheck = addPositionsToCheck;
                    distance += 1;
                }
                else
                {
                    Debug.Log("Невозможно дойти (" + Name + "_" + Surname + ")");
                    break;
                }
            }

            List<Vector3Int> path = new List<Vector3Int>();

            path.Add(new Vector3Int(finishInt.x, 0, finishInt.z));

            for (int i = distance - 1; i > 0; i--)
            {
                Vector3Int newPosition = new Vector3Int(path[path.Count - 1].x, 0, path[path.Count - 1].z);

                if (path[path.Count - 1].x + 1 < pathMap.GetLength(0) - 1)
                {
                    if (pathMap[path[path.Count - 1].x + 1, path[path.Count - 1].z] == i)
                    {
                        newPosition = new Vector3Int(path[path.Count - 1].x + 1, 0, path[path.Count - 1].z);
                    }
                }
                if (path[path.Count - 1].x - 1 > 0)
                {
                    if (pathMap[path[path.Count - 1].x - 1, path[path.Count - 1].z] == i)
                    {
                        newPosition = new Vector3Int(path[path.Count - 1].x - 1, 0, path[path.Count - 1].z);
                    }
                }
                if (path[path.Count - 1].z + 1 < pathMap.GetLength(1) - 1)
                {
                    if (pathMap[path[path.Count - 1].x, path[path.Count - 1].z + 1] == i)
                    {
                        newPosition = new Vector3Int(path[path.Count - 1].x, 0, path[path.Count - 1].z + 1);
                    }
                }
                if (path[path.Count - 1].z - 1 > 0)
                {
                    if (pathMap[path[path.Count - 1].x, path[path.Count - 1].z - 1] == i)
                    {
                        newPosition = new Vector3Int(path[path.Count - 1].x, 0, path[path.Count - 1].z - 1);
                    }
                }

                path.Add(newPosition);
            }

            //--------------TEST
            //string test = "";
            //for (int i = 0; i < path.Count; i++)
            //{
            //    test += path[i].x + "_" + path[i].z + "/";
            //}
            //Debug.Log(test);
            //Debug.Log(startInt);
            //Debug.Log(finishInt);
            //--------------TEST

            Path = new List<Vector3>();
            foreach (Vector3Int item in path)
            {
                Path.Add(new Vector3(item.x, 0.5f, item.z));
            }

            PositionOld = transform.position;
            PositionNew = Path[Path.Count - 1];

            IsMoving = true;
        }
        else Debug.Log("Там стена, невозможно пройти (" + Name + "_" + Surname + ")");
    }
    #endregion
    #region Actions
    private void FixedUpdate()
    {
        if (IsMoving)
            Moving();
    }
    private void Moving()
    {
        if (transform.position != PositionNew)
        {
            transform.position = Vector3.Lerp(PositionOld, PositionNew, MovingProgress);
            MovingProgress += MovingStep;
        }
        else if (Path.Count > 0)
        {
            MovingProgress = 0;

            Path.RemoveAt(Path.Count - 1);

            if (Path.Count > 0)
            {
                PositionOld = transform.position;
                PositionNew = Path[Path.Count - 1];
            }
        }
        else IsMoving = false;
    }
    private IEnumerator UpdateStats()
    {
        //Chanjes
        if (!IsSleep)
        {
            if (Task == CreatureTaskType.Works) EnergyCurrent -= 2;
            else EnergyCurrent -= 1;
        }

        if (Task == CreatureTaskType.Works && Random.Range(0, 2) == 0) HungerCurrent += 1;
        else if (Random.Range(0, 3) == 0) HungerCurrent += 1;

        if (HungerCurrent > 19) HealthCurrent -= 5;

        if (EnergyCurrent < 20) HappinessCurrent -= 2;

        if (MyBed != null && !IsSick)
        {
            if (MyBed.transform.position.x != transform.position.x &&
               MyBed.transform.position.z != transform.position.z)
            {
                HappinessCurrent += 1;
            }
        }
        else HappinessCurrent += 1;

        //Heat check
        int heatCheck = 0;

        if (World.CurrentLevel.Temperature > 25) heatCheck += 4;
        else if (World.CurrentLevel.Temperature > 20) heatCheck += 2;
        else if (World.CurrentLevel.Temperature > 15) heatCheck += 1;
        else if (World.CurrentLevel.Temperature > 10) heatCheck -= 1;
        else if (World.CurrentLevel.Temperature > 5) heatCheck -= 2;
        else if (World.CurrentLevel.Temperature > 0) heatCheck -= 4;
        else if (World.CurrentLevel.Temperature > -5) heatCheck -= 8;
        else if (World.CurrentLevel.Temperature > -10) heatCheck -= 16;

        if (MyBed != null)
        {
            if (MyBed.transform.position.x != transform.position.x &&
               MyBed.transform.position.z != transform.position.z)
            {
                HeatCurrent += heatCheck;
            }
        }
        else HeatCurrent += heatCheck;

        //Correct
        if (HeatCurrent > HealthMax) HeatCurrent = HealthMax;
        if (EnergyCurrent < 0) EnergyCurrent = 0;
        if (EnergyCurrent > EnergyMax) EnergyCurrent = EnergyMax;
        if (HungerCurrent < 0) HungerCurrent = 0;
        if (HungerCurrent > HungerMax) HungerCurrent = HungerMax;
        if (HappinessCurrent < 0) HappinessCurrent = 0;
        if (HappinessCurrent > HappinessMax) HappinessCurrent = HappinessMax;
        if (HeatCurrent < 0) HeatCurrent = 0;
        if (HeatCurrent > HeatMax) HeatCurrent = HeatMax;

        if (HealthCurrent < 1)
            Death(Name + " " + Surname + " Погиб от недостатка здоровья");
        else if (HeatCurrent < 1)
            Death(Name + " " + Surname + " Погиб от обморожения");

        if (HappinessCurrent < 1)
            Breakdown = true;

        //Sick
        if (IsSick)
        {
            if (StageOfTheDisease == 0)
            {
                HealthCurrent -= 1;

                if (MyBed != null)
                {
                    if (MyBed.transform.position.x == transform.position.x &&
                       MyBed.transform.position.z == transform.position.z)
                    {
                        if (EnergyCurrent > 50)
                        {
                            IsSick = false;
                            StageOfTheDisease = 0;
                        }
                    }
                }

                if (Random.Range(0, 6) == 0) StageOfTheDisease = 1;
            }
            else if (StageOfTheDisease == 1)
            {
                HealthCurrent -= 3;
                HappinessCurrent -= 1;

                InfectARandomSettler();
                InfectARandomSettler();

                if (Random.Range(0, 8) == 0) StageOfTheDisease = 2;
            }
            else if (StageOfTheDisease == 2)
            {
                HealthCurrent -= 10;

                transform.Find("Skin").Find("Head").GetComponent<SpriteRenderer>().color = new Color32(78, 236, 145, 255);
                transform.Find("Skin").Find("Face").GetComponent<SpriteRenderer>().color = new Color32(78, 236, 145, 255);
                transform.Find("Skin").Find("Body").GetComponent<SpriteRenderer>().color = new Color32(78, 236, 145, 255);

                InfectARandomSettler();
                InfectARandomSettler();
                InfectARandomSettler();

                if (Random.Range(0, 10) == 0) Death(Name + " " + Surname + " был заражен и погиб");
            }
        }

        yield return new WaitForSeconds(7f);
        StartCoroutine(UpdateStats());
    }
    private void InfectARandomSettler()
    {
        if (Random.Range(0, 3) == 0)
        {
            World.CurrentLevel.Settlers[Random.Range(0, World.CurrentLevel.Settlers.Count)].IsSick = true;
        }
    }
    private void Death(string message)
    {
        if (MyBed != null)
            MyBed.EmptyTheBed();

        SoundMaster.CreateSound(Sound.Dead);

        UI.EventShowMessage(message);

        World.CurrentLevel.Settlers.Remove(this);
        Destroy(gameObject);
    }
    #endregion
}