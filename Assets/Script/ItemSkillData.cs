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
		Use (obj);
	}

	override public void Use(Creature obj)
	{
		switch(RefItemID)
		{
		case 21:
			obj.WeaponHolder.ActiveWeaponSkillFire(Const.NuclearRefItemId, obj.transform.eulerAngles.y);
			break;
		case 22:
			obj.ApplyMachoSkill();
			break;
		case 23:
			obj.ApplyHealingSkill();
			break;
		case 24:
			obj.ApplyDamageMultiplySkill();
			break;
		}
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
