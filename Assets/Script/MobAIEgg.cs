using UnityEngine;
using System.Collections;

public class MobAIEgg : MobAI {

	Parabola 	m_parabola;
	
	float[]		m_hSpeed = {5f, 7f};
	float[]		m_vRadian = {-3.14f, 3.14f};
	float[]		m_hRadian = {1.3f, 1.5f};
	bool		m_ingDeathEffect;

	override public void	Init(Creature mob)
	{
		base.Init(mob);

		m_parabola = new Parabola(mob.gameObject, 
		                          Random.Range(m_hSpeed[0], m_hSpeed[1]), 
		                           Random.Range(m_vRadian[0], m_vRadian[1]), 
		                          Random.Range(m_hRadian[0], m_hRadian[1]), 
		                          1);
	}

	IEnumerator EffectEgg()
	{
		yield return new WaitForSeconds (3f);		
		m_mob.Death();

	}

	// Update is called once per frame
	override public void Update () {

		if (false == m_parabola.Update())
		{
			if (m_ingDeathEffect == false)
			{
				m_ingDeathEffect = true;

				m_mob.StartCoroutine(EffectEgg());
			}
		}

		if (m_mob.AutoAttack() == false)
		{
			if (m_target)
			{
				m_mob.RotateToTarget(m_target.transform.position);
			}
		}
	}

}
