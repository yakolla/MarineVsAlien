using UnityEngine;
using System.Collections;

public class Gun : Weapon {

	[SerializeField]
	float		m_minBulletSpeed = 3f;

	[SerializeField]
	float		m_maxBulletSpeed = 10f;

	float		m_bulletSpeed = 3f;

	override public void LevelUp()
	{
		base.LevelUp();

		if (Level % WeaponStat.incBulletOnLevel == 0)
		{
			m_bulletSpeed = m_minBulletSpeed+(m_maxBulletSpeed-m_minBulletSpeed)/(float)(Level/RefItem.maxLevel);
			m_bulletSpeed = Mathf.Min(m_minBulletSpeed, m_maxBulletSpeed);
		}
	}

	override public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		GunBullet bullet = base.CreateBullet(targetAngle, startPos) as GunBullet;

		bullet.BulletSpeed = m_bulletSpeed;

		return bullet;
	}
}
