using System;

public enum GameMode
{
    Default
}
public enum Map
{
    Default
}

public enum GameQueue
{
    solo,
    team
}

[Serializable]
public class UserData
{
    public string userName;
    public string userAuthId;
    public GameInfo userGamePreferences;
    
}

[Serializable]
public class GameInfo
{
    public GameMode gameMode;
    public Map map;
    public GameQueue gameQueue;

    public string ToMultiplayQueue()
    {
        return "";
    }
}
