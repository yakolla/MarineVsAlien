using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;

public class Spawn : MonoBehaviour {



	Champ		m_champ = null;


	[SerializeField]
	GameObject		m_prefSpawnEffect = null;
	
	GameObject[]	m_prefItemBoxSkins = new GameObject[(int)ItemData.Type.Count];
	GameObject		m_prefItemBox;

	[SerializeField]
	AudioClip	m_normalBg;
	
	[SerializeField]
	AudioClip	m_bossBg;

	Transform[]	m_goalPoints;
	int			m_goalPointIndex;

	FollowingCamera	m_followingCamera = null;

	int				m_mobsOfCheckOnDeath = 0;

	RefWorldMap		m_refWorldMap;
	Dungeon			m_dungeon;
	int				m_hive = 0;

	DropShip		m_dropShip;

	BoxCollider		m_edgeRect;

	YGUISystem.GUILable	m_stageText;
	YGUISystem.GUILable	m_waveText;

	float			m_dropBuffItemTime;


	[SerializeField]
	int				m_wave = 0;
	int				m_relWave = 0;
	// Use this for initialization
	void Awake () {

		m_dropShip = transform.parent.Find("dropship").GetComponent<DropShip>();
		m_edgeRect = transform.Find("EdgeRect").GetComponent<BoxCollider>();

		m_followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		m_followingCamera.SetMainTarget(m_dropShip.gameObject);

		m_dungeon = transform.parent.GetComponent<Dungeon>();
		int dungeonId = m_dungeon.DungeonId;
		m_refWorldMap = RefData.Instance.RefWorldMaps[dungeonId];

		m_prefItemBox = Resources.Load<GameObject>("Pref/ItemBox/ItemBox");
		string[] itemTypeNames = Enum.GetNames(typeof(ItemData.Type));
		for(int i = 0; i < itemTypeNames.Length-1; ++i)
		{
			m_prefItemBoxSkins[i] = Resources.Load<GameObject>("Pref/ItemBox/item_" + itemTypeNames[i] + "_skin");
			if (m_prefItemBoxSkins[i] == null)
				Debug.Log("Pref/ItemBox/item_" + itemTypeNames[i] + "_skin");
		}

		m_stageText = new YGUISystem.GUILable(GameObject.Find("HudGUI/StatusGUI/Stage").gameObject);
		m_waveText = new YGUISystem.GUILable(GameObject.Find("HudGUI/StatusGUI/Wave").gameObject);

		Transform[] goals = transform.Find("GoalPoint").transform.GetComponentsInChildren<Transform>();
		m_goalPoints = new Transform[goals.Length-1];
		for(int i = 0; i < goals.Length-1; ++i)
			m_goalPoints[i] = goals[i+1];

	}

	IEnumerator EffectWaveText(string msg, float alpha)
	{
		m_stageText.Text.text = msg;
		Color color = m_stageText.Text.color;
		color.a = alpha;
		m_stageText.Text.color = color;
		yield return new WaitForSeconds (0.2f);

		if (alpha > 0)
		{
			StartCoroutine(EffectWaveText(msg, alpha-0.1f));
		}

	}

	public RefWave	GetCurrentWave()
	{
		return m_refWorldMap.waves[m_hive];
	}

	public void StartWave(int wave, Champ champ)
	{
		GPlusPlatform.Instance.AnalyticsTrackScreen("StartWave");

		if (m_wave == 0)
			m_wave = wave;
		
		Warehouse.Instance.ResetNewGameStats();

		m_dropBuffItemTime = Time.time+90f;
		m_dropShip.SetChamp(champ);
		m_dropShip.GetComponent<Animator>().SetTrigger("Move");
		StartCoroutine(spawnMobPer());

#if UNITY_EDITOR
		if (Const.CHEAT_MODE)
		{
			champ.RemainStatPoint = 10;
		}
#endif
	}

	bool checkBossAlive()
	{
		return m_mobsOfCheckOnDeath > 0;
	}

	class SpawnMobDescResult
	{
		public List<RefMob>	spawnMobs = new List<RefMob>();
		public List<bool> 	spawnMobBoss = new List<bool>();
		public List<int> 	spawnMobCount = new List<int>();
		public List<bool> 	spawnMobMonitored = new List<bool>();
		public List<MobSpawnEffectType> spawnEffectType = new List<MobSpawnEffectType>();
	}

	void	buildSpawnMob(SpawnMobDescResult result, RefMobSpawnRatio.Desc spawnRatioDesc, RefMob[] mobs, bool monitoredDeath, bool boss)
	{
		if (spawnRatioDesc == null)
			return;

		if (spawnRatioDesc.chance < Random.Range(0f, 1f))
			return;

		float progress = ProgressStage();

		int minIndex = 0;
		int maxIndex = Mathf.Min((int)progress, mobs.Length-1);
		//Debug.Log("min:" + minIndex + ", max:" + maxIndex + ", progress:" + progress);
		minIndex = Mathf.Clamp(minIndex, 0, mobs.Length-1);

		int mobCount = (int)(spawnRatioDesc.count[0]);
		int stage = GetStage()-1;
		if (boss == true)
		{
			if (stage < RefData.Instance.RefBossMobs.Length)
			{
				result.spawnMobs.Add(mobs[maxIndex]);
			}
			else
			{
				result.spawnMobs.Add(mobs[Random.Range(minIndex, maxIndex+1)]);
				mobCount += (int)((spawnRatioDesc.count[1]-spawnRatioDesc.count[0]) * Mathf.Min(1, stage/RefData.Instance.RefBossMobs.Length*(1/(float)RefData.Instance.RefBossMobs.Length)));
			}
		}
		else
		{
			result.spawnMobs.Add(mobs[Random.Range(minIndex, maxIndex+1)]);
			mobCount += (int)((spawnRatioDesc.count[1]-spawnRatioDesc.count[0]) * Mathf.Min(1, stage/RefData.Instance.RefBossMobs.Length*(1/(float)RefData.Instance.RefBossMobs.Length)));

		}


		result.spawnMobCount.Add(mobCount);
		result.spawnMobMonitored.Add(monitoredDeath);
		result.spawnMobBoss.Add(boss);
		result.spawnEffectType.Add(spawnRatioDesc.spawnEffectType);
	}

	public int GetStage()
	{
		return m_wave/GetCurrentWave().mobSpawns.Length + 1;
	}

	IEnumerator spawnMobPerCore(RefMobSpawn mobSpawn)
	{
		float stage = Mathf.Min(SpawnMobLevel()-1, 100)*0.01f;
		int mobSpawnRepeatCount = (int)(mobSpawn.repeatCount[0] * (1f-stage) + mobSpawn.repeatCount[1] * stage);
		for(int r = 0; r < mobSpawnRepeatCount; ++r)
		{
			yield return new WaitForSeconds (mobSpawn.interval);
			
			if (m_champ == null)
			{
				break;
			}
			
			SpawnMobDescResult spawnMobDescResult = new SpawnMobDescResult();
			
			buildSpawnMob(spawnMobDescResult, mobSpawn.refMobIds.melee, RefData.Instance.RefMeleeMobs, true, false);
			buildSpawnMob(spawnMobDescResult, mobSpawn.refMobIds.range, RefData.Instance.RefRangeMobs, true, false);
			buildSpawnMob(spawnMobDescResult, mobSpawn.refMobIds.boss, RefData.Instance.RefBossMobs, true, true);
			buildSpawnMob(spawnMobDescResult, mobSpawn.refMobIds.itemPandora, RefData.Instance.RefItemPandoraMobs, false, false);
			buildSpawnMob(spawnMobDescResult, mobSpawn.refMobIds.itemDummy, RefData.Instance.RefItemDummyMobs, false, false);
			buildSpawnMob(spawnMobDescResult, mobSpawn.refMobIds.miniBoss, RefData.Instance.RefMiniBossMobs, true, false);
			buildSpawnMob(spawnMobDescResult, mobSpawn.refMobIds.skilled, RefData.Instance.RefSkilledMobs, false, false);
			

			Vector3 cp = m_champ.transform.position;

			for(int ii = 0;  ii < spawnMobDescResult.spawnMobs.Count; ++ii)
			{
				for(int i = 0; i < spawnMobDescResult.spawnMobCount[ii]; ++i)
				{
					
					RefMob refMob = spawnMobDescResult.spawnMobs[ii];
					Vector3 enemyPos = cp;

					float[] angles = {0f, 3.14f};
					float angle = 0f;
					float length = 8f;
					switch(m_goalPointIndex)
					{
					case 0:
						angle = 0f;
						length = 16;
						if (m_champ != null)
							m_champ.m_creatureProperty.AttackRange=2;
						break;
					case 5:
						angle = 3.14f;
						length = 16;
						if (m_champ != null)
							m_champ.m_creatureProperty.AttackRange=2;
						break;
					default:
						angle = angles[Random.Range(0, angles.Length)];
						if (m_champ != null)
							m_champ.m_creatureProperty.AttackRange=0;
						break;
					}							

					if (refMob.baseCreatureProperty.moveSpeed > 0f && refMob.nearByChampOnSpawn == false)
					{
						enemyPos.x += Mathf.Cos(angle) * length;
						enemyPos.z += Mathf.Sin(angle) * length;							
					}
					else
					{
						enemyPos.x += Mathf.Cos(angle) * 4f;
						enemyPos.z += Mathf.Sin(angle) * 4f;							
					}
					
					yield return new WaitForSeconds (0.1f);
					
					Creature cre = SpawnMob(refMob, SpawnMobLevel(), enemyPos, spawnMobDescResult.spawnMobBoss[ii], spawnMobDescResult.spawnMobMonitored[ii]);
					cre.gameObject.SetActive(false);
					
					switch(spawnMobDescResult.spawnEffectType[ii])
					{
					case MobSpawnEffectType.Falling:
						StartCoroutine(  EffectSpawnMob1(cre.transform.position, cre) );
						break;
					default:
						StartCoroutine(  EffectSpawnMob(cre.transform.position, cre) );
						break;
					}
					
					
				}
			}
		}
	}

	IEnumerator spawnMobPer()
	{

		while(true)
		{
			if (m_champ == null)
			{
				yield return null;
			}
			else
			{
				
				RefMobSpawn mobSpawn = GetCurrentWave().mobSpawns[m_wave%GetCurrentWave().mobSpawns.Length];



				Warehouse.Instance.WaveIndex = m_wave;
				m_waveText.Text.text = "Wave " + (m_wave+1);

				Warehouse.Instance.GameBestStats.SetBestStats(Warehouse.Instance.NewGameStats);	

				yield return StartCoroutine(MoveChamp());

				if (m_wave == 0 && Warehouse.Instance.GameTutorial.m_unlockedTap == false)
				{
					Const.GetTutorialMgr().SetTutorial("Tap");
				}

				if (mobSpawn.boss == true)
				{
					StartCoroutine(EffectWaveText("Boss", 3));
					m_champ.ShakeCamera(3f);
					audio.clip = m_bossBg;
					audio.Play();
				}
				else
				{
					if (audio.clip != m_normalBg)
					{
						audio.clip = m_normalBg;
						audio.Play();
					}
				}

				if (m_dropBuffItemTime < Time.time)
				{
					m_dropBuffItemTime = Time.time+Random.Range(90f, 150f);
					yield return StartCoroutine(spawnMobPerCore(GetCurrentWave().randomSkillItemSpawns[m_wave%GetCurrentWave().randomSkillItemSpawns.Length]));
				}

				yield return StartCoroutine(spawnMobPerCore(mobSpawn));

				while(checkBossAlive())
				{
					yield return new WaitForSeconds(0.5f);
				}

				if (mobSpawn.boss == false)
				{									
					yield return StartCoroutine(spawnMobPerCore(GetCurrentWave().randomMobSpawns[m_wave%GetCurrentWave().randomMobSpawns.Length]));
				}

				yield return new WaitForSeconds(3f);

				if (Warehouse.Instance.GameTutorial.m_unlockedWeaponTab == false)
				{
					Warehouse.Instance.GameTutorial.m_unlockedWeaponTab = true;
					Const.GetTutorialMgr().SetTutorial("WeaponTab");
				}
				else if (Warehouse.Instance.GameTutorial.m_unlockedSkillTab == false)
				{
					Warehouse.Instance.GameTutorial.m_unlockedSkillTab = true;
					Const.GetTutorialMgr().SetTutorial("SkillTab");
				}
				else if (Warehouse.Instance.GameTutorial.m_unlockedFollowerTab == false)
				{
					Warehouse.Instance.GameTutorial.m_unlockedFollowerTab = true;
					Const.GetTutorialMgr().SetTutorial("FollowerTab");
				}
				else if (Warehouse.Instance.GameTutorial.m_unlockedStatTab == false)
				{
					Warehouse.Instance.GameTutorial.m_unlockedStatTab = true;
					Const.GetTutorialMgr().SetTutorial("StatTab");
				}

				yield return new WaitForSeconds(mobSpawn.interval);

				m_wave++;

				if (mobSpawn.boss == true)
				{
					Const.SaveGame((SavedGameRequestStatus, ISavedGameMetadata)=>{});					
				}
			}
		}
	}

	IEnumerator MoveChamp()
	{
		if (m_champ != null)
		{
			NavMeshAgent nav = m_champ.GetComponent<NavMeshAgent>();
			if ((m_relWave/m_goalPoints.Length)%2==0)
				m_goalPointIndex = m_relWave%m_goalPoints.Length;
			else
				m_goalPointIndex = m_goalPoints.Length-m_relWave%m_goalPoints.Length-1;

			nav.SetDestination(m_goalPoints[m_goalPointIndex].transform.position);
			m_champ.RotateToTarget(m_goalPoints[m_goalPointIndex].transform.position);

			while(nav.pathPending || nav.pathStatus != NavMeshPathStatus.PathComplete || nav.remainingDistance > 0)
			{
				yield return null;
			}

			if (m_goalPointIndex == 0)
				m_champ.RotateToTarget(0f);
			else if (m_goalPointIndex == m_goalPoints.Length-1)
				m_champ.RotateToTarget(180f);
		}
		++m_relWave;
		yield return null;
	}

	public int SpawnMobLevel()
	{
		return (int)(1 + ProgressStage());
	}

	public float ProgressStage()
	{
		return (float)(m_wave)/GetCurrentWave().mobSpawns.Length;
	}

	public void OnKillMob(Mob mob)
	{
		SpawnItemBox(mob.RefDropItems, mob.transform.position);

		if (mob.Boss)
		{
			if (Warehouse.Instance.GameTutorial.m_unlockedFollowerTab == true)
			{
				ItemObject petObj = Warehouse.Instance.FindItem(Const.FollowerPetRefItemId);
				if (petObj.Item.Lock == true)
				{
					petObj.Item.Lock = false;
					petObj.Item.Level = 1;
					petObj.Item.Equip(m_champ);

					Const.GetWindowGui(Const.WindowGUIType.FoundItemGUI).GetComponent<FoundItemGUI>().SetItemObj(petObj);
					Const.GetWindowGui(Const.WindowGUIType.FoundItemGUI).SetActive(true);
				}

			}

			SpawnItemBox(GetCurrentWave().itemSpawn.bossDefaultItem, mob.transform.position);
			TimeEffector.Instance.BulletTime(0.005f);

		}

		if (true == mob.CheckOnDeath)
		{
			m_mobsOfCheckOnDeath--;
		}

		if (m_champ)
		{
			++Warehouse.Instance.AlienEssence.Item.Count;
			++Warehouse.Instance.UpdateGameStats.KilledMobs;
		}
	}

	IEnumerator SpawnEffectDestroy(GameObject obj, float delay)
	{		
		yield return new WaitForSeconds (delay);
		
		GameObjectPool.Instance.Free(obj);		
	}


	public IEnumerator EffectSpawnMob(Vector3 pos, Creature creature)
	{		

		Vector3 enemyPos = pos;


		if (m_prefSpawnEffect != null)
		{
			enemyPos.y = m_prefSpawnEffect.transform.position.y;

			GameObject spawnEffect = GameObjectPool.Instance.Alloc(m_prefSpawnEffect, enemyPos, m_prefSpawnEffect.transform.rotation) as GameObject;
			ParticleSystem particle = spawnEffect.GetComponentInChildren<ParticleSystem>();
			
			StartCoroutine(SpawnEffectDestroy(spawnEffect, particle.duration));
		}

		yield return new WaitForSeconds (1f);

		if (creature != null)
			creature.gameObject.SetActive(true);


	}

	IEnumerator EffectSpawnMob1(Vector3 pos, Creature creature)
	{	
		/*
		Creature.Type oriType = creature.CreatureType;
		creature.CreatureType = Creature.Type.Npc;

		pos.y = Random.Range(10,15);
		creature.transform.position = pos;

		Parabola parabola = new Parabola(creature.gameObject, 10f, 0f, 90*Mathf.Deg2Rad, 20f, 2);
		creature.gameObject.SetActive(true);

		while(parabola.Update())
		{
			yield return null;
		}

		creature.CreatureType = oriType;
		*/

		pos.y = Random.Range(10,15);
		creature.transform.position = pos;
		creature.gameObject.SetActive(true);
		while(creature != null && creature.rigidbody.IsSleeping() == false)
		{
			yield return null;
		}

		if (creature != null)
		{
			BoxCollider boxCollider = creature.GetComponent<BoxCollider>();
			Vector3 size = boxCollider.size;
			size.y = 5;
			creature.rigidbody.useGravity = false;
			boxCollider.size = size;
			boxCollider.isTrigger = true;
		}

	}

	IEnumerator EffectSpawnItemBox(ItemBox itemBox, float time)
	{				
		yield return new WaitForSeconds (time);
		itemBox.gameObject.SetActive(true);
		
	}
	
	public Mob SpawnMob(RefMob refMob, int mobLevel, Vector3 pos, bool boss, bool monitoredDeath)
	{
		RefItemSpawn[] refDropItems = null;
		if (GetCurrentWave().itemSpawn.mapMobItems.ContainsKey(refMob.id))
		{
			refDropItems = GetCurrentWave().itemSpawn.mapMobItems[refMob.id].refDropItems;
		}
		else
		{
			refDropItems = GetCurrentWave().itemSpawn.defaultItem;
		}

		GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/"+refMob.prefHead);
		GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/mon_skin/" + refMob.prefBody);
		if (prefEnemyBody == null)
		{
			Debug.Log(refMob.prefBody);
			return null;
		}
		Vector3 enemyPos = pos;

		enemyPos.x = Mathf.Clamp(enemyPos.x, m_edgeRect.transform.position.x-m_edgeRect.size.x/2, m_edgeRect.transform.position.x+m_edgeRect.size.x/2);
		enemyPos.z = Mathf.Clamp(enemyPos.z, m_edgeRect.transform.position.z-m_edgeRect.size.z/2, m_edgeRect.transform.position.z+m_edgeRect.size.z/2);

		GameObject enemyObj = Creature.InstanceCreature(prefEnemy, prefEnemyBody, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;

		enemyObj.transform.localScale = new Vector3(refMob.scale, refMob.scale, refMob.scale);

		Mob enemy = enemyObj.GetComponent<Mob>();
		enemy.Init(refMob, mobLevel, refDropItems, boss);
		enemy.m_creatureProperty.HP = enemy.m_creatureProperty.MaxHP;
		enemy.m_creatureProperty.SP = enemy.m_creatureProperty.MaxSP;

		for(int i = 1; i < mobLevel; ++i)
			enemy.WeaponHolder.LevelUp(0);

		enemy.SetTarget(m_champ);

		Debug.Log(refMob.prefBody + ", Lv : " + mobLevel + ", HP: " + enemy.m_creatureProperty.HP + ", PA:" + enemy.m_creatureProperty.PhysicalAttackDamage + ", PD:" + enemy.m_creatureProperty.PhysicalDefencePoint + ", scale:" + refMob.scale + " pos:" + enemyPos);

	
		if (monitoredDeath == true)
		{
			enemy.CheckOnDeath = true;
			m_mobsOfCheckOnDeath++;
		}

		return enemy;
	}
	
	public void SpawnItemBox(RefItemSpawn[] refDropItems, Vector3 pos)
	{		
		if (refDropItems == null)
		{
			refDropItems = GetCurrentWave().itemSpawn.defaultItem;
		}

		float goldAlpha = m_wave*0.1f;
		int spawnedItemCount = 0;
		foreach(RefItemSpawn desc in refDropItems)
		{
			for(int i = 0; i < desc.count; ++i)
			{
				float ratio = Random.Range(0f, 1f);
				if (desc.refItem.type == ItemData.Type.WeaponParts)
				{
					ratio += Mathf.Min(m_wave, Const.MaxWave)*0.001f;
				}
				else if (desc.refItem.type == ItemData.Type.WeaponDNA)
				{
					if (GetStage() < GetCurrentWave().mobSpawns.Length)
						ratio = 0f;
				}

				if (ratio > 0 && ratio <= desc.ratio)
				{
					float scale = 1f;
					ItemData item = null;
					switch(desc.refItem.type)
					{
					case ItemData.Type.Gold:
						item = new ItemGoldData(1);
						if (m_champ != null)
						{
							item.Count *= m_champ.GoldLevel;
							int extraCount = (int)(item.Count*m_champ.m_creatureProperty.GainExtraGold);
							item.Count += extraCount;
						}

						break;
					case ItemData.Type.HealPosion:
						item = new ItemHealPosionData(Random.Range(desc.minValue, desc.maxValue));
						break;
					case ItemData.Type.Weapon:
						item = new ItemWeaponData(desc.refItem.id);
						break;
					case ItemData.Type.WeaponParts:
						if (m_champ != null && m_champ.WeaponHolder.MainWeapon != null)
						{
							ItemObject hasItem = Warehouse.Instance.FindItem(desc.refItemId);
							if (hasItem == null)
								item = new ItemWeaponPartsData(desc.refItemId);
							else if (hasItem != null && hasItem.Item.Level < hasItem.Item.RefItem.maxLevel)
								item = new ItemWeaponPartsData(desc.refItemId);
						}
						break;					
					case ItemData.Type.WeaponDNA:
						item = new ItemWeaponDNAData(desc.refItem.id);
						break;
					case ItemData.Type.Accessory:
						item = new ItemAccessoryData(desc.refItem.id);					
						break;
					case ItemData.Type.GoldMedal:
						{
							item = new ItemGoldMedalData(Random.Range(desc.minValue, desc.maxValue));
						}
						break;
					case ItemData.Type.Skill:
						item = new ItemSkillData(Random.Range(desc.minValue, desc.maxValue+1));	
						break;
					case ItemData.Type.XPPotion:
						item = new ItemXPPotionData(Random.Range(desc.minValue, desc.maxValue));		
						item.Count += (int)(item.Count*goldAlpha);
						if (m_champ != null)
							item.Count += (int)(item.Count*m_champ.m_creatureProperty.GainExtraExp);
						break;
					case ItemData.Type.MobEgg:
						break;					
					}
					
					if (item != null)
					{

						++spawnedItemCount;

						GameObject itemBoxObj = (GameObject)Instantiate(m_prefItemBox, pos, Quaternion.Euler(0f, 0f, 0f));
						ItemBox itemBox = itemBoxObj.GetComponent<ItemBox>();
						itemBox.Item = item;
						itemBox.PickupCallback = (Creature obj)=>{
							
						};

						GameObject itemSkinObj = (GameObject)Instantiate(m_prefItemBoxSkins[(int)desc.refItem.type], pos, Quaternion.Euler(0f, 0f, 0f));
						if (desc.refItem.type == ItemData.Type.Skill)
						{
							itemSkinObj.transform.Find(item.RefItem.codeName).gameObject.SetActive(true);
							itemBox.LifeTime = 60f;
						}
						else if (desc.refItem.type == ItemData.Type.WeaponParts)
						{
							itemSkinObj.transform.Find(item.RefItem.codeName).gameObject.SetActive(true);
						}

						itemSkinObj.transform.parent = itemBoxObj.transform;
						itemSkinObj.transform.localPosition = Vector3.zero;
						itemSkinObj.transform.localRotation = m_prefItemBoxSkins[(int)desc.refItem.type].transform.rotation;
						itemBoxObj.transform.localScale = Vector3.one * scale;
						itemBoxObj.SetActive(false);

						StartCoroutine(EffectSpawnItemBox(itemBox, 0.15f*spawnedItemCount));
					}
					
				}
			}

		}
		
	}


	public void SharePotinsChamps(Creature cre, ItemData.Type type, int xp, bool enableEffect)
	{

		if (cre.CreatureType != Creature.Type.Champ)
			return;

		switch(type)
		{
		case ItemData.Type.XPPotion:
			m_champ.GiveExp(xp);
			if (enableEffect == true)
				m_champ.ApplyPickUpItemEffect(type, Const.GetPrefItemEatEffect(RefData.Instance.RefItems[6]), xp);
			/*
			foreach(Follower f in m_followers)
			{
				f.GiveExp(xp);
				if (enableEffect == true)
					f.ApplyPickUpItemEffect(type, Const.GetPrefItemEatEffect(RefData.Instance.RefItems[6]), xp);
			}*/
			break;
		case ItemData.Type.HealPosion:
			m_champ.Heal(xp);
			if (enableEffect == true)
				m_champ.ApplyPickUpItemEffect(type, Const.GetPrefItemEatEffect(RefData.Instance.RefItems[2]), xp);
			/*
			foreach(Follower f in m_followers)
			{
				f.Heal(xp);
				if (enableEffect == true)
					f.ApplyPickUpItemEffect(type, Const.GetPrefItemEatEffect(RefData.Instance.RefItems[2]), xp);
			}
			*/
			break;
		case ItemData.Type.Gold:
			Warehouse.Instance.Gold.Item.Count += xp;
			if (enableEffect == true)
				m_champ.ApplyPickUpItemEffect(type, Const.GetPrefItemEatEffect(RefData.Instance.RefItems[1]), xp);
			break;
		}


	}



	// Update is called once per frame
	void Update () {
		if (m_champ == null)
		{
			GameObject obj = GameObject.Find("Champ");
			if (obj != null)
			{
				m_champ = obj.GetComponent<Champ>();
			}
		}

		Warehouse.Instance.UpdateGameStats.Update();

	}

}
