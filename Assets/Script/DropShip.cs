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

		foreach(ItemObject itemFollowerObject in Warehouse.Instance.Items[ItemData.Type.Follower])
		{
			if (itemFollowerObject.Item.Level > 0)
			{
				itemFollowerObject.Item.Equip(m_champ);
			}
		}
	}
}

