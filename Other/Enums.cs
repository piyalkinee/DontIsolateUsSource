#region Game
public enum GameMode
{
    GameJam
}
public enum ResourceType
{
    Null,
    Wood,
    Meal
}
public enum CreatureTaskType
{
    DoingNothing,
    Works,
    Isolates
}
public enum CreatureActionType
{
    Null,
    GoToARandomPoint,
    GoEat,
    GoToBed,
    GoToWork,
    TakeTheBed,
    GoToTheChest,
    TalkNonsense,
    Exile
}
public enum CreatureWorkType
{
    Null,
    Lumberjack,
    Collector
}
public enum TemperatureDay
{
    HotDay,
    WarmDay,
    CoolDay,
    ColdDay,
    VeryColdDay
}
public enum Sound
{
    Build,
    ClickButton,
    Dead,
    NewDay
}
#endregion
#region Objects types
public enum CreatureType
{
    Null,
    Settler
}
public enum BuildingType
{
    Null,
    Bed,
    Chest,
    Bonfire,
    Door
}
public enum BuildingVisualizationType
{
    Sprite,
    Model
}
public enum ItemType
{
    Null,
    Axe,
    Log,
    Apple,
    BottelWithWater
}
public enum BlockType
{
    Null,
    Dirt,
    Grass,
    Stone
}
public enum NatureType
{
    Null,
    Tree1,
    Tree2
}
#endregion
#region GameUI
public enum ToolType
{
    Null,
    Dig,
    Build
}
public enum DialogType
{
    Null,
    ToMeny,
    SettlerPanel,
    SettlersTask
}
#endregion