using UnityEngine;
using System.Collections;

public class MobAIFollow : MobAI {

	Creature m_owner;

	public void SetOwner(Creature owner)
	{
		m_owner = owner;
	}
	// Update is called once per frame
	override public void Update () {

		if (TimeEffector.Instance.IsStop() == true)
			return;

		if (m_mob.AutoAttack() == false)
		{
			if (m_target)
			{
				if (5f < Vector3.Distance(m_owner.transform.position, m_target.transform.position))
				{
					m_navAgent.Stop();
					SetTarget(null);
				}
				else
				{
					m_navAgent.SetDestination(m_target.transform.position);
					m_mob.RotateToTarget(m_target.transform.position);
				}

			}
			else
			{
				if (m_owner != null)
					m_navAgent.SetDestination(m_owner.transform.position);
			}
		}
		else
			m_navAgent.Stop();

	}


}
