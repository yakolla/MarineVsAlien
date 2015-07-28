using UnityEngine;
using System.Collections;

public class BurningAgonyLauncher : Weapon {

	BurningAgonyBullet	m_bullet;


	override public void StartFiring(float targetAngle)
	{		
		if (canConsumeSP() == true )
		{
			if (m_firing == false)
			{
				if (m_bullet == null)
				{
					m_bullet = CreateBullet(m_firingDescs[0], m_gunPoint.transform.position) as BurningAgonyBullet;
					m_bullet.BombRange = (Level-1);
				}
				
				m_bullet.StartFiring();
				m_bullet.gameObject.SetActive(true);
				m_bullet.Damage = Damage;
				
				Vector3 euler = m_bullet.transform.rotation.eulerAngles;
				euler.y = transform.eulerAngles.y+targetAngle;
				m_bullet.transform.eulerAngles = euler;
				
				this.audio.Play();
			}
		}
		else
		{
			StopFiring();
			return;
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
		
	}

	override public void LevelUp()
	{
		base.LevelUp();

		if (m_bullet != null)
		{
			m_bullet.BombRange = (Level-1);
		}
	}
}
