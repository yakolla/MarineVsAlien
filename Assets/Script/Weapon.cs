using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

	[System.Serializable]
	public struct FiringDesc
	{
		public float 	angle;
		public float	delayTime;
	}

	[SerializeField]
	public List<FiringDesc>		m_firingDescs = new List<FiringDesc>();

	protected GameObject		m_gunPoint;

	[SerializeField]
	GameObject					m_prefBullet = null;

	[SerializeField]
	GameObject					m_prefGunPointEffect = null;
	ParticleSystem				m_gunPointEffect;

	protected	bool			m_firing = false;

	protected float				m_lastCreated = 0;
	protected Creature 			m_creature;

	[SerializeField]
	float						m_damageRatio = 1f;

	WeaponStat					m_weaponStat;

	Weapon m_subWeapon;

	public delegate void CallbackOnCreateBullet();
	public CallbackOnCreateBullet	m_callbackCreateBullet = delegate(){};


	int					m_maxLevel;
	protected int		m_level;
	protected int		m_evolution;
	RefItem				m_refWeaponItem;


	protected void Start()
	{
		if (m_prefGunPointEffect != null)
		{
			GameObject obj = Instantiate (m_prefGunPointEffect, Vector3.zero, transform.rotation) as GameObject;
			
			obj.transform.parent = transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = m_prefGunPointEffect.transform.localScale;
			obj.transform.localRotation = m_prefGunPointEffect.transform.localRotation;
			m_gunPointEffect = obj.GetComponent<ParticleSystem>();

		}
	}


	public virtual void Init(Creature creature, ItemWeaponData weaponData, RefMob.WeaponDesc weaponDesc)
	{
		m_creature = creature;
		m_gunPoint = creature.WeaponHolder.gameObject;
		m_refWeaponItem = weaponData.RefItem;

		m_lastCreated = Time.time;
		m_firing = false;
		m_level = 0;
		m_maxLevel = weaponDesc.maxLevel;

		m_weaponStat = new WeaponStat();
		if (weaponDesc.weaponStat == null)
		{
			m_weaponStat.OverrideStat(m_refWeaponItem.weaponStat);
		}
		else
		{
			m_weaponStat.OverrideStat(weaponDesc.weaponStat);
			m_weaponStat.OverrideStat(m_refWeaponItem.weaponStat);

		}
	
		for(int i = 0; i <= m_weaponStat.firingCount; ++i)
			MoreFire();

		m_evolution = weaponData.Evolution;

		if (m_evolution > 0)
		{
			for(int i = 1; i <= m_maxLevel; ++i)
			{
				if (canCreateMoreFire(i))
					MoreFire();
			}
		}

		for(int i = 0; i < weaponData.Level; ++i)
			LevelUp();

	}

	virtual public bool MoreFire()
	{
		if (m_refWeaponItem.evolutionFiring == null)
		{
			if (m_firingDescs.Count == 0)
			{
				m_firingDescs.Add(new Weapon.FiringDesc());
				return true;
			}

			return false;
		}

		int count = m_firingDescs.Count;

		if (count > Const.MaxFiringCount)
			return false;

		float angle = m_refWeaponItem.evolutionFiring.angle*((count+1)/2);
		if (count % 2 == 1)
		{
			angle *= -1;
		}
		
		float delay = m_refWeaponItem.evolutionFiring.delay*count;
		
		
		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = angle;
		desc.delayTime = delay;
		
		m_firingDescs.Add(desc);

		return true;
	}


	virtual public void LevelUp()
	{
		++m_level;

		if (canCreateMoreFire(m_level))
		{
			MoreFire();
		}
	}

	bool canCreateMoreFire(int lv)
	{
		return lv > 1 && lv % WeaponStat.incBulletOnLevel == 0;
	}

	public void EvolutionUp()
	{
		++m_evolution;
		m_level = 1;
	}

	public RefItem RefWeaponItem
	{
		get {return m_refWeaponItem;}
	}

	public int SP
	{
		get{return (int)(m_refWeaponItem.consumedSP*Level);}
	}

	public int Damage
	{
		get {return GetDamage(m_creature.m_creatureProperty);}
	}

	public int GetDamage(CreatureProperty pro)
	{
		return GetDamage(pro, m_evolution);
	}

	public int GetDamage(CreatureProperty pro, int evolution)
	{
		int damage = (int)((pro.PhysicalAttackDamage+(Level+m_maxLevel*evolution))*m_damageRatio);
		return (int)(damage + damage*pro.DamageMultiPlier);
	}

	protected void playGunPointEffect()
	{
		if (m_gunPointEffect != null)
		{
			m_gunPointEffect.gameObject.SetActive(true);
			if (m_gunPointEffect.isPaused || m_gunPointEffect.isStopped)
			{
				m_gunPointEffect.Play();
			}
			
		}
	}

	protected void stopGunPointEffect()
	{
		if (m_gunPointEffect != null)
		{
			m_gunPointEffect.gameObject.SetActive(false);
			m_gunPointEffect.Stop();
		}
	}

	public void ConsumeSP()
	{
		m_creature.ConsumeSP(SP);
	}

	public Vector3 GunPointPos
	{
		get{return m_gunPoint.transform.position;}
	}

	virtual public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		GameObject obj = GameObjectPool.Instance.Alloc(m_prefBullet, startPos, Quaternion.Euler(0, transform.rotation.eulerAngles.y+targetAngle.angle, 0));
		Bullet bullet = obj.GetComponent<Bullet>();
		bullet.Init(m_creature, this, targetAngle);
		obj.transform.localScale = m_prefBullet.transform.localScale;

		playGunPointEffect();

		this.audio.Play();

		m_callbackCreateBullet();

		return bullet;
	}

	protected IEnumerator DelayToStartFiring(Weapon.FiringDesc targetAngle, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (canConsumeSP())
		{
			CreateBullet(targetAngle, m_gunPoint.transform.position);
			ConsumeSP();
		}

	}

	protected float coolDownTime()
	{
		return m_weaponStat.coolTime*m_creature.m_creatureProperty.AttackCoolTime;
	}

	public bool canConsumeSP()
	{
		return SP == 0 || SP < m_creature.m_creatureProperty.SP;
	}

	protected bool isCoolTime()
	{
		bool coolTime = m_lastCreated +  coolDownTime() <= Time.time;
		bool sp = canConsumeSP();
		return coolTime && sp;
	}

	protected float remainCoolTimeRatio()
	{
		float cool = coolDownTime();
		return Mathf.Min(1f, 1f-((m_lastCreated + cool)-Time.time)/cool);
	}

	protected void DidStartFiring(float delay)
	{
		m_lastCreated = Time.time+delay;
	}

	virtual public void StartFiring(float targetAngle)
	{		
		if ( isCoolTime() == true )
		{
			float oriAng = targetAngle;
			float delay = 0f;
			for(int i = 0; i < m_firingDescs.Count; ++i)
			{
				float ang = m_firingDescs[i].angle-oriAng;
				targetAngle = ang;
				//targetAngle.y = m_firingDescs[i].angle;
				StartCoroutine(DelayToStartFiring(m_firingDescs[i], m_firingDescs[i].delayTime));
				delay = m_firingDescs[i].delayTime;
			}
			
			DidStartFiring(delay);
		}

		m_firing = true;
	}

	virtual public void StopFiring()
	{
		m_firing = false;

		if (m_gunPointEffect != null)
		{
			//m_gunPointEffect.gameObject.SetActive(false);
			//m_gunPointEffect.Stop();
		}

	}


	public WeaponStat WeaponStat
	{
		get { return m_weaponStat; }
	}

	public void SetSubWeapon(Weapon weapon)
	{
		if (m_subWeapon != null)
		{
			GameObject.DestroyObject(m_subWeapon.gameObject);
		}

		m_subWeapon = weapon;
	}

	public Weapon GetSubWeapon()
	{
		return m_subWeapon;
	}

	public int Level
	{
		get {return m_level;}
	}

	public int Evolution
	{
		get {return m_evolution;}
	}
}

