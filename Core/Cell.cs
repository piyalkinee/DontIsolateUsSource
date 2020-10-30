using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    #region Fields
    //Characteristic
    public Vector3 Position;
    //Enums
    public BlockType FloorBlock;
    public BlockType WallBlock;
    public NatureType NatureBlock;
    public BuildingType BuildingBlock;
    //Scripts
    public World World;
    //GameObjets
    public GameObject GameObjectInWorld;
    #endregion
    #region Constructor
    public Cell(World coreScript, Vector3 position, BlockType floorBlock, BlockType wallBlock, NatureType natureType, BuildingType buildingType)
    {
        World = coreScript;

        Position = position;
        FloorBlock = floorBlock;
        WallBlock = wallBlock;
        NatureBlock = natureType;
        BuildingBlock = buildingType;

        CreateGameObjects();
    }
    #endregion
    #region Game objects
    public void CreateGameObjects()
    {
        World.DestroyObject(this);

        GameObjectInWorld = new GameObject();
        GameObjectInWorld.transform.parent = World.Blocks.transform;
        GameObjectInWorld.name = "Object_x" + Position.x + "_z" + Position.z;

        if (FloorBlock != BlockType.Null)
        {
            GameObject FloorBlockObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            FloorBlockObject.name = "Floor";
            FloorBlockObject.transform.parent = GameObjectInWorld.transform;
            FloorBlockObject.transform.position = new Vector3(Position.x, -0.5f, Position.z);
            FloorBlockObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("material_" + FloorBlock.ToString());
            FloorBlockObject.AddComponent<BoxCollider>().isTrigger = true;
            FloorBlockObject.AddComponent<ObjectInWorld>();
        }

        if (WallBlock != BlockType.Null)
        {
            GameObject WallBlockObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            WallBlockObject.name = "Wall";
            WallBlockObject.transform.parent = GameObjectInWorld.transform;
            WallBlockObject.transform.position = new Vector3(Position.x, 0.5f, Position.z);
            WallBlockObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("material_" + WallBlock.ToString());
            WallBlockObject.AddComponent<BoxCollider>().isTrigger = true;
            WallBlockObject.AddComponent<ObjectInWorld>();
        }

        if (NatureBlock != NatureType.Null)
        {
            GameObject NatureBlockObject = new GameObject();
            NatureBlockObject.name = "Nature";
            NatureBlockObject.transform.parent = GameObjectInWorld.transform;
            NatureBlockObject.transform.position = new Vector3(Position.x, 0, Position.z);
            NatureBlockObject.AddComponent<MeshFilter>().mesh = Resources.Load<Mesh>("mesh_" + NatureBlock.ToString());
            NatureBlockObject.AddComponent<MeshRenderer>();

            switch (NatureBlock)
            {
                case NatureType.Tree1:
                case NatureType.Tree2:
                    switch (Random.Range(0, 8))
                    {
                        case 0: NatureBlockObject.transform.Rotate(0, 0, 0); break;
                        case 1: NatureBlockObject.transform.Rotate(0, 45, 0); break;
                        case 2: NatureBlockObject.transform.Rotate(0, 90, 0); break;
                        case 3: NatureBlockObject.transform.Rotate(0, 135, 0); break;
                        case 4: NatureBlockObject.transform.Rotate(0, 180, 0); break;
                        case 5: NatureBlockObject.transform.Rotate(0, 225, 0); break;
                        case 6: NatureBlockObject.transform.Rotate(0, 270, 0); break;
                        case 7: NatureBlockObject.transform.Rotate(0, 315, 0); break;
                    }

                    ResourceSource resourceSource = NatureBlockObject.AddComponent<ResourceSource>();
                    World.CurrentLevel.Trees.Add(resourceSource);

                    if (Random.Range(0, 3) == 0)
                    {
                        resourceSource.Replace(ResourceType.Meal, Random.Range(1, 21));
                    }
                   

                    switch (Random.Range(0, 5))
                    {
                        case 0:
                            NatureBlockObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                            resourceSource.Replace(ResourceType.Wood, Random.Range(6, 15));
                            break;
                        case 1:
                            NatureBlockObject.transform.localScale = new Vector3(1, 1, 1);
                            resourceSource.Replace(ResourceType.Wood, Random.Range(8, 17));
                            break;
                        case 2:
                            NatureBlockObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                            resourceSource.Replace(ResourceType.Wood, Random.Range(10, 19));
                            break;
                        case 3:
                            NatureBlockObject.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
                            resourceSource.Replace(ResourceType.Wood, Random.Range(12, 21));
                            break;
                        case 4:
                            NatureBlockObject.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                            resourceSource.Replace(ResourceType.Wood, Random.Range(14, 23));
                            break;
                    }


                    NatureBlockObject.GetComponent<MeshRenderer>().materials = new[] { Resources.Load<Material>("material_Leaves"), Resources.Load<Material>("material_Wood") };
                    break;
            }
        }

        if (BuildingBlock != BuildingType.Null)
        {
            GameObject BuildingBlockObject = GameObject.Instantiate(Resources.Load<GameObject>("building_" + BuildingBlock), new Vector3(Position.x, 0, Position.z), Quaternion.Euler(0, World.BuildZoneObjectRotate.y, 0), GameObjectInWorld.transform);
            BuildingBlockObject.name = "Building";
            BuildingBlockObject.AddComponent<BuildingObject>().GetType(BuildingBlock);
        }
    }
    #endregion
}
