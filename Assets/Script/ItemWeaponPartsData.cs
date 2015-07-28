using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeaponPartsData : ItemData{

	string 	m_weaponName;
	public ItemWeaponPartsData(int refItemId, int count) : base(refItemId, count)
	{
		m_weaponName = RefItem.codeName;
	}

	override public string Description()
	{
		return "<color=white>" + m_weaponName + "</color>" + "\n" +  base.Description();
	}
}
