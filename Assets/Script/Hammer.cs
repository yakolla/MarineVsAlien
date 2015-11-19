using UnityEngine;
using System.Collections;

public class Hammer : Weapon {

	override public bool MoreFire()
	{
		if (m_firingDescs.Count == 0)
			return base.MoreFire();

		return false;
	}
	
	override public void LevelUp()
	{
		++m_level;
	}

}
