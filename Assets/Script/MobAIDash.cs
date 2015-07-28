using UnityEngine;
using System.Collections;

public class MobAIDash : MobAI {

	Vector3	m_goal;
	bool	m_breakMode = false;
	float	m_speed;
	GameObject	m_prefAttackGuidedLine;
	const float DashSpeed = 10f;
	float	m_dashTimeout = 0f;

	override public void Init(Creature mob)
	{
		base.Init(mob);

		m_mob.EnableNavMeshObstacleAvoidance(false);

		m_prefAttackGuidedLine = Resources.Load<GameObject>("Pref/ef laser point _enemy");
	}

	override public void SetTarget(Creature obj )
	{
		base.SetTarget(obj);

		if (obj != null)
			m_goal = obj.transform.position;


		float d = Vector3.Distance(m_mob.transform.position, m_goal);
		if (d < 10f)
		{
			d = 50f;
		}
		m_dashTimeout = d/DashSpeed + Time.time;
		m_navAgent.SetDestination(m_goal);
		m_speed = 0;
		m_breakMode = false;
		Const.GetSpawn().StartCoroutine(EffectAttackGudiedLine(m_mob.transform.position, m_goal, 0));

	}

	IEnumerator EffectAttackGudiedLine(Vector3 start, Vector3 goal, float t)
	{		
		float targetHorAngle = Mathf.Atan2(goal.z-start.z, goal.x-start.x) * Mathf.Rad2Deg;
		GameObject guidedLine = MonoBehaviour.Instantiate (m_prefAttackGuidedLine, start, Quaternion.Euler (0, -targetHorAngle, 0)) as GameObject;
		Vector3 scale = Vector3.one;
		scale.x = Vector3.Distance(start, goal)*2;
		guidedLine.transform.localScale = scale;
		while(t < 1f)
		{
			t += 0.01f;
			yield return null;
		}

		MonoBehaviour.DestroyObject(guidedLine);
	}

	// Update is called once per frame
	override public void Update () {

		if (TimeEffector.Instance.IsStop() == true)
			return;

		m_mob.AutoAttack();
		if (m_target)
		{
			if (m_breakMode == false)
			{
				m_speed = DashSpeed;
			}
			else
			{							
				m_speed = 0f;
				SetTarget(m_target);

			}
			m_mob.RotateToTarget(m_goal);
			m_navAgent.speed = m_speed;
			if (m_dashTimeout < Time.time)
			{
				m_breakMode = true;
			}

		}


	}


}
