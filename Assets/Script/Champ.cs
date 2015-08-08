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

	new void Start () {

		base.Start();

		ApplyGameOptions();

		SetFollowingCamera(null);
		FollowingCamera followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		followingCamera.SetMainTarget(gameObject);

		m_leftJoypad = GameObject.Find("HudGUI/Joypad/LeftJoypad").GetComponent<Joypad>();
		m_rightJoypad = GameObject.Find("HudGUI/Joypad/RightJoypad").GetComponent<Joypad>();

		m_bloodWarningAnimator = GameObject.Find("HudGUI/Blood Warning").GetComponent<Animator>();

	}

	override public void Init(RefMob refMob, int level)
	{
		base.Init(refMob, level);

		MobKills = 0;
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

		ApplyBuff(null, DamageDesc.BuffType.LevelUp, 10f, null);
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

	public int MobKills
	{
		get {return m_mobKills.Value;}
		set {m_mobKills.Value = value;}
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
		for (int i = 0; i < touchedCount; ++i) 
		{
			Ray ray = Camera.main.ScreenPointToRay( touchPos[i] );
			RaycastHit[] hits;
			hits = Physics.RaycastAll(ray, Mathf.Infinity, 1<<9 | 1<<10);
			foreach(RaycastHit hit in hits )
			{
				Creature target = hit.transform.GetComponent<Creature>();
				if (target != null && IsEnemy(target, this))
				{
					target.TakeDamage(this, new DamageDesc(10, DamageDesc.Type.Normal, DamageDesc.BuffType.Nothing, null));
				}
				else if (hit.transform.tag.CompareTo("ItemBox") == 0)
				{

					ItemBox itemBox = hit.transform.gameObject.GetComponent<ItemBox>();
					itemBox.StartPickupEffect(this);

				}
			}
		}

		Warehouse.Instance.GameDataContext.m_level.Value = m_creatureProperty.Level;
		Warehouse.Instance.GameDataContext.m_hp.Value = m_creatureProperty.HP;
		Warehouse.Instance.GameDataContext.m_xp.Value = m_creatureProperty.Exp;

		TimeEffector.Instance.Update();
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

	public void ApplyMachoSkill()
	{
		ApplyBuff(null, DamageDesc.BuffType.Macho, 5f, null);
		
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

		Warehouse.Instance.NewGameStats.KilledMobs = MobKills;
		Warehouse.Instance.NewGameStats.WaveIndex = Warehouse.Instance.WaveIndex;

		Warehouse.Instance.KilledMobs = MobKills;
		Warehouse.Instance.GameBestStats.SetBestStats(Warehouse.Instance.NewGameStats);	

		m_creatureProperty.Level = 1;
		m_creatureProperty.Exp = 0;
		Warehouse.Instance.GameDataContext.m_level.Value = m_creatureProperty.Level;
		Warehouse.Instance.GameDataContext.m_hp.Value = m_creatureProperty.MaxHP;
		Warehouse.Instance.GameDataContext.m_xp.Value = m_creatureProperty.Exp;

		ShowGameOverGUI();

		base.Death();
	}

	void	ShowGameOverGUI()
	{
		GameObject.Find("HudGUI/GameOverGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	override public bool ApplyBuff(Creature offender, DamageDesc.BuffType type, float time, DamageDesc damageDesc)
	{
		if (false == base.ApplyBuff(offender, type, time, damageDesc))
			return false;

		switch(type)
		{		
		case DamageDesc.BuffType.LevelUp:
			DamageText(type.ToString(), Color.cyan, DamageNumberSprite.MovementType.RisingUp);
			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "ChampLevelUp", "ChampLV:" + m_creatureProperty.Level, 0);
			break;
		case DamageDesc.BuffType.Macho:
			DamageText(type.ToString(), Color.cyan, DamageNumberSprite.MovementType.RisingUp);
			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Combo", "Combo100", 0);
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
