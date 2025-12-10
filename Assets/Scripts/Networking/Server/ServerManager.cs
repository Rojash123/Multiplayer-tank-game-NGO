using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class ServerManager : IDisposable
{
    private string serverIP;
    private int serverPort;
    private int serverQport;
    private MultiplayAllocationService multiplayAllocationService;
    private MatchplayBackfiller matchplayBackfiller;
    private NetworkObject playerPrefab;
    
    public NetworkServer NetworkServer {  get; private set; }
    public ServerManager(string serverIp, int serverPort, int serverQport, NetworkManager manager,NetworkObject playerPrefab)
    {
        this.serverIP = serverIp;
        this.serverPort = serverPort;
        this.serverQport = serverQport;
        this.playerPrefab = playerPrefab;
        NetworkServer = new NetworkServer(manager,playerPrefab);
        multiplayAllocationService = new();
    }
    public void Dispose()
    {
        NetworkServer.OnUserJoined -= UserJoined;
        NetworkServer.OnUserLeft -= UserLeft;
        matchplayBackfiller?.Dispose();
        multiplayAllocationService?.Dispose();
        NetworkServer?.Dispose();
    }
    public async Task StartGameAsync()
    {
        await multiplayAllocationService.BeginServerCheck();
        try
        {
            MatchmakingResults matchMakerPayload = await GetMatchMakerPayLoad();
            if (matchMakerPayload != null)
            {
                await StartBackFill(matchMakerPayload);
                NetworkServer.OnUserJoined += UserJoined;
                NetworkServer.OnUserLeft += UserLeft;
            }
            else
            {
                MyDebug.LogWarning("match maker payload timed out");
            }
        }
        catch (Exception ex)
        {
            MyDebug.LogWarning(ex.Message);
        }
        if (!NetworkServer.OpenConnection(serverIP, serverPort))
        {
            MyDebug.LogWarning("network server didnot start as expected");
            return;
        }
    }
    private async Task StartBackFill(MatchmakingResults matchMakerPayload)
    {
        matchplayBackfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}", matchMakerPayload.QueueName, matchMakerPayload.MatchProperties, 20);
        if (matchplayBackfiller.NeedsPlayers())
        {
            await matchplayBackfiller.BeginBackfilling();
        }
    }
    private void UserJoined(UserData user)
    {
        matchplayBackfiller.AddPlayerToMatch(user);
        multiplayAllocationService.AddPlayer();
        if (!matchplayBackfiller.NeedsPlayers() && matchplayBackfiller.IsBackfilling)
        {
            _ = matchplayBackfiller.StopBackfill();
        }
    }
    private void UserLeft(UserData user)
    {
        int playerCount = matchplayBackfiller.RemovePlayerFromMatch(user.userAuthId);
        multiplayAllocationService.RemovePlayer();
        if (playerCount <= 0)
        {
            CloseServer();
        }
        if (matchplayBackfiller.NeedsPlayers() && !matchplayBackfiller.IsBackfilling)
        {
            _ = matchplayBackfiller.BeginBackfilling();
        }
    }
    private async void CloseServer()
    {
        await matchplayBackfiller.StopBackfill();
        Dispose();
        Application.Quit();
    }
    private async Task<MatchmakingResults> GetMatchMakerPayLoad()
    {
        Task<MatchmakingResults> matchMakingPayloadTask = multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();
        if (await Task.WhenAny(matchMakingPayloadTask, Task.Delay(20000)) == matchMakingPayloadTask)
        {
            return matchMakingPayloadTask.Result;
        }
        return null;
    }
}
