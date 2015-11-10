using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


public class RefBaseData
{
	public int 				id;
}

[System.Serializable]
public class RefCreatureBaseProperty
{
	public int	maxHP;
	public int	physicalDamage;
	public int	maxSP;
	public float spRegen;

	[Range (0, 100)]
	public int	physicalDefence;
	public float	evolutionPerLevel;
	public int		exp;
	public float 	expPerLevel;
	[Range (0, 1)]
	public float 	phyDamagePerLevel;
	[Range (0, 1)]
	public float 	phyDefencePerLevel;
	[Range (0, 1)]
	public float 	hpPerLevel;
	[Range (0, 10)]
	public float	moveSpeed;
	public float	aniSpeedRatio = 1f;
	public float	rotationSpeedRatio = 1000f;
	[Range (0, 1)]
	public float 	spPerLevel;
	public float	navMeshBaseOffset;
	public float	lifeSteal;
	public float	criticalChance;
	public float	criticalDamage = 1f;
	public float	attackCoolTime = 1f;
	public bool		backwardOnDamage = true;
	public float	bulletLength = 0f;
}

public class RefEvolutionFiring
{
	public float			angle;
	public float			delay;
}

public class WeaponStat
{
	public struct BuffOnHitDesc
	{
		public float chance;
		[JsonConverter(typeof(StringEnumConverter))]
		public DamageDesc.BuffType buffType;
	}
	public float			coolTime;
	public float			range;
	public int				firingCount;
	public int				incBulletOnLevel = 2;
	public int				skillId;
	public int				summonRefMobId;
	public BuffOnHitDesc	buffOnHitDesc;

	public void OverrideStat(WeaponStat weaponStat)
	{
		if (coolTime == 0)
			coolTime = weaponStat.coolTime;

		if (range == 0)
			range = weaponStat.range;

		if (firingCount == 0)
			firingCount = weaponStat.firingCount;



		if (skillId == 0)
			skillId = weaponStat.skillId;

		if (summonRefMobId == 0)
			summonRefMobId = weaponStat.summonRefMobId;

		if (buffOnHitDesc.chance == 0 && buffOnHitDesc.buffType == DamageDesc.BuffType.Nothing)
			buffOnHitDesc = weaponStat.buffOnHitDesc;

		if (incBulletOnLevel == 2)
			incBulletOnLevel = weaponStat.incBulletOnLevel;

	}
}

public class RefPrice
{
	public int				refItemId;
	public int				count;
}


public class RefPriceCondition
{

	public class RefOptionPerLevel
	{
		public int level;
		public RefItemOption option;
		public bool levelPer = false;
	}
	public RefPrice[]		conds;
	public RefPrice[]		else_conds;	
	public float			pricePerLevel = 1f;
	public RefOptionPerLevel[] optionPerLevel;
}

public class RefItemOption
{
	[JsonConverter(typeof(StringEnumConverter))]
	public ItemData.Option  type;

	public float[] 			values;
}

public class RefItem : RefBaseData
{
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	public 	ItemData.Type 		type;
	public 	string 				codeName;
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	public 	MultiLang.ID		name;
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	public  MultiLang.ID		desc;
	public	string				icon;
	public 	string				partName;
	public	RefPriceCondition	levelup;
	public	RefPriceCondition	evolution;
	public 	RefPriceCondition	unlock;
	public 	RefEvolutionFiring	evolutionFiring;
	public  WeaponStat			weaponStat;
	public 	RefItemOption[]		options;
	public 	int					weaponId;
	public 	int					followerId;
	public 	bool				defaultLock = false;
	public  int					maxLevel = Const.MaxItemLevel;
	public  int					maxEvolution = Const.MaxEvolutionLevel;
	public 	float				consumedSP;
}

public class RefItemOptionSpawn
{	
	[JsonConverter(typeof(StringEnumConverter))]
	public ItemData.Option  type;
	public int 				minValue;
	public int 				maxValue;
	[Range(0F, 1F)]
	public float			ratio;
	
}

public class RefItemSpawn
{
	public int					refItemId;
	public int 					minValue;
	public int 					maxValue;
	[Range(0F, 1F)]
	public float				ratio;
	public int					count = 1;
	public RefItemOptionSpawn[] itemOptionSpawns;

	[JsonIgnore]
	public RefItem				refItem;
}

public class RefEggMob
{
	public int					reqLevel = 1;
	public int					refMobId;
	public int					perCount = 1;
	public float				perTime = 0f;
	public int					maxCount;

	[JsonIgnore]
	public RefMob				refMob;
}

public enum MobAIType
{
	Normal,
	Rotation,
	FlyingAround,
	ItemShuttle,
	Dummy,
	Bomber,
	Egg,
	Pickup,
	Follow,
}

public enum MobSpawnEffectType
{
	Normal,
	Falling,
}


public class RefMob : RefBaseData
{
	public class WeaponDesc
	{
		public int reqLevel = 0;
		public int refItemId;
		public int level = 1;
		public int maxLevel;
		public int evolution = 0;
		public bool passive = false;
		public WeaponStat weaponStat;
	}
	public string				prefHead = "mob";
	public string				prefBody;
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	public 	MultiLang.ID		name;
	public WeaponDesc[]			refWeaponItems;
	public bool					nearByChampOnSpawn;
	public RefEggMob			eggMob;
	public RefCreatureBaseProperty		baseCreatureProperty;

	[JsonConverter(typeof(StringEnumConverter))]
	public MobAIType			mobAI;
	public float				scale = 1f;
	public float				mass = 1f;
	public int					consumedSP;
}

public class RefMobClass
{
	public RefMob[]				melee;
	public RefMob[]				range;	
	public RefMob[]				miniBoss;
	public RefMob[]				boss;
	public RefMob[]				shuttle;
	public RefMob[]				egg;
	public RefMob[]				itemPandora;
	public RefMob[]				itemDummy;
	public RefMob[]				follower;
	public RefMob[]				champ;
	public RefMob[]				skilled;
}

public class RefMobSpawnRatio
{
	public class Desc
	{
		public float[]	ratio;
		public int[]	count;
		public float	chance = 1f;
		[JsonConverter(typeof(StringEnumConverter))]
		public MobSpawnEffectType spawnEffectType = MobSpawnEffectType.Normal;
	}
	public Desc	melee;
	public Desc	range;
	public Desc	miniBoss;
	public Desc	boss;
	public Desc	itemPandora;
	public Desc	itemDummy;
	public Desc	skilled;
}

public class RefMobSpawn
{
	public RefMobSpawnRatio		refMobIds;	
	public float 			interval;
	public int[]			repeatCount;
	public int[]			mobCount;

	public bool				boss = false;

}

public class RefMobItemSpawn
{
	public int	refMobId;
	public RefItemSpawn[]	refDropItems;
}

public class RefWaveItemSpawn
{
	public RefMobItemSpawn[] mobItems;
	public RefItemSpawn[]	defaultItem;
	public RefItemSpawn[]	bossDefaultItem;

	[JsonIgnore]
	public Dictionary<int, RefMobItemSpawn>	 mapMobItems = new Dictionary<int, RefMobItemSpawn>();
}

public class RefWave
{
	public RefMobSpawn[]	mobSpawns;
	public RefMobSpawn[]	randomMobSpawns;
	public RefMobSpawn[]	randomSkillItemSpawns;
	public RefWaveItemSpawn	itemSpawn;
}

public class RefWorldMap : RefBaseData
{
	public string			name;
	public RefWave[]		waves;

}

public class RefData {


	Dictionary<int, RefWorldMap>	m_refWorldMaps = new Dictionary<int, RefWorldMap>();
	RefMobClass		m_refMobClass = new RefMobClass();
	Dictionary<int, RefItemSpawn>	m_refItemSpawns = new Dictionary<int, RefItemSpawn>();
	Dictionary<int, RefItem>		m_refItems = new Dictionary<int, RefItem>();
	Dictionary<int, RefMob>			m_refMobs = new Dictionary<int, RefMob>();
	Dictionary<int, RefMob>			m_refChamps = new Dictionary<int, RefMob>();
	string[]		m_texts	= null;

	static RefData m_ins = null;
	static public RefData Instance
	{
		get {
		
			if (m_ins == null)
			{
				m_ins = new RefData();
				m_ins.Load();
			}
			
			return m_ins;
		}

	}

	void Load()
	{
		if (Application.systemLanguage == SystemLanguage.Korean)
			Deserialize(ref m_texts, "ko/MultiLang");
		else
			Deserialize(ref m_texts, "en/MultiLang");

		DeserializeArray(m_refWorldMaps, "RefWorldMap");
		Deserialize(ref m_refMobClass, "RefMob");
		DeserializeArray(m_refItems, "RefItem");

		foreach(KeyValuePair<int, RefWorldMap> pair in m_refWorldMaps)
		{
			foreach(RefWave wave in pair.Value.waves)
			{

				if (wave.itemSpawn != null)
				{
					foreach(RefItemSpawn itemSpawn in wave.itemSpawn.defaultItem)
					{
						itemSpawn.refItem = m_refItems[itemSpawn.refItemId];
					}

					foreach(RefItemSpawn itemSpawn in wave.itemSpawn.bossDefaultItem)
					{
						itemSpawn.refItem = m_refItems[itemSpawn.refItemId];
					}

					foreach(RefMobItemSpawn mobItemSpawn in wave.itemSpawn.mobItems)
					{
						foreach(RefItemSpawn itemSpawn in mobItemSpawn.refDropItems)
						{
							itemSpawn.refItem = m_refItems[itemSpawn.refItemId];
						}

						wave.itemSpawn.mapMobItems[mobItemSpawn.refMobId] = mobItemSpawn;
					}
				}
			}
		}

		List<RefMob[]> mobs= new List<RefMob[]>(){	m_refMobClass.melee, m_refMobClass.range, m_refMobClass.boss, m_refMobClass.egg, 
													m_refMobClass.shuttle, m_refMobClass.follower, m_refMobClass.miniBoss, m_refMobClass.skilled};

		foreach(RefMob[] refMobs in mobs)
		{
			foreach(RefMob refMob in refMobs)
			{
				if (true == m_refMobs.ContainsKey(refMob.id))
				{
					Debug.Log("duplicated mob key:" + refMob.id);
				}
				m_refMobs.Add(refMob.id, refMob);
			}
		}

		foreach(RefMob refMob in m_refMobClass.champ)
		{
			if (true == m_refChamps.ContainsKey(refMob.id))
			{
				Debug.Log("duplicated champ key:" + refMob.id);
			}
			m_refChamps.Add(refMob.id, refMob);
		}

		mobs.Add(m_refMobClass.champ);

		foreach(RefMob[] refMobs in mobs)
		{
			foreach(RefMob refMob in refMobs)
			{		
				if (refMob.eggMob != null)
				{
					refMob.eggMob.refMob = m_refMobs[refMob.eggMob.refMobId];
					if (refMob.eggMob.refMob == null)
					{
						Debug.Log("null refMob.eggMob.refMobId:" + refMob.eggMob.refMobId);
					}				
				}
			}
		}
	}
	
	void DeserializeArray<T>(Dictionary<int, T> records, string fileName) where T : RefBaseData
	{ 
		TextAsset textDocument =  Resources.Load("RefData/" + fileName) as TextAsset;

		T[] datas = JsonConvert.DeserializeObject<T[]>(textDocument.text);				
		foreach(T data in datas)
		{
			records[data.id] = data;
		}

	}

	void Deserialize<T>(ref T records, string fileName)
	{ 
		TextAsset textDocument =  Resources.Load("RefData/" + fileName) as TextAsset;

		records = JsonConvert.DeserializeObject<T>(textDocument.text);			

		
	}

	public Dictionary<int, RefWorldMap> RefWorldMaps
	{
		get {return m_refWorldMaps;}
	}

	public Dictionary<int, RefItem> RefItems
	{
		get {return m_refItems;}
	}

	public Dictionary<int, RefMob> RefMobs
	{
		get {return m_refMobs;}
	}

	public RefMob[] RefMeleeMobs
	{
		get {return m_refMobClass.melee;}
	}

	public RefMob[] RefRangeMobs
	{
		get {return m_refMobClass.range;}
	}

	public RefMob[] RefBossMobs
	{
		get {return m_refMobClass.boss;}
	}

	public RefMob[] RefMiniBossMobs
	{
		get {return m_refMobClass.miniBoss;}
	}

	public RefMob[] RefShuttleMobs
	{
		get {return m_refMobClass.shuttle;}
	}

	public RefMob[] RefItemPandoraMobs
	{
		get {return m_refMobClass.itemPandora;}
	}

	public RefMob[] RefItemDummyMobs
	{
		get {return m_refMobClass.itemDummy;}
	}

	public RefMob[] RefFollowerMobs
	{
		get {return m_refMobClass.follower;}
	}

	public RefMob RefChamp
	{
		get {return m_refChamps[1];}
	}

	public RefMob[] RefSkilledMobs
	{
		get {return m_refMobClass.skilled;}
	}

	public string RefTexts(MultiLang.ID id)
	{
		return m_texts[(int)id];

	}
}
