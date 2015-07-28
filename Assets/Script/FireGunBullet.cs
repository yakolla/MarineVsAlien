using UnityEngine;
using System.Collections;

public class FireGunBullet : Bullet {

	[SerializeField]
	float	m_damageOnTime = 0.3f;

	float			m_lastDamageTime = 0f;
	int				m_lastFrame = 0;
	BoxCollider		m_collider;

	ParticleSystem	m_particleSystem;
	float			m_firingStartTime;

	Vector3			m_oriColliderCenter;
	Vector3			m_oriColliderSize;
	Vector3			m_oriScale;

	Weapon			m_weapon;

	void Awake()
	{
		m_collider = GetComponent<BoxCollider>();
		m_particleSystem = transform.Find("Body/Particle System").particleSystem;

		m_damageType = DamageDesc.Type.Fire;

		m_oriColliderCenter = m_collider.center;
		m_oriColliderSize = m_collider.size;
		m_oriScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () 
	{

		Vector3 scale = m_oriScale;
		scale.x *= 1f+m_ownerCreature.m_creatureProperty.BulletLength;
		transform.localScale = scale;

		m_particleSystem.startSpeed = scale.x*2;
		m_particleSystem.startSize = scale.x;

		float t = Mathf.Min(1f, (Time.time - m_firingStartTime)*1.2f);
		m_collider.center = new Vector3(m_oriColliderCenter.x*t, m_collider.center.y, m_collider.center.z);
		m_collider.size = new Vector3(m_oriColliderSize.x*t, m_collider.size.y, m_collider.size.z);

	}

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		Vector3 scale = transform.localScale;
		base.Init(ownerCreature, weapon, targetAngle);

		m_weapon = weapon;
		transform.parent = ownerCreature.WeaponHolder.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(new Vector3(0, targetAngle.angle, 0));
		transform.localScale = scale;

	}

	override public void StartFiring()
	{
		base.StartFiring();
		m_firingStartTime = Time.time;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		m_particleSystem.Clear();
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
