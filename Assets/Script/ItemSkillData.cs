using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemSkillData : ItemData{

	public int m_refWeaponItemId;
	[JsonConstructor]
	private ItemSkillData()
	{
	}

	public ItemSkillData(int refItemId) : base(refItemId, 1)
	{
		m_refWeaponItemId = RefData.Instance.RefItems[refItemId].weaponId;
	}

	override public void Pickup(Creature obj)
	{
		base.Pickup(obj);
		Use (obj);
	}

	override public void Use(Creature obj)
	{
		if (obj.WeaponHolder.GetActiveSkillWeapon(m_refWeaponItemId) == false)
		{
			obj.EquipActiveSkillWeapon(new ItemWeaponData(m_refWeaponItemId), null);
		}
		obj.WeaponHolder.ActiveWeaponSkillFire(m_refWeaponItemId, obj.transform.eulerAngles.y);
	}

	override public void NoUse(Creature obj)
	{
	}
	
	override public string Description()
	{
		return "<color=white>" + RefItem.codeName + "</color>" + "\n" +  base.Description();
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Skill)
			return false;

		ItemSkillData itemData = item as ItemSkillData;
		return m_refWeaponItemId == itemData.m_refWeaponItemId;
	}

}
