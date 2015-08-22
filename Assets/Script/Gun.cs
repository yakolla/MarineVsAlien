using UnityEngine;
using System.Collections;

public class Gun : Weapon {

	[SerializeField]
	float		m_bulletSpeed = 3f;

	override public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		GunBullet bullet = base.CreateBullet(targetAngle, startPos) as GunBullet;

		bullet.BulletSpeed = m_bulletSpeed;

		return bullet;
	}
}
