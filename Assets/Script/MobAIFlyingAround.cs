using UnityEngine;
using System.Collections;

public class MobAIFlyingAround  : MobAI {

	float elapsed = 0f;
	Vector3			m_oriPos;
	float	movingTime = 0f;

	override public void Init(Creature mob)
	{
		base.Init(mob);
		m_oriPos = mob.transform.position;
		elapsed = 0;
	}

	// Update is called once per frame
	override public void Update () {

		elapsed += Time.deltaTime;
		movingTime -= Time.deltaTime;

		if (elapsed > 30f)
		{
			m_mob.Death();
			return;
		}

		if (movingTime <= 0f)
		{
			Vector3 pos = Camera.main.transform.position * Random.Range(-3f, 3f);
			pos.y = m_mob.transform.position.y;
			pos.z = Mathf.Clamp(pos.z, m_oriPos.z-3f, m_oriPos.z+3f);
			pos.x = Mathf.Clamp(pos.x, Camera.main.transform.position.x-3f, Camera.main.transform.position.x+3f);

			m_navAgent.SetDestination(pos);

			movingTime = Random.Range(1f, 2f);
			
		}

	}

}
