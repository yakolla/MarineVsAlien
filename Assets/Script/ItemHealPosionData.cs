using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemHealPosionData : ItemData{

	public ItemHealPosionData(int heal) : base(2, heal)
	{
	}

	override public void Pickup(Creature obj){
		Equip(obj);

	}

	override public void Equip(Creature obj){
		Const.GetSpawn().SharePotinsChamps(obj, ItemData.Type.HealPosion, Count, true);
	}

}
