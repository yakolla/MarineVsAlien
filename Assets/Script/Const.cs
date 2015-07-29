using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.UI;

public class Const {

	public enum GUI_WindowID
	{
		ChampGuage,
		ChampSkill,
		ChampLevelUp,
		ChampInventory,
		ChampGoods,
		PopupShop,
		MainMenu,
	}

	public const int MaxItemLevel = 900;
	public const int MaxShowDamageNumber = 3;
	public const int SpecialButtons = 3;
	public const int Guages = 3;
	public const int MaxCallableFollowers = 2;
	public const int MaxFiringCount = MaxItemLevel+3;
	public const float MaxAlphaMoveSpeed = 2f;
	public const int AccessoriesSlots = 4;
	public const int HalfAccessoriesSlots = AccessoriesSlots/2;
	public const int StartPosYOfPriceButtonImage = 10;
	public const int StartPosYOfGemPriceButtonImage = -5;
	public const int EngineeringBayRefItemId = 51;
	public const int AcademyRefItemId = 52;
	public const int RandomAbilityRefItemId = 1101;
	public const int SlotMachineRollRefItemId = 1102;
	public const int AbilitySlots = 3;
	public const int ChampGunRefItemId = 108;
	public const int ChampFiregunRefItemId = 101;
	public const int ChampLightningLauncherRefItemId = 102;
	public const int ChampGuidedRocketLauncherRefItemId = 106;
	public const int ChampRocketLauncherRefItemId = 111;
	public const int ChampBoomerangLauncherRefItemId = 120;
	public const int BootsRefItemId = 10;
	public const int NuclearSkillRefItemId = 21;
	public const int GemRefItemId = 8;
	public const int EmbersRefItemId = 132;
	public const int GoldMedalRefItemId = 5;
	public const int GunMarineRefItemId = 1001;
	public const int FireMarineRefItemId = 1002;
	public const int RocketMarineRefItemId = 1003;
	public const int GuidedRocketMarineRefItemId = 1004;
	public const int BoomerangMarineRefItemId = 1005;
	public const int PetRefMobId = 30007;


	public const string LEADERBOARD_KILLED_MOBS = "CgkI4IXrjtcPEAIQBg";

	public const string DisabledStringColor = "<color=silver>";
	public const string EnabledStringColor = "<color=yellow>";

	public static bool			CHEAT_MODE = false;

	public static float GetItemLevelupWorth(int level, RefPriceCondition cond)
	{
		return cond.pricePerLevel * level;
	}

	public static bool CheckAvailableItem(RefPrice[] conds, float itemWorth)
	{
		if (conds == null)
			return true;

		foreach(RefPrice price in conds)
		{
			ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId);
			if (inventoryItemObj == null)
				return false;
			
			if (inventoryItemObj != null)
			{
				if (inventoryItemObj.Item.Count < price.count*itemWorth)
					return false;
			}
		}
		
		return true;
	}
	
	public static void PayPriceItem(RefPrice[] conds, float itemWorth)
	{
		if (conds == null)
			return;

		foreach(RefPrice price in conds)
		{
			Warehouse.Instance.PullItem(Warehouse.Instance.FindItem(price.refItemId), (int)(price.count*itemWorth));
		}
	}

	public static Texture2D getScreenshot() {
		Texture2D tex = new Texture2D(Screen.width, Screen.height);
		tex.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);
		tex.Apply();
		return tex;
	}

	public static void DestroyChildrenObjects(GameObject obj)
	{
		Transform[] children = obj.transform.GetComponentsInChildren<Transform>();
		for(int i = 0; i < children.Length; ++i)
		{
			if (children[i].gameObject == obj)
				continue;

			GameObject.DestroyObject(children[i].gameObject);
		}
	}
	public static void SaveGame(System.Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			GPlusPlatform.Instance.OpenGame(Warehouse.Instance.FileName, (SavedGameRequestStatus status, ISavedGameMetadata game)=>{
				if (status == SavedGameRequestStatus.Success) 
				{
					System.TimeSpan totalPlayingTime = game.TotalTimePlayed;
					totalPlayingTime += new System.TimeSpan(System.TimeSpan.TicksPerSecond*(long)(Warehouse.Instance.SaveTime));					
					Warehouse.Instance.SaveTime = Time.time;
					GPlusPlatform.Instance.SaveGame(game, Warehouse.Instance.Serialize(), totalPlayingTime, Const.getScreenshot(), callback);
				} 
				else {
					callback(status, game);
				}
			});
		}
		else
		{
			callback(SavedGameRequestStatus.Success, null);
		}
	}


	static GameObject loadingGUI = null;
	public static void ShowLoadingGUI(string name)
	{
		if (loadingGUI == null)
			loadingGUI = GameObject.Instantiate(Resources.Load("Pref/LoadingGUI")) as GameObject;

		loadingGUI.transform.Find("Panel/Image/Text").GetComponent<Text>().text = name;
		ShowLoadingGUI();
	}

	public static void ShowLoadingGUI()
	{
		if (loadingGUI != null)
			loadingGUI.SetActive(true);
	}

	public static void HideLoadingGUI()
	{
		if (loadingGUI != null)
			loadingGUI.SetActive(false);
	}


	static Spawn spawn  = null;
	public static Spawn GetSpawn()
	{
		if (spawn == null)
		{
			spawn = GameObject.Find("Dungeon/Spawn").GetComponent<Spawn>();
		}

		return spawn;
	}

	public static GameObject GetPrefItemEatEffect(RefItem refItem)
	{
		GameObject pref = Resources.Load<GameObject>("Pref/ItemBox/ef " + refItem.codeName + " eat");
		if (pref == null)
		{
			pref = Resources.Load<GameObject>("Pref/ItemBox/ef item eat");
		}

		return pref;
	}
}
