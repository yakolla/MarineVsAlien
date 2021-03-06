using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropShip : MonoBehaviour {

	Champ	m_champ;

	public void SetChamp(Champ champ)
	{
		m_champ = champ;

	}

	void OnDropChamp()
	{
		m_champ.gameObject.SetActive(true);
		m_champ.transform.position = new Vector3(transform.position.x, m_champ.transform.position.y, transform.position.z);

		Warehouse.Instance.Items[ItemData.Type.Follower].ForEach(item=>{
			if (item.Item.Level > 0)
			{
				item.Item.Equip(m_champ);
			}
		});
	}
}

