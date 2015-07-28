using UnityEngine;
using System.Collections;

public class SummonMobLauncher : Weapon {

	override public void LevelUp()
	{
		++m_level;
		MoreFire();
	}
}
