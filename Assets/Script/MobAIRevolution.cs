using UnityEngine;
using System.Collections;

public class MobAIRevolution  : MobAI {

	float time = 0f;
	WeaponHolder	m_weaponHolder;
	Vector3			m_oriPos;

	override public void Init(Creature mob)
	{
		base.Init(mob);
		m_oriPos = mob.transform.position;

		m_weaponHolder = mob.transform.Find("WeaponHolder").gameObject.GetComponent<WeaponHolder>();
	}

	// Update is called once per frame
	int Lerp(int start, int end, float time)
	{
		return (int)(start * (1-time) + end * time);
	}

	// Update is called once per frame
	override public void Update () {
		int angle  = Lerp(0, 360, time);
		
		m_weaponHolder.StartFiring(m_mob.RotateToTarget(angle));

		if (m_mob.AutoAttack() == false)
		{

		}

		Vector3 pos = Vector3.zero;
		pos.x = m_oriPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * 3f;
		pos.z = m_oriPos.z + Mathf.Sin(angle * Mathf.Deg2Rad) * 3f;
		
		m_mob.transform.position = pos;

		time += Time.deltaTime * 0.1f;
		time -= (int)time;
	}

}
