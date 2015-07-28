using UnityEngine;
using System.Collections;

public class RushBullet : Bullet {
	
	// Use this for initialization
	void Start () 
	{		

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
