using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature : MonoBehaviour {

	public enum Type
	{
		Champ = 1,
		Mob = 2,
		Npc = 4,
		ChampNpc = Champ+Npc,
	}

	public enum CrowdControlType
	{
		Nothing = 0x0,
		Airborne = 0x1,
		Stun = 0x2,
		LeapStrike = 0x4,
	}

	public enum BehaviourType
	{
		ALive,
		Death,
	}

	protected	BehaviourType	m_behaviourType = BehaviourType.ALive;

	int	m_crowdControl = (int)CrowdControlType.Nothing;
	// Use this for initialization
	protected NavMeshAgent	m_navAgent;

	protected WeaponHolder	m_weaponHolder;
	protected Material		m_material;

	Creature				m_targeting;

	[SerializeField]
	protected GameObject	m_prefDeathEffect;

	[SerializeField]
	protected Type			m_creatureType;

	GameObject				m_prefDamageSprite;

	public CreatureProperty	m_creatureProperty;
	int						m_ingTakenDamageEffect = 0;

	protected GameObject	m_aimpoint;
	RefMob					m_refMob;


	bool					m_checkOnDeath = false;

	Animator				m_animator;
	
	Spawn					m_spawn;
	RefItemSpawn[]			m_dropItems;
	protected struct DamageEffect
	{
		public float endTime;
		public bool	m_run;
	}
	DamageEffect[]	m_damageEffects = new DamageEffect[(int)DamageDesc.Type.Count];

	protected DamageEffect[]	m_buffEffects = new DamageEffect[(int)DamageDesc.BuffType.Count+1];
	float		m_pushbackSpeedOnDamage = 0f;

	Texture damagedTexture;
	Texture normalTexture;

	protected void Start () {
		m_aimpoint = transform.Find("Body/Aimpoint").gameObject;
		m_animator = transform.Find("Body").GetComponent<Animator>();

		m_prefDamageSprite = Resources.Load<GameObject>("Pref/DamageNumberSprite");

	}

	virtual public void Init(RefMob refMob, int level)
	{

		m_navAgent = GetComponent<NavMeshAgent>();
		m_targeting = null;
		m_ingTakenDamageEffect = 0;
		m_pushbackSpeedOnDamage = 0;
		m_behaviourType = BehaviourType.ALive;

		m_weaponHolder = this.transform.Find("WeaponHolder").gameObject.GetComponent<WeaponHolder>();
		m_weaponHolder.Init();

		damagedTexture = Resources.Load<Texture>("ani/damage monster");
		normalTexture = Resources.Load<Texture>("ani/monster");
		ChangeNormalColor();

		m_refMob = refMob;
		m_creatureProperty.init(this, m_refMob.baseCreatureProperty, level);		
		rigidbody.mass = refMob.mass;
		m_navAgent.baseOffset = m_refMob.baseCreatureProperty.navMeshBaseOffset;
	}

	virtual public Creature GetOwner()
	{
		return null;
	}

	public Weapon instanceWeapon(ItemWeaponData weaponData, WeaponStat weaponStat)
	{
		GameObject obj = Instantiate (weaponData.PrefWeapon, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
		Weapon weapon = obj.GetComponent<Weapon>();
		
		obj.transform.parent = m_weaponHolder.transform;
		obj.transform.localPosition = weaponData.PrefWeapon.transform.localPosition;
		obj.transform.localRotation = weaponData.PrefWeapon.transform.localRotation;
		obj.transform.localScale = weaponData.PrefWeapon.transform.localScale;
		
		weapon.Init(this, weaponData, weaponStat);

		return weapon;
	}

	virtual public void EquipWeapon(ItemWeaponData weaponData, WeaponStat weaponStat)
	{		
		Weapon weapon = instanceWeapon(weaponData, weaponStat);

		weapon.m_callbackCreateBullet = delegate() {
			if (m_animator != null)
			{
				m_animator.SetTrigger("Attack");
			}
		};

		m_weaponHolder.EquipWeapon(weapon);

		if (weapon.WeaponStat.skillId > 0)
		{
			EquipActiveSkillWeapon(new ItemWeaponData(weapon.WeaponStat.skillId), null);
		}
	}

	public void EquipPassiveSkillWeapon(ItemWeaponData weaponData, WeaponStat weaponStat)
	{
		Weapon weapon = instanceWeapon(weaponData, weaponStat);
		
		m_weaponHolder.EquipPassiveSkillWeapon(weapon);
	}

	public void EquipActiveSkillWeapon(ItemWeaponData weaponData, WeaponStat weaponStat)
	{
		Weapon weapon = instanceWeapon(weaponData, weaponStat);
		
		m_weaponHolder.EquipActiveSkillWeapon(weapon);
	}

	public void SetSubWeapon(Weapon weapon, ItemWeaponData weaponData, WeaponStat weaponStat)
	{
		Weapon subWeapon = instanceWeapon(weaponData, weaponStat);
		
		weapon.SetSubWeapon(subWeapon);
	}

	public Vector3	AimpointLocalPos
	{
		get {return m_aimpoint.transform.localPosition;}
	}

	public Creature	Targetting
	{
		get {return m_targeting;}
	}

	public virtual void SetTarget(Creature target)
	{
		m_targeting = target;
	}

	public bool CheckOnDeath
	{
		set {m_checkOnDeath = value;}
		get {return m_checkOnDeath;}
	}

	public float RotateToTarget(Vector3 pos)
	{
		/*
		Vector3 gunPoint = m_weaponHolder.transform.position;
		gunPoint.x = transform.position.x;
		gunPoint.z = transform.position.z;
		*/
		Vector3 gunPoint = transform.position;
		float targetHorAngle = Mathf.Atan2(pos.z-gunPoint.z, pos.x-gunPoint.x) * Mathf.Rad2Deg;
		transform.eulerAngles = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, -targetHorAngle, 0)), m_creatureProperty.RotationSpeedRatio*Time.deltaTime).eulerAngles;

		return targetHorAngle;
	}

	public float RotateToTarget(float angle)
	{	
		Vector3 euler = transform.eulerAngles;
		euler.y = -angle;
		transform.eulerAngles = euler;
		
		return angle;
	}

	static public bool IsEnemy(Creature a, Creature b)
	{
		if (a.CreatureType == Type.ChampNpc)
			return false;

		int bType = (int)(b.CreatureType & ~Type.Npc);
		int aType = (int)(a.CreatureType & ~Type.Npc);

		if (aType == 0 || bType == 0)
			return false;

		return bType != aType;
	}

	public void EnableNavmeshUpdatePos(bool enable)
	{
		m_navAgent.updatePosition = enable;
		m_navAgent.updateRotation = enable;
	}

	public void EnableNavmesh(bool enable)
	{
		m_navAgent.enabled = enable;
	}

	public void EnableNavMeshObstacleAvoidance(bool enable)
	{
		if (enable == false)
		{
			m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
			m_navAgent.autoBraking = false;
		}
		else
		{
			m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
			m_navAgent.autoBraking = true;
		}
	}

	public RefMob RefMob
	{
		get {return m_refMob;}
		set {m_refMob = value;}
	}

	protected void Update()
	{

		//if (m_navAgent.speed != m_creatureProperty.MoveSpeed)
		{
			m_navAgent.speed = m_creatureProperty.MoveSpeed;
			m_animator.speed = m_creatureProperty.AniSpeed;
		}


		if (true == m_creatureProperty.BackwardOnDamage)
		{
			if (m_pushbackSpeedOnDamage > 0)
			{
				m_pushbackSpeedOnDamage -= 2f;
				EnableNavmeshUpdatePos(false);
			}
			else
			{
				EnableNavmeshUpdatePos(true);
				rigidbody.velocity = Vector3.zero;
			}
		}

		m_creatureProperty.Update();
	}

	public Creature.Type GetMyEnemyType()
	{
		switch(m_creatureType)
		{
		case Type.Champ:
		case Type.ChampNpc:
			return Type.Mob;
		case Type.Mob:
			return Type.Champ;
		}

		return Type.Npc;
	}

	public RefItemSpawn[] RefDropItems
	{
		set {m_dropItems = value;}
		get {return m_dropItems;}
	}

	public WeaponHolder WeaponHolder
	{
		get {return m_weaponHolder;}
	}

	protected bool inAttackRange(Creature targeting, float overrideRange)
	{
		float dist = Vector3.Distance(transform.position, targeting.transform.position);

		if (overrideRange == 0f)
		{
			if (dist <= m_weaponHolder.AttackRange())
			{
				return true;
			}
		}
		else
		{
			if (dist <= overrideRange)
			{
				return true;
			}
		}


		return false;
	}

	public void SetFollowingCamera(GameObject next)
	{
		FollowingCamera followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		followingCamera.SetTarget(gameObject, next);
	}

	public Creature SearchTarget(Creature.Type targetTags, Creature[] skipTargets, float range)
	{
		Creature lastTarget = null;
		Creature[] targets = Bullet.SearchTarget(transform.position, targetTags, range);
		if (targets == null)
			return null;

		foreach(Creature target in targets)
		{
			bool isSkip = false;
			if (skipTargets != null)
			{
				foreach(Creature skip in skipTargets)
				{
					if (skip == null)
						continue;
					
					if (skip.gameObject == target)
					{
						isSkip = true;
						break;
					}
				}
			}
			
			if (isSkip == true)
			{
				continue;
			}

			lastTarget = target;
			if (inAttackRange(target, 0f))
			{
				return target;
			}
		}
		
		return lastTarget;
	}

	protected bool HasCrowdControl()
	{
		return m_crowdControl != (int)CrowdControlType.Nothing;
	}

	virtual public bool AutoAttack() {


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

				m_weaponHolder.StopFiring();
				return false;
			}
		}

		SetTarget(null);
		m_weaponHolder.StopFiring();
		return false;
	}

	void ChangeNormalColor()
	{
		Renderer[] renders = GetComponentsInChildren<Renderer>();
		if (renders != null)
		{
			int len = renders.Length;
			for(int i = 0; i < len; ++i)
			{
				if (renders[i] && renders[i].material && renders[i].material.mainTexture)
				{
					if (renders[i].material.mainTexture.name.CompareTo("damage monster") == 0)
					{
						renders[i].material.mainTexture = normalTexture;
					}
					
				}
			}
		}
	}

	protected IEnumerator BodyRedColoredOnTakenDamage()
	{
		Renderer[] renders = GetComponentsInChildren<Renderer>();
		if (renders != null)
		{

			Color color = new Color(0f,1f,0f,0f);
			int len = renders.Length;

			for(int i = 0; i < len; ++i)
			{
				if (renders[i] && renders[i].material && renders[i].material.mainTexture)
				{
					if (renders[i].material.mainTexture.name.CompareTo("monster") == 0)
					{
						renders[i].material.mainTexture = damagedTexture;
					}

				}
			}
		}

		yield return new WaitForSeconds(0.3f);
			
		ChangeNormalColor();
		--m_ingTakenDamageEffect;
	}



	IEnumerator UpdateDamageEffect(GameObject effect)
	{
		while(effect.particleSystem.IsAlive())
		{
			yield return null;
		}
		DestroyObject(effect);
	}

	IEnumerator UpdatePickupItemEffect(GameObject effect)
	{
		while(effect.particleSystem.IsAlive())
		{
			yield return null;
		}
		DestroyObject(effect);
	}

	public void CrowdControl(CrowdControlType type, bool enable)
	{
		if (enable == true)
		{
			m_crowdControl |= (int)type;
		}
		else
		{
			m_crowdControl &= ~(int)type;
		}
	}

	IEnumerator EffectAirborne()
	{	
		
		DamageText(CrowdControlType.Airborne.ToString(), Color.white, DamageNumberSprite.MovementType.FloatingUp);
		CrowdControl(CrowdControlType.Airborne, true);
		Parabola parabola = new Parabola(gameObject, 8, 0f, 90*Mathf.Deg2Rad, 1);
		while(parabola.Update())
		{
			yield return null;
		}

		m_buffEffects[(int)DamageDesc.BuffType.Airborne].m_run = false;
		CrowdControl(CrowdControlType.Airborne, false);

	}

	IEnumerator EffectStun()
	{		
		DamageText(CrowdControlType.Stun.ToString(), Color.white, DamageNumberSprite.MovementType.FloatingUp);
		CrowdControl(CrowdControlType.Stun, true);
		float ori = m_creatureProperty.BetaMoveSpeed;
		m_creatureProperty.BetaMoveSpeed = 0f;
		yield return new WaitForSeconds(2f);

		m_creatureProperty.BetaMoveSpeed += ori;
		m_buffEffects[(int)DamageDesc.BuffType.Stun].m_run = false;
		CrowdControl(CrowdControlType.Stun, false);
	}

	IEnumerator EffectSlow(float time)
	{		
		
		DamageText(DamageDesc.BuffType.Slow.ToString(), Color.white, DamageNumberSprite.MovementType.FloatingUp);
		float ori = m_creatureProperty.BetaMoveSpeed / 2f;
		m_creatureProperty.BetaMoveSpeed -= ori;
		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.Slow].m_run = false;
		m_creatureProperty.BetaMoveSpeed += ori;


	}

	IEnumerator EffectSteamPack(float time)
	{
		GameObject pref = Resources.Load<GameObject>("Pref/ef level up");
		GameObject effect = (GameObject)Instantiate(pref);
		effect.transform.parent = transform;
		effect.transform.localPosition = pref.transform.position;
		effect.transform.localRotation = pref.transform.rotation;

		m_creatureProperty.BulletLength += 1f;
		m_creatureProperty.AlphaAttackCoolTime -= 0.5f;
		m_creatureProperty.BetaMoveSpeed += 1f;

		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.LevelUp].m_run = false;
		m_creatureProperty.BulletLength -= 1f;
		m_creatureProperty.AlphaAttackCoolTime += 0.5f;
		m_creatureProperty.BetaMoveSpeed -= 1f;

		DestroyObject(effect);
	}

	IEnumerator EffectDash(DamageDesc damageDesc, float time)
	{	
		Vector3 pos = transform.position;
		pos.y = damageDesc.PrefEffect.transform.position.y;
		GameObject dmgEffect = (GameObject)Instantiate(damageDesc.PrefEffect, pos, damageDesc.PrefEffect.transform.rotation);

		float finished = Time.time + time;
		rigidbody.AddForce(damageDesc.Dir*10f, ForceMode.Impulse);
		while(Time.time < finished)
		{
			yield return null;
		}
		
		rigidbody.velocity = Vector3.zero;
		m_buffEffects[(int)DamageDesc.BuffType.Dash].m_run = false;
		DestroyObject(dmgEffect);
	}

	IEnumerator EffectBurning(float time, Creature offender, DamageDesc damageDesc)
	{		
		while(time > 0)
		{
			yield return new WaitForSeconds(0.3f);
			time -= 0.3f;
			damageDesc.PushbackOnDamage = false;
			TakeDamage(offender, damageDesc);
		}
		m_buffEffects[(int)DamageDesc.BuffType.Poison].m_run = false;
	}

	IEnumerator EffectMacho(float time)
	{
		GameObject pref = Resources.Load<GameObject>("Pref/ef combo skill");
		GameObject effect = (GameObject)Instantiate(pref);
		effect.transform.parent = transform;
		effect.transform.localPosition = pref.transform.position;
		effect.transform.localRotation = pref.transform.rotation;

		m_creatureProperty.SP = m_creatureProperty.MaxSP;
		m_creatureProperty.BulletLength += 1f;
		m_creatureProperty.AlphaAttackCoolTime -= 0.5f;
		m_creatureProperty.BetaMoveSpeed += 1f;
		Vector3 scale = transform.localScale;
		transform.localScale += scale;
		
		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.Macho].m_run = false;
		m_creatureProperty.AlphaAttackCoolTime += 0.5f;
		m_creatureProperty.BulletLength -= 1f;
		m_creatureProperty.BetaMoveSpeed -= 1f;
		transform.localScale -= scale;

		DestroyObject(effect);
	}

	IEnumerator EffectDamageMultiply(float time, float damageRatio)
	{
		GameObject pref = Resources.Load<GameObject>("Pref/ef combo skill");
		GameObject effect = (GameObject)Instantiate(pref);
		effect.transform.parent = transform;
		effect.transform.localPosition = pref.transform.position;
		effect.transform.localRotation = pref.transform.rotation;
		
		m_creatureProperty.DamageRatio += damageRatio;
		
		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.DamageMultiply].m_run = false;
		m_creatureProperty.DamageRatio -= damageRatio;
		
		DestroyObject(effect);
	}

	IEnumerator EffectHealing(float time, float damageRatio)
	{
		float timeout = Time.time+time;
		while(Time.time < timeout)
		{
			int heal = (int)(m_creatureProperty.MaxHP*damageRatio);
			Heal(heal);
			DamageText("Heal " + heal, Color.green, DamageNumberSprite.MovementType.Parabola);
			yield return new WaitForSeconds(1f);
		}
		
		m_buffEffects[(int)DamageDesc.BuffType.Healing].m_run = false;
		

	}

	void ApplyDamageEffect(DamageDesc.Type type, GameObject prefEffect)
	{
		if (prefEffect == null)
			return;

		GameObject dmgEffect = (GameObject)Instantiate(prefEffect, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
		dmgEffect.transform.parent = m_aimpoint.transform;
		dmgEffect.transform.localPosition = Vector3.zero;
		dmgEffect.transform.localScale = m_aimpoint.transform.localScale;
		StartCoroutine(UpdateDamageEffect(dmgEffect));	
	}

	virtual public bool ApplyBuff(Creature offender, DamageDesc.BuffType type, float time, DamageDesc damageDesc)
	{
		//if (m_buffEffects[(int)type].m_run == true)
		//	return false;
		switch(type)
		{
		case DamageDesc.BuffType.Airborne:
		case DamageDesc.BuffType.Stun:
		case DamageDesc.BuffType.Slow:
			if (m_buffEffects[(int)type].m_run == true)
				return false;
			break;
		}

		m_buffEffects[(int)type].m_run = true;

		switch(type)
		{
		case DamageDesc.BuffType.Airborne:
			if (m_buffEffects[(int)type].m_run == true)
				return false;
			StartCoroutine(EffectAirborne());
			break;
		case DamageDesc.BuffType.Stun:
			if (m_buffEffects[(int)type].m_run == true)
				return false;
			StartCoroutine(EffectStun());
			break;
		case DamageDesc.BuffType.Slow:
			if (m_buffEffects[(int)type].m_run == true)
				return false;
			StartCoroutine(EffectSlow(time));
			break;
		case DamageDesc.BuffType.LevelUp:
			StartCoroutine(EffectSteamPack(time));
			break;
		case DamageDesc.BuffType.Poison:
			StartCoroutine(EffectBurning(time, null, damageDesc));
			break;
		case DamageDesc.BuffType.Macho:
			StartCoroutine(EffectMacho(time));
			break;
		case DamageDesc.BuffType.Dash:
			StartCoroutine(EffectDash(damageDesc, time));
			break;
		case DamageDesc.BuffType.DamageMultiply:
			StartCoroutine(EffectDamageMultiply(time, damageDesc.DamageRatio));
			break;
		case DamageDesc.BuffType.Healing:
			StartCoroutine(EffectHealing(time, damageDesc.DamageRatio));
			break;
		}

		return true;
	}

	public void ApplyPickUpItemEffect(ItemData.Type type, GameObject prefEffect, int value)
	{
		GameObject dmgEffect = (GameObject)Instantiate(prefEffect, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
		dmgEffect.transform.parent = m_aimpoint.transform;
		dmgEffect.transform.localPosition = Vector3.zero;
		dmgEffect.transform.particleSystem.startSize = gameObject.transform.localScale.x+prefEffect.transform.localScale.x;
		StartCoroutine(UpdatePickupItemEffect(dmgEffect));
	
		string strDamage = value.ToString();

		switch(type)
		{
		case ItemData.Type.Gold:
			{
				DamageText("Gold " + strDamage, Color.yellow, DamageNumberSprite.MovementType.Parabola);
			}
			break;
		case ItemData.Type.HealPosion:
			{
			DamageText("Heal " + strDamage, Color.green, DamageNumberSprite.MovementType.Parabola);
			}
			break;
		case ItemData.Type.XPPotion:
			{
			DamageText("XP " + strDamage, Color.blue, DamageNumberSprite.MovementType.Parabola);
			}
			break;
		}
	}

	public DamageNumberSprite DamageText(string damage, Color color, DamageNumberSprite.MovementType movementType)
	{
		GameObject gui = (GameObject)GameObjectPool.Instance.Alloc(m_prefDamageSprite, m_aimpoint.transform.position, m_prefDamageSprite.transform.localRotation);
		DamageNumberSprite sprite = gui.GetComponent<DamageNumberSprite>();
		sprite.Init(this, damage, color, movementType);

		return sprite;
	}
	
	virtual public int TakeDamage(Creature offender, DamageDesc damageDesc)
	{
		if (m_behaviourType == BehaviourType.Death)
			return 0;

		if (m_buffEffects[(int)DamageDesc.BuffType.Macho].m_run == true)
		{
			DamageText("Blocked", Color.white, DamageNumberSprite.MovementType.Parabola);
			return 0;
		}

		if (m_buffEffects[(int)DamageDesc.BuffType.Dash].m_run == true)
		{
			DamageText("Blocked", Color.white, DamageNumberSprite.MovementType.Parabola);
			return 0;
		}

		bool critical = false;
		float criticalDamage = 1f;
		if (offender != null)
		{
			if (Random.Range(0, 1f) < offender.m_creatureProperty.CriticalChance)
			{
				critical = true;
				criticalDamage = 1f+offender.m_creatureProperty.CriticalDamage;
			}
		}

		int dmg = (int)(damageDesc.Damage*criticalDamage);
		dmg -= (int)(dmg*m_creatureProperty.DamageReduction);
		dmg = Mathf.Max(0, dmg-m_creatureProperty.PhysicalDefencePoint);
		if (dmg == 0)
		{
			dmg = Random.Range(0, 2);
		}

		if (dmg > 0 && m_creatureProperty.Shield > 0)
		{
			--m_creatureProperty.Shield;
			DamageText("Shielded", Color.white, DamageNumberSprite.MovementType.Parabola);
			return 0;
		}

		string strDamage = dmg.ToString();
		if (dmg == 0)
		{
			strDamage = "Blocked";
			DamageText(strDamage, Color.white, DamageNumberSprite.MovementType.Parabola);
			return 0;
		}
		
		if (m_ingTakenDamageEffect < Const.MaxShowDamageNumber)
		{
			++m_ingTakenDamageEffect;
			Color color = Color.white;
			if (critical == true)
			{
				strDamage = dmg.ToString();
				color = Color.red;
			}
			else if (damageDesc.DamageBuffType == DamageDesc.BuffType.Poison)
			{
				color = Color.magenta;
			}

			DamageText(strDamage, color, DamageNumberSprite.MovementType.Parabola);

			StartCoroutine(BodyRedColoredOnTakenDamage());

			ApplyDamageEffect(damageDesc.DamageType, damageDesc.PrefEffect);
		}

		if (true == m_creatureProperty.BackwardOnDamage && damageDesc.PushbackOnDamage && m_pushbackSpeedOnDamage == 0f)
		{
			m_pushbackSpeedOnDamage = 10f / rigidbody.mass;
			rigidbody.AddForce(transform.right*-2f, ForceMode.Impulse);
			rigidbody.AddTorque(transform.forward*2f, ForceMode.Impulse);
			rigidbody.maxAngularVelocity = 2f;
		
			EnableNavmeshUpdatePos(false);
		}

		ApplyBuff(offender, damageDesc.DamageBuffType, 2f, damageDesc);

		m_creatureProperty.HP-=dmg;
		if (m_creatureProperty.HP == 0)
		{
			if (offender != null)
			{
				int lifeSteal = (int)(offender.m_creatureProperty.LifeSteal);
				if (lifeSteal > 0)
				{
					offender.DamageText(lifeSteal.ToString(), Color.green, DamageNumberSprite.MovementType.RisingUp);
					offender.Heal(lifeSteal);
				}
				Const.GetSpawn().SharePotinsChamps(offender, ItemData.Type.XPPotion, m_creatureProperty.RewardExp, false);
			}

			Death();
		}

		return dmg;
	}

	virtual public void GiveExp(int exp)
	{

	}

	public void Heal(int heal)
	{
		m_creatureProperty.HP += heal;
	}

	public Type CreatureType
	{
		get { return m_creatureType; }
		set {
			m_creatureType = value;
			tag = m_creatureType.ToString();
		}
	}

	public void ShakeCamera(float time)
	{
		CameraShake shake = Camera.main.gameObject.GetComponent<CameraShake>();
		shake.shake = time;
		shake.enabled = true;
	}
	
	virtual public void Death()
	{
		if (m_behaviourType == BehaviourType.Death)
			return;
		
		m_behaviourType = BehaviourType.Death;

		GameObject effect = (GameObject)GameObject.Instantiate(m_prefDeathEffect, transform.position, transform.rotation);
		effect.transform.localScale = transform.localScale;
		
		AudioClip sfx = Resources.Load<AudioClip>("SFX/"+RefMob.prefBody+"_death");
		if (sfx != null)
		{
			effect.audio.clip = sfx;
			effect.audio.Play();
		}
		
		Const.DestroyChildrenObjects(m_weaponHolder.gameObject);
		
		Const.DestroyChildrenObjects(m_aimpoint);
		
		GameObject body = gameObject.transform.Find("Body").gameObject;
		body.transform.parent = null;
		GameObjectPool.Instance.Free(body);
		DestroyObject(gameObject);
		
		ShakeCamera(0.1f);
	}


	static public GameObject InstanceCreature(GameObject prefHead, GameObject prefBody, Vector3 pos, Quaternion rotation)
	{
		GameObject obj = (GameObject)GameObject.Instantiate(prefHead, pos, rotation);

		GameObject enemyBody = GameObjectPool.Instance.Alloc (prefBody, Vector3.zero, Quaternion.Euler (0, 0, 0)) as GameObject;
		enemyBody.name = "Body";
		enemyBody.transform.parent = obj.transform;
		enemyBody.transform.localPosition = Vector3.zero;
		enemyBody.transform.localRotation = prefBody.transform.rotation;
		enemyBody.transform.localScale = prefBody.transform.localScale;

		return obj;
	}
}
