using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class ChampSettingGUI : MonoBehaviour {




	GameObject	m_weaponPanel;
	GameObject	m_statPanel;
	GameObject	m_followerPanel;
	GameObject	m_skillPanel;
	GeneralInfoPanel	m_generalInfoPanel;

	class TabDesc
	{
		public YGUISystem.GUIButton m_tab;
		public GameObject	m_checked;
		public ItemData.Type[] m_itemTypes;
		public Image	m_tabImage;
		public int		m_index;

		public TabDesc(int index, MultiLang.ID name, ItemData.Type[] itemTypes, YGUISystem.GUIButton tab)
		{
			m_index = index;
			m_tab = tab;
			m_checked = tab.Button.transform.Find("Checked").gameObject;
			m_tab.Lable.Text.text = RefData.Instance.RefTexts(name);
			m_itemTypes = itemTypes;
			m_tabImage = tab.Button.gameObject.GetComponent<Image>();

		}

		public void Update(int selectedTabIndex)
		{
			m_tab.Update();

			if (m_index == selectedTabIndex)
				m_tabImage.color = Color.black;
			else
				m_tabImage.color = Color.white;
		}
	}
	TabDesc[]	m_tabs = new TabDesc[4];
	int	m_selectedTabIndex = -1;

	[SerializeField]
	Transform		m_spawnChamp;

	[SerializeField]
	bool		m_cheat = true;
	[SerializeField]
	bool		m_eqipedAllItem = false;
	[SerializeField]
	int			m_cheatGem = 1000;

	Champ		m_champ;

	public class EquippedContext
	{
		public ItemObject		m_itemObject;
		public GUIInventorySlot m_inventorySlot;
	}
	EquippedContext		m_equipedWeapon = new EquippedContext();
	EquippedContext[]	m_equipedAccessories = new EquippedContext[Const.AccessoriesSlots];
	Spawn		m_spawn;

	string		log;


	void Start()
	{
		GameObjectPool.Instance.Clear();
		System.GC.Collect();
		Const.HideLoadingGUI();

		Const.CHEAT_MODE = m_cheat;

		m_spawn = GameObject.Find("Dungeon/Spawn").GetComponent<Spawn>();
		GPlusPlatform.Instance.InitAnalytics(GameObject.Find("GAv3").GetComponent<GoogleAnalyticsV3>());
		GPlusPlatform.Instance.AnalyticsTrackScreen("SettingGUI");

		if (m_cheat == true)
		{
#if UNITY_EDITOR
			if (Warehouse.Instance.InvenSize == 0)
			{
				Warehouse.Instance.PushItem(new ItemGoldData(100000));
				Warehouse.Instance.PushItem(new ItemGoldMedalData(1000));
				Warehouse.Instance.PushItem(new ItemGemData(m_cheatGem));
				Warehouse.Instance.PushItem(new ItemWeaponDNAData(0));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampTapRefItemId));	
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGunRefItemId));				
				//Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampLightningLauncherRefItemId));
				//Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampFiregunRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGuidedRocketLauncherRefItemId));
				//Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampRocketLauncherRefItemId));
				//Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampBoomerangLauncherRefItemId));

				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerGunMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerFireMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerLightningMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerRocketMarineRefItemId));				
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerBoomerangMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerGrenadeMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerMeleeRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerPetRefItemId));
				//Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerRedMarineRefItemId));

				Warehouse.Instance.PushItem(new ItemCheatData(Const.EngineeringBayRefItemId));
				Warehouse.Instance.PushItem(new ItemCheatData(Const.AcademyRefItemId));
				Warehouse.Instance.PushItem(new ItemStatData(2001));
				Warehouse.Instance.PushItem(new ItemStatData(2002));
				//Warehouse.Instance.PushItem(new ItemStatData(2003));
				//Warehouse.Instance.PushItem(new ItemStatData(2004));
				Warehouse.Instance.PushItem(new ItemStatData(2006));
				Warehouse.Instance.PushItem(new ItemStatData(2008));
				Warehouse.Instance.PushItem(new ItemStatData(2005));				
				Warehouse.Instance.PushItem(new ItemWeaponDNAData(0));


				Warehouse.Instance.PushItem(new ItemSkillData(21));
				Warehouse.Instance.PushItem(new ItemSkillData(22));
				Warehouse.Instance.PushItem(new ItemSkillData(23));
				Warehouse.Instance.PushItem(new ItemSkillData(24));
				Warehouse.Instance.PushItem(new ItemSkillData(25));

				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3001));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3004));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3003));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3002));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3005));


				Warehouse.Instance.FindItem(Const.ChampTapRefItemId).Item.Level = 900;
				Warehouse.Instance.FindItem(Const.ChampGunRefItemId).Item.Lock = false;
				Warehouse.Instance.FindItem(Const.ChampGunRefItemId).Item.Level = 900;
				Warehouse.Instance.FindItem(Const.FollowerGunMarineRefItemId).Item.Lock = false;
				Warehouse.Instance.FindItem(Const.FollowerGunMarineRefItemId).Item.Level = 900;
				Warehouse.Instance.FindItem(Const.FollowerFireMarineRefItemId).Item.Lock = false;
				Warehouse.Instance.FindItem(Const.FollowerFireMarineRefItemId).Item.Level = 900;
				Warehouse.Instance.FindItem(Const.FollowerLightningMarineRefItemId).Item.Lock = false;
				Warehouse.Instance.FindItem(Const.FollowerLightningMarineRefItemId).Item.Level = 900;
				Warehouse.Instance.FindItem(Const.FollowerRocketMarineRefItemId).Item.Lock = false;
				Warehouse.Instance.FindItem(Const.FollowerRocketMarineRefItemId).Item.Level = 900;
				Warehouse.Instance.FindItem(Const.FollowerBoomerangMarineRefItemId).Item.Lock = false;
				Warehouse.Instance.FindItem(Const.FollowerBoomerangMarineRefItemId).Item.Level = 900;
				Warehouse.Instance.FindItem(Const.FollowerGrenadeMarineRefItemId).Item.Lock = false;
				Warehouse.Instance.FindItem(Const.FollowerGrenadeMarineRefItemId).Item.Level = 900;
				Warehouse.Instance.FindItem(Const.FollowerMeleeRefItemId).Item.Lock = false;
				Warehouse.Instance.FindItem(Const.FollowerMeleeRefItemId).Item.Level = 900;
				Warehouse.Instance.FindItem(Const.FollowerPetRefItemId).Item.Lock = false;
				Warehouse.Instance.FindItem(Const.FollowerPetRefItemId).Item.Level = 9;
				//Warehouse.Instance.FindItem(Const.FollowerRedMarineRefItemId).Item.Lock = false;
				//Warehouse.Instance.FindItem(Const.FollowerRedMarineRefItemId).Item.Level = 1;

				Warehouse.Instance.FindItem(21).Item.Lock = false;
				Warehouse.Instance.FindItem(21).Item.Level = 9;
				Warehouse.Instance.FindItem(22).Item.Lock = false;
				Warehouse.Instance.FindItem(22).Item.Level = 9;
				Warehouse.Instance.FindItem(23).Item.Lock = false;
				Warehouse.Instance.FindItem(23).Item.Level = 9;
				Warehouse.Instance.FindItem(24).Item.Lock = false;
				Warehouse.Instance.FindItem(24).Item.Level = 9;
				Warehouse.Instance.FindItem(25).Item.Lock = false;
				Warehouse.Instance.FindItem(25).Item.Level = 1;

				Warehouse.Instance.FindItem(2001).Item.Lock = false;
				Warehouse.Instance.FindItem(2001).Item.Level = 900;
				Warehouse.Instance.FindItem(2002).Item.Lock = false;
				Warehouse.Instance.FindItem(2002).Item.Level = 900;
				Warehouse.Instance.FindItem(2006).Item.Lock = false;
				Warehouse.Instance.FindItem(2006).Item.Level = 900;
				Warehouse.Instance.FindItem(2008).Item.Lock = false;
				Warehouse.Instance.FindItem(2008).Item.Level = 900;
				Warehouse.Instance.FindItem(2005).Item.Lock = false;
				Warehouse.Instance.FindItem(2005).Item.Level = 900;

				if (m_eqipedAllItem == true)
				{
					Warehouse.Instance.FindItem(3001).Item.Lock = false;
					Warehouse.Instance.FindItem(3001).Item.Level = 900;
					Warehouse.Instance.FindItem(3002).Item.Lock = false;
					Warehouse.Instance.FindItem(3002).Item.Level = 900;
					Warehouse.Instance.FindItem(3003).Item.Lock = false;
					Warehouse.Instance.FindItem(3003).Item.Level = 900;
					Warehouse.Instance.FindItem(3004).Item.Lock = false;
					Warehouse.Instance.FindItem(3004).Item.Level = 900;
					Warehouse.Instance.FindItem(3005).Item.Lock = false;
					Warehouse.Instance.FindItem(3005).Item.Level = 900;
				}


			}

			Warehouse.Instance.GameTutorial.m_unlockedWeaponTab = true;
			Warehouse.Instance.GameTutorial.m_unlockedStatTab = true;
			Warehouse.Instance.GameTutorial.m_unlockedSkillTab = true;
			Warehouse.Instance.GameTutorial.m_unlockedFollowerTab = true;
#endif
		}
		else
		{
			//Load();

			if (Warehouse.Instance.InvenSize == 0)
			{
				Warehouse.Instance.PushItem(new ItemGoldData(0));
				Warehouse.Instance.PushItem(new ItemGoldMedalData(0));
				Warehouse.Instance.PushItem(new ItemGemData(0));
				Warehouse.Instance.PushItem(new ItemWeaponDNAData(0));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampTapRefItemId));	
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGunRefItemId));
				//Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampLightningLauncherRefItemId));
				//Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampFiregunRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGuidedRocketLauncherRefItemId));


				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerGunMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerFireMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerLightningMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerRocketMarineRefItemId));				
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerBoomerangMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerGrenadeMarineRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerMeleeRefItemId));
				Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerPetRefItemId));
				//Warehouse.Instance.PushItem(new ItemFollowerData(Const.FollowerRedMarineRefItemId));

				Warehouse.Instance.PushItem(new ItemCheatData(Const.EngineeringBayRefItemId));
				Warehouse.Instance.PushItem(new ItemCheatData(Const.AcademyRefItemId));
				Warehouse.Instance.PushItem(new ItemStatData(2001));
				Warehouse.Instance.PushItem(new ItemStatData(2002));
				//Warehouse.Instance.PushItem(new ItemStatData(2003));
				//Warehouse.Instance.PushItem(new ItemStatData(2004));
				Warehouse.Instance.PushItem(new ItemStatData(2006));
				Warehouse.Instance.PushItem(new ItemStatData(2008));
				Warehouse.Instance.PushItem(new ItemStatData(2005));
				Warehouse.Instance.PushItem(new ItemWeaponDNAData(0));


				Warehouse.Instance.PushItem(new ItemSkillData(21));
				Warehouse.Instance.PushItem(new ItemSkillData(22));
				Warehouse.Instance.PushItem(new ItemSkillData(23));
				Warehouse.Instance.PushItem(new ItemSkillData(24));
				Warehouse.Instance.PushItem(new ItemSkillData(25));

				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3001));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3004));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3003));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3002));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3005));
			}
		}

		for(int i = 0; i < m_equipedAccessories.Length; ++i)
		{
			m_equipedAccessories[i] = new EquippedContext();
		}


		m_tabs[0] = new TabDesc(0, MultiLang.ID.Weapon, new ItemData.Type[]{ItemData.Type.Weapon, ItemData.Type.WeaponParts}, new YGUISystem.GUIButton(transform.Find("WeaponTab").gameObject, ()=>{return Warehouse.Instance.GameTutorial.m_unlockedWeaponTab && m_champ != null && m_champ.gameObject.activeSelf;}));
		m_tabs[1] = new TabDesc(1, MultiLang.ID.Skill, new ItemData.Type[]{ItemData.Type.Skill}, new YGUISystem.GUIButton(transform.Find("SkillTab").gameObject, ()=>{return Warehouse.Instance.GameTutorial.m_unlockedSkillTab && m_champ != null && m_champ.gameObject.activeSelf;}));
		m_tabs[2] = new TabDesc(2, MultiLang.ID.Follower, new ItemData.Type[]{ItemData.Type.Follower}, new YGUISystem.GUIButton(transform.Find("FollowerTab").gameObject, ()=>{return Warehouse.Instance.GameTutorial.m_unlockedFollowerTab && m_champ != null && m_champ.gameObject.activeSelf;}));
		m_tabs[3] = new TabDesc(3, MultiLang.ID.Stat, new ItemData.Type[]{ItemData.Type.Stat, ItemData.Type.WeaponDNA}, new YGUISystem.GUIButton(transform.Find("StatTab").gameObject, ()=>{return Warehouse.Instance.GameTutorial.m_unlockedStatTab && m_champ != null && m_champ.gameObject.activeSelf;}));

		m_weaponPanel = settingItemList(m_tabs[0], "WeaponPanel");
		m_statPanel = settingItemList(m_tabs[3], "StatPanel");
		m_followerPanel = settingItemList(m_tabs[2], "FollowerPanel");
		m_skillPanel = settingItemList(m_tabs[1], "SkillPanel");
		m_generalInfoPanel = transform.Find("GeneralInfoPanel/ScrollView").gameObject.GetComponent<GeneralInfoPanel>();
		OnClickStart();
	}

	void SetButtonRole(Const.ButtonRole role, GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, ItemObject item)
	{
		priceGemButton.RemoveAllListeners();

		switch(role)
		{
		case Const.ButtonRole.Levelup:
			{

				priceGemButton.EnableChecker = ()=>{
				return item.Item.RefItem.levelup.conds != null && item.Item.Lock == false && item.Item.Level < item.Item.RefItem.maxLevel;};
				
				priceGemButton.SetPrices(role, item.Item.RefItem);
				
				priceGemButton.AddListener(() => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_priceButton, item), () => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_gemButton, item) );
			priceGemButton.SetLable(RefData.Instance.RefTexts(MultiLang.ID.LevelUp));
			}
			break;

		case Const.ButtonRole.Unlock:
			{
			priceGemButton.EnableChecker = ()=>{
				
				return item.Item.RefItem.unlock.conds != null && item.Item.Lock == true;};

				priceGemButton.SetPrices(role, item.Item.RefItem);

				priceGemButton.AddListener(() => OnClickUnlock(invSlot, priceGemButton, priceGemButton.m_priceButton, item), () => OnClickUnlock(invSlot, priceGemButton, priceGemButton.m_gemButton, item) );
				priceGemButton.SetLable(RefData.Instance.RefTexts(MultiLang.ID.Unlock));

			}
			break;

		case Const.ButtonRole.Evolution:
			{
			priceGemButton.EnableChecker = ()=>{
				return item.Item.RefItem.evolution.conds != null && item.Item.Level == item.Item.RefItem.maxLevel && item.Item.Evolution < item.Item.RefItem.maxEvolution;};
			
			priceGemButton.SetPrices(role, item.Item.RefItem);
			
			priceGemButton.AddListener(() => OnClickEvolution(invSlot, priceGemButton, priceGemButton.m_priceButton, item), () => OnClickEvolution(invSlot, priceGemButton, priceGemButton.m_gemButton, item) );
			priceGemButton.SetLable(RefData.Instance.RefTexts(MultiLang.ID.Evolution));
			}
			break;

		default:
			{
			}
			break;
		}

	}

	float checkTime = 0f;
	int	checkIndex = 0;
	void Update()
	{
	
		foreach(TabDesc tab in m_tabs)
		{
			tab.Update(m_selectedTabIndex);
		}

		if (checkTime < Time.time)
		{
			if (m_tabs[checkIndex%m_tabs.Length].m_tab.Button.interactable)
				checkAvailableItem(m_tabs[checkIndex%m_tabs.Length]);

			checkTime = Time.time + 0.3f;
			++checkIndex;
		}

	}

	IEnumerator AutoGoldUpdate()
	{
		Warehouse.Instance.AutoEarnGold = 0;

		WWW www = new WWW("http://currentmillis.com/api/millis-since-unix-epoch.php");
		yield return www;

		if (www.error == null)
		{
			System.DateTime now = new System.DateTime (1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(double.Parse(www.text));

			Warehouse.Instance.AutoEarnGold = (int)now.Subtract(Warehouse.Instance.LastModifiedFileTime).TotalMinutes*5*Warehouse.Instance.GameBestStats.WaveIndex;
			Warehouse.Instance.LastModifiedFileTime = now;
		}
		/*
		if (Application.platform == RuntimePlatform.Android)
		{
			GPlusPlatform.Instance.OpenGame("meta.sav", (SavedGameRequestStatus status, ISavedGameMetadata game)=>{
				if (status == SavedGameRequestStatus.Success)
				{
					GPlusPlatform.Instance.SaveGame(game, new byte[]{0}, new System.TimeSpan(), null, (SavedGameRequestStatus status1, ISavedGameMetadata game1)=>{
						Warehouse.Instance.AutoEarnGold = (int)game.LastModifiedTimestamp.Subtract(Warehouse.Instance.LastModifiedFileTime).TotalMinutes*5;
						Warehouse.Instance.LastModifiedFileTime = game.LastModifiedTimestamp;
					});
				} 
			});
		}*/
	}

	public void StartSpinButton(YGUISystem.GUIButton button)
	{
		button.Button.audio.Play();
	}

	void PopupShop()
	{
		GameObject.Find("HudGUI/ShopGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	public void OnClickStart()
	{
		Warehouse.Instance.NewGameStats.Reset();

		GameObject champObj = Creature.InstanceCreature(Resources.Load<GameObject>("Pref/Champ"), Resources.Load<GameObject>("Pref/mon_skin/" + RefData.Instance.RefChamp.prefBody), m_spawnChamp.position, m_spawnChamp.localRotation);	
		champObj.name = "Champ";

		Champ champ = champObj.GetComponent<Champ>();
		champ.Init(RefData.Instance.RefChamp, Warehouse.Instance.GameDataContext.m_level.Value);

		m_champ = champ;

		foreach(ItemObject itemWeaponObject in Warehouse.Instance.Items[ItemData.Type.Weapon])
		{
			if (itemWeaponObject.Item.Level > 0)
			{
				itemWeaponObject.Item.Equip(champ);
			}
		}

		foreach(ItemObject itemStatObject in Warehouse.Instance.Items[ItemData.Type.Stat])
		{
			if (itemStatObject.Item.Level > 0)
			{
				itemStatObject.Item.Equip(m_champ);
			}
		}

		foreach(ItemObject itemStatObject in Warehouse.Instance.Items[ItemData.Type.WeaponParts])
		{
			if (itemStatObject.Item.Level > 0)
			{
				itemStatObject.Item.Equip(m_champ);
			}
		}

		m_champ.WeaponHolder.EquipActiveSkillWeapon(m_champ.instanceWeapon(new ItemWeaponData(Const.NuclearRefItemId), new RefMob.WeaponDesc()));

		m_champ.m_creatureProperty.Exp = Warehouse.Instance.GameDataContext.m_xp.Value;
		m_champ.m_creatureProperty.HP = Warehouse.Instance.GameDataContext.m_hp.Value;
		m_champ.m_creatureProperty.SP = Warehouse.Instance.GameDataContext.m_sp.Value;

		if (m_champ.m_creatureProperty.HP == 0)
		{
			m_champ.m_creatureProperty.HP = m_champ.m_creatureProperty.MaxHP;
		}

		m_generalInfoPanel.SetChamp(m_champ);
		m_spawn.StartWave(Warehouse.Instance.CurrentWaveIndex, champ);

		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Play", "Retry:"+Warehouse.Instance.RetryCount, 0);
		
		champObj.SetActive(false);

		StartCoroutine(AutoGoldUpdate());

	}

	public void OnSlotMachine()
	{
		GameObject.Find("HudGUI/SlotMachineGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	public void OnClickLevelup(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, ItemObject selectedItem)
	{

		if (selectedItem.Item.Level < selectedItem.Item.RefItem.maxLevel)
		{
			if (button.TryToPay())
			{
				invSlot.IconAnimator.SetTrigger("Levelup");
				StartSpinButton(priceGemButton.m_priceButton.GUIImageButton);
				++selectedItem.Item.Level;

				if (selectedItem.Item.RefItem.type == ItemData.Type.Weapon)
				{
					m_champ.WeaponHolder.LevelUp(selectedItem.Item.RefItem.id);
					selectedItem.Item.Use(m_champ);
				}
				else if (selectedItem.Item.RefItem.type == ItemData.Type.Follower)
				{
					ItemFollowerData itemFollowerData = selectedItem.Item as ItemFollowerData;
					itemFollowerData.m_follower.LevelUp();
					itemFollowerData.Use(itemFollowerData.m_follower);
				}
				else if (selectedItem.Item.RefItem.type == ItemData.Type.Stat
				         || selectedItem.Item.RefItem.type == ItemData.Type.WeaponParts)
				{
					selectedItem.Item.Use(m_champ);
				}

				if (selectedItem.Item.Level == selectedItem.Item.RefItem.maxLevel)
				{
					priceGemButton.SetPrices(Const.ButtonRole.Nothing, null);
					SetButtonRole(Const.ButtonRole.Evolution, invSlot, priceGemButton, selectedItem);
				}

				invSlot.ItemDesc = selectedItem.Item.Description();


				GPlusPlatform.Instance.AnalyticsTrackEvent("Weapon", "Levelup", selectedItem.Item.RefItem.name + "_Lv:" + selectedItem.Item.Level, 0);
			}
			else
			{
				if (priceGemButton.m_gemButton == button)
					PopupShop();
			}
		}

	}

	public void OnClickUnlock(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, ItemObject selectedItem)
	{


		if (selectedItem.Item.Lock == true)
		{
			if (button.TryToPay() == true)
			{
				invSlot.IconAnimator.SetTrigger("Levelup");

				selectedItem.Item.Lock = false;
				selectedItem.Item.Level = 1;

				invSlot.ItemDesc = selectedItem.Item.Description();

				SetButtonRole(Const.ButtonRole.Levelup, invSlot, priceGemButton, selectedItem);
				switch(selectedItem.Item.RefItem.type)
				{
				case ItemData.Type.Follower:
					selectedItem.Item.Equip(m_champ);
					break;
				case ItemData.Type.Stat:
					selectedItem.Item.Equip(m_champ);
					break;
				case ItemData.Type.Weapon:
					selectedItem.Item.Equip(m_champ);
					break;
				}
			}
			else
			{
				if (priceGemButton.m_gemButton == button)
					PopupShop();
			}
		}
	}

	public void OnClickEvolution(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, ItemObject selectedItem)
	{
		
		if (selectedItem.Item.Evolution < selectedItem.Item.RefItem.maxEvolution)
		{
			if (button.TryToPay())
			{
				invSlot.IconAnimator.SetTrigger("Levelup");
				StartSpinButton(priceGemButton.m_priceButton.GUIImageButton);
				++selectedItem.Item.Evolution;
				selectedItem.Item.Level = 1;

				switch(selectedItem.Item.RefItem.type)
				{
				case ItemData.Type.Follower:
					ItemFollowerData itemFollowerData = selectedItem.Item as ItemFollowerData;
					itemFollowerData.m_follower.EvolutionUp();
					itemFollowerData.NoApplyOptions(itemFollowerData.m_follower);
					itemFollowerData.Use(itemFollowerData.m_follower);
					break;
				case ItemData.Type.Stat:
					break;
				case ItemData.Type.Weapon:
					m_champ.WeaponHolder.EvolutionUp(selectedItem.Item.RefItem.id);					
					selectedItem.Item.NoApplyOptions(m_champ);
					selectedItem.Item.Use(m_champ);
					break;
				}

				if (selectedItem.Item.Evolution == selectedItem.Item.RefItem.maxEvolution)
				{
					priceGemButton.SetPrices(Const.ButtonRole.Nothing, null);
				}
				
				invSlot.ItemDesc = selectedItem.Item.Description();
				SetButtonRole(Const.ButtonRole.Levelup, invSlot, priceGemButton, selectedItem);
				
				GPlusPlatform.Instance.AnalyticsTrackEvent("Weapon", "Evolution", selectedItem.Item.RefItem.name + "_Evolution:" + selectedItem.Item.Evolution, 0);
			}
			else
			{
				if (priceGemButton.m_gemButton == button)
					PopupShop();
			}
		}
		
	}

	bool checkAvailableItem(TabDesc tab)
	{
		bool check = false;
		foreach(ItemData.Type itemType in tab.m_itemTypes)
		{
			foreach(ItemObject itemObj in Warehouse.Instance.Items[itemType])
			{
				if (itemObj.Item.Lock == true)
				{
					if (itemObj.Item.RefItem.unlock != null)
						check = Const.CheckAvailableItem(itemObj.Item.RefItem.unlock.conds, 1f);
				}
				else
				{
					if (itemObj.Item.RefItem.maxLevel == itemObj.Item.Level)
					{
						if (itemObj.Item.RefItem.evolution != null)
							check = Const.CheckAvailableItem(itemObj.Item.RefItem.evolution.conds, Const.GetItemLevelupWorth(itemObj.Item.Evolution, itemObj.Item.RefItem.evolution));
					}
					else
					{
						if (itemObj.Item.RefItem.levelup != null)
							check = Const.CheckAvailableItem(itemObj.Item.RefItem.levelup.conds, Const.GetItemLevelupWorth(itemObj.Item.Level, itemObj.Item.RefItem.levelup));
					}

				}
				
				if (check == true)
				{
					tab.m_checked.SetActive(true);
					return true;
				}
			}
		}

		tab.m_checked.SetActive(false);
		return false;
	}

	GameObject settingItemList(TabDesc tab, string panel)
	{
		RectTransform rectScrollView = transform.Find(panel + "/ScrollView").gameObject.GetComponent<RectTransform>();
		GameObject contentsObj = transform.Find(panel+ "/ScrollView/Contents").gameObject;
		RectTransform rectInventoryObj = contentsObj.GetComponent<RectTransform>();

		
		GameObject prefGUIInventorySlot = Resources.Load<GameObject>("Pref/GUIInventorySlot");
		RectTransform	rectGUIInventorySlot = prefGUIInventorySlot.GetComponent<RectTransform>();
		
		int itemAddedCount = 0;
		int itemIndex = 0;
		int maxCount = 0;
		foreach(ItemData.Type itemType in tab.m_itemTypes)
			maxCount += Warehouse.Instance.Items[itemType].Count;
		int equipItemIndex = 0;

		foreach(ItemData.Type itemType in tab.m_itemTypes)
		{
			foreach(ItemObject item in Warehouse.Instance.Items[itemType])
			{
				
				GameObject obj = Instantiate(prefGUIInventorySlot) as GameObject;
				GUIInventorySlot invSlot = obj.GetComponent<GUIInventorySlot>();
				
				obj.transform.parent = contentsObj.transform;
				obj.transform.localScale = prefGUIInventorySlot.transform.localScale;
				obj.transform.localPosition = new Vector3(0f, rectGUIInventorySlot.rect.height/2*(maxCount-1)-rectGUIInventorySlot.rect.height*itemAddedCount, 0);
				
				invSlot.Init(tab.m_tab.Button.gameObject, item);
				invSlot.PriceButton0.EnableChecker = ()=>{return false;};
				
				int capturedItemIndex = itemIndex;
				

				if (item.Item.Lock == true)
				{
					if (item.Item.RefItem.type == ItemData.Type.WeaponParts)
					{
						if (item.Item.RefItem.levelup != null)
						{
							SetButtonRole(Const.ButtonRole.Levelup, invSlot, invSlot.PriceButton0, item);
						}
					}
					else
					{
						if (item.Item.RefItem.unlock != null)
						{
							SetButtonRole(Const.ButtonRole.Unlock, invSlot, invSlot.PriceButton0, item);
						}
					}

				}
				else
				{
					if (item.Item.RefItem.maxLevel == item.Item.Level)
					{
						if (item.Item.RefItem.evolution != null)
						{
							SetButtonRole(Const.ButtonRole.Evolution, invSlot, invSlot.PriceButton0, item);
						}
					}
					else
					{
						if (item.Item.RefItem.levelup != null)
						{
							SetButtonRole(Const.ButtonRole.Levelup, invSlot, invSlot.PriceButton0, item);
						}
					}

				}
				

								
				++itemAddedCount;
			}
		}
		Vector2 rectContents = new Vector2(	rectInventoryObj.rect.width, rectGUIInventorySlot.rect.height*itemAddedCount);
		rectInventoryObj.sizeDelta = rectContents;
		//rectInventoryObj.position = new Vector3(rectInventoryObj.position.x, -(rectContents.y/2-rectScrollView.rect.height/2), rectInventoryObj.position.z);
		rectInventoryObj.localPosition = new Vector3(0, -(rectContents.y/2-rectScrollView.rect.height/2-rectGUIInventorySlot.rect.height*equipItemIndex), 0);
		
		return transform.Find(panel).gameObject;
	}

	public void OnClickInventory()
	{
		m_weaponPanel.SetActive(true);
		m_statPanel.SetActive(false);
		m_followerPanel.SetActive(false);
		m_skillPanel.SetActive(false);
		m_generalInfoPanel.gameObject.SetActive(false);
		m_selectedTabIndex = 0;
		Const.GetTutorialMgr().SetTutorial("Nothing");
	}

	public void OnClickSkill()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(false);
		m_followerPanel.SetActive(false);
		m_skillPanel.SetActive(true);
		m_generalInfoPanel.gameObject.SetActive(false);
		m_selectedTabIndex = 1;
		Const.GetTutorialMgr().SetTutorial("Nothing");
	}

	public void OnClickFollower()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(false);
		m_followerPanel.SetActive(true);
		m_skillPanel.SetActive(false);
		m_generalInfoPanel.gameObject.SetActive(false);
		m_selectedTabIndex = 2;
		Const.GetTutorialMgr().SetTutorial("Nothing");
	}

	public void OnClickStat()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(true);
		m_followerPanel.SetActive(false);
		m_skillPanel.SetActive(false);
		m_generalInfoPanel.gameObject.SetActive(false);
		m_selectedTabIndex = 3;
		Const.GetTutorialMgr().SetTutorial("Nothing");
	}

	public void OnClickGeneralInfo()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(false);
		m_followerPanel.SetActive(false);
		m_skillPanel.SetActive(false);
		m_selectedTabIndex = -1;
		m_generalInfoPanel.gameObject.SetActive(true);
	}

	public void OnEscapeKeyUp()
	{		
		if (m_generalInfoPanel.gameObject.activeSelf == true)
			Const.GetWindowGui(Const.WindowGUIType.MainTitleGUI).SetActive(true);
		else
			OnClickGeneralInfo();
	}
}

