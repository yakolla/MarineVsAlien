using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemXPPotionData : ItemData{

	public ItemXPPotionData(int count) : base(6, count)
	{

	}

	override public void Pickup(Creature obj){
		Equip(obj);
	}
	
	override public void Equip(Creature obj){
		Const.GetSpawn().SharePotinsChamps(obj, ItemData.Type.XPPotion, Count, false);
	}
}
