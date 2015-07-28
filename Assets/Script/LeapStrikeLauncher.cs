using UnityEngine;
using System.Collections;

public class LeapStrikeLauncher : Weapon {

	Parabola	m_parabola;

	EffectTargetingCircle	m_effectTargetingPoint = null;


	override public void StartFiring(float targetAngle)
	{
		if ( isCoolTime() == true && m_creature.Targetting != null)
		{
			float d = Vector3.Distance(m_creature.transform.position, m_creature.Targetting.transform.position);

			m_parabola = new Parabola(m_creature.gameObject, d*1.7f, targetAngle*Mathf.Deg2Rad, 70*Mathf.Deg2Rad, 1);
			m_parabola.TimeScale = 0.80f;

			m_effectTargetingPoint = new EffectTargetingCircle();
			m_effectTargetingPoint.Init(m_parabola.DestPosition);

			m_creature.CrowdControl(Creature.CrowdControlType.LeapStrike, true);

			DidStartFiring(0f);
		}
		m_firing = true;
	}
	
	void Update()
	{
		if (m_parabola != null && false == m_parabola.Update())
		{
			m_creature.CrowdControl(Creature.CrowdControlType.LeapStrike, false);
			CreateBullet(m_firingDescs[0], transform.position);
			m_parabola = null;
			m_effectTargetingPoint.Death();
		}
	}
}
