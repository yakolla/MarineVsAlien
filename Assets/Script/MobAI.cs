using UnityEngine;
using System.Collections;

public class MobAI {

	protected Creature				m_mob;
	protected Creature 				m_target;
	protected NavMeshAgent			m_navAgent;

	virtual public void	Init(Creature mob)
	{
		m_mob = mob;
		m_navAgent = mob.GetComponent<NavMeshAgent>();
	}

	virtual public void SetTarget(Creature obj )
	{
		m_target = obj;
	}
	
	// Update is called once per frame
	virtual public void Update () {

	}

	virtual public void OnTriggerEnter(Collider other) {

	}
}
