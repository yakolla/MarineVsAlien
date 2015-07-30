using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


public class ItemData {
	
	public enum Type
	{
		Gold,
		HealPosion,
		Weapon,
		WeaponParts,
		WeaponDNA,
		Follower,
		GoldMedal,
		XPPotion,
		MobEgg,
		Gem,
		Accessory,
		ItemPandora,
		RandomAbility,
		Skill,
		Cheat,
		Stat,
		Count
	}

	
	public enum Option
	{
		DamageMultiplier,
		DamageReduction,
		MoveSpeed,
		Weapon,
		Str,
		MaxHp,
		Count
	}



	int				m_refItemId;
	SecuredType.XInt	m_count = 0;
	SecuredType.XInt	m_level = 1;
	bool			m_lock = false;

	RefItem				m_refItem;

	[JsonConstructor]
	protected ItemData()
	{
	}

	public ItemData(int refItemId, int count)
	{
		RefItemID = refItemId;
		m_count = count;
		Lock = m_refItem.defaultLock;
	}

	virtual public string Description()
	{
		return "<color=white>" + 
				"Level:" + Level + "\n" + 
				"</color>" +
				OptionsDescription();
	}

	protected string OptionsDescription()
	{
		if (m_refItem.levelup == null || m_refItem.levelup.optionPerLevel == null)
			return "";

		string desc = "\n";
		foreach(RefPriceCondition.RefOptionPerLevel op in m_refItem.levelup.optionPerLevel)
		{
			desc += (Level >= op.level ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv" + op.level + ":";
			switch(op.option.type)
			{
			case Option.Weapon:
				desc += RefData.Instance.RefItems[(int)op.option.values[0]].name + "</color>\n";
				break;
			case Option.DamageMultiplier:
			case Option.DamageReduction:
				desc += op.option.type.ToString() + ":"+ (op.option.values[0]*100) + "%</color>\n";
				break;
			default:
				desc += op.option.type.ToString() + ":"+ op.option.values[0] + "</color>\n";
				break;
			}
		}

		return desc;
	}

	virtual public void Pickup(Creature obj){Warehouse.Instance.PushItem(this);}
	virtual public void Equip(Creature obj){}
	virtual public void Use(Creature obj){}
	virtual public bool Usable(Creature obj){return true;}
	virtual public void NoUse(Creature obj){}
	virtual public bool Compare(ItemData item)
	{
		return item.RefItem.type == RefItem.type;
	}

	public void ApplyOptions(Creature obj)
	{
		if (m_refItem.levelup == null || m_refItem.levelup.optionPerLevel == null)
			return;

		foreach(RefPriceCondition.RefOptionPerLevel op in m_refItem.levelup.optionPerLevel)
		{
			if (op.level > Level)
				continue;

			switch(op.option.type)
			{
			case Option.DamageMultiplier:
				obj.m_creatureProperty.DamageRatio += op.option.values[0];
				break;
			case Option.MoveSpeed:
				obj.m_creatureProperty.AlphaMoveSpeed += op.option.values[0];
				break;
			case Option.DamageReduction:
				obj.m_creatureProperty.DamageReduction += op.option.values[0];
				break;
			case Option.Weapon:
				int weaponRefItemId = (int)op.option.values[0];
				if (weaponRefItemId == Const.EmbersRefItemId)
				{
					obj.SetSubWeapon(obj.WeaponHolder.MainWeapon, new ItemWeaponData(Const.EmbersRefItemId), null);
				}
				else
				{
					ItemWeaponData weaponData = new ItemWeaponData((int)op.option.values[0]);

					weaponData.Level = (int)op.option.values[1];
					obj.EquipPassiveSkillWeapon(weaponData, null);
				}
				break;
			case Option.Str:
				obj.m_creatureProperty.AlphaPhysicalAttackDamage += (int)op.option.values[0];
				break;
			case Option.MaxHp:
				obj.m_creatureProperty.AlphaMaxHP += (int)op.option.values[0];
				break;
			}

		}
	}

	public void NoApplyOptions(Creature obj)
	{
		if (m_refItem.levelup == null || m_refItem.levelup.optionPerLevel == null)
			return;
		
		foreach(RefPriceCondition.RefOptionPerLevel op in m_refItem.levelup.optionPerLevel)
		{
			if (op.level > Level)
				continue;

			switch(op.option.type)
			{
			case Option.DamageMultiplier:
				obj.m_creatureProperty.DamageRatio -= op.option.values[0];
				break;
			case Option.MoveSpeed:
				obj.m_creatureProperty.AlphaMoveSpeed -= op.option.values[0];
				break;
			case Option.DamageReduction:
				obj.m_creatureProperty.DamageReduction -= op.option.values[0];
				break;
			}
		}
	}

	public int RefItemID
	{
		get {return m_refItemId;}
		set {
			m_refItemId = value;

			m_refItem = RefData.Instance.RefItems[m_refItemId];
		}
	}

	public int Level
	{
		get {

			if (Lock == true)
				return 0;

			return m_level.Value;	
		}
		set {
			m_level.Value = value;
		}
	}

	public int Count
	{
		get {return m_count.Value;}
		set {m_count.Value = value;}
	}

	[JsonIgnore]
	public RefItem RefItem
	{
		get {return m_refItem;}
	}

	public bool Lock
	{
		get {return m_lock;}
		set {m_lock = value;}

	}

}
