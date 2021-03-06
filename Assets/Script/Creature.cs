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
		MobNpc = Mob+Npc,
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
	protected GameObject	m_hppoint;
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

	List<Creature>	m_followers = new List<Creature>();

	protected void Start () {


	}

	virtual public void Init(RefMob refMob, int level, int evolution)
	{

		m_navAgent = GetComponent<NavMeshAgent>();
		m_targeting = null;
		m_ingTakenDamageEffect = 0;
		m_pushbackSpeedOnDamage = 0;
		m_behaviourType = BehaviourType.ALive;

		m_weaponHolder = transform.Find("WeaponHolder").gameObject.GetComponent<WeaponHolder>();
		m_weaponHolder.Init();

		if (transform.Find("Body/WeaponPoint") != null)
		{
			m_weaponHolder.transform.position = transform.Find("Body/WeaponPoint").transform.position;
		}

		damagedTexture = Resources.Load<Texture>("ani/damage monster");
		normalTexture = Resources.Load<Texture>("ani/monster");
		ChangeNormalColor();

		m_refMob = refMob;
		m_creatureProperty.init(this, m_refMob.baseCreatureProperty, level, evolution);		
		rigidbody.mass = refMob.mass;
		m_navAgent.baseOffset = m_refMob.baseCreatureProperty.navMeshBaseOffset;

		m_aimpoint = transform.Find("Body/Aimpoint").gameObject;
		m_hppoint = m_aimpoint;
		
		if (transform.Find("Body/HPPoint"))
		{
			m_hppoint = transform.Find("Body/HPPoint").gameObject;
		}
		m_animator = transform.Find("Body").GetComponent<Animator>();
		
		m_prefDamageSprite = Resources.Load<GameObject>("Pref/DamageNumberSprite");
	}

	virtual public Creature GetOwner()
	{
		return null;
	}

	public Weapon instanceWeapon(ItemWeaponData weaponData, RefMob.WeaponDesc weaponDesc)
	{
		GameObject obj = Instantiate (weaponData.PrefWeapon, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
		Weapon weapon = obj.GetComponent<Weapon>();
		
		obj.transform.parent = m_weaponHolder.transform;
		obj.transform.localPosition = weaponData.PrefWeapon.transform.localPosition;
		obj.transform.localRotation = weaponData.PrefWeapon.transform.localRotation;
		obj.transform.localScale = weaponData.PrefWeapon.transform.localScale;
		
		weapon.Init(this, weaponData, weaponDesc);

		return weapon;
	}

	public virtual Weapon EquipWeapon(ItemWeaponData weaponData, RefMob.WeaponDesc weaponDesc)
	{		
		Weapon weapon = instanceWeapon(weaponData, weaponDesc);

		weapon.m_callbackCreateBullet = delegate() {
			if (m_animator != null)
			{
				m_animator.SetTrigger("Attack");
			}
		};

		m_weaponHolder.EquipWeapon(weapon);

		if (weapon.WeaponStat.skillId > 0)
		{
			EquipActiveSkillWeapon(new ItemWeaponData(weapon.WeaponStat.skillId), new RefMob.WeaponDesc());
		}

		return weapon;
	}

	public void EquipPassiveSkillWeapon(ItemWeaponData weaponData, RefMob.WeaponDesc weaponDesc)
	{
		Weapon weapon = instanceWeapon(weaponData, weaponDesc);
		
		m_weaponHolder.EquipPassiveSkillWeapon(weapon);
	}

	public virtual Weapon EquipActiveSkillWeapon(ItemWeaponData weaponData, RefMob.WeaponDesc weaponDesc)
	{
		Weapon weapon = instanceWeapon(weaponData, weaponDesc);
		
		m_weaponHolder.EquipActiveSkillWeapon(weapon);

		return weapon;
	}

	public void SetSubWeapon(Weapon weapon, ItemWeaponData weaponData, RefMob.WeaponDesc weaponDesc)
	{
		Weapon subWeapon = instanceWeapon(weaponData, weaponDesc);
		
		weapon.SetSubWeapon(subWeapon);
	}

	public Transform	HPPointTransform
	{
		get {return m_hppoint.transform;}
	}

	public Creature	Targetting
	{
		get {return m_targeting;}
	}

	public virtual void SetTarget(Creature target)
	{
		m_targeting = target;
	}

	public void AddFollower(Creature follower)
	{
		m_followers.Add(follower);
	}

	public List<Creature> Followers
	{
		get{return m_followers;}
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
		//transform.eulerAngles = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, -targetHorAngle, 0)), m_creatureProperty.RotationSpeedRatio*Time.deltaTime).eulerAngles;
		transform.eulerAngles = Quaternion.Euler(new Vector3(0, -targetHorAngle, 0)).eulerAngles;
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
				m_pushbackSpeedOnDamage -= 2.8f;
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

	public bool inAttackRange(Creature targeting, float overrideRange)
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
		if (gameObject != null)
		{
			DamageText(CrowdControlType.Airborne.ToString(), Vector3.one, Color.white, DamageNumberSprite.MovementType.FloatingUp);
			CrowdControl(CrowdControlType.Airborne, true);
			Parabola parabola = new Parabola(gameObject, 8, 0f, 90*Mathf.Deg2Rad, 1);
			while(parabola.Update())
			{
				yield return null;
			}

			m_buffEffects[(int)DamageDesc.BuffType.Airborne].m_run = false;
			CrowdControl(CrowdControlType.Airborne, false);
		}

	}

	IEnumerator EffectStun()
	{		
		if (gameObject != null)
		{
			DamageText(CrowdControlType.Stun.ToString(), Vector3.one, Color.white, DamageNumberSprite.MovementType.FloatingUp);
			CrowdControl(CrowdControlType.Stun, true);
			float ori = m_creatureProperty.BetaMoveSpeed;
			m_creatureProperty.BetaMoveSpeed = 0f;
			yield return new WaitForSeconds(2f);

			m_creatureProperty.BetaMoveSpeed += ori;
			m_buffEffects[(int)DamageDesc.BuffType.Stun].m_run = false;
			CrowdControl(CrowdControlType.Stun, false);
		}
	}

	IEnumerator EffectSlow(float time)
	{		
		if (gameObject != null)
		{
			DamageText(DamageDesc.BuffType.Slow.ToString(), Vector3.one, Color.white, DamageNumberSprite.MovementType.FloatingUp);
			float ori = m_creatureProperty.BetaMoveSpeed / 2f;
			m_creatureProperty.BetaMoveSpeed -= ori;
			yield return new WaitForSeconds(time);
			
			m_buffEffects[(int)DamageDesc.BuffType.Slow].m_run = false;
			m_creatureProperty.BetaMoveSpeed += ori;

		}
	}

	IEnumerator EffectSteamPack(float time)
	{
		GameObject pref = Resources.Load<GameObject>("Pref/ef level up");
		GameObject effect = (GameObject)Instantiate(pref);
		effect.transform.parent = transform;
		effect.transform.localPosition = pref.transform.position;
		effect.transform.localRotation = pref.transform.rotation;

		m_creatureProperty.BulletAlphaLength += 1f;
		m_creatureProperty.AlphaAttackCoolTime -= 0.5f;
		m_creatureProperty.BetaMoveSpeed += 1f;

		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.LevelUp].m_run = false;
		m_creatureProperty.BulletAlphaLength -= 1f;
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
		GameObject pref = Resources.Load<GameObject>("Pref/ef_moojuk skill");
		GameObject effect = (GameObject)Instantiate(pref);
		effect.transform.parent = transform;
		effect.transform.localPosition = pref.transform.position;
		effect.transform.localRotation = pref.transform.rotation;

		m_creatureProperty.SP = m_creatureProperty.MaxSP;
		m_creatureProperty.BulletAlphaLength += 1f;
		m_creatureProperty.AlphaAttackCoolTime -= 0.5f;

		bool scalable = false;
		if (transform.localScale.y < 1.5f)
			scalable = true;

		Vector3 scale = transform.localScale*0.3f;
		if (scalable == true)
			transform.localScale += scale;

		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.Macho].m_run = false;
		m_creatureProperty.AlphaAttackCoolTime += 0.5f;
		m_creatureProperty.BulletAlphaLength -= 1f;

		if (scalable == true)
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
		
		m_creatureProperty.DamageMultiPlier += damageRatio;
		
		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.DamageMultiply].m_run = false;
		m_creatureProperty.DamageMultiPlier -= damageRatio;
		
		DestroyObject(effect);
	}

	IEnumerator EffectHealing(float time, float damageRatio)
	{
		float timeout = Time.time+time;
		while(Time.time < timeout)
		{
			if (gameObject != null)
			{
				long heal = (long)(m_creatureProperty.MaxHP*damageRatio);
				Heal(heal);

				DamageText(heal + "HP", Vector3.one, Color.green, DamageNumberSprite.MovementType.ParabolaAlpha);
			}
			yield return new WaitForSeconds(1f);
		}
		
		m_buffEffects[(int)DamageDesc.BuffType.Healing].m_run = false;
		

	}

	IEnumerator EffectZzz(float time)
	{
		float timeout = Time.time+time;
		while(Time.time < timeout)
		{
			if (gameObject != null)
			{
				DamageNumberSprite sprite = FoceDamageText("SP...", Vector3.one, Color.white, DamageNumberSprite.MovementType.FloatingUp);
				sprite.Duration = time;
			}
			yield return new WaitForSeconds(time);
		}
		
		m_buffEffects[(int)DamageDesc.BuffType.Zzz].m_run = false;
	}

	void ApplyDamageEffect(DamageDesc.Type type, GameObject prefEffect)
	{
		if (prefEffect == null)
			return;

		if (Warehouse.Instance.NewGameStats.DamageEffectPerSec > 10)
			return;
		
		Warehouse.Instance.NewGameStats.DamageEffect+=1;

		GameObject dmgEffect = (GameObject)Instantiate(prefEffect, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
		dmgEffect.transform.parent = m_aimpoint.transform;
		dmgEffect.transform.localPosition = Vector3.zero;
		dmgEffect.transform.localScale = m_aimpoint.transform.localScale;
		Bullet.ParticleScale(dmgEffect, m_aimpoint.transform.localScale.x);
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
		case DamageDesc.BuffType.Poison:
			if (m_buffEffects[(int)type].m_run == true)
				return false;
			break;
		}

		m_buffEffects[(int)type].m_run = true;

		switch(type)
		{
		case DamageDesc.BuffType.Airborne:
			StartCoroutine(EffectAirborne());
			break;
		case DamageDesc.BuffType.Stun:
			StartCoroutine(EffectStun());
			break;
		case DamageDesc.BuffType.Slow:
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
		case DamageDesc.BuffType.Zzz:
			StartCoroutine(EffectZzz(time));
			break;
		}

		return true;
	}

	public void ApplyPickUpItemEffect(ItemData.Type type, GameObject prefEffect, long value)
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
			DamageText(strDamage + "G", Vector3.one, Color.yellow, DamageNumberSprite.MovementType.ParabolaAlpha);
			}
			break;
		case ItemData.Type.HealPosion:
			{
			DamageText(strDamage+ "HP", Vector3.one, Color.green, DamageNumberSprite.MovementType.ParabolaAlpha);
			}
			break;
		case ItemData.Type.XPPotion:
			{
			DamageText(strDamage + "XP", Vector3.one, Color.blue, DamageNumberSprite.MovementType.ParabolaAlpha);
			}
			break;
		}
	}

	public bool ApplyMachoSkill()
	{
		if (m_buffEffects[(int)DamageDesc.BuffType.Macho].m_run == true)
			return false;
		
		ApplyBuff(null, DamageDesc.BuffType.Macho, 5f, null);
		return true;
	}

	public bool ApplyZzz(float time)
	{
		if (m_buffEffects[(int)DamageDesc.BuffType.Zzz].m_run == true)
			return false;
		
		ApplyBuff(null, DamageDesc.BuffType.Zzz, time, null);
		return true;
	}
	
	public bool ApplyHealingSkill()
	{
		if (m_buffEffects[(int)DamageDesc.BuffType.Healing].m_run == true)
			return false;
		
		DamageDesc desc = new DamageDesc(0, DamageDesc.Type.Normal, DamageDesc.BuffType.Nothing, null);
		desc.DamageRatio = 0.1f;
		ApplyBuff(null, DamageDesc.BuffType.Healing, 10f, desc);
		return true;
	}
	
	public bool ApplyDamageMultiplySkill()
	{
		DamageDesc desc = new DamageDesc(0, DamageDesc.Type.Normal, DamageDesc.BuffType.Nothing, null);
		desc.DamageRatio = 10f;
		ApplyBuff(null, DamageDesc.BuffType.DamageMultiply, 20f, desc);
		return true;
	}

	public DamageNumberSprite DamageText(string damage, Vector3 scale, Color color, DamageNumberSprite.MovementType movementType)
	{

		if (Warehouse.Instance.NewGameStats.DamageTextPerSec > 10)
			return null;

		Warehouse.Instance.NewGameStats.DamageText+=1;

		return FoceDamageText(damage, scale, color, movementType);
	}

	public DamageNumberSprite FoceDamageText(string damage, Vector3 scale, Color color, DamageNumberSprite.MovementType movementType)
	{
		
		GameObject gui = (GameObject)GameObjectPool.Instance.Alloc(m_prefDamageSprite, m_aimpoint.transform.position, m_prefDamageSprite.transform.localRotation);
		DamageNumberSprite sprite = gui.GetComponent<DamageNumberSprite>();
		sprite.Init(this, damage, color, movementType);
		sprite.transform.localScale = scale;
		return sprite;
	}
	
	virtual public long TakeDamage(Creature offender, DamageDesc damageDesc)
	{
		if (m_behaviourType == BehaviourType.Death)
			return 0;

		if (m_buffEffects[(int)DamageDesc.BuffType.Macho].m_run == true)
		{
			DamageText(RefData.Instance.RefTexts(MultiLang.ID.Blocked), Vector3.one, Color.white, DamageNumberSprite.MovementType.ParabolaAlpha);
			return 0;
		}

		if (m_buffEffects[(int)DamageDesc.BuffType.Dash].m_run == true)
		{
			DamageText(RefData.Instance.RefTexts(MultiLang.ID.Blocked), Vector3.one, Color.white, DamageNumberSprite.MovementType.ParabolaAlpha);
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

		long dmg = (long)(damageDesc.Damage*criticalDamage);
		dmg -= (long)(dmg*m_creatureProperty.DamageReduction);

		if (dmg <= 0)
		{
			dmg = Random.Range(0, 2);
		}

		if (dmg > 0)
		{
			if (Random.Range(0f, 1f) < m_creatureProperty.Dodge)
			{
				DamageText(RefData.Instance.RefTexts(MultiLang.ID.Dodged), Vector3.one, Color.white, DamageNumberSprite.MovementType.ParabolaAlpha);
				return 0;
			}

			if (m_creatureProperty.Shield > 0)
			{
				--m_creatureProperty.Shield;
				DamageText(RefData.Instance.RefTexts(MultiLang.ID.Shielded), Vector3.one, Color.white, DamageNumberSprite.MovementType.ParabolaAlpha);
				return 0;
			}
		}

		string strDamage = dmg.ToString();
		if (dmg == 0)
		{
			strDamage = RefData.Instance.RefTexts(MultiLang.ID.Blocked);
			DamageText(strDamage, Vector3.one, Color.white, DamageNumberSprite.MovementType.ParabolaAlpha);
			return 0;
		}
		
		if (m_ingTakenDamageEffect < Const.MaxShowDamageNumber)
		{
			Vector3 damageTextScale = Vector3.one;
			++m_ingTakenDamageEffect;
			Color color = Color.white;
			if (critical == true)
			{
				strDamage = dmg.ToString();
				color = Color.red;
				damageTextScale *= 1.1f;
			}
			else if (damageDesc.DamageBuffType == DamageDesc.BuffType.Poison)
			{
				color = Color.magenta;
			}
			else
			{
				if (offender != null && offender.RefMob.mobAI == MobAIType.Follow)
				{
					damageTextScale *= 0.8f;
					switch(offender.RefMob.id)
					{
					case 30001:
						color = Color.clear;
						break;
					case 30002:
						color = Color.red;
						break;
					case 30003:
						color = Color.blue;
						break;
					case 30004:
						color = Color.cyan;
						break;
					case 30005:
						color = Color.yellow;
						break;
					case 30006:
						color = Color.magenta;
						break;
					case 30007:
						color = Color.grey;
						break;
					}
				}
			}

			DamageText(strDamage, damageTextScale, color, DamageNumberSprite.MovementType.ParabolaAlpha);

			StartCoroutine(BodyRedColoredOnTakenDamage());

			ApplyDamageEffect(damageDesc.DamageType, damageDesc.PrefEffect);
		}

		if (true == m_creatureProperty.BackwardOnDamage && damageDesc.PushbackOnDamage && m_pushbackSpeedOnDamage <= 0f)
		{
			if (Random.Range(0, 10) == 0)
			{
				m_pushbackSpeedOnDamage = 10f / rigidbody.mass;
				rigidbody.AddForce(transform.right*-2f, ForceMode.Impulse);
				rigidbody.AddTorque(transform.forward*2f, ForceMode.Impulse);
				rigidbody.maxAngularVelocity = 2f;
		
				EnableNavmeshUpdatePos(false);
			}
		}

		ApplyBuff(offender, damageDesc.DamageBuffType, 2f, damageDesc);

		if (offender != null && damageDesc.LifeSteal == true)
		{
			long lifeSteal = (long)(offender.m_creatureProperty.LifeSteal);
			if (lifeSteal > 0)
			{
				offender.DamageText(lifeSteal.ToString() + "L", Vector3.one, Color.green, DamageNumberSprite.MovementType.ParabolaAlpha);
				offender.Heal(lifeSteal);
			}
		}

		m_creatureProperty.HP-=dmg;
		if (m_creatureProperty.HP == 0)
		{			
			Const.GetSpawn().SharePotinsChamps(offender, ItemData.Type.XPPotion, m_creatureProperty.RewardExp, false);
			Death();
		}

		if (offender != null && (offender.CreatureType & Type.Champ) > 0)
		{
			Warehouse.Instance.NewGameStats.DealDamages += dmg;
		}
		else if (CreatureType == Type.Champ)
		{
			Warehouse.Instance.NewGameStats.TakenDamages += dmg;
		}

		return dmg;
	}

	virtual public void ConsumeSP(int sp)
	{
		m_creatureProperty.SP -= sp;
	}

	virtual public void GiveExp(int exp)
	{

	}

	public void Heal(long heal)
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
		
		Const.GetSpawn().ShakeCamera(0.1f);
	}


	static public GameObject InstanceCreature(string prefHead, GameObject prefBody, Vector3 pos, Quaternion rotation)
	{
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>(prefHead), pos, rotation);

		GameObject enemyBody = GameObjectPool.Instance.Alloc (prefBody, Vector3.zero, Quaternion.Euler (0, 0, 0)) as GameObject;
		enemyBody.name = "Body";
		enemyBody.transform.parent = obj.transform;
		enemyBody.transform.localPosition = Vector3.zero;
		enemyBody.transform.localRotation = prefBody.transform.rotation;
		enemyBody.transform.localScale = prefBody.transform.localScale;

		return obj;
	}

	static public GameObject InstanceCreature(GameObject head, GameObject prefBody, Vector3 pos, Quaternion rotation)
	{
		GameObject enemyBody = GameObject.Instantiate (prefBody, Vector3.zero, Quaternion.Euler (0, 0, 0)) as GameObject;
		enemyBody.name = "Body";
		enemyBody.transform.parent = head.transform;
		enemyBody.transform.localPosition = Vector3.zero;
		enemyBody.transform.localRotation = prefBody.transform.rotation;
		enemyBody.transform.localScale = prefBody.transform.localScale;

		return head;
	}
}
