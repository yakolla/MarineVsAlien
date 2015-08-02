using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class WarehouseData
{
	[JsonIgnore]
	public int					m_version = 1;
	[JsonIgnore]
	public Dictionary<ItemData.Type, List<ItemObject>>	m_items = new Dictionary<ItemData.Type, List<ItemObject>>();
	
	public SecuredType.XInt	m_waveIndex = 0;
	public SecuredType.XInt	m_killedMobs = 0;
	
	public GameStatistics			m_gameBestStats = new GameStatistics();
	public Options					m_options = new Options();
	public EquipItems				m_equipItems = new EquipItems();
	public GameDataContext			m_gameDataContext = new GameDataContext();

	public class 	GameStatistics
	{
		public SecuredType.XInt		m_waveIndex = 0;
		public SecuredType.XInt		m_killedMobs = 0;
		
		public void SetBestStats(GameStatistics newStats)
		{
			KilledMobs = Mathf.Max(KilledMobs, newStats.KilledMobs);
			WaveIndex = Mathf.Max(WaveIndex, newStats.WaveIndex);
		}
		
		[JsonIgnore]
		public int KilledMobs
		{
			set{m_killedMobs.Value = value;}
			get{return m_killedMobs.Value;}
		}
		
		[JsonIgnore]
		public int WaveIndex
		{
			set{m_waveIndex.Value = value;}
			get{return m_waveIndex.Value;}
		}
	}
	
	public class Options
	{
		public float	m_sfxVolume = 1f;	
		public float	m_bgmVolume = 1f;
		public bool		m_autoTarget = true;
	}
	
	public class EquipItems
	{
		public int		m_weaponRefItemId = Const.ChampGunRefItemId;
		public int[]	m_accessoryRefItemId = new int[Const.AccessoriesSlots];
	}

	public class GameDataContext
	{
		public SecuredType.XInt	m_hp = 100;
		public SecuredType.XInt	m_xp = 0;
		public SecuredType.XInt	m_level = 1;
	}
}

public class Warehouse {

	WarehouseData	m_warehouseData = new WarehouseData();
	public string				m_fileName;

	ItemObject			m_gold;
	ItemObject			m_goldMedal;
	ItemObject			m_gem;

	WarehouseData.GameStatistics			m_newGameStats = new WarehouseData.GameStatistics();

	float					m_playTime = 0f;
	float					m_saveTime = 0f;
	public float PlayTime
	{
		get { 
			if (m_playTime == 0f)
				return 0f;

			return Time.time-m_playTime; 
		}
		set { m_playTime = value; }
	}

	public float SaveTime
	{
		get { 
			if (m_saveTime == 0f)
				return 0f;
			
			return Time.time-m_saveTime; 
		}
		set { m_saveTime = value; }
	}

	static Warehouse m_ins = null;
	static public Warehouse Instance
	{
		get {
			if (m_ins == null)
			{
				m_ins = new Warehouse();
			}

			return m_ins;
		}
	}

	public Warehouse()
	{
		initInven(m_warehouseData.m_items);
	}

	public void Reset()
	{
		m_warehouseData = new WarehouseData();
		initInven(m_warehouseData.m_items);

	}

	public void ResetNewGameStats()
	{	
		m_warehouseData.m_gameBestStats.SetBestStats(m_newGameStats);
		m_newGameStats = new WarehouseData.GameStatistics();	
		Warehouse.Instance.PlayTime = Time.time;
		Warehouse.Instance.SaveTime = Time.time;
	}

	public void PushItem(ItemData item)
	{
		ItemObject itemObj = FindItem(item.RefItem.id);
		if (itemObj == null)
		{
			m_warehouseData.m_items[item.RefItem.type].Add(new ItemObject(item));
		}
		else
		{
			itemObj.Item.Count += item.Count;
		}
	}

	public void PullItem(ItemObject itemObj, int count)
	{
		if (itemObj == null)
		{
			return;
		}

		if (count <= itemObj.Item.Count)
		{
			itemObj.Item.Count -= count;
			if (itemObj.Item.Count == 0)
			{
				RemoveItem(itemObj);
			}
		}

	}

	public void RemoveItem(ItemObject obj)
	{
		m_warehouseData.m_items[obj.Item.RefItem.type].Remove(obj);
	}

	public ItemObject FindItem(int refItemId)
	{
		foreach(ItemObject obj in m_warehouseData.m_items[RefData.Instance.RefItems[refItemId].type])
		{
			if (obj.Item.RefItem.id == refItemId)
			{
				return obj;
			}
		}

		return null;
	}

	public Dictionary<ItemData.Type, List<ItemObject>> Items
	{
		get{return m_warehouseData.m_items;}
	}

	public ItemObject Gold
	{
		get { return m_gold; }
	}
	
	public ItemObject GoldMedal
	{
		get { return m_goldMedal; }
	}
	
	public ItemObject Gem
	{
		get { return m_gem; }
	}

	public string FileName
	{
		get {return m_fileName;}
		set {m_fileName = value;}
	}

	public WarehouseData.GameStatistics NewGameStats
	{
		get {return m_newGameStats;}
	}

	public int InvenSize
	{
		get{
			int size = 0;
			foreach(KeyValuePair<ItemData.Type, List<ItemObject>> pair in Warehouse.Instance.Items)
			{
				size += pair.Value.Count;
			}
			
			return size;
		}
	}
	
	public int WaveIndex
	{
		get {return m_warehouseData.m_waveIndex.Value;}
		set {m_warehouseData.m_waveIndex.Value = value;}
	}
	
	public int KilledMobs
	{
		get {return m_warehouseData.m_killedMobs.Value;}
		set {m_warehouseData.m_killedMobs.Value = value;}
	}	
	
	public WarehouseData.GameStatistics GameBestStats
	{
		get {return m_warehouseData.m_gameBestStats;}
	}
	
	public WarehouseData.Options GameOptions
	{
		get {return m_warehouseData.m_options;}
	}
	
	public WarehouseData.EquipItems ChampEquipItems
	{
		get {return m_warehouseData.m_equipItems;}
	}

	public WarehouseData.GameDataContext GameDataContext
	{
		get {return m_warehouseData.m_gameDataContext;}
	}
		
	static protected void initInven(Dictionary<ItemData.Type, List<ItemObject>> items)
	{
		items.Clear();
		foreach (ItemData.Type type in System.Enum.GetValues(typeof(ItemData.Type)))
		{
			if (type == ItemData.Type.Count)
				continue;
			
			items.Add(type, new List<ItemObject>());
		}
	}

	public byte[] Serialize()
	{
		
		MemoryStream stream = new MemoryStream();
		
		StreamWriter writer = new StreamWriter(stream);

		writer.WriteLine(JsonConvert.SerializeObject(m_warehouseData.m_version));
		writer.WriteLine(JsonConvert.SerializeObject(InvenSize));
		
		foreach(KeyValuePair<ItemData.Type, List<ItemObject>> pair in m_warehouseData.m_items)
		{
			foreach(ItemObject itemObj in pair.Value)
			{
				writer.WriteLine(JsonConvert.SerializeObject(itemObj.Item.RefItem.type));
				//Debug.Log(JsonConvert.SerializeObject(itemObj.Item));
				writer.WriteLine(JsonConvert.SerializeObject(itemObj.Item));
			}
		}

		writer.Write(JsonConvert.SerializeObject(m_warehouseData));
		
		writer.Close();
		
		return stream.ToArray();
	}

	public void Deserialize(byte[] data)
	{

		MemoryStream stream = new MemoryStream(data);
		
		StreamReader reader = new StreamReader(stream);
		
		int version = JsonConvert.DeserializeObject<int>(reader.ReadLine());
		int count = JsonConvert.DeserializeObject<int>(reader.ReadLine());

		Dictionary<ItemData.Type, List<ItemObject>>	items = new Dictionary<ItemData.Type, List<ItemObject>>();
		initInven(items);

		for(int i = 0; i < count; ++i)
		{
			ItemData.Type type = JsonConvert.DeserializeObject<ItemData.Type>(reader.ReadLine());
			
			switch(type)
			{
			case ItemData.Type.Gold:
				ItemGoldData goldData = JsonConvert.DeserializeObject<ItemGoldData>(reader.ReadLine());
				items[type].Add(new ItemObject(goldData));
				break;
			case ItemData.Type.GoldMedal:
				ItemGoldMedalData goldMedalData = JsonConvert.DeserializeObject<ItemGoldMedalData>(reader.ReadLine());
				items[type].Add(new ItemObject(goldMedalData));
				break;
			case ItemData.Type.Gem:
				ItemGemData gemData = JsonConvert.DeserializeObject<ItemGemData>(reader.ReadLine());
				items[type].Add(new ItemObject(gemData));
				break;
			case ItemData.Type.Weapon:
				ItemWeaponData weaponData = JsonConvert.DeserializeObject<ItemWeaponData>(reader.ReadLine());
				items[type].Add(new ItemObject(weaponData));
				break;
			case ItemData.Type.WeaponDNA:
				ItemWeaponDNAData weaponDNAData = JsonConvert.DeserializeObject<ItemWeaponDNAData>(reader.ReadLine());
				items[type].Add(new ItemObject(weaponDNAData));
				break;
			case ItemData.Type.WeaponParts:
				ItemWeaponPartsData weaponPartsData = JsonConvert.DeserializeObject<ItemWeaponPartsData>(reader.ReadLine());
				items[type].Add(new ItemObject(weaponPartsData));
				break;
			case ItemData.Type.Follower:
				ItemFollowerData followerData = JsonConvert.DeserializeObject<ItemFollowerData>(reader.ReadLine());
				items[type].Add(new ItemObject(followerData));
				break;
			case ItemData.Type.Accessory:
				ItemAccessoryData accessoryData = JsonConvert.DeserializeObject<ItemAccessoryData>(reader.ReadLine());
				items[type].Add(new ItemObject(accessoryData));
				break;
			case ItemData.Type.Skill:
				ItemSkillData skillData = JsonConvert.DeserializeObject<ItemSkillData>(reader.ReadLine());
				items[type].Add(new ItemObject(skillData));
				break;
			case ItemData.Type.Cheat:
				ItemCheatData cheatData = JsonConvert.DeserializeObject<ItemCheatData>(reader.ReadLine());
				items[type].Add(new ItemObject(cheatData));
				break;
			case ItemData.Type.Stat:
				ItemStatData statData = JsonConvert.DeserializeObject<ItemStatData>(reader.ReadLine());
				items[type].Add(new ItemObject(statData));
				break;
				
			default:
				Debug.Log(type);
				reader.ReadLine();
				break;
			}
		}

		m_warehouseData = JsonConvert.DeserializeObject<WarehouseData>(reader.ReadToEnd());
		m_warehouseData.m_version = version;
		m_warehouseData.m_items = items;

		m_gold = FindItem(1);
		m_goldMedal = FindItem(5);
		m_gem	= FindItem(8);

		reader.Close();
	}

	public int NeedTotalGem
	{
		get{
			int totalGem = 0;
			foreach(KeyValuePair<ItemData.Type, List<ItemObject>> pair in Warehouse.Instance.Items)
			{
				foreach(ItemObject obj in pair.Value)
				{			
					RefPriceCondition condUnlock = obj.Item.RefItem.unlock;
					if (condUnlock != null)
					{
						foreach(RefPrice refPrice in condUnlock.else_conds)
						{
							if (refPrice.refItemId == Const.GemRefItemId)
							{
								totalGem += refPrice.count;
								break;
							}
						}
					}

					RefPriceCondition condLevel = obj.Item.RefItem.levelup;
					if (condLevel != null)
					{
						foreach(RefPrice refPrice in condLevel.else_conds)
						{
							if (refPrice.refItemId == Const.GemRefItemId)
							{
								for(int lv = obj.Item.Level; lv < obj.Item.RefItem.maxLevel; ++lv)
								{
									totalGem += (int)(refPrice.count*Const.GetItemLevelupWorth(lv, condLevel));
								}
								break;
							}
						}
					}
				}
			}
			return totalGem-Gem.Item.Count;
		}
	}
}
