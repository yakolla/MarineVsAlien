using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeaponPartsData : ItemData{

	string 	m_weaponName;
	public ItemWeaponPartsData(int refItemId) : base(refItemId, 1)
	{
		m_weaponName = RefItem.codeName;
	}

	override public string Description()
	{
		return "<color=white>" + m_weaponName + "</color>" + "\n" +  base.Description();
	}

	override public void Pickup(Creature obj)
	{
		base.Pickup(obj);
		Equip(obj);
	}
	
	override public void Equip(Creature obj)
	{
		base.Equip(obj);
		
		if (RefItem.partName != null)
		{
			string[] tokens = RefItem.partName.Split('_');

			obj.transform.Find("Body/"+tokens[0]+"_a").gameObject.SetActive(false);
			obj.transform.Find("Body/"+tokens[0]+"_b").gameObject.SetActive(false);

			obj.transform.Find("Body/"+RefItem.partName).gameObject.SetActive(true);
		}
	}
}
