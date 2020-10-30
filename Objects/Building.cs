using System;
using UnityEngine;

public class Building
{
    #region Fields
    //Main
    public BuildingType Type { get; private set; }
    public int Strength { get; private set; }
    public int CostWood { get; private set; }
    public BlockType ConstructionSurface { get; private set; }
    public BuildingVisualizationType Visualization { get; private set; }
    //Additional
    public Inventory Inventory { get; private set; }
    public bool Working { get; private set; }
    public int WorkingCycleCost { get; private set; }
    #endregion
    #region Constructor
    public Building(BuildingType buildingType, int strength, int costWood, BlockType constructionSurface)
    {
        Type = buildingType;
        Strength = strength;
        CostWood = costWood;
        ConstructionSurface = constructionSurface;

        //Visualization
        switch (buildingType)
        {
            default: Visualization = BuildingVisualizationType.Sprite; break;
            case BuildingType.Door: Visualization = BuildingVisualizationType.Model; break;
        }

        //Additional fields
        switch (buildingType)
        {
            case BuildingType.Chest:
                Inventory = new Inventory(20);
                break;
            case BuildingType.Bonfire:
                Working = true;
                WorkingCycleCost = 5;
                break;
        }
    }
    #endregion
}
