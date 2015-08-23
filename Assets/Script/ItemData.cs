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
		AlienEssence,
		Count
	}

	
	public enum Option
	{
		DamageMultiplier,
		DamageReduction,
		TapDamage,
		MoveSpeed,
		Weapon,
		Strength,
		MaxHp,
		MaxSp,
		RegenSp,
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

	virtual protected string itemName()
	{
		if (RefItem.name != null)
			return RefItem.name;

		return RefItem.codeName;
	}

	virtual public string Description()
	{
		string sp = "";
		if (RefItem.consumedSP > 0)
			sp = "SP:" + RefItem.consumedSP*Level + "\n";

		return "<color=white>" + 
				itemName() + "\n" + 
				"Level:" + Level + "\n" + 
				sp +
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
				//head = "per Lv" + op.level + ":" + op.option.type.ToString() + ":";
				head = op.option.type.ToString() + ":";
				optionValue = op.option.values[0]*(Level/op.level);
			}

			switch(op.option.type)
			{
			case Option.Weapon:
				desc += head + RefData.Instance.RefItems[(int)optionValue].name + "</color>\n";
				break;
			case Option.DamageMultiplier:
				desc += head + "Damage" + ":"+ (optionValue*100) + "%</color>\n";
				break;
			case Option.DamageReduction:						
			case Option.CriticalChance:
			case Option.CriticalDamage:
			case Option.GainExtraGold:
				desc += head + (optionValue*100) + "%</color>\n";
				break;
			default:
				desc += head + optionValue + "</color>\n";
				break;
			}
		}

		return desc;
	}

	virtual public void Pickup(Creature obj){Warehouse.Instance.PushItem(this);}
	virtual public void Equip(Creature obj){
			ApplyOptions(obj, false);
	}
	virtual public void Use(Creature obj){}
	virtual public bool Usable(Creature obj){return true;}
	virtual public void NoUse(Creature obj){}
	virtual public bool Compare(ItemData item)
	{
		return item.RefItem.type == RefItem.type;
	}

	public void ApplyOptions(Creature obj, bool levelup)
	{
		if (m_refItem.levelup == null || m_refItem.levelup.optionPerLevel == null)
			return;

		foreach(RefPriceCondition.RefOptionPerLevel op in m_refItem.levelup.optionPerLevel)
		{
			float optionValue = 0f;
			if (op.levelPer == false)
			{
				if (Level == op.level)
					optionValue = op.option.values[0];
			}
			else
			{
				if (levelup == false)
					optionValue = op.option.values[0]*(Level/op.level);
				else
				{
					if (Level%op.level == 0)
						optionValue = op.option.values[0];
				}
			}

			switch(op.option.type)
			{
			case Option.DamageMultiplier:
				obj.m_creatureProperty.DamageRatio += optionValue;
				break;
			case Option.MoveSpeed:
				obj.m_creatureProperty.AlphaMoveSpeed += optionValue;
				break;
			case Option.DamageReduction:
				obj.m_creatureProperty.DamageReduction += optionValue;
				break;
			case Option.Weapon:
				int weaponRefItemId = (int)optionValue;
				if (weaponRefItemId == 0)
					break;

				if (weaponRefItemId == Const.EmbersRefItemId)
				{
					obj.SetSubWeapon(obj.WeaponHolder.MainWeapon, new ItemWeaponData(Const.EmbersRefItemId), null);
				}
				else
				{
					ItemWeaponData weaponData = new ItemWeaponData((int)optionValue);

					weaponData.Level = (int)op.option.values[1];
					obj.EquipPassiveSkillWeapon(weaponData, null);
				}
				break;
			case Option.Strength:
				obj.m_creatureProperty.AlphaPhysicalAttackDamage += (int)optionValue;
				break;
			case Option.MaxHp:
				obj.m_creatureProperty.AlphaMaxHP += (int)optionValue;
				break;
			case Option.MaxSp:
				obj.m_creatureProperty.AlphaMaxSP += (int)optionValue;
				break;
			case Option.RegenSp:
				obj.m_creatureProperty.AlphaSPRegen += optionValue;
				break;
			case Option.TapDamage:
				obj.m_creatureProperty.TabDamage += (int)optionValue;
				break;
			case Option.CriticalChance:
				obj.m_creatureProperty.AlphaCriticalChance += optionValue;
				break;
			case Option.CriticalDamage:
				obj.m_creatureProperty.AlphaCriticalDamage += optionValue;
				break;
			case Option.GainExtraGold:
				obj.m_creatureProperty.GainExtraGold += optionValue;
				Creature owner = obj.GetOwner();
				if (owner != null)
				{
					owner.m_creatureProperty.GainExtraGold += optionValue;
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
