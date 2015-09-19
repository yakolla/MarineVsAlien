using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemWeaponData : ItemData{

	Creature m_owner;
	Weapon	m_weapon;
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
		get{return RefData.Instance.RefTexts(RefItem.name);}
	}


	override public void Equip(Creature obj)
	{
		obj.EquipWeapon(this, null);
		base.Equip(obj);

		m_owner = obj;
		for(int i = 0; i < m_owner.WeaponHolder.Weapons.Count; ++i)
		{
			if (m_owner.WeaponHolder.Weapons[i].RefItem.id == RefItem.id)
			{
				m_weapon = m_owner.WeaponHolder.Weapons[i];
				break;
			}
		}
	}

	override public void Use(Creature obj)
	{	
		ApplyOptions(obj, true);
	}

	override public string Description()
	{
		string desc = base.Description();
		
		if (m_weapon != null)
			desc += RefData.Instance.RefTexts(MultiLang.ID.Damage) + ":" + "<color=yellow>" + m_weapon.Damage + "</color>";
		
		return desc;
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Weapon)
			return base.Compare(item);

		ItemWeaponData weapon = (ItemWeaponData)item;
		return WeaponName.CompareTo(weapon.WeaponName) == 0;
	}

}
