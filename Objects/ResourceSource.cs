using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSource : MonoBehaviour
{
    #region Fields
    public int StockOfWood { get; private set; }
    public int StockOfMeal { get; private set; }
    #endregion
    #region Methods
    public void Add(ResourceType resource, int amount)
    {
        switch (resource)
        {
            case ResourceType.Wood: StockOfWood += amount; break;
            case ResourceType.Meal: StockOfMeal += amount; break;
        }
    }
    public void Replace(ResourceType resource, int amount)
    {
        switch (resource)
        {
            case ResourceType.Wood: StockOfWood = amount; break;
            case ResourceType.Meal: StockOfMeal = amount; break;
        }
    }
    public int TakeOne(ResourceType resource)
    {
        switch (resource)
        {
            default: return 0;
            case ResourceType.Wood:
                if (StockOfWood > 0)
                {
                    StockOfWood -= 1;
                    return 1;
                }
                else return 0;
            case ResourceType.Meal:
                if (StockOfMeal > 0)
                {
                    StockOfMeal -= 1;
                    return 1;
                }
                else return 0;
        }
    }
    public void Destroy()
    {
        if (StockOfWood < 1)
        {
            GameObject.Find("-World-").GetComponent<World>().CurrentLevel.Trees.Remove(this);
            Destroy(transform.gameObject);
        }
    }
    #endregion
}