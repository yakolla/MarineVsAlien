using UnityEngine;
using System.Collections;

public class GuidedRocketLauncher : Weapon {

	int	m_aliveBullets = 0;

	public override void Init(Creature creature, ItemWeaponData weaponData, WeaponStat weaponStat)
	{
		base.Init(creature, weaponData, weaponStat);

		m_aliveBullets = 0;
	}

	override public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		if (m_aliveBullets >= m_firingDescs.Count)
			return null;

		++m_aliveBullets;
		return base.CreateBullet(targetAngle, startPos);

	}

	public void OnDestroyBullet()
	{
		--m_aliveBullets;
	}
}
