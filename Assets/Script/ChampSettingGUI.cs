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


	YGUISystem.GUIButton	m_weapon;
	YGUISystem.GUILockButton[]	m_accessories = new YGUISystem.GUILockButton[Const.AccessoriesSlots];
	YGUISystem.GUIButton	m_start;

	GameObject	m_weaponPanel;
	GameObject	m_statPanel;
	GameObject	m_followerPanel;
	GameObject	m_skillPanel;
	GeneralInfoPanel	m_generalInfoPanel;

	YGUISystem.GUIButton[]	m_tabs = new YGUISystem.GUIButton[4];

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

				if (m_eqipedAllItem == true)
				{
					Warehouse.Instance.FindItem(3001).Item.Lock = false;
					Warehouse.Instance.FindItem(3001).Item.Level = 99;
					Warehouse.Instance.FindItem(3002).Item.Lock = false;
					Warehouse.Instance.FindItem(3002).Item.Level = 99;
					Warehouse.Instance.FindItem(3003).Item.Lock = false;
					Warehouse.Instance.FindItem(3003).Item.Level = 99;
					Warehouse.Instance.FindItem(3004).Item.Lock = false;
					Warehouse.Instance.FindItem(3004).Item.Level = 99;
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

		m_weapon = new YGUISystem.GUIButton(transform.Find("WeaponButton").gameObject, ()=>{return true;});
		for(int i = 0; i < m_accessories.Length; ++i)
		{
			m_accessories[i] = new YGUISystem.GUILockButton(transform.Find("AccessoryButton" + i).gameObject, ()=>{return true;});

			if (i < Const.HalfAccessoriesSlots)
				m_accessories[i].Lock = false;
		}

		m_start = new YGUISystem.GUIButton(transform.Find("StartButton").gameObject, ()=>{return m_equipedWeapon != null;});


		m_tabs[0] = new YGUISystem.GUIButton(transform.Find("WeaponTab").gameObject, ()=>{return Warehouse.Instance.GameTutorial.m_unlockedWeaponTab && m_champ != null && m_champ.gameObject.activeSelf;});
		m_tabs[0].Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Weapon);
		m_tabs[1] = new YGUISystem.GUIButton(transform.Find("SkillTab").gameObject, ()=>{return Warehouse.Instance.GameTutorial.m_unlockedSkillTab && m_champ != null && m_champ.gameObject.activeSelf;});
		m_tabs[1].Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Skill);
		m_tabs[2] = new YGUISystem.GUIButton(transform.Find("FollowerTab").gameObject, ()=>{return Warehouse.Instance.GameTutorial.m_unlockedFollowerTab && m_champ != null && m_champ.gameObject.activeSelf;});
		m_tabs[2].Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Follower);
		m_tabs[3] = new YGUISystem.GUIButton(transform.Find("StatTab").gameObject, ()=>{return Warehouse.Instance.GameTutorial.m_unlockedStatTab && m_champ != null && m_champ.gameObject.activeSelf;});
		m_tabs[3].Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Stat);

		m_weaponPanel = settingItemList("WeaponPanel", new ItemData.Type[]{ItemData.Type.Weapon, ItemData.Type.WeaponParts});
		m_statPanel = settingItemList("StatPanel", new ItemData.Type[]{ItemData.Type.Stat, ItemData.Type.WeaponDNA});
		m_followerPanel = settingItemList("FollowerPanel", new ItemData.Type[]{ItemData.Type.Follower});
		m_skillPanel = settingItemList("SkillPanel", new ItemData.Type[]{ItemData.Type.Skill});
		m_generalInfoPanel = transform.Find("GeneralInfoPanel/ScrollView").gameObject.GetComponent<GeneralInfoPanel>();
		OnClickStart();
	}
	enum ButtonRole
	{
		Nothing,
		Equip,
		Unequip,
		Levelup,
		Unlock,
	}
	void SetButtonRole(ButtonRole role, GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, ItemObject item)
	{
		priceGemButton.RemoveAllListeners();

		switch(role)
		{
		case ButtonRole.Equip:
			{
				priceGemButton.SetActive(false);
				priceGemButton.EnableChecker = ()=>{return item.Item.RefItem.type != ItemData.Type.Cheat || item.Item.RefItem.type != ItemData.Type.Stat;};

				priceGemButton.SetPrices(null, null);
				priceGemButton.AddListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, item), () => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_gemButton, item) );

				if (item.Item.RefItem.type == ItemData.Type.Follower)
					priceGemButton.SetLable("Follow");
				else
					priceGemButton.SetLable("Equip");
		}
			break;
		case ButtonRole.Unequip:
			{
			priceGemButton.SetActive(false);

			priceGemButton.EnableChecker = ()=>{return true;};

				priceGemButton.SetPrices(null, null);
			priceGemButton.AddListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, item), () => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_gemButton, item) );

			if (item.Item.RefItem.type == ItemData.Type.Follower)
				priceGemButton.SetLable("Unfollow");
			else
				priceGemButton.SetLable("Unequip");
				
			}
			break;

		case ButtonRole.Levelup:
			{

				priceGemButton.EnableChecker = ()=>{
				return item.Item.RefItem.levelup.conds != null && item.Item.Lock == false && item.Item.Level < item.Item.RefItem.maxLevel;};
				
				priceGemButton.SetPrices(item.Item.RefItem.levelup.conds, item.Item.RefItem.levelup.else_conds);
				
				priceGemButton.AddListener(() => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_priceButton, item), () => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_gemButton, item) );
			priceGemButton.SetLable(RefData.Instance.RefTexts(MultiLang.ID.LevelUp));
			}
			break;

		case ButtonRole.Unlock:
			{
			priceGemButton.EnableChecker = ()=>{
				
				return item.Item.RefItem.unlock.conds != null && item.Item.Lock == true;};

				priceGemButton.SetPrices(item.Item.RefItem.unlock.conds, item.Item.RefItem.unlock.else_conds);

				priceGemButton.AddListener(() => OnClickUnlock(invSlot, priceGemButton, priceGemButton.m_priceButton, item), () => OnClickUnlock(invSlot, priceGemButton, priceGemButton.m_gemButton, item) );
				priceGemButton.SetLable(RefData.Instance.RefTexts(MultiLang.ID.Unlock));

			}
			break;

		default:
			{
			}
			break;
		}

	}

	void Update()
	{
		m_start.Update();

		foreach(YGUISystem.GUIButton button in m_tabs)
		{
			button.Update();
		}
		/*
		if (m_champ == null)
			OnClickGeneralInfo();
*/
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
		button.Button.animator.SetBool("Spin", true);
		button.Button.audio.Play();
	}

	void PopupShop()
	{
		GameObject.Find("HudGUI/ShopGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	public void OnClickStart()
	{
		Warehouse.Instance.UpdateGameStats.Reset();

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

		m_champ.WeaponHolder.EquipActiveSkillWeapon(m_champ.instanceWeapon(new ItemWeaponData(Const.NuclearRefItemId), null));

		m_champ.m_creatureProperty.Exp = Warehouse.Instance.GameDataContext.m_xp.Value;
		m_champ.m_creatureProperty.HP = Warehouse.Instance.GameDataContext.m_hp.Value;
		m_champ.m_creatureProperty.SP = Warehouse.Instance.GameDataContext.m_sp.Value;

		if (m_champ.m_creatureProperty.HP == 0)
		{
			m_champ.m_creatureProperty.HP = m_champ.m_creatureProperty.MaxHP;
		}

		m_generalInfoPanel.SetChamp(m_champ);
		m_spawn.StartWave(Warehouse.Instance.WaveIndex, champ);

		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Play", "Retry:"+Warehouse.Instance.RetryCount, 0);
		
		champObj.SetActive(false);

		StartCoroutine(AutoGoldUpdate());

	}

	public void OnSlotMachine()
	{
		GameObject.Find("HudGUI/SlotMachineGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	public void OnClickEquip(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, ItemObject selectedItem)
	{

		bool inEquipSlot = m_equipedWeapon.m_itemObject == selectedItem;
		if (inEquipSlot == true)
		{
			return;
		}

		switch(selectedItem.Item.RefItem.type)
		{
		case ItemData.Type.Weapon:
		{
			if (m_equipedWeapon.m_inventorySlot != null)
			{
				m_equipedWeapon.m_inventorySlot.Check(false);
			}
			m_equipedWeapon.m_itemObject = selectedItem;
			m_equipedWeapon.m_inventorySlot = invSlot;
			m_weapon.Icon.Image = selectedItem.ItemIcon;
			Warehouse.Instance.ChampEquipItems.m_weaponRefItemId = selectedItem.Item.RefItemID;
			//invSlot.Check(true);
			SetButtonRole(ButtonRole.Unequip, invSlot, priceGemButton, selectedItem);

			selectedItem.Item.Equip(m_champ);

		}break;

		}
	}

	public void OnClickLevelup(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, ItemObject selectedItem)
	{

		if (selectedItem.Item.Level < selectedItem.Item.RefItem.maxLevel)
		{
			if (button.TryToPay())
			{
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
					priceGemButton.SetPrices(null, null);
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
				selectedItem.Item.Lock = false;
				selectedItem.Item.Level = 1;

				invSlot.ItemDesc = selectedItem.Item.Description();

				SetButtonRole(ButtonRole.Equip, invSlot, priceGemButton, selectedItem);
				switch(selectedItem.Item.RefItem.type)
				{
				case ItemData.Type.Follower:
					selectedItem.Item.Equip(m_champ);
					break;
				case ItemData.Type.Stat:
					selectedItem.Item.Equip(m_champ);
					break;
				case ItemData.Type.Weapon:
					OnClickEquip(invSlot, priceGemButton, button, selectedItem);
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


	GameObject settingItemList(string panel, ItemData.Type[] itemTypes)
	{
		RectTransform rectScrollView = transform.Find(panel + "/ScrollView").gameObject.GetComponent<RectTransform>();
		GameObject contentsObj = transform.Find(panel+ "/ScrollView/Contents").gameObject;
		RectTransform rectInventoryObj = contentsObj.GetComponent<RectTransform>();

		
		GameObject prefGUIInventorySlot = Resources.Load<GameObject>("Pref/GUIInventorySlot");
		RectTransform	rectGUIInventorySlot = prefGUIInventorySlot.GetComponent<RectTransform>();
		
		int itemAddedCount = 0;
		int itemIndex = 0;
		int maxCount = 0;
		foreach(ItemData.Type itemType in itemTypes)
			maxCount += Warehouse.Instance.Items[itemType].Count;
		int equipItemIndex = 0;

		foreach(ItemData.Type itemType in itemTypes)
		{
			foreach(ItemObject item in Warehouse.Instance.Items[itemType])
			{
				
				GameObject obj = Instantiate(prefGUIInventorySlot) as GameObject;
				GUIInventorySlot invSlot = obj.GetComponent<GUIInventorySlot>();
				
				obj.transform.parent = contentsObj.transform;
				obj.transform.localScale = prefGUIInventorySlot.transform.localScale;
				obj.transform.localPosition = new Vector3(0f, rectGUIInventorySlot.rect.height/2*(maxCount-1)-rectGUIInventorySlot.rect.height*itemAddedCount, 0);
				
				invSlot.Init(item);
				invSlot.PriceButton0.EnableChecker = ()=>{return false;};
				invSlot.PriceButton1.EnableChecker = ()=>{return false;};
				
				int capturedItemIndex = itemIndex;
				
				
				if (item.Item.Lock == true)
				{
					if (item.Item.RefItem.unlock != null)
					{
						SetButtonRole(ButtonRole.Unlock, invSlot, invSlot.PriceButton0, item);
					}
					else
					{
						invSlot.PriceButton0.SetActive(false);
					}
				}
				else
				{
					SetButtonRole(ButtonRole.Equip, invSlot, invSlot.PriceButton0, item);
				}
				
				if (item.Item.RefItem.levelup != null)
				{
					SetButtonRole(ButtonRole.Levelup, invSlot, invSlot.PriceButton1, item);
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

		Const.GetTutorialMgr().SetTutorial("Nothing");
	}

	public void OnClickStat()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(true);
		m_followerPanel.SetActive(false);
		m_skillPanel.SetActive(false);
		m_generalInfoPanel.gameObject.SetActive(false);

		Const.GetTutorialMgr().SetTutorial("Nothing");
	}

	public void OnClickFollower()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(false);
		m_followerPanel.SetActive(true);
		m_skillPanel.SetActive(false);
		m_generalInfoPanel.gameObject.SetActive(false);

		Const.GetTutorialMgr().SetTutorial("Nothing");
	}

	public void OnClickSkill()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(false);
		m_followerPanel.SetActive(false);
		m_skillPanel.SetActive(true);
		m_generalInfoPanel.gameObject.SetActive(false);

		Const.GetTutorialMgr().SetTutorial("Nothing");
	}

	public void OnClickGeneralInfo()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(false);
		m_followerPanel.SetActive(false);
		m_skillPanel.SetActive(false);
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

