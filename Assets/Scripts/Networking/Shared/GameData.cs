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
    public int teamIndex = -1;
    public string userName;
    public string userAuthId;
    public GameInfo userGamePreferences=new();
    
}

[Serializable]
public class GameInfo
{
    public GameMode gameMode;
    public Map map;
    public GameQueue gameQueue;

    public string ToMultiplayQueue()
    {
        return gameQueue switch 
        {
            GameQueue.solo=>"solo-queue",
            GameQueue.team=>"team-queue",
            _=>"solo-queue"
        };
    }
}
