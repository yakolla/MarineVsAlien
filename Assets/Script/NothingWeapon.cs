using UnityEngine;
using System.Collections;

public class NothingWeapon : Weapon {
	override public void StartFiring(float targetAngle)
	{
		DidStartFiring(0f);
	}
}
