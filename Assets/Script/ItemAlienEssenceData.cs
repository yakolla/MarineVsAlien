using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemAlienEssenceData : ItemData{

	public ItemAlienEssenceData(int count) : base(11, count)
	{

	}

	override public void Pickup(Creature obj){
		Warehouse.Instance.AlienEssence.Item.Count += Count;
	}
}
