using UnityEngine;
using System.Collections;

public class Gun : Weapon {

	[SerializeField]
	float		m_bulletSpeed = 3f;

	[SerializeField]
	float		m_maxBulletSpeed = 10f;

	override public void LevelUp()
	{
		base.LevelUp();

		if (Level % 2 == 1)
		{
			m_bulletSpeed += Level/2*2;
			m_bulletSpeed = Mathf.Min(m_bulletSpeed, m_maxBulletSpeed);
		}
	}

	override public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		GunBullet bullet = base.CreateBullet(targetAngle, startPos) as GunBullet;

		bullet.BulletSpeed = m_bulletSpeed;

		return bullet;
	}
}
