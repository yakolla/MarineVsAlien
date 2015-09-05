using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningLauncher : Weapon {

	LightningBullet	m_bullet;
	int		m_maxChaining = 1;
	float		m_fadeOutEffect = 0f;
	float		m_accSp;

	override public void StartFiring(float targetAngle)
	{		
		if (m_creature.m_creatureProperty.SP > 0)
		{
			if (null == m_bullet)
			{
				m_bullet = CreateBullet(m_firingDescs[0], m_gunPoint.transform.position) as LightningBullet;
				
				StartCoroutine(delayStopFiring());
			}
			m_bullet.Damage = Damage;
			m_bullet.gameObject.SetActive(true);
			m_bullet.MaxChaining = m_maxChaining;

			if (this.audio.isPlaying == false)
				this.audio.Play();

			m_accSp += SP * Time.deltaTime;
			if (m_accSp >= 1)
			{
				m_creature.ConsumeSP((int)m_accSp);
				m_accSp -= (int)m_accSp;
			}
		}
		else
		{
			StopFiring();
			return;
		}

		if (null != m_bullet)
		{
			Vector3 euler = m_bullet.transform.rotation.eulerAngles;
			euler.y = transform.eulerAngles.y;
			m_bullet.transform.eulerAngles = euler;
		}

		playGunPointEffect();
		m_firing = true;
	}

	IEnumerator delayStopFiring()
	{
		bool finished = false;
		while(true)
		{
			if (m_firing == true)
				finished = false;

			if(m_firing == false && finished == false && m_fadeOutEffect <= 0f)
			{
				this.audio.Stop();
				if (m_bullet != null)
				{
					m_bullet.StopFiring();
					m_bullet.gameObject.SetActive(false);
				}
				
				//stopGunPointEffect();
				finished = true;
			}

			m_fadeOutEffect -= Time.deltaTime;

			yield return null;
		}
	}
	
	override public void StopFiring()
	{
		if (m_firing == true)
			m_fadeOutEffect = 0.3f;

		base.StopFiring();


	}

	override public bool MoreFire()
	{
		if (false == base.MoreFire())
			return false;
		
		m_maxChaining += 1;
		return true;
	}

}
