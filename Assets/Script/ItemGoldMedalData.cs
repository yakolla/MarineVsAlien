using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemGoldMedalData : ItemData{

	public ItemGoldMedalData(int count) : base(5, count)
	{

	}

	override public void Pickup(Creature obj){
		Warehouse.Instance.GoldMedal.Item.Count += Count;
	}
}
