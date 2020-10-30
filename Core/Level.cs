using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    #region Fields
    //Level properties
    public int Day;
    public int Temperature;
    public int NumberOfPicksMax;
    public int NumberOfPicksCurrent;
    public int ResourceWoodCurrent;
    public int ResourceMealCurrent;
    //Objects
    public List<Creature> Settlers;
    public List<BuildingObject> Chests;
    public List<BuildingObject> Beds;
    public List<ResourceSource> Trees;
    //Tech
    public Cell[,] Cells;
    public TemperatureDay TodayTemperature;
    #endregion
    #region Constructor
    public Level()
    {
        //Tech
        Cells = new Cell[24, 24];
        Settlers = new List<Creature>();
        Chests = new List<BuildingObject>();
        Beds = new List<BuildingObject>();
        Trees = new List<ResourceSource>();
        //Initial properties
        Day = 1;
        TodayTemperature = TemperatureDay.WarmDay;
        Temperature = 22;
        NumberOfPicksMax = 15;
        NumberOfPicksCurrent = NumberOfPicksMax;
        ResourceWoodCurrent = 70;
        ResourceMealCurrent = 30;
    }
    #endregion
    #region Methods
    public void WoodChange(int number)
    {
        ResourceWoodCurrent += number;

        if (ResourceWoodCurrent < 0)
            ResourceWoodCurrent = 0;
    }
    public void MealChange(int number)
    {
        ResourceMealCurrent += number;

        if (ResourceMealCurrent < 0)
            ResourceMealCurrent = 0;
    }
    public void PicksChange(int number)
    {
        NumberOfPicksCurrent += number;

        if (NumberOfPicksCurrent < 0)
            NumberOfPicksCurrent = 0;
        else if (NumberOfPicksCurrent > NumberOfPicksMax)
            NumberOfPicksCurrent = NumberOfPicksMax;
    }
    public bool CheckAvailabilityOfBeds()
    {
        bool resolt = false;

        foreach (BuildingObject bed in Beds)
        {
            if (!bed.TheBedIsBusy)
            {
                resolt = true;
            }
        }

        return resolt;
    }
    #endregion
}
