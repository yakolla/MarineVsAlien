using UnityEngine;
using System.Collections;

public class MeleeBullet : Bullet {
	
	public float	m_damageOnTime = 0.3f;
	
	float			m_lastDamageTime = 0f;

	// Use this for initialization
	void Start () 
	{
		m_damageType = DamageDesc.Type.Normal;
		m_lastDamageTime = Time.time;
	}

	public void SetCollider(bool enable)
	{
		if (enable == false)
			return;

		RaycastHit hit;
		Vector3 fwd = transform.TransformDirection(Vector3.right);
		if (Physics.Raycast(transform.position, fwd, out hit, 5f, 1<<9))
		{
			Creature creature = hit.transform.gameObject.GetComponent<Creature>();
			if (creature && Creature.IsEnemy(creature, m_ownerCreature))
			{				
				float dist = Vector3.Distance(m_ownerCreature.transform.position, creature.transform.position);
				if (dist < 2f)
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

}
