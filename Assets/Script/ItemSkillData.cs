using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemSkillData : ItemData{

	[JsonConstructor]
	private ItemSkillData()
	{
	}

	public ItemSkillData(int refItemId) : base(refItemId, 1)
	{

	}

	override public void Pickup(Creature obj)
	{
		base.Pickup(obj);
		Use (obj);
	}

	override public void Use(Creature obj)
	{
		if (obj.WeaponHolder.GetActiveSkillWeapon(RefItem.weaponId) == false)
		{
			obj.EquipActiveSkillWeapon(new ItemWeaponData(RefItem.weaponId), null);
		}
		obj.WeaponHolder.ActiveWeaponSkillFire(RefItem.weaponId, obj.transform.eulerAngles.y);
	}

	override public void NoUse(Creature obj)
	{
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Skill)
			return false;

		ItemSkillData itemData = item as ItemSkillData;
		return RefItem.weaponId == itemData.RefItem.weaponId;
	}

}
