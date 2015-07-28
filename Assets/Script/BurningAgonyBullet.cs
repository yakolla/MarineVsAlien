using UnityEngine;
using System.Collections;

public class BurningAgonyBullet : Bullet {

	[SerializeField]
	float	m_damageOnTime = 0.3f;

	float			m_lastDamageTime = 0f;
	int				m_lastFrame = 0;
	BoxCollider		m_collider;
	float			m_firingStartTime;
	float			m_oriSize;
	float			m_particleOriSize;

	Weapon			m_weapon;
	float			m_bombRange;

	ParticleSystem	m_particle;

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		Vector3 scale = transform.localScale;
		base.Init(ownerCreature, weapon, targetAngle);

		m_weapon = weapon;
		transform.parent = ownerCreature.WeaponHolder.transform;
		Vector3 pos = ownerCreature.transform.position;
		pos.y = ownerCreature.WeaponHolder.transform.localPosition.y;
		transform.position = pos;
		transform.localRotation = Quaternion.Euler(new Vector3(0, targetAngle.angle, 0));
		transform.localScale = scale;

		m_collider = GetComponent<BoxCollider>();
		m_oriSize = m_collider.size.x;

		m_particle = transform.Find("Body/Particle System").GetComponent<ParticleSystem>();
		m_particleOriSize = m_particle.startSize;
	}


	override public void StartFiring()
	{
		base.StartFiring();
		m_firingStartTime = Time.time;
	}
	
	override public void StopFiring()
	{
		base.StopFiring();
	}

	public float BombRange
	{
		set{
			m_bombRange = value;

			Vector3 scale = m_collider.size;
			scale.x = m_oriSize+m_bombRange;
			scale.z = m_oriSize+m_bombRange;

			m_collider.size = scale;
			m_particle.startSize = m_particleOriSize+m_bombRange*2;

		}
		get{return m_bombRange;}
	}
	
	void OnTriggerStay(Collider other) 
	{
		if (m_lastDamageTime+(m_damageOnTime*m_ownerCreature.m_creatureProperty.AttackCoolTime)<Time.time)
		{
			m_lastFrame = Time.frameCount;
			m_lastDamageTime = Time.time;
			
			TryToSetDamageBuffType(m_weapon);
			
		}
		
		if (m_lastFrame == Time.frameCount)
		{
			Creature creature = other.gameObject.GetComponent<Creature>();
			if (creature && Creature.IsEnemy(creature, m_ownerCreature))
			{
				GiveDamage(creature);
			}
		}
	}
}
