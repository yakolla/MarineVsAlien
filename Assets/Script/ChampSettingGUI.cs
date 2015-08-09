using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class ChampSettingGUI : MonoBehaviour {


	YGUISystem.GUIButton	m_weapon;
	YGUISystem.GUILockButton[]	m_accessories = new YGUISystem.GUILockButton[Const.AccessoriesSlots];
	YGUISystem.GUIButton	m_start;

	GameObject	m_weaponPanel;
	GameObject	m_statPanel;
	GameObject	m_followerPanel;
	GameObject	m_skillPanel;

	[SerializeField]
	Transform		m_spawnChamp;

	[SerializeField]
	bool		m_cheat = true;

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
				Warehouse.Instance.PushItem(new ItemGemData(12000));
				
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGunRefItemId));
				
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampLightningLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampFiregunRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGuidedRocketLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampRocketLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampBoomerangLauncherRefItemId));
				
				foreach(RefMob follower in RefData.Instance.RefFollowerMobs)
				{
					ItemFollowerData followerData = new ItemFollowerData(follower.id);
					Warehouse.Instance.PushItem(followerData);
				}
				
				Warehouse.Instance.PushItem(new ItemCheatData(Const.EngineeringBayRefItemId));
				Warehouse.Instance.PushItem(new ItemCheatData(Const.AcademyRefItemId));
				Warehouse.Instance.PushItem(new ItemStatData(2001));
				Warehouse.Instance.PushItem(new ItemStatData(2002));
				Warehouse.Instance.PushItem(new ItemStatData(2003));
				Warehouse.Instance.PushItem(new ItemStatData(2004));
				Warehouse.Instance.PushItem(new ItemStatData(2005));

				Warehouse.Instance.PushItem(new ItemStatData(21));
				Warehouse.Instance.PushItem(new ItemStatData(22));
				Warehouse.Instance.PushItem(new ItemStatData(23));
			}
			byte[] data = Warehouse.Instance.Serialize();
			Warehouse.Instance.Deserialize(data);

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

				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGunRefItemId));

				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampLightningLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampFiregunRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGuidedRocketLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampRocketLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampBoomerangLauncherRefItemId));

				foreach(RefMob follower in RefData.Instance.RefFollowerMobs)
				{
					ItemFollowerData followerData = new ItemFollowerData(follower.id);
					Warehouse.Instance.PushItem(followerData);
				}

				Warehouse.Instance.PushItem(new ItemCheatData(Const.EngineeringBayRefItemId));
				Warehouse.Instance.PushItem(new ItemCheatData(Const.AcademyRefItemId));
				Warehouse.Instance.PushItem(new ItemStatData(2001));
				Warehouse.Instance.PushItem(new ItemStatData(2002));
				Warehouse.Instance.PushItem(new ItemStatData(2003));
				Warehouse.Instance.PushItem(new ItemStatData(2004));
				Warehouse.Instance.PushItem(new ItemStatData(2005));

				Warehouse.Instance.PushItem(new ItemStatData(21));
				Warehouse.Instance.PushItem(new ItemStatData(22));
				Warehouse.Instance.PushItem(new ItemStatData(23));
			}

			byte[] data = Warehouse.Instance.Serialize();
			Warehouse.Instance.Deserialize(data);

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

		m_weaponPanel = settingItemList("WeaponPanel", ItemData.Type.Weapon);
		m_statPanel = settingItemList("StatPanel", ItemData.Type.Stat);
		m_followerPanel = settingItemList("FollowerPanel", ItemData.Type.Follower);
		m_skillPanel = settingItemList("SkillPanel", ItemData.Type.Skill);

		OnClickStat();

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

			if (item.Item.RefItem.type == ItemData.Type.Weapon)
				invSlot.SetListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, item));
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
				
			if (item.Item.RefItem.type == ItemData.Type.Weapon)
				invSlot.SetListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, item));
			}
			break;

		case ButtonRole.Levelup:
			{

			priceGemButton.EnableChecker = ()=>{return item.Item.RefItem.levelup.conds != null && item.Item.Lock == false && item.Item.Level < item.Item.RefItem.maxLevel;};
				
				priceGemButton.SetPrices(item.Item.RefItem.levelup.conds, item.Item.RefItem.levelup.else_conds);
				priceGemButton.m_priceButton.NormalWorth = Const.GetItemLevelupWorth(item.Item.Level, item.Item.RefItem.levelup);
				priceGemButton.m_gemButton.NormalWorth = Const.GetItemLevelupWorth(item.Item.Level, item.Item.RefItem.levelup);

			priceGemButton.AddListener(() => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_priceButton, item), () => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_gemButton, item) );
					priceGemButton.SetLable("Levelup");
				}
			break;

		case ButtonRole.Unlock:
			{
			priceGemButton.EnableChecker = ()=>{return item.Item.RefItem.unlock.conds != null;};

				priceGemButton.SetPrices(item.Item.RefItem.unlock.conds, item.Item.RefItem.unlock.else_conds);

			priceGemButton.AddListener(() => OnClickUnlock(invSlot, priceGemButton, priceGemButton.m_priceButton, item), () => OnClickUnlock(invSlot, priceGemButton, priceGemButton.m_gemButton, item) );
				priceGemButton.SetLable("Unlock");

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

		m_champ.WeaponHolder.EquipActiveSkillWeapon(m_champ.instanceWeapon(new ItemWeaponData(Const.NuclearRefItemId), null));

		foreach(ItemObject itemFollowerObject in Warehouse.Instance.Items[ItemData.Type.Follower])
		{
			if (itemFollowerObject.Item.Level > 0)
			{
				itemFollowerObject.Item.Equip(m_champ);
			}
		}

		for(int x = 0; x < m_equipedAccessories.Length; ++x)
		{
			if (m_equipedAccessories[x].m_itemObject != null)
			{
				m_equipedAccessories[x].m_itemObject.Item.Equip(champ);
				champ.AccessoryItems[x] = m_equipedAccessories[x].m_itemObject;
				Warehouse.Instance.ChampEquipItems.m_accessoryRefItemId[x] = m_equipedAccessories[x].m_itemObject.Item.RefItemID;
			}
		}

		m_champ.m_creatureProperty.Exp = Warehouse.Instance.GameDataContext.m_xp.Value;
		m_champ.m_creatureProperty.HP = Warehouse.Instance.GameDataContext.m_hp.Value;

		m_champ.MobKills = Warehouse.Instance.KilledMobs;
		m_spawn.StartWave(Warehouse.Instance.WaveIndex, champ);


		GPlusPlatform.Instance.AnalyticsTrackEvent("Start", "Setting", "Stage:"+Warehouse.Instance.WaveIndex, 0);

		
		champObj.SetActive(false);

		//gameObject.SetActive(false);

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

	void UpdateAccessorySlots()
	{
		for(int i = Const.HalfAccessoriesSlots; i < Cheat.HowManyAccessorySlot; ++i)
		{
			m_accessories[i].Lock = false;
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
					m_champ.WeaponHolder.MainWeapon.LevelUp();
				else if (selectedItem.Item.RefItem.type == ItemData.Type.Follower)
				{
					ItemFollowerData itemFollowerData = selectedItem.Item as ItemFollowerData;
					itemFollowerData.m_follower.LevelUp();
					itemFollowerData.Use(itemFollowerData.m_follower);
				}
				else if (selectedItem.Item.RefItem.type == ItemData.Type.Stat)
				{
					selectedItem.Item.Use(m_champ);
				}

				if (selectedItem.Item.Level == selectedItem.Item.RefItem.maxLevel)
				{
					priceGemButton.SetPrices(null, null);


				}

				priceGemButton.m_priceButton.NormalWorth = Const.GetItemLevelupWorth(selectedItem.Item.Level, selectedItem.Item.RefItem.levelup);
				priceGemButton.m_gemButton.NormalWorth = Const.GetItemLevelupWorth(selectedItem.Item.Level, selectedItem.Item.RefItem.levelup);

				invSlot.ItemDesc = selectedItem.Item.Description();

				UpdateAccessorySlots();

				GPlusPlatform.Instance.AnalyticsTrackEvent("Weapon", "Levelup", selectedItem.Item.RefItem.codeName + "_Lv:" + selectedItem.Item.Level, 0);
			}
			else
			{
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


				invSlot.PriceButton1.m_priceButton.NormalWorth = Const.GetItemLevelupWorth(selectedItem.Item.Level, selectedItem.Item.RefItem.levelup);
				invSlot.PriceButton1.m_gemButton.NormalWorth = Const.GetItemLevelupWorth(selectedItem.Item.Level, selectedItem.Item.RefItem.levelup);

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
				PopupShop();
			}
		}
	}

	GameObject settingInventory()
	{
		RectTransform rectScrollView = transform.Find("InvPanel/ScrollView").gameObject.GetComponent<RectTransform>();
		GameObject contentsObj = transform.Find("InvPanel/ScrollView/Contents").gameObject;
		RectTransform rectInventoryObj = contentsObj.GetComponent<RectTransform>();
		Vector2 rectContents = new Vector2(	rectInventoryObj.rect.width, 0);
		
		GameObject prefGUIInventorySlot = Resources.Load<GameObject>("Pref/GUIInventorySlot");
		RectTransform	rectGUIInventorySlot = prefGUIInventorySlot.GetComponent<RectTransform>();

		int itemAddedCount = 0;
		int itemIndex = 0;
		int maxCount = Warehouse.Instance.InvenSize-Warehouse.Instance.Items[ItemData.Type.Stat].Count
			-Warehouse.Instance.Items[ItemData.Type.Gem].Count
				-Warehouse.Instance.Items[ItemData.Type.Gold].Count
				-Warehouse.Instance.Items[ItemData.Type.GoldMedal].Count;
		int equipItemIndex = 0;

		foreach(KeyValuePair<ItemData.Type, List<ItemObject>> pair in Warehouse.Instance.Items)
		{
			foreach(ItemObject item in pair.Value)
			{
				if (item.Item.RefItem.type == ItemData.Type.Stat ||
				    item.Item.RefItem.type == ItemData.Type.Gem ||
				    item.Item.RefItem.type == ItemData.Type.Gold ||
				    item.Item.RefItem.type == ItemData.Type.GoldMedal)
					continue;
				
				GameObject obj = Instantiate(prefGUIInventorySlot) as GameObject;
				GUIInventorySlot invSlot = obj.GetComponent<GUIInventorySlot>();
				
				obj.transform.parent = contentsObj.transform;
				obj.transform.localScale = prefGUIInventorySlot.transform.localScale;
				obj.transform.localPosition = new Vector3(0f, rectGUIInventorySlot.rect.height/2*(maxCount-1)-rectGUIInventorySlot.rect.height*itemAddedCount, 0);
				
				invSlot.Init(item.ItemIcon, item.Item.Description());
				invSlot.PriceButton0.EnableChecker = ()=>{return false;};
				invSlot.PriceButton1.EnableChecker = ()=>{return false;};
				
				int capturedItemIndex = itemIndex;
				
				

					if (item.Item.Lock == true)
					{
						if (item.Item.RefItem.unlock != null)
						{
							SetButtonRole(ButtonRole.Unlock, invSlot, invSlot.PriceButton0, item);
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
					
				
				invSlot.Update();
				
				if (Warehouse.Instance.ChampEquipItems.m_weaponRefItemId == item.Item.RefItemID)
				{
					equipItemIndex = itemIndex;
					OnClickEquip(invSlot, invSlot.PriceButton0, invSlot.PriceButton0.m_priceButton, item);
				}
				else
				{
					for(int x = 0; x < m_equipedAccessories.Length; ++x)
					{
						if (Warehouse.Instance.ChampEquipItems.m_accessoryRefItemId[x] == item.Item.RefItemID)
						{
							OnClickEquip(invSlot, invSlot.PriceButton0, invSlot.PriceButton0.m_priceButton, item);
							break;
						}
					}
				}
				
				++itemAddedCount;
			}
		}

		rectContents.y = rectGUIInventorySlot.rect.height*itemAddedCount;
		rectInventoryObj.sizeDelta = rectContents;
		//rectInventoryObj.position = new Vector3(rectInventoryObj.position.x, -(rectContents.y/2-rectScrollView.rect.height/2), rectInventoryObj.position.z);
		rectInventoryObj.localPosition = new Vector3(0, -(rectContents.y/2-rectScrollView.rect.height/2-rectGUIInventorySlot.rect.height*equipItemIndex), 0);

		UpdateAccessorySlots();

		return transform.Find("InvPanel").gameObject;
	}


	GameObject settingItemList(string panel, ItemData.Type itemType)
	{
		RectTransform rectScrollView = transform.Find(panel + "/ScrollView").gameObject.GetComponent<RectTransform>();
		GameObject contentsObj = transform.Find(panel+ "/ScrollView/Contents").gameObject;
		RectTransform rectInventoryObj = contentsObj.GetComponent<RectTransform>();

		
		GameObject prefGUIInventorySlot = Resources.Load<GameObject>("Pref/GUIInventorySlot");
		RectTransform	rectGUIInventorySlot = prefGUIInventorySlot.GetComponent<RectTransform>();
		
		int itemAddedCount = 0;
		int itemIndex = 0;
		int maxCount = Warehouse.Instance.Items[itemType].Count;
		int equipItemIndex = 0;
		
		foreach(ItemObject item in Warehouse.Instance.Items[itemType])
		{
			
			GameObject obj = Instantiate(prefGUIInventorySlot) as GameObject;
			GUIInventorySlot invSlot = obj.GetComponent<GUIInventorySlot>();
			
			obj.transform.parent = contentsObj.transform;
			obj.transform.localScale = prefGUIInventorySlot.transform.localScale;
			obj.transform.localPosition = new Vector3(0f, rectGUIInventorySlot.rect.height/2*(maxCount-1)-rectGUIInventorySlot.rect.height*itemAddedCount, 0);
			
			invSlot.Init(item.ItemIcon, item.Item.Description());
			invSlot.PriceButton0.EnableChecker = ()=>{return false;};
			invSlot.PriceButton1.EnableChecker = ()=>{return false;};
			
			int capturedItemIndex = itemIndex;
			
			
			if (item.Item.Lock == true)
			{
				if (item.Item.RefItem.unlock != null)
				{
					SetButtonRole(ButtonRole.Unlock, invSlot, invSlot.PriceButton0, item);
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
			
			invSlot.Update();
			
			++itemAddedCount;
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
	}

	public void OnClickStat()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(true);
		m_followerPanel.SetActive(false);
		m_skillPanel.SetActive(false);
	}

	public void OnClickFollower()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(false);
		m_followerPanel.SetActive(true);
		m_skillPanel.SetActive(false);
	}

	public void OnClickSkill()
	{
		m_weaponPanel.SetActive(false);
		m_statPanel.SetActive(false);
		m_followerPanel.SetActive(false);
		m_skillPanel.SetActive(true);
	}
}

