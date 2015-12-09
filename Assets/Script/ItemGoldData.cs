using UnityEngine;
using System.Collections;

public class ItemGoldData : ItemData{

	public ItemGoldData(long gold) : base(1, gold)
	{

	}

	override public void Pickup(Creature obj){
		Warehouse.Instance.Gold.Item.Count += Count;

	}

}
