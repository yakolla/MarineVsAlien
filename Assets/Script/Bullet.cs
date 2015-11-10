using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {

	protected bool 			m_isDestroying = false;
	protected Vector3		m_gunPoint;
	bool					m_firing = false;
	int						m_damage;
	protected 				Weapon.FiringDesc		m_targetAngle;
	protected				Creature	m_ownerCreature;

	Weapon					m_onHitWeapon;

	[SerializeField]
	GameObject 				m_prefDamageEffect = null;

	protected DamageDesc.Type	m_damageType = DamageDesc.Type.Normal;
	protected DamageDesc.BuffType m_damageBuffType = DamageDesc.BuffType.Nothing;

	virtual public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		m_gunPoint = weapon.GunPointPos;
		m_ownerCreature = ownerCreature;
		m_damage = weapon.Damage;
		m_targetAngle = targetAngle;
		m_isDestroying = false;
		m_onHitWeapon = weapon.GetSubWeapon();

		TryToSetDamageBuffType(weapon);


		StartFiring();
	}

	public void TryToSetDamageBuffType(Weapon weapon)
	{
		m_damageBuffType = DamageDesc.BuffType.Nothing;

		if (Random.Range(0, 1f) < weapon.WeaponStat.buffOnHitDesc.chance)
		{
			m_damageBuffType = weapon.WeaponStat.buffOnHitDesc.buffType;
		}	
	}

	static public Creature[] SearchTarget(Vector3 pos, Creature.Type targetTags, float range)
	{
		return SearchTarget(pos, targetTags, range, null);
	}

	static public Creature[] SearchTarget(Vector3 pos, Creature.Type targetTags, float range, Creature[] skip)
	{
		
		Collider[] hitColliders = Physics.OverlapSphere(pos, range, 1<<9);
		if (hitColliders.Length == 0)
			return null;
		
		Creature[] testSearchedTargets = new Creature[hitColliders.Length];
		int i = 0;
		int searchedCount = 0;
		while (i < hitColliders.Length) {
			
			Creature creature = hitColliders[i].gameObject.GetComponent<Creature>();
			if (creature != null)
			{
				if (targetTags == creature.CreatureType)
				{
					bool already = false;

					if (skip != null)
					{
						for(int y = 0; y < skip.Length; ++y)
						{
							if (creature == skip[y])
							{
								already = true;
								break;
							}
						}
					}

					if (already == false)
					{
						testSearchedTargets[searchedCount] = creature;
						++searchedCount;
					}

				}
			}
			
			i++;
		}
		
		if (searchedCount == 0)
			return null;
		
		Creature[] searchead = new Creature[searchedCount];
		for(i = 0; i < searchedCount; ++i)
			searchead[i] = testSearchedTargets[i];
		
		return searchead;
	}

	IEnumerator destoryBombObject(GameObject bombEffect, float duration)
	{
		yield return new WaitForSeconds (duration);
		GameObjectPool.Instance.Free(this.gameObject);
		DestroyObject(bombEffect);
	}

	virtual protected void bomb(float bombRange, GameObject prefBombEffect)
	{
		m_isDestroying = true;
		bombRange += m_ownerCreature.m_creatureProperty.SplashRadius;

		Creature[] searchedTargets = SearchTarget(transform.position, m_ownerCreature.GetMyEnemyType(), bombRange*0.5f);
		if (searchedTargets != null)
		{
			foreach(Creature creature in searchedTargets)
			{
				GiveDamage(creature);
			}
		}

		if (m_onHitWeapon != null)
		{
			if(m_onHitWeapon.canConsumeSP())
			{
				m_onHitWeapon.CreateBullet(m_targetAngle, transform.position);
				m_onHitWeapon.ConsumeSP();
			}
		}

		Vector3 bombPos = transform.position;
		bombPos.y = prefBombEffect.transform.position.y;
		
		GameObject bombEffect = (GameObject)Instantiate(prefBombEffect, bombPos, prefBombEffect.transform.rotation);
		float duration = ParticleScale(bombEffect, bombRange);

		this.audio.Play();
		StartCoroutine(destoryBombObject(bombEffect, duration));
	}

	static public float ParticleScale(GameObject obj, float size)
	{
		ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>();

		float duration = 0;
		foreach(ParticleSystem ps in particleSystems)
		{
			ps.maxParticles = (int)size;
			ps.startSize *= size;
			if (duration < ps.duration)
				duration = ps.duration;
		}

		return duration;
	}

	virtual public void StartFiring()
	{
		m_firing = true;

	}

	virtual public void StopFiring()
	{
		m_firing = false;
	}

	public bool IsFiring()
	{
		return m_firing;
	}

	protected void GiveDamage(Creature target)
	{
		target.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, m_damageType, m_damageBuffType, PrefDamageEffect));
	}

	public int Damage
	{
		set {m_damage = value;}
	}

	protected GameObject PrefDamageEffect
	{
		get {return m_prefDamageEffect;}
	}

}
