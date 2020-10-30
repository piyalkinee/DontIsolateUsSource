using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObject : MonoBehaviour
{
    #region Fields
    public BuildingType Type { get; private set; }
    #endregion
    #region Methods
    public void GetType(BuildingType type)
    {
        Type = type;
        switch (Type)
        {
            case BuildingType.Bed:
                GameObject.Find("-World-").GetComponent<World>().CurrentLevel.Beds.Add(this);
                TheBedIsBusy = false;
                break;
            case BuildingType.Chest: GameObject.Find("-World-").GetComponent<World>().CurrentLevel.Chests.Add(this); break;
        }
    }
    public void Destroy()
    {
        switch (Type)
        {
            case BuildingType.Bed: GameObject.Find("-World-").GetComponent<World>().CurrentLevel.Beds.Remove(this); break;
            case BuildingType.Chest: GameObject.Find("-World-").GetComponent<World>().CurrentLevel.Chests.Remove(this); break;
        }
        Destroy(transform.gameObject);
    }
    #endregion
    #region Bed
    //Fields
    public bool TheBedIsBusy;
    //Methods
    public BuildingObject TakeTheBed()
    {
        if (!TheBedIsBusy)
        {
            TheBedIsBusy = true;
            Material[] newFabric = new Material[]
            {
                Resources.Load<Material>("material_FabricRed"),
                Resources.Load<Material>("material_FabricWhite"),
                Resources.Load<Material>("material_Wood")
            };
            GetComponent<MeshRenderer>().materials = newFabric;
            return this;
        }
        else return null;
    }
    public void EmptyTheBed()
    {
        if (TheBedIsBusy)
        {
            TheBedIsBusy = false;
            Material[] newFabric = new Material[]
            {
                Resources.Load<Material>("material_FabricGray"),
                Resources.Load<Material>("material_FabricWhite"),
                Resources.Load<Material>("material_Wood")
            };
            GetComponent<MeshRenderer>().materials = newFabric;
        }
    }
    #endregion
}
