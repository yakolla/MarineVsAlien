using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemWeaponData : ItemData{


	public ItemWeaponData(int refItemId) : base(refItemId, 1)
	{

	}

	[JsonIgnore]
	public GameObject PrefWeapon
	{
		get {return Resources.Load<GameObject>("Pref/Weapon/" + RefItem.codeName);}
	}

	public string WeaponName
	{
		get{return RefItem.name;}
	}


	override public void Equip(Creature obj)
	{
		obj.EquipWeapon(this, null);

		base.Equip(obj);
	}

	override public void Use(Creature obj)
	{	
		ApplyOptions(obj, true);
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Weapon)
			return base.Compare(item);

		ItemWeaponData weapon = (ItemWeaponData)item;
		return WeaponName.CompareTo(weapon.WeaponName) == 0;
	}

}
