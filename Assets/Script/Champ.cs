using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Champ : Creature {


	[SerializeField]
	bool	m_enableAutoTarget = true;

	int			m_remainStatPoint = 0;

	SecuredType.XInt			m_mobKills;

	int[]			m_skillStacks	= new int[9];

	ItemObject[]	m_accessoryItems = new ItemObject[Const.AccessoriesSlots];


	Vector3		m_moveDir;
	float		m_lastLevelupTime;

	float		m_autoMoveTime;
	float		m_autoMoveAngTime;

	ADMob					m_admob;
	float		m_idleTime;

	Weapon		m_tapWeapon;

	new void Start () {

		base.Start();

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();

		ApplyGameOptions();

		FollowingCamera followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		followingCamera.SetMainTarget(gameObject);


	
	}

	override public void Init(RefMob refMob, int level)
	{
		base.Init(refMob, level);

		m_lastLevelupTime = Time.time;
	}

	public int RemainStatPoint
	{
		get{return m_remainStatPoint;}
		set{m_remainStatPoint = value;}
	}

	public float LastLevelupTime
	{
		get {return m_lastLevelupTime;}
	}

	void LevelUp()
	{
		m_remainStatPoint+=1*Cheat.HowManyAbilityPointRatioOnLevelUp;

		ApplyBuff(null, DamageDesc.BuffType.Macho, 10f, null);
		m_lastLevelupTime = Time.time;
	}

	override public bool AutoAttack() {
		
		
		if (HasCrowdControl() == false)
		{
			if (Targetting == null)
			{
				SetTarget(SearchTarget(GetMyEnemyType(), null, 50f));
			}
			
			if (Targetting != null)
			{
				if (true == inAttackRange(Targetting, 0f))
				{
					m_weaponHolder.StartFiring(RotateToTarget(Targetting.transform.position));
					return true;
				}
			}
		}
		
		SetTarget(null);
		m_weaponHolder.StopFiring();
		return false;
	}

	public Vector3 MoveDir
	{
		get {return m_moveDir;}
	}

	public int[] SkillStacks
	{
		get {return m_skillStacks;}
		set {m_skillStacks = value;}
	}

	public ItemObject[]	AccessoryItems
	{
		get {return m_accessoryItems;}
	}

	public int GoldLevel
	{
		get{
			return m_creatureProperty.Level + Warehouse.Instance.NewGameStats.WaveIndex;
		}
	}

	public void ApplyGameOptions()
	{

		Const.GetSpawn().audio.ignoreListenerVolume = false;
		Const.GetSpawn().audio.volume = Warehouse.Instance.GameOptions.m_bgmVolume;
		Const.GetSpawn().audio.ignoreListenerVolume = true;

		AudioListener.volume = Warehouse.Instance.GameOptions.m_sfxVolume;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	/*
	void OnGUI()
	{
		UpdateChampMovement();
	}
*/

	override public Weapon EquipWeapon(ItemWeaponData weaponData, WeaponStat weaponStat)
	{
		Weapon weapon = base.EquipWeapon(weaponData, weaponStat);
		if (weaponData.RefItem.partName != null)
			transform.Find("Body/"+weaponData.RefItem.partName).gameObject.SetActive(true);

		if (weapon.RefItem.id == Const.ChampTapRefItemId)
			m_tapWeapon = weapon;

		return weapon;
	}
	// Update is called once per frame
	new void Update () {
		base.Update();

		if (m_enableAutoTarget)
		{
			if (AutoAttack() == false)
			{
				m_weaponHolder.StopFiring();
			}
		}


		int touchedCount = 0;
		Vector3[] touchPos = new Vector3[5];
#if UNITY_EDITOR
		touchedCount = Input.GetMouseButtonUp(0) == true ? 1 : 0;
		if (touchedCount > 0)
			touchPos[0] = Input.mousePosition;

#else
		touchedCount = Input.touchCount;
		if (touchedCount > 0)
		{
			int aa = 0;
			for(int i = 0; i < touchedCount; ++i)
			{
				if (Input.GetTouch (i).phase == TouchPhase.Began)
				{
					touchPos[aa] = Input.GetTouch(i).position;
					++aa;
				}
			}

			touchedCount = aa;
		}
#endif
		bool hitted = false;
		for (int i = 0; i < touchedCount; ++i) 
		{

			StartCoroutine(EffectTouch(Camera.main.ScreenToWorldPoint(touchPos[i])));

			Ray ray = Camera.main.ScreenPointToRay( touchPos[i] );
			RaycastHit[] hits;
			hits = Physics.RaycastAll(ray, Mathf.Infinity, 1<<9 | 1<<10 | 1<<11);
			foreach(RaycastHit hit in hits )
			{
				Creature target = hit.transform.GetComponent<Creature>();
				if (null == WeaponHolder.MainWeapon && target != null)
					RotateToTarget(target.transform.position);

				if (target != null && IsEnemy(target, this))
				{
					target.TakeDamage(this, new DamageDesc(m_tapWeapon.GetDamage(m_creatureProperty), DamageDesc.Type.Normal, DamageDesc.BuffType.Nothing, null));
					hitted = true;
				}
				else if (hit.transform.tag.CompareTo("ItemBox") == 0)
				{
					ItemBox itemBox = hit.transform.gameObject.GetComponent<ItemBox>();
					itemBox.StartPickupEffect(this);
					hitted = true;
				}
			}
		}

		if (hitted == false)
		{
			Creature[] targets = Bullet.SearchTarget(transform.position, GetMyEnemyType(), 4f+m_creatureProperty.AttackRange);
			int length = 0;
			if (targets != null)
				length = Mathf.Min(touchedCount, targets.Length);
			for(int i = 0; i < length; ++i)
			{
				targets[i].TakeDamage(this, new DamageDesc(m_tapWeapon.GetDamage(m_creatureProperty), DamageDesc.Type.Normal, DamageDesc.BuffType.Nothing, null));
			}
		}

		Warehouse.Instance.GameDataContext.m_level.Value = m_creatureProperty.Level;
		Warehouse.Instance.GameDataContext.m_hp.Value = m_creatureProperty.HP;
		Warehouse.Instance.GameDataContext.m_xp.Value = m_creatureProperty.Exp;
		Warehouse.Instance.GameDataContext.m_sp.Value = m_creatureProperty.SP;

		if (touchedCount > 0)
		{
			m_idleTime = 0f;
			m_admob.ShowBanner(false);

			if (Warehouse.Instance.GameTutorial.m_unlockedTap == false)
			{
				Const.GetTutorialMgr().SetTutorial("Nothing");				
				Warehouse.Instance.GameTutorial.m_unlockedTap = true;
			}
		}
		else
		{
			m_idleTime += Time.deltaTime;
			if (m_idleTime > 180f)
			{
				m_admob.ShowBanner(true);
				m_idleTime = 0f;
			}
		}

		TimeEffector.Instance.Update();
	}

	override public void ConsumeSP(int sp)
	{
		base.ConsumeSP(sp);

		Warehouse.Instance.NewGameStats.ConsumedSP += sp;
	}

	override public void GiveExp(int exp)
	{
		m_creatureProperty.giveExp(exp);
	}

	IEnumerator EffectTouch(Vector3 pos)
	{
		GameObject ef = Resources.Load("Pref/ef_touch") as GameObject;
		pos.z = transform.position.z-2;
		pos.y -= 1.5f;
		GameObject obj = GameObjectPool.Instance.Alloc(ef, pos, ef.transform.rotation);
		while(obj.particleSystem.isPlaying)
		{
			yield return null;
		}
		GameObjectPool.Instance.Free(obj);

	}

	override public void Death()
	{
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Death", "Wave"+Warehouse.Instance.NewGameStats.WaveIndex, 0);

		m_creatureProperty.Level = 1;
		m_creatureProperty.Exp = 0;
		Warehouse.Instance.GameDataContext.m_level.Value = m_creatureProperty.Level;
		Warehouse.Instance.GameDataContext.m_hp.Value = m_creatureProperty.MaxHP;
		Warehouse.Instance.GameDataContext.m_xp.Value = m_creatureProperty.Exp;
		Warehouse.Instance.GameDataContext.m_sp.Value = m_creatureProperty.SP;

		base.Death();

		Const.GetSpawn().StartCoroutine(ShowGameOverGUI());
	}

	IEnumerator	ShowGameOverGUI()
	{
		yield return new WaitForSeconds(3f);
		GameObject.Find("HudGUI/GameOverGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	override public bool ApplyBuff(Creature offender, DamageDesc.BuffType type, float time, DamageDesc damageDesc)
	{
		if (false == base.ApplyBuff(offender, type, time, damageDesc))
			return false;

		switch(type)
		{		
		case DamageDesc.BuffType.LevelUp:
			DamageText(type.ToString(), Vector3.one, Color.cyan, DamageNumberSprite.MovementType.RisingUp);
			break;
		case DamageDesc.BuffType.Macho:
			DamageText(RefData.Instance.RefTexts(MultiLang.ID.MachoSkill), Vector3.one, Color.cyan, DamageNumberSprite.MovementType.RisingUp);
			break;		
		}
		return true;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.CompareTo("ItemBox") == 0)
		{
			if (3f > Vector3.Distance(transform.position, other.transform.position))
			{
				ItemBox itemBox = other.gameObject.GetComponent<ItemBox>();
				itemBox.StartPickupEffect(this);
			}
		};

	}
}
