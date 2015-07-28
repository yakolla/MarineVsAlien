using UnityEngine;
using System.Collections;

public class PumpLauncher : Weapon {

	override public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		return base.CreateBullet(targetAngle, m_creature.transform.position);
	}

	override public void StartFiring(float targetAngle)
	{
		DidStartFiring(0f);
	}

	override public void StopFiring()
	{	
	}

	void Update()
	{
		if (isCoolTime())
		{
			base.StartFiring(0f);
		}
	}
}
