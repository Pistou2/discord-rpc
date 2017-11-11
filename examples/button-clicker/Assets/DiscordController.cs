using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class DiscordJoinEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordSpectateEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordJoinRequestEvent : UnityEngine.Events.UnityEvent<DiscordRpc.JoinRequest> { }

public class DiscordController : MonoBehaviour
{
    public DiscordRpc.RichPresence presence;
    public string applicationId;
    public string optionalSteamId;
    public int callbackCalls;
    public int clickCounter;
    public UnityEngine.Events.UnityEvent onConnect;
    public UnityEngine.Events.UnityEvent onDisconnect;
    public DiscordJoinEvent onJoin;
    public DiscordJoinEvent onSpectate;
    public DiscordJoinRequestEvent onJoinRequest;

	public UnityEngine.UI.Dropdown dropDown;

    private string[,] mapValues = new string[,]{{"birmingham_power_station","Playing on Earth, Birmingham Power Station"},
  {"gliese_lake","Playing on GJ 1214b, Gliese Lake"},
  {"ophiucus_valley","Playing on GJ 1214b, Ophiucus Valley"},
  {"spitzer_dam","Playing on GJ 1214b, Spitzer Dam"},
  {"tharsis_rift","Playing on Mars, Tharsis Rift"},
  {"thionium_canyon", "Playing on Mars, Thionium Canyon"},
  {"vanguards_end","Playing on Earth, Vanguard's End"}};

    DiscordRpc.EventHandlers handlers;

	//region perso


    //region map

    public void OnDropdown_Map_Change(int index)	{

    presence.largeImageKey = mapValues[index,0];
    presence.largeImageText = mapValues[index,1];

    RefreshRPC ();

  }


    //endregion

  //region gamemode

  public void OnClickGame_TDM()	{

		presence.state = "";

    presence.smallImageKey = "team_death_match";
    presence.smallImageText = "Team Deathmatch";

    presence.startTimestamp = 0;
    presence.endTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 15*60;

		RefreshRPC ();
	}

  public void OnClickGame_CG()	{

		presence.state = "";

    presence.smallImageKey = "custom_game";
    presence.smallImageText = "Custom Game";

    presence.startTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    presence.endTimestamp = 0;

		RefreshRPC ();
	}

  public void OnClickGame_Ba()	{

		presence.state = "";

    presence.smallImageKey = "battle_arena";
    presence.smallImageText = "Battle Arena";

    presence.startTimestamp = 0;
    presence.endTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 15*60;

		RefreshRPC ();
	}

  public void OnClickGame_Brawl()	{

		presence.state = "";

    presence.smallImageKey = "brawl";
    presence.smallImageText = "Brawl";

    presence.startTimestamp = 0;
    presence.endTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 15*60;

		RefreshRPC ();
	}

  public void OnClickGame_Ai()	{

		presence.state = "";

    presence.smallImageKey = "ai";
    presence.smallImageText = "AI Mode";

    presence.startTimestamp = 0;
    presence.endTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 15*60;

		RefreshRPC ();
	}

  public void OnClickGame_La()	{

		presence.state = "";

    presence.smallImageKey = "league_arena";
    presence.smallImageText = "League Arena";

    presence.startTimestamp = 0;
    presence.endTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 15*60;

		RefreshRPC ();
	}

  //endregion

	public void OnUpdate_Details(string text){

		presence.details = text;

		RefreshRPC ();
	}

	public void OnClickStatus_Away()	{

		presence.state = string.Format("Away");

    presence.largeImageKey = "robocraft";
    presence.largeImageText = "";
    presence.smallImageKey = "freejam";
    presence.smallImageText = "♥♥♥";

    presence.endTimestamp = 0;
		presence.startTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

		RefreshRPC ();
	}

	public void OnClickStatus_Chat()	{

		presence.state = "";

    presence.startTimestamp = 0;
    presence.endTimestamp = 0;

    presence.largeImageKey = "robocraft";
    presence.largeImageText = "";
    presence.smallImageKey = "chat";
    presence.smallImageText = "In Lobby";


		RefreshRPC ();
	}

	public void RefreshRPC(){

		DiscordRpc.UpdatePresence(ref presence);
	}
	//endregion

	public void OnClick()
	{
		Debug.Log("Discord: on click!");
		clickCounter++;

		//presence.details = string.Format("Farming Robits : {0} ", clickCounter);


		DiscordRpc.UpdatePresence(ref presence);

	}

    public void ReadyCallback()
    {
        ++callbackCalls;
        Debug.Log("Discord: ready");
        onConnect.Invoke();
    }

    public void DisconnectedCallback(int errorCode, string message)
    {
        ++callbackCalls;
        Debug.Log(string.Format("Discord: disconnect {0}: {1}", errorCode, message));
        onDisconnect.Invoke();
    }

    public void ErrorCallback(int errorCode, string message)
    {
        ++callbackCalls;
        Debug.Log(string.Format("Discord: error {0}: {1}", errorCode, message));
    }

    public void JoinCallback(string secret)
    {
        ++callbackCalls;
        Debug.Log(string.Format("Discord: join ({0})", secret));
        onJoin.Invoke(secret);
    }

    public void SpectateCallback(string secret)
    {
        ++callbackCalls;
        Debug.Log(string.Format("Discord: spectate ({0})", secret));
        onSpectate.Invoke(secret);
    }

    public void RequestCallback(DiscordRpc.JoinRequest request)
    {
        ++callbackCalls;
        Debug.Log(string.Format("Discord: join request {0}: {1}", request.username, request.userId));
        onJoinRequest.Invoke(request);
    }

    void Start()
    {
		//fill the dropdown
		List<string> mapNames = new List<string>();

		for (int i = 0; i < mapValues.GetLength (0); i++) {
			mapNames.Add (mapValues [i, 0]);
		}

		dropDown.AddOptions(mapNames);
    }

    void Update()
    {
        DiscordRpc.RunCallbacks();
    }

    void OnEnable()
    {
        Debug.Log("Discord: init");
        callbackCalls = 0;

        handlers = new DiscordRpc.EventHandlers();
        handlers.readyCallback = ReadyCallback;
        handlers.disconnectedCallback += DisconnectedCallback;
        handlers.errorCallback += ErrorCallback;
        handlers.joinCallback += JoinCallback;
        handlers.spectateCallback += SpectateCallback;
        handlers.requestCallback += RequestCallback;
        DiscordRpc.Initialize(applicationId, ref handlers, true, optionalSteamId);
    }

    void OnDisable()
    {
        Debug.Log("Discord: shutdown");
        DiscordRpc.Shutdown();
    }

    void OnDestroy()
    {

    }
}
