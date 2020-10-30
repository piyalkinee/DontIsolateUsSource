using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    #region Application
    private void Awake()
    {
        Cursor.SetCursor(Resources.Load<Texture2D>("cursor_Hand"), Vector2.zero, CursorMode.ForceSoftware);
        SoundMaster.StartSoundSystem();

        LoadBuildings();
        LoadItems();
        LoadingSkins();
    }
    public void Quit()
    {
        Application.Quit();
    }
    #endregion
    #region Gameplay

    #endregion
    #region Content
    //Fields
    public Dictionary<BuildingType, Building> Buildings { get; private set; }
    public List<ItemType> Items { get; private set; }
    public List<Sprite> SkinsHead { get; private set; }
    public List<Sprite> SkinsFace { get; private set; }
    public List<Sprite> SkinsBody { get; private set; }
    //Methods
    public void StartGameMode(GameMode gameMode)
    {
        GameObject.Find("-World-").GetComponent<World>().StartGenerator();
        GameObject.Find("camera_Main").GetComponent<CameraControl>().StartCamera();
    }
    public void LoadBuildings()
    {
        Buildings = new Dictionary<BuildingType, Building>();

        Buildings.Add(BuildingType.Bed, new Building(BuildingType.Bed, 100, 20, BlockType.Null));
        Buildings.Add(BuildingType.Chest, new Building(BuildingType.Chest, 50, 15, BlockType.Null));
        //Buildings.Add(BuildingType.Bonfire, new Building(BuildingType.Bonfire, 20, 5, BlockType.Null));
        //Buildings.Add(BuildingType.Door, new Building(BuildingType.Door, 200, 30, BlockType.Null));
    }
    public void LoadItems()
    {
        Items = new List<ItemType>();

        Items.Add(ItemType.Apple);
        Items.Add(ItemType.Log);
    }
    public void LoadingSkins()
    {
        SkinsHead = new List<Sprite>();
        SkinsFace = new List<Sprite>();
        SkinsBody = new List<Sprite>();

        for (int i = 1; i <= 4; i++)
        {
            SkinsHead.Add(Resources.Load<Sprite>("sprite_Head" + (i)));
        }

        for (int i = 1; i <= 4; i++)
        {
            for (int j = 1; j <= 3; j++)
            {
                SkinsFace.Add(Resources.Load<Sprite>("sprite_Face" + (i) + (j)));
            }
        }

        for (int i = 1; i <= 4; i++)
        {
            SkinsBody.Add(Resources.Load<Sprite>("sprite_Body" + (i)));
        }
    }
    #endregion
}
