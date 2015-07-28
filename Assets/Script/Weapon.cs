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



	protected int		m_level;

	RefItem				m_refItem;


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


	public virtual void Init(Creature creature, ItemWeaponData weaponData, WeaponStat weaponStat)
	{
		m_creature = creature;
		m_gunPoint = creature.WeaponHolder.gameObject;
		m_refItem = weaponData.RefItem;

		Weapon.FiringDesc desc = DefaultFiringDesc();
		m_firingDescs.Clear();
		m_firingDescs.Add(desc);

		m_lastCreated = Time.time;
		m_firing = false;
		m_level = 0;
		m_weaponStat = new WeaponStat();
		if (weaponStat == null)
		{
			m_weaponStat.OverrideStat(m_refItem.weaponStat);
		}
		else
		{
			m_weaponStat.OverrideStat(weaponStat);
			m_weaponStat.OverrideStat(m_refItem.weaponStat);
		}
	
		for(int i = 0; i < m_weaponStat.firingCount; ++i)
			MoreFire();

		for(int i = 0; i < weaponData.Level; ++i)
			LevelUp();
	}

	virtual protected Weapon.FiringDesc DefaultFiringDesc()
	{
		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = 0;
		desc.delayTime = 0;

		return desc;
	}

	virtual public bool MoreFire()
	{
		if (m_refItem.evolutionFiring == null)
			return false;

		int count = m_firingDescs.Count;

		if (count > Const.MaxFiringCount)
			return false;

		float angle = m_refItem.evolutionFiring.angle*((count+1)/2);
		if (count % 2 == 1)
		{
			angle *= -1;
		}
		
		float delay = m_refItem.evolutionFiring.delay*count;
		
		
		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = angle;
		desc.delayTime = delay;
		
		m_firingDescs.Add(desc);

		return true;
	}


	virtual public void LevelUp()
	{
		++m_level;
		if (m_level % 2 == 0)
		{
			MoreFire();
		}
		else
		{

		}

	}

	public RefItem RefItem
	{
		get {return m_refItem;}
	}

	public int SP
	{
		get{return GetSP(m_refItem, Level);}
	}

	static public int GetSP(RefItem refItem, int level)
	{
		return refItem.weaponStat.spPerLevel*level;
	}

	public int Damage
	{
		get {return GetDamage(m_creature.m_creatureProperty);}
	}

	public int GetDamage(CreatureProperty pro)
	{
		return (int)(pro.PhysicalAttackDamage*m_damageRatio);
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
		m_creature.m_creatureProperty.SP -= SP;
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

		CreateBullet(targetAngle, m_gunPoint.transform.position);

	}

	protected float coolDownTime()
	{
		const float maxCool = 0.5f;
		float levelRatio = (m_level-1)/(float)Const.MaxItemLevel;
		float coolPerLevel = (1-levelRatio)*1 + levelRatio*maxCool;
		return m_weaponStat.coolTime*m_creature.m_creatureProperty.AttackCoolTime*coolPerLevel;
	}

	public bool canConsumeSP()
	{
		return SP <= m_creature.m_creatureProperty.SP;
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
			
			ConsumeSP();
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
}

