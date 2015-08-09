using UnityEngine;
using System.Collections;

public class Melee : Weapon {

	MeleeBullet	m_bullet;

	override public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		Bullet bullet = base.CreateBullet(targetAngle, startPos);
		Vector3 scale = bullet.transform.localScale;
		scale.x = WeaponStat.range;
		bullet.transform.localScale = scale;

		return bullet;
	}
	
	override public void StartFiring(float targetAngle)
	{		
		if (isCoolTime() == true )
		{
			if (null == m_bullet)
			{
				m_bullet = CreateBullet(m_firingDescs[0], m_gunPoint.transform.position) as MeleeBullet;
			}
			else
			{
				playGunPointEffect();
				this.audio.Play();
				m_callbackCreateBullet();
			}
			m_bullet.Damage = Damage;
			m_bullet.SetCollider(true);
			DidStartFiring(0f);
		}
		else
		{			
			if (m_bullet != null)
				m_bullet.SetCollider(false);
		}

		if (null != m_bullet)
		{
			Vector3 euler = m_bullet.transform.rotation.eulerAngles;
			euler.y = transform.eulerAngles.y;
			m_bullet.transform.eulerAngles = euler;			
		}
		

		m_firing = true;
	}
	
	override public void StopFiring()
	{
		base.StopFiring();
		this.audio.Stop();
		if (m_bullet != null)
		{
			m_bullet.StopFiring();
			m_bullet.gameObject.SetActive(false);
		}
		
		stopGunPointEffect();
		
	}
	
	override public bool MoreFire()
	{
		if (m_firingDescs.Count == 0)
			return base.MoreFire();

		return false;
	}
	
	override public void LevelUp()
	{
		
	}
}
