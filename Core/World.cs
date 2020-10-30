using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    #region Main
    //Level
    public Level CurrentLevel;
    //Objects
    public GameObject Blocks;
    public GameObject Creatures;
    //Scripts
    private Game Game;
    private UI UI;
    //Tech
    private GameObject BuildZoneObject;
    public Vector3 BuildZoneObjectRotate { get; private set; }
    //Methods
    private void Awake()
    {
        Game = GameObject.Find("-Main-").GetComponent<Game>();
        UI = GameObject.Find("-UI-").GetComponent<UI>();
    }
    #endregion
    #region Generator
    //Main
    public void StartGenerator()
    {
        Clear();

        Blocks = new GameObject("Blocks");
        Blocks.transform.parent = transform;
        Creatures = new GameObject("Creatures");
        Creatures.transform.parent = transform;

        CurrentLevel = new Level();

        //Start cycles
        GenerateEmptyField();
        GenerateCentralRock();
        GenerateTrees();

        for (int i = 0; i < 4; i++)
        {
            CreateCreature();
        }

        Invoke("StartGameEventsSystem", 30f);
    }
    public void Clear()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }
    //Cycles
    private void GenerateEmptyField()
    {
        for (int x = 0; x < CurrentLevel.Cells.GetLength(0); x++)
        {
            for (int z = 0; z < CurrentLevel.Cells.GetLength(1); z++)
            {
                CurrentLevel.Cells[x, z] = new Cell(this, new Vector3(x, 0, z), BlockType.Grass, BlockType.Null, NatureType.Null, BuildingType.Null);
            }
        }
    }
    private void GenerateCentralRock()
    {
        int MiddelX = CurrentLevel.Cells.GetLength(0) / 2;
        int MiddelZ = CurrentLevel.Cells.GetLength(1) / 2;

        int Iteration = 10;
        float Chance = 1f;

        List<Vector3Int> RockPositions = new List<Vector3Int>
        {
            new Vector3Int(MiddelX, 0, MiddelZ)
        };

        while (Iteration > 0)
        {
            List<Vector3Int> RockPositionsForCorrecting = new List<Vector3Int>();

            foreach (Vector3Int position in RockPositions)
            {
                if (Random.Range(0, 1f) < Chance && position.x > 0)
                {
                    if (CurrentLevel.Cells[position.x - 1, position.z].FloorBlock != BlockType.Stone)
                        RockPositionsForCorrecting.Add(new Vector3Int(position.x - 1, 0, position.z));
                }
                if (Random.Range(0, 1f) < Chance && position.x < CurrentLevel.Cells.GetLength(0) - 2)
                {
                    if (CurrentLevel.Cells[position.x + 1, position.z].FloorBlock != BlockType.Stone)
                        RockPositionsForCorrecting.Add(new Vector3Int(position.x + 1, 0, position.z));
                }
                if (Random.Range(0, 1f) < Chance && position.z > 0)
                {
                    if (CurrentLevel.Cells[position.x, position.z - 1].FloorBlock != BlockType.Stone)
                        RockPositionsForCorrecting.Add(new Vector3Int(position.x, 0, position.z - 1));
                }
                if (Random.Range(0, 1f) < Chance && position.z < CurrentLevel.Cells.GetLength(1) - 2)
                {
                    if (CurrentLevel.Cells[position.x, position.z + 1].FloorBlock != BlockType.Stone)
                        RockPositionsForCorrecting.Add(new Vector3Int(position.x, 0, position.z + 1));
                }
            }

            RockPositions.AddRange(RockPositionsForCorrecting);

            Chance -= 0.12f;
            Iteration -= 1;

            if (Chance < 0.01f) break;
        }

        foreach (Vector3Int positions in RockPositions)
        {
            CurrentLevel.Cells[positions.x, positions.z] = new Cell(this, positions, BlockType.Stone, BlockType.Stone, NatureType.Null, BuildingType.Null);
        }
    }
    private void GenerateTrees()
    {
        int treeAmount = (int)(4 * ((CurrentLevel.Cells.GetLength(0) + CurrentLevel.Cells.GetLength(1) / 2)));

        for (int i = 0; i < treeAmount; i++)
        {
            bool treeCreate = false;

            while (!treeCreate)
            {
                int randomX = Random.Range(0, CurrentLevel.Cells.GetLength(0));
                int randomZ = Random.Range(0, CurrentLevel.Cells.GetLength(1));

                if (CurrentLevel.Cells[randomX, randomZ].FloorBlock == BlockType.Grass &&
                    CurrentLevel.Cells[randomX, randomZ].NatureBlock != NatureType.Tree1 &&
                    CurrentLevel.Cells[randomX, randomZ].NatureBlock != NatureType.Tree2)
                {
                    bool ThereAreNoWallsNearby = true;

                    if (randomX > 0 && randomX < CurrentLevel.Cells.GetLength(0) - 2 &&
                        randomZ > 0 && randomZ < CurrentLevel.Cells.GetLength(1) - 2)
                    {
                        if (CurrentLevel.Cells[randomX + 1, randomZ].WallBlock != BlockType.Null ||
                            CurrentLevel.Cells[randomX - 1, randomZ].WallBlock != BlockType.Null ||
                            CurrentLevel.Cells[randomX, randomZ + 1].WallBlock != BlockType.Null ||
                            CurrentLevel.Cells[randomX, randomZ - 1].WallBlock != BlockType.Null)
                            ThereAreNoWallsNearby = false;
                    }

                    if (ThereAreNoWallsNearby)
                    {
                        switch (Random.Range(0, 3))
                        {
                            case 0: CurrentLevel.Cells[randomX, randomZ] = new Cell(this, new Vector3(randomX, 0, randomZ), BlockType.Grass, BlockType.Null, NatureType.Tree1, BuildingType.Null); break;
                            case 1: CurrentLevel.Cells[randomX, randomZ] = new Cell(this, new Vector3(randomX, 0, randomZ), BlockType.Grass, BlockType.Null, NatureType.Tree2, BuildingType.Null); break;
                        }
                        treeCreate = true;
                    }
                }
            }
        }
    }
    #endregion
    #region ChangingTheWorld
    public void CreateCreature()
    {
        Vector3 spawnPoint;

        switch (Random.Range(0, 4))
        {
            default:
            case 0: spawnPoint = new Vector3(Random.Range(0, CurrentLevel.Cells.GetLength(0)), 0, 0); break;
            case 1: spawnPoint = new Vector3(0, 0, Random.Range(0, CurrentLevel.Cells.GetLength(1))); break;
            case 2: spawnPoint = new Vector3(Random.Range(0, CurrentLevel.Cells.GetLength(0)), 0, CurrentLevel.Cells.GetLength(1) - 1); break;
            case 3: spawnPoint = new Vector3(CurrentLevel.Cells.GetLength(0) - 1, 0, Random.Range(0, CurrentLevel.Cells.GetLength(1))); break;
        }

        GameObject spawnObject = Instantiate(Resources.Load<GameObject>("creature"), new Vector3(spawnPoint.x, 0.5f, spawnPoint.z), Quaternion.Euler(0, 45, 0), Creatures.transform);
        spawnObject.GetComponent<Creature>().Constructor(NameGenerator.NewName(), NameGenerator.NewSurname(), 100, 100, 20, 100, 100);
        CurrentLevel.Settlers.Add(spawnObject.GetComponent<Creature>());
    }
    public void DigBlock(Transform blockTransform)
    {
        int x = (int)blockTransform.position.x;
        int z = (int)blockTransform.position.z;

        if (CurrentLevel.Cells[x, z].WallBlock == BlockType.Stone)
        {
            if (CurrentLevel.NumberOfPicksCurrent > 0)
            {
                CurrentLevel.Cells[(int)blockTransform.position.x, (int)blockTransform.position.z] = new Cell(this, new Vector3(x, 0, z), BlockType.Stone, BlockType.Null, NatureType.Null, BuildingType.Null);
                CurrentLevel.NumberOfPicksCurrent -= 1;
            }
        }
    }
    public void BuildBlock(BuildingType buildingType, Transform blockTransform)
    {
        int x = (int)blockTransform.position.x;
        int z = (int)blockTransform.position.z;

        if (CurrentLevel.Cells[x, z].WallBlock == BlockType.Null &&
            CurrentLevel.Cells[x, z].NatureBlock == NatureType.Null)
        {
            if (CurrentLevel.Chests.Count > 0 || buildingType == BuildingType.Chest)
            {
                if (CurrentLevel.ResourceWoodCurrent >= Game.Buildings[buildingType].CostWood)
                {
                    BlockType floor = CurrentLevel.Cells[(int)blockTransform.position.x, (int)blockTransform.position.z].FloorBlock;

                    CurrentLevel.Cells[x, z] = new Cell(this, new Vector3(x, 0, z), floor, BlockType.Null, NatureType.Null, buildingType);
                    CurrentLevel.ResourceWoodCurrent -= Game.Buildings[buildingType].CostWood;
                }
            }
        }
    }
    public void BuildBlockShow(BuildingType buildingType, Transform blockTransform)
    {
        int x = (int)blockTransform.position.x;
        int z = (int)blockTransform.position.z;

        if (!BuildZoneObject)
        {
            if (buildingType == BuildingType.Bonfire)
                BuildZoneObjectRotate = new Vector3(0, 0, 0);

            BuildZoneObject = Instantiate(Resources.Load<GameObject>("building_" + buildingType), new Vector3(x, 0, z), Quaternion.Euler(BuildZoneObjectRotate.x, BuildZoneObjectRotate.y, BuildZoneObjectRotate.z), Blocks.transform);
            BuildZoneObject.name = buildingType.ToString();

            Material[] zoneMaterial = new Material[BuildZoneObject.GetComponent<MeshRenderer>().materials.Length];

            for (int i = 0; i < zoneMaterial.Length; i++)
            {
                zoneMaterial[i] = Resources.Load<Material>("material_BuildZone");
            }

            BuildZoneObject.GetComponent<MeshRenderer>().materials = zoneMaterial;
        }
        else BuildZoneObject.transform.position = new Vector3(x, 0, z);
    }
    public void BuildBlockShowRemove()
    {
        if (BuildZoneObject)
        {
            Destroy(BuildZoneObject);
        }
    }
    public void BuildBlockShowRotate()
    {
        if (BuildZoneObject)
        {
            if (BuildZoneObject.name != "Bonfire")
            {
                switch (BuildZoneObjectRotate.y)
                {
                    case 0: BuildZoneObjectRotate = new Vector3(0, 90, 0); break;
                    case 90: BuildZoneObjectRotate = new Vector3(0, 180, 0); break;
                    case 180: BuildZoneObjectRotate = new Vector3(0, 270, 0); break;
                    case 270: BuildZoneObjectRotate = new Vector3(0, 0, 0); break;
                }
                BuildZoneObject.transform.rotation = Quaternion.Euler(0, BuildZoneObjectRotate.y, 0);
            }
            else BuildZoneObjectRotate = new Vector3(0, 0, 0);
        }
    }
    public void DestroyObject(Cell cell)
    {
        if (CurrentLevel.Cells[(int)cell.Position.x, (int)cell.Position.z] != null)
            Destroy(CurrentLevel.Cells[(int)cell.Position.x, (int)cell.Position.z].GameObjectInWorld);
    }
    public void ChangeTemperature(int newTemperature)
    {
        CurrentLevel.Temperature = newTemperature;
        UI.EventTemperatureChanged();
    }
    public void ChangeDay()
    {
        SoundMaster.CreateSound(Sound.NewDay);

        CurrentLevel.Day += 1;
        UI.EventDayChanged();
    }
    #endregion
    #region GameEvents
    public void StartGameEventsSystem()
    {
        StartCoroutine(RandomEvent());
        StartCoroutine(NextDay());
    }
    public IEnumerator RandomEvent()
    {
        switch (Random.Range(0, 17))
        {
            default:
            case 0:
                CreateCreature();
                UI.EventShowMessage("К вам прибыл новый поселенец!");
                break;
            case 1:
                for (int i = 0; i < 2; i++)
                {
                    CreateCreature();
                }
                UI.EventShowMessage("К вам прибылыло 2 новых поселенца!");
                break;
            case 2:
                CurrentLevel.MealChange(5);
                UI.EventShowMessage("Поселенцы нашли 5 яблок");
                break;
            case 3:
                CurrentLevel.WoodChange(5);
                UI.EventShowMessage("Поселенцы нашли 5 бревен");
                break;
            case 4:
                CurrentLevel.MealChange(15);
                UI.EventShowMessage("Поселенцы нашли тайник с яблоками +15 яблок");
                break;
            case 5:
                CurrentLevel.WoodChange(15);
                UI.EventShowMessage("Поселенцы нашли тайник с бревнами +15 бревен");
                break;
            case 6:
                foreach (Creature settler in CurrentLevel.Settlers)
                {
                    settler.HappinessCurrent -= 10;
                }
                UI.EventShowMessage("DTF закрыли, настроение -10 едениц");
                break;
            case 7:
                CurrentLevel.ResourceMealCurrent -= 5;
                UI.EventShowMessage("Сгнило 5 яблок");
                break;
            case 8:
                CurrentLevel.WoodChange(-5);
                UI.EventShowMessage("Сломались кровати -5 бревен");
                break;
            case 9:
                CurrentLevel.MealChange(-10);
                UI.EventShowMessage("Сгнило 10 яблок");
                break;
            case 10:
                CurrentLevel.WoodChange(-10);
                UI.EventShowMessage("Сломались кровати -10 бревен");
                break;
            case 11:
                if (CurrentLevel.Settlers.Count > 1)
                {
                    Creature firstSettler = CurrentLevel.Settlers[0];
                    Creature secondSettler = CurrentLevel.Settlers[Random.Range(1, CurrentLevel.Settlers.Count)];

                    if (Random.Range(0, 3) == 0)
                        secondSettler.IsSick = true;

                    UI.EventShowMessage(firstSettler.Name + " " + firstSettler.Surname + " подозревает что " + secondSettler.Name + " " + secondSettler.Surname + " болен");
                }
                break;
            case 12:
                CurrentLevel.WoodChange(-10);
                CurrentLevel.MealChange(-10);
                UI.EventShowMessage("Пропали ресурсы 10 бревен и 10 яблок!");
                break;
            case 13:
                foreach (Creature settler in CurrentLevel.Settlers)
                {
                    settler.HealthCurrent -= 10;
                }
                UI.EventShowMessage("Все поселенцы переболели легкой простудой -10 едениц здоровья");
                break;
            case 14:
                foreach (Creature settler in CurrentLevel.Settlers)
                {
                    settler.HappinessCurrent -= 20;
                }
                UI.EventShowMessage("Поселенцы чувствуют себя  безнадежно - 20 счастья у всех");
                break;
            case 15:
                if (Random.Range(0, 2) == 0)
                    ChangeTemperature(CurrentLevel.Temperature - 5);
                else
                    ChangeTemperature(CurrentLevel.Temperature + 5);

                UI.EventShowMessage("Температура изменилась на 5 градусов");
                break;
            case 16:
                if (Random.Range(0, 2) == 0)
                    ChangeTemperature(CurrentLevel.Temperature - 10);
                else
                    ChangeTemperature(CurrentLevel.Temperature + 10);

                UI.EventShowMessage("Температура изменилась на 10 градусов");
                break;
        }

        yield return new WaitForSeconds(75);

        StartCoroutine(RandomEvent());
    }
    public IEnumerator NextDay()
    {
        //Temperature
        int TemperatureMax = 0;
        int TemperatureMin = 0;
        switch (CurrentLevel.TodayTemperature)
        {
            case TemperatureDay.HotDay:
                switch (Random.Range(0, 2))
                {
                    default:
                    case 0:
                        CurrentLevel.TodayTemperature = TemperatureDay.HotDay;
                        TemperatureMax = 35;
                        TemperatureMin = 25;
                        break;
                    case 1:
                        CurrentLevel.TodayTemperature = TemperatureDay.WarmDay;
                        TemperatureMax = 30;
                        TemperatureMin = 20;
                        break;
                }
                break;
            default:
            case TemperatureDay.WarmDay:
                switch (Random.Range(0, 3))
                {
                    default:
                    case 0:
                        CurrentLevel.TodayTemperature = TemperatureDay.HotDay;
                        TemperatureMax = 35;
                        TemperatureMin = 25;
                        break;
                    case 1:
                        CurrentLevel.TodayTemperature = TemperatureDay.WarmDay;
                        TemperatureMax = 30;
                        TemperatureMin = 20;
                        break;
                    case 2:
                        CurrentLevel.TodayTemperature = TemperatureDay.CoolDay;
                        TemperatureMax = 25;
                        TemperatureMin = 15;
                        break;
                }
                break;
            case TemperatureDay.CoolDay:
                switch (Random.Range(0, 3))
                {
                    default:
                    case 0:
                        CurrentLevel.TodayTemperature = TemperatureDay.WarmDay;
                        TemperatureMax = 30;
                        TemperatureMin = 20;
                        break;
                    case 1:
                        CurrentLevel.TodayTemperature = TemperatureDay.CoolDay;
                        TemperatureMax = 25;
                        TemperatureMin = 15;
                        break;
                    case 2:
                        CurrentLevel.TodayTemperature = TemperatureDay.ColdDay;
                        TemperatureMax = 20;
                        TemperatureMin = 10;
                        break;
                }
                break;
            case TemperatureDay.ColdDay:
                switch (Random.Range(0, 3))
                {
                    default:
                    case 0:
                        CurrentLevel.TodayTemperature = TemperatureDay.CoolDay;
                        TemperatureMax = 25;
                        TemperatureMin = 15;
                        break;
                    case 1:
                        CurrentLevel.TodayTemperature = TemperatureDay.ColdDay;
                        TemperatureMax = 20;
                        TemperatureMin = 5;
                        break;
                    case 2:
                        CurrentLevel.TodayTemperature = TemperatureDay.VeryColdDay;
                        TemperatureMax = 10;
                        TemperatureMin = -10;
                        break;
                }
                break;
            case TemperatureDay.VeryColdDay:
                switch (Random.Range(0, 2))
                {
                    default:
                    case 0:
                        CurrentLevel.TodayTemperature = TemperatureDay.ColdDay;
                        TemperatureMax = 15;
                        TemperatureMin = 0;
                        break;
                    case 1:
                        CurrentLevel.TodayTemperature = TemperatureDay.VeryColdDay;
                        TemperatureMax = 5;
                        TemperatureMin = -20;
                        break;
                }
                break;
        }
        ChangeTemperature(Random.Range(TemperatureMin, TemperatureMax + 1));

        //Day
        string dayMessage = "Наступил новый день ";

        ChangeDay();

        switch (Random.Range(0, 2))
        {
            default:
            case 0:
                for (int i = 0; i < 2; i++)
                {
                    CreateCreature();
                }
                dayMessage += "сегодня пришел поселенец, ";
                break;
            case 1:
                CurrentLevel.PicksChange(2);
                dayMessage += "сегодня нашли 2 кирки, ";
                break;
        }

        switch (CurrentLevel.TodayTemperature)
        {
            case TemperatureDay.HotDay: dayMessage += "день жаркий"; break;
            case TemperatureDay.WarmDay: dayMessage += "день теплый"; break;
            case TemperatureDay.CoolDay: dayMessage += "день прохладный"; break;
            case TemperatureDay.ColdDay: dayMessage += "день холодный"; break;
            case TemperatureDay.VeryColdDay: dayMessage += "день очень холодный"; break;
        }

        if (CurrentLevel.Settlers.Count > 0 && Random.Range(0, 2) == 0)
            CurrentLevel.Settlers[Random.Range(0, CurrentLevel.Settlers.Count)].IsSick = true;

        UI.EventShowMessage(dayMessage);

        yield return new WaitForSeconds(60f);
        StartCoroutine(NextDay());
    }
    #endregion
}