using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Champ : Creature {


	Joypad		m_leftJoypad;
	Joypad		m_rightJoypad;

	[SerializeField]
	bool	m_enableAutoTarget = true;

	int			m_remainStatPoint = 0;

	SecuredType.XInt			m_mobKills;

	int			m_machoSkillStacks = 0;
	int			m_nuclearSkillStacks = 0;

	ItemObject[]	m_accessoryItems = new ItemObject[Const.AccessoriesSlots];

	Animator	m_bloodWarningAnimator;

	Vector3		m_moveDir;
	float		m_lastLevelupTime;

	float		m_autoMoveTime;
	float		m_autoMoveAngTime;

	ADMob					m_admob;
	float		m_idleTime;

	new void Start () {

		base.Start();

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();

		ApplyGameOptions();

		FollowingCamera followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		followingCamera.SetMainTarget(gameObject);

		m_leftJoypad = GameObject.Find("HudGUI/Joypad/LeftJoypad").GetComponent<Joypad>();
		m_rightJoypad = GameObject.Find("HudGUI/Joypad/RightJoypad").GetComponent<Joypad>();

		m_bloodWarningAnimator = GameObject.Find("HudGUI/Blood Warning").GetComponent<Animator>();

	}

	override public void Init(RefMob refMob, int level)
	{
		base.Init(refMob, level);

		m_machoSkillStacks = 0;
		m_remainStatPoint = 0;
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

	void UpdateChampMovement()
	{
		m_moveDir = Vector3.zero;
		if (HasCrowdControl())
			return;

		Vector3 pos = Vector3.zero;
		float step = (1*m_creatureProperty.MoveSpeed)*Time.fixedDeltaTime;



		if (Application.platform == RuntimePlatform.Android)
		{
			if (m_leftJoypad.Dragging)
			{
				pos.x = m_leftJoypad.Position.x*step;
				pos.z = m_leftJoypad.Position.y*step;

				transform.rigidbody.MovePosition(transform.position+pos);
				
				//m_navAgent.SetDestination(transform.position+pos);
			}
			else
			{
				if (Cheat.AutoMove)
				{
					if (m_autoMoveTime+5f < Time.time)
					{
						m_autoMoveTime = Time.time;
						m_leftJoypad.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
					}
					
					pos.x = Mathf.Cos(m_leftJoypad.transform.eulerAngles.z*Mathf.Deg2Rad)*step;
					pos.z = Mathf.Sin(m_leftJoypad.transform.eulerAngles.z*Mathf.Deg2Rad)*step;
					
					transform.rigidbody.MovePosition(transform.position+pos);
				}
			}
		}
		else
		{
			if (m_leftJoypad.Dragging)
			{
				pos.x = m_leftJoypad.Position.x*step;
				pos.z = m_leftJoypad.Position.y*step;

				transform.rigidbody.MovePosition(transform.position+pos);
				//m_navAgent.SetDestination(transform.position+pos);
			}
			else
			{
				if (Cheat.AutoMove)
				{
					if (m_autoMoveTime+5f < Time.time)
					{
						m_autoMoveTime = Time.time;
						m_leftJoypad.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
					}
					
					pos.x = Mathf.Cos(m_leftJoypad.transform.eulerAngles.z*Mathf.Deg2Rad)*step;
					pos.z = Mathf.Sin(m_leftJoypad.transform.eulerAngles.z*Mathf.Deg2Rad)*step;
					
					transform.rigidbody.MovePosition(transform.position+pos);
				}
			}
		}

		m_moveDir = pos.normalized;
	}

	public Vector3 MoveDir
	{
		get {return m_moveDir;}
	}

	public int MachoSkillStack
	{
		get {return m_machoSkillStacks;}
		set {m_machoSkillStacks = value;}
	}

	public int NuclearSkillStack
	{
		get {return m_nuclearSkillStacks;}
		set {m_nuclearSkillStacks = value;}
	}

	public ItemObject[]	AccessoryItems
	{
		get {return m_accessoryItems;}
	}

	public void ApplyGameOptions()
	{

		Const.GetSpawn().audio.ignoreListenerVolume = false;
		Const.GetSpawn().audio.volume = Warehouse.Instance.GameOptions.m_bgmVolume;
		Const.GetSpawn().audio.ignoreListenerVolume = true;

		AudioListener.volume = Warehouse.Instance.GameOptions.m_sfxVolume;
		m_enableAutoTarget = Warehouse.Instance.GameOptions.m_autoTarget;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	/*
	void OnGUI()
	{
		UpdateChampMovement();
	}
*/

	override public void EquipWeapon(ItemWeaponData weaponData, WeaponStat weaponStat)
	{
		base.EquipWeapon(weaponData, weaponStat);
		if (weaponData.RefItem.partName != null)
			transform.Find("Body/"+weaponData.RefItem.partName).gameObject.SetActive(true);
	}
	// Update is called once per frame
	new void Update () {
		base.Update();

		if (m_enableAutoTarget)
		{
			if (m_rightJoypad.Dragging)
			{
				Vector3 pos = Vector3.zero;
				pos.x = m_rightJoypad.Position.x*10;
				pos.z = m_rightJoypad.Position.y*10;
				m_weaponHolder.StartFiring(RotateToTarget(transform.position+pos));
			}
			else if (AutoAttack() == false)
			{
				m_weaponHolder.StopFiring();
			}

			Vector3 ang = m_rightJoypad.transform.eulerAngles;
			ang.z = -transform.eulerAngles.y;
			m_rightJoypad.transform.eulerAngles = ang;
		}
		else
		{
			if (m_rightJoypad.Dragging)
			{
				Vector3 pos = Vector3.zero;
				pos.x = m_rightJoypad.Position.x*10;
				pos.z = m_rightJoypad.Position.y*10;
				m_weaponHolder.StartFiring(RotateToTarget(transform.position+pos));
			}
			else
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
					target.TakeDamage(this, new DamageDesc(m_creatureProperty.TapDamage, DamageDesc.Type.Normal, DamageDesc.BuffType.Nothing, null));
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
				targets[i].TakeDamage(this, new DamageDesc(m_creatureProperty.TapDamage, DamageDesc.Type.Normal, DamageDesc.BuffType.Nothing, null));
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

		Warehouse.Instance.UpdateGameStats.ConsumedSP += sp;
	}

	override public void GiveExp(int exp)
	{
		m_creatureProperty.giveExp(exp);
	}

	override public int TakeDamage(Creature offender, DamageDesc damageDesc)
	{
		int dmg = base.TakeDamage(offender, damageDesc);
		//if (0 < dmg)
		//	m_bloodWarningAnimator.SetTrigger("Warning");

		return dmg;
	}



	override public void Death()
	{
		if (MachoSkillStack > 0)
		{
			m_creatureProperty.HP = 1;
			ApplyMachoSkill();
			return;
		}

		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Death", "Wave"+Warehouse.Instance.WaveIndex, 0);

		Warehouse.Instance.NewGameStats.KilledMobs = Warehouse.Instance.AlienEssence.Item.Count;
		Warehouse.Instance.GameBestStats.SetBestStats(Warehouse.Instance.NewGameStats);	

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
			DamageText(type.ToString(), Vector3.one, Color.cyan, DamageNumberSprite.MovementType.RisingUp);
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
