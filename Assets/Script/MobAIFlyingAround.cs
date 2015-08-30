using UnityEngine;
using System.Collections;

public class MobAIFlyingAround  : MobAI {

	float time = 0f;
	Vector3			m_oriPos;

	override public void Init(Creature mob)
	{
		base.Init(mob);
		m_oriPos = mob.transform.position;
		time = Time.time;
	}

	// Update is called once per frame
	int Lerp(int start, int end, float time)
	{
		return (int)(start * (1-time) + end * time);
	}

	// Update is called once per frame
	override public void Update () {
		int angle  = Lerp(0, 360, time);

		if (m_mob.AutoAttack() == false)
		{

		}

		if (time+10f < Time.time)
		{
			m_mob.Death();
		}
	}

}
