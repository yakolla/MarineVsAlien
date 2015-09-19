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
		DmgMultiplier,
		DmgReduction,
		TapDamage,
		MoveSpeed,
		Weapon,
		Strength,
		MaxHp,
		MaxSp,
		RegenSp,
		Critical,
		CriticalDmg,
		GainExtraGold,
		LifeSteal,
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
			return RefData.Instance.RefTexts(RefItem.name);

		return RefItem.codeName;
	}

	virtual public string Description()
	{
		string sp = "";
		if (RefItem.consumedSP > 0)
			sp = "SP:" + "<color=yellow>"+RefItem.consumedSP*Level+"</color>" + "\n";

		return  "<color=white>" + 
				"<size=20>"+itemName()+"</size>" + "\n" + 
				"Lv:" + "<color=yellow>"+Level+"</color>" + "\n" + 
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
				head = "Lv" + "<color=yellow>"+op.level+"</color>" + ":";
				optionValue = op.option.values[0];
			}
			else
			{
				optionValue = op.option.values[0]*(Level/op.level);
			}

			switch(op.option.type)
			{
			case Option.Weapon: 
				head += RefData.Instance.RefTexts(RefData.Instance.RefItems[(int)optionValue].name);
				break;
			case Option.DmgReduction:
				head += RefData.Instance.RefTexts(MultiLang.ID.Deffence) + ":";
				break;
			case Option.DmgMultiplier:
				head += RefData.Instance.RefTexts(MultiLang.ID.DamageMultiplier) + ":";
				break;
			case Option.Critical:
				head += RefData.Instance.RefTexts(MultiLang.ID.CriticalChance) + ":";
				break;
			case Option.CriticalDmg:
				head += RefData.Instance.RefTexts(MultiLang.ID.CriticalDamage) + ":";
				break;
			case Option.GainExtraGold:
				head += RefData.Instance.RefTexts(MultiLang.ID.GainExtraGold) + ":";
				break;
			case Option.LifeSteal:
				head += RefData.Instance.RefTexts(MultiLang.ID.LifeSteal) + ":";
				break;
			case Option.MaxHp:
				head += RefData.Instance.RefTexts(MultiLang.ID.MaxHP) + ":";
				break;
			case Option.MaxSp:
				head += RefData.Instance.RefTexts(MultiLang.ID.MaxSP) + ":";
				break;
			case Option.MoveSpeed:
				head += RefData.Instance.RefTexts(MultiLang.ID.MoveSpeed) + ":";
				break;
			case Option.RegenSp:
				head += RefData.Instance.RefTexts(MultiLang.ID.RegenSP) + ":";
				break;
			case Option.Strength:
				head += RefData.Instance.RefTexts(MultiLang.ID.Strength) + ":";
				break;
			case Option.TapDamage:
				head += RefData.Instance.RefTexts(MultiLang.ID.TapDamage) + ":";
				break;
			default:
				head += op.option.type.ToString() + ":";
				break;
			}



			switch(op.option.type)
			{
			case Option.Weapon:
				desc += head + "</color>\n";
				break;
			case Option.DmgMultiplier:
			case Option.DmgReduction:
			case Option.Critical:
			case Option.CriticalDmg:
			case Option.GainExtraGold:
			case Option.LifeSteal:
				desc += head + "<color=yellow>"+(optionValue*100)+"</color>" + "%</color>\n";
				break;
			default:
				desc += head + "<color=yellow>"+optionValue+"</color>" + "</color>\n";
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
				if (levelup == false)
				{
					if (Level >= op.level)
						optionValue = op.option.values[0];
				}
				else
				{
					if (Level == op.level)
						optionValue = op.option.values[0];
				}
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
			case Option.DmgMultiplier:
				obj.m_creatureProperty.DamageMultiPlier += optionValue;

				break;
			case Option.MoveSpeed:
				obj.m_creatureProperty.AlphaMoveSpeed += optionValue;
				break;
			case Option.DmgReduction:
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
				obj.m_creatureProperty.TapDamage += (int)optionValue;
				break;
			case Option.Critical:
				obj.m_creatureProperty.AlphaCriticalChance += optionValue;
				break;
			case Option.CriticalDmg:
				obj.m_creatureProperty.AlphaCriticalDamage += optionValue;
				break;
			case Option.LifeSteal:
				obj.m_creatureProperty.AlphaLifeSteal += optionValue;
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
