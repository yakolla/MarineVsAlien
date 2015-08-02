using UnityEngine;
using System.Collections;

public class ItemGoldData : ItemData{

	public ItemGoldData(int gold) : base(1, gold)
	{

	}

	override public void Pickup(Creature obj){
		Warehouse.Instance.Gold.Item.Count += Count;

	}

	override public void Equip(Creature obj){

	}

}
