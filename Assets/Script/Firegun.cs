using UnityEngine;
using System.Collections;

public class Firegun : Weapon {

	FireGunBullet[]	m_bullet;
	float		m_accSp;

	override public void StartFiring(float targetAngle)
	{		
		if (m_creature.m_creatureProperty.SP > 0)
		{
			if (m_firing == false)
			{
				for(int i = 0; i < m_firingDescs.Count; ++i)
				{
					targetAngle = m_firingDescs[i].angle;
					if (m_bullet[i] == null)
					{
						m_bullet[i] = CreateBullet(m_firingDescs[i], m_gunPoint.transform.position) as FireGunBullet;
					}

					m_bullet[i].StartFiring();
					m_bullet[i].gameObject.SetActive(true);
					m_bullet[i].Damage = Damage;

					Vector3 euler = m_bullet[i].transform.rotation.eulerAngles;
					euler.y = transform.eulerAngles.y+targetAngle;
					m_bullet[i].transform.eulerAngles = euler;
				}

				this.audio.Play();
			}

			m_accSp += SP * Time.deltaTime;
			if (m_accSp >= 1)
			{
				m_creature.m_creatureProperty.SP -= (int)m_accSp;
				m_accSp -= (int)m_accSp;
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
			foreach(Bullet bullet in m_bullet)
			{
				if (bullet == null)
					continue;

				bullet.StopFiring();				
				bullet.gameObject.SetActive(false);
			}
		}

	}



	override public void LevelUp()
	{
		base.LevelUp();

		m_bullet = new FireGunBullet[m_firingDescs.Count];
		
	}
}
