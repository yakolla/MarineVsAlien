using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeaponPartsData : ItemData{

	public ItemWeaponPartsData(int refItemId) : base(refItemId, 1)
	{
	}

	override public string Description()
	{
		return "<color=white>" + RefItem.name + "</color>" + "\n" +  base.Description();
	}

	override public void Pickup(Creature obj)
	{
		base.Pickup(obj);
		ItemObject itemObj = Warehouse.Instance.FindItem(RefItem.id);

		if (itemObj.Item.Lock == true)
		{
			itemObj.Item.Lock = false;
			itemObj.Item.Level = 1;
			Equip(obj);
		}
		else
		{
			itemObj.Item.Level = Mathf.Min(RefItem.maxLevel, itemObj.Item.Level+1);
			Use(obj);
		}
	}

	override public void Use(Creature obj)
	{	
		ApplyOptions(obj, true);
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
