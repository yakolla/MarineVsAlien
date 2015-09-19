using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobAIPickup : MobAI {

	List<GameObject> m_hitColliders = new List<GameObject>();
	Creature m_owner;
	float	m_searchableTime;

	public void SetOwner(Creature owner)
	{
		m_owner = owner;
	}
	// Update is called once per frame
	override public void Update () {

		if (m_owner == null)
			return;

		if (m_hitColliders.Count == 0 && m_searchableTime < Time.time)
		{
			Collider[] collider = Physics.OverlapSphere(m_mob.transform.position, 20, 1<<10);
			foreach(Collider col in collider)
			{
				m_hitColliders.Add(col.gameObject);
			}

			m_searchableTime = Time.time+1f;
		}

		GameObject target = m_owner.gameObject;

		if (m_hitColliders.Count > 0)
		{
			if (m_hitColliders[0] == null)
				m_hitColliders.RemoveAt(0);

			else
			{
				if (1f > Vector3.Distance(m_mob.transform.position, m_hitColliders[0].transform.position))
				{
					PickupItem(m_hitColliders[0]);
					m_hitColliders.RemoveAt(0);
				}
				else
				{
					target = m_hitColliders[0];
				}
			}

		}

		m_navAgent.SetDestination(target.transform.position);
		m_mob.RotateToTarget(target.transform.position);
	}

	void PickupItem(GameObject obj) {
		ItemBox itemBox = obj.GetComponent<ItemBox>();
		if (itemBox.ItemType == ItemData.Type.Skill)
			return;

		itemBox.StartPickupEffect(m_owner);
	}
}
