using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeaponDNAData : ItemData{

	public ItemWeaponDNAData(int count) : base(4, count)
	{

	}

	override public void Pickup(Creature obj){
		Warehouse.Instance.WeaponDNA.Item.Count += Count;
	}

	override public string Description()
	{
		string desc = "Count:" + "<color=yellow>" + Count + "</color>";		

		return desc;
	}
}
