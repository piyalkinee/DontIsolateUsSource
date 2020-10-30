using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    #region Fields
    public int Size { get; private set; }
    private ItemType[] Items;
    #endregion
    #region Constructor
    public Inventory(int size)
    {
        Size = size;
        Items = new ItemType[Size];
    }
    #endregion
    #region Methods
    public bool Put(ItemType item)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == ItemType.Null)
            {
                Items[i] = item;
                return true;
            }
        }
        return false;
    }
    public bool Get(ItemType type)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (type == Items[i])
            {
                ItemType getItem = Items[i];
                Items[i] = ItemType.Null;
                return true;
            }
        }
        return false;
    }
    public bool HaveEmptySlots()
    {
        bool resolt = false;
        foreach(var item in Items)
        {
            if (item == ItemType.Null)
                resolt = true;
        }
        return resolt;
    }
    public bool AllSlotsAreFree()
    {
        bool resolt = true;
        foreach (var item in Items)
        {
            if (item != ItemType.Null)
                resolt = false;
        }
        return resolt;
    }
    public ItemType ShowOne(int id)
    {
        return Items[id];
    }
    public bool CheckOne(int id)
    {
        if (Items[id] != ItemType.Null)
            return true;
        else return false;
    }
    #endregion
}
