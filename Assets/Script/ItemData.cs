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
		Strength,
		MaxHp,
		MaxSp,
		CriticalChance,
		CriticalDamage,
		GainExtraGold,
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
			desc += (Level >= op.level ? Const.EnabledStringColor : Const.DisabledStringColor);
			string head = null;
			float optionValue = 0f;
			if (op.levelPer == false)
			{
				head = "Lv" + op.level + ":";
				optionValue = op.option.values[0];
			}
			else
			{
				head = "per Lv" + op.level + ":" + op.option.type.ToString() + ":";
				optionValue = op.option.values[0]*(Level/op.level);
			}

			switch(op.option.type)
			{
			case Option.Weapon:
				desc += head + RefData.Instance.RefItems[(int)optionValue].name + "</color>\n";
				break;
			case Option.DamageMultiplier:
			case Option.DamageReduction:
				desc += head + op.option.type.ToString() + ":"+ (optionValue*100) + "%</color>\n";
				break;
			case Option.Strength:
			case Option.MaxHp:
			case Option.MaxSp:
				desc += head + (optionValue) + "</color>\n";
				break;
			case Option.CriticalChance:
			case Option.CriticalDamage:
			case Option.GainExtraGold:
				desc += head + (optionValue*100) + "%</color>\n";
				break;
			default:
				desc += op.option.type.ToString() + ":"+ op.option.values[0] + "</color>\n";
				break;
			}
		}

		return desc;
	}

	virtual public void Pickup(Creature obj){Warehouse.Instance.PushItem(this);}
	virtual public void Equip(Creature obj){
		for(int i = 0; i < Level; ++i)
			ApplyOptions(obj);
	}
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
			if (Level < op.level)
				continue;
			if (op.levelPer == false && op.level != Level)
				continue;
			if (op.levelPer == true && (Level%op.level) > 0)
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
			case Option.Strength:
				obj.m_creatureProperty.AlphaPhysicalAttackDamage += (int)op.option.values[0];
				break;
			case Option.MaxHp:
				obj.m_creatureProperty.AlphaMaxHP += (int)op.option.values[0];
				break;
			case Option.MaxSp:
				obj.m_creatureProperty.AlphaMaxSP += (int)op.option.values[0];
				break;
			case Option.CriticalChance:
				obj.m_creatureProperty.AlphaCriticalChance += op.option.values[0];
				break;
			case Option.CriticalDamage:
				obj.m_creatureProperty.AlphaCriticalDamage += op.option.values[0];
				break;
			case Option.GainExtraGold:
				obj.m_creatureProperty.GainExtraGold += op.option.values[0];
				Creature owner = obj.GetOwner();
				if (owner != null)
				{
					owner.m_creatureProperty.GainExtraGold += op.option.values[0];
				}
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
