using UnityEngine;
using System.Collections;

public class MobAIItemShuttle : MobAI {

	GameObject[]	m_planeSpots;
	Vector3	m_start;
	Vector3	m_goal;
	Vector3	m_dir;
	float	m_droppedItemTime;
	float	m_elapsed = 0f;
	const float	m_dropFullTime = 10f;
	const float m_dropTryTime = 1f;
	const float	m_dropPosY = 8F;

	override public void	Init(Creature mob)
	{
		base.Init(mob);
		m_planeSpots = GameObject.FindGameObjectsWithTag("ItemShuttleSpot");
		m_mob.CreatureType = Creature.Type.Npc;

		m_navAgent.enabled = false;
		m_droppedItemTime = 0f;

		if (1 < m_planeSpots.Length)
		{
			int start = Random.Range(0, m_planeSpots.Length);
			int goal = start;
			
			while(start == goal)
			{
				goal = Random.Range(0, m_planeSpots.Length);
			}
			
			
			m_start = m_planeSpots[start].transform.position;
			m_goal = m_planeSpots[goal].transform.position;
			m_mob.transform.position = m_start;
			
			float targetHorAngle = Mathf.Atan2(m_goal.z-m_start.z, m_goal.x-m_start.x) * Mathf.Rad2Deg;
			m_mob.transform.eulerAngles = new Vector3(0, -targetHorAngle, 0);
			
			
		}
	}

	// Update is called once per frame
	override public void Update () {

		m_mob.transform.position = Vector3.MoveTowards(m_mob.transform.position, m_goal, m_mob.m_creatureProperty.MoveSpeed * Time.deltaTime);

		m_elapsed += Time.deltaTime;
		m_droppedItemTime += Time.deltaTime;

		if (m_elapsed >= m_dropTryTime)
		{
			if (0.1 > Vector3.Distance(m_mob.transform.position, m_goal))
			{
				m_mob.Death();
				return;
			}

			float ratio = Random.Range(0f, 1f);
			
			if (ratio < m_droppedItemTime/m_dropFullTime)
			{
				Vector3 pos = m_mob.transform.position;
				pos.y = m_dropPosY;
				Const.GetSpawn().SpawnItemBox(m_mob.RefDropItems, pos);
				m_droppedItemTime = 0f;
			}

			m_elapsed = 0f;
		}




	}

}
