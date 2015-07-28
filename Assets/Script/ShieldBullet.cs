using UnityEngine;
using System.Collections;

public class ShieldBullet : Bullet {

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		Vector3 scale = transform.localScale;
		base.Init(ownerCreature, weapon, targetAngle);
		
		transform.parent = ownerCreature.WeaponHolder.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(new Vector3(0, targetAngle.angle, 0));
		transform.localScale = scale;
		
	}
}
