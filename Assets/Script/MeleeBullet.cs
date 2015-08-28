using UnityEngine;
using System.Collections;

public class MeleeBullet : Bullet {
	
	public float	m_damageOnTime = 0.3f;

	BoxCollider		m_boxCollider;

	// Use this for initialization
	void Start () 
	{
		m_damageType = DamageDesc.Type.Normal;
		m_boxCollider = transform.GetComponent<BoxCollider>();
	}

	public void SetCollider(bool enable)
	{
		if (m_boxCollider != null)
			m_boxCollider.enabled = enable;

		if (enable == false)
			return;

		if (m_boxCollider == null)
		{
			RaycastHit hit;
			Vector3 fwd = transform.TransformDirection(Vector3.right);
			if (Physics.Raycast(transform.position, fwd, out hit, 3f, 1<<9))
			{
				Creature creature = hit.transform.gameObject.GetComponent<Creature>();
				if (creature && Creature.IsEnemy(creature, m_ownerCreature))
				{
					GiveDamage(creature);				
				}
			}
		}
	}
	
	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		base.Init(ownerCreature, weapon, targetAngle);
		
		transform.parent = ownerCreature.WeaponHolder.transform;
		transform.localPosition = Vector3.zero;

	}

	void OnTriggerEnter(Collider other) {
		
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
		}
	}
}
