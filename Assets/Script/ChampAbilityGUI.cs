using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChampAbilityGUI : MonoBehaviour {

	Champ		m_champ;

	YGUISystem.GUIButton[]	m_statButtons = new YGUISystem.GUIButton[Const.AbilitySlots];
	YGUISystem.GUILable		m_remainPointText;
	YGUISystem.GUIPriceButton	m_rollButton;

	int			m_usedCountOfRandomAbilityItem = 0;

	ChampStatusGUI	m_statusGUI;

	CreatureProperty	m_backup = new CreatureProperty();

	ADMob		m_adMob;
	bool		m_autoAssigned = false;

	class Ability
	{
		public float		m_chance;
		public string		m_name;
		public System.Func<string> m_compare;
		public System.Action	m_functor;
		public System.Func<bool> m_availableFunctor;


		public Ability(float chance, string name, System.Func<string> compare, System.Action functor)
			:this(chance, name, compare, functor, ()=>{return true;})
		{
			
		}

		public Ability(float chance, string name, System.Func<string> compare, System.Action functor, System.Func<bool> availableFunctor)
		{
			m_chance = chance;
			m_name = name;
			m_functor = functor;
			m_compare = compare;
			m_availableFunctor = availableFunctor;
		}
	}

	enum AbilityCategory
	{
		ChampStat,
		Skill,
		Weapon
	}

	Dictionary<AbilityCategory, List<Ability>>	m_abilities = new Dictionary<AbilityCategory, List<Ability>>();
	Ability[]	m_abilitySlots = new Ability[Const.AbilitySlots];

	void Start ()
	{
		m_champ = GameObject.Find("Champ").GetComponent<Champ>();
		m_statusGUI = transform.parent.parent.Find("StatusGUI").GetComponent<ChampStatusGUI>();

		List<Ability> basicAbili = new List<Ability>();
		List<Ability> skillAbili = new List<Ability>();
		List<Ability> utilAbili = new List<Ability>();

		basicAbili.Add(new Ability(0.3f, "Damage", 
		()=>{
			m_backup.AlphaPhysicalAttackDamage+=3;
			int backup = m_champ.WeaponHolder.MainWeapon.GetDamage(m_backup);
			
			return m_champ.WeaponHolder.MainWeapon.GetDamage(m_champ.m_creatureProperty) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaPhysicalAttackDamage+=3;
			--m_champ.RemainStatPoint;
		}));
		/*
		m_abilities.Add(new Ability(0.3f, "Inc Defence", 
		()=>{
			m_backup.AlphaPhysicalDefencePoint+=3;
			return m_champ.m_creatureProperty.PhysicalDefencePoint + " -> " + "<color=yellow>" + (m_backup.PhysicalDefencePoint) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaPhysicalDefencePoint+=3;
			--m_champ.RemainStatPoint;
		}));
		*/
		basicAbili.Add(new Ability(0.3f, "Health Max Up",
		()=>{
			m_backup.AlphaMaxHP+=50;
			return m_champ.m_creatureProperty.MaxHP + " -> " + "<color=yellow>" + (m_backup.MaxHP) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaMaxHP+=50;
			//m_champ.m_creatureProperty.Heal((int)m_champ.m_creatureProperty.MaxHP);
			--m_champ.RemainStatPoint;
		}));
		/*
		basicAbili.Add(new Ability(0.3f, "SP Max Up", 
		                                ()=>{
			m_backup.AlphaMaxSP+=30;
			return m_champ.m_creatureProperty.MaxSP + " -> " + "<color=yellow>" + (m_backup.MaxSP) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaMaxSP+=30;
			--m_champ.RemainStatPoint;
		}));


		basicAbili.Add(new Ability(0.3f, "SP Recovery Up", 
		                                ()=>{
			m_backup.AlphaSPRecoveryPerSec+=1;
			return m_champ.m_creatureProperty.SPRecoveryPerSec + " -> " + "<color=yellow>" + (m_backup.SPRecoveryPerSec) + "/sec</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaSPRecoveryPerSec+=1;
			--m_champ.RemainStatPoint;
		}));
*/
		basicAbili.Add(new Ability(0.3f, "Move Speed Up", 
		                                ()=>{
			m_backup.AlphaMoveSpeed+=1;
			return m_champ.m_creatureProperty.AlphaMoveSpeed + " -> " + "<color=yellow>" + (m_backup.AlphaMoveSpeed) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaMoveSpeed+=1;
			--m_champ.RemainStatPoint;
		},
		()=>{
			return m_champ.m_creatureProperty.AlphaMoveSpeed < Const.MaxAlphaMoveSpeed;
		}
		));


		basicAbili.Add(new Ability(0.3f, "Critical Pack",
		()=>{
			m_backup.AlphaCriticalChance += 0.1f;
			m_backup.AlphaCriticalDamage += 0.5f;
			return "Chance:"+(m_champ.m_creatureProperty.CriticalChance*100) + " -> " + "<color=yellow>" + (m_backup.CriticalChance*100) + "%</color>" + "\n" +
				"Damage:"+(m_champ.m_creatureProperty.CriticalDamage*100) + " -> " + "<color=yellow>" + (m_backup.CriticalDamage*100) + "%</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaCriticalChance += 0.1f;
			m_champ.m_creatureProperty.AlphaCriticalDamage += 0.5f;
			--m_champ.RemainStatPoint;
		},
		()=>{
			switch(m_champ.WeaponHolder.MainWeapon.RefItem.id)
			{
			case Const.ChampLightningLauncherRefItemId:
			case Const.ChampFiregunRefItemId:			
				return false;
			}
			
			return true;
		}
		));

/*
		m_abilities.Add(new Ability(0.1f, "Life Per Kill", 
		()=>{
			m_backup.AlphaLifeSteal += 1f;
			return (m_champ.m_creatureProperty.LifeSteal) + " -> " + "<color=yellow>" + (m_backup.LifeSteal) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaLifeSteal += 1f;
			--m_champ.RemainStatPoint;
		}));
*/

		/*
		basicAbili.Add(new Ability(0.01f, "Weapon Lv Up", 
		                            ()=>{
			Weapon weapon = m_champ.WeaponHolder.MainWeapon;
			int backup = weapon.Level+1;
			int ori = weapon.Level;
			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.WeaponHolder.MainWeapon.LevelUp();
			--m_champ.RemainStatPoint;
		}
		));
*/
		skillAbili.Add(new Ability(0.3f, RefData.Instance.RefTexts(RefData.Instance.RefItems[Const.EmbersRefItemId].name), 
		                            ()=>{
			Weapon weapon = m_champ.WeaponHolder.MainWeapon.GetSubWeapon();
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}

			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
				//"SP:" + Weapon.GetSP(RefData.Instance.RefItems[132], ori) + " -> " + "<color=yellow>" +Weapon.GetSP(RefData.Instance.RefItems[132], backup)+ "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.MainWeapon.GetSubWeapon();
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.SetSubWeapon(m_champ.WeaponHolder.MainWeapon, new ItemWeaponData(Const.EmbersRefItemId), null);
			}

			--m_champ.RemainStatPoint;
		},
		()=>{
			switch(m_champ.WeaponHolder.MainWeapon.RefItem.id)
			{
			case Const.ChampGunRefItemId:
			case Const.ChampLightningLauncherRefItemId:
			case Const.ChampFiregunRefItemId:
			case Const.ChampBoomerangLauncherRefItemId:
				return false;
			}

			Weapon weapon = m_champ.WeaponHolder.MainWeapon.GetSubWeapon();
			if (weapon != null)
			{
				if (weapon.Level >= weapon.RefItem.maxLevel)
					return false;
			}
			return true;
		}
		));

		foreach (DamageDesc.BuffType buffType in System.Enum.GetValues(typeof(DamageDesc.BuffType)))
		{
			bool skip = false;
			switch(buffType)
			{
			case DamageDesc.BuffType.Count:
			case DamageDesc.BuffType.LevelUp:
			case DamageDesc.BuffType.Nothing:
			case DamageDesc.BuffType.Macho:
				skip = true;
				break;
			}

			if (skip == true)
				continue;

			string name = buffType.ToString() + " Chance";
			DamageDesc.BuffType capturedBuffType = buffType;

			utilAbili.Add(new Ability(0.3f, name, 
			                            ()=>{
				float oriChance = m_champ.WeaponHolder.MainWeapon.WeaponStat.buffOnHitDesc.chance;
				float toChance = oriChance + 0.1f;
				return (oriChance*100) + " -> " + "<color=yellow>" + (toChance*100) + "%</color>";
			},
			()=>{
				m_champ.WeaponHolder.MainWeapon.WeaponStat.buffOnHitDesc.chance += 0.1f;
				--m_champ.RemainStatPoint;
			},
			()=>{

				return (m_champ.WeaponHolder.MainWeapon.WeaponStat.buffOnHitDesc.buffType == capturedBuffType &&
				        m_champ.WeaponHolder.MainWeapon.WeaponStat.buffOnHitDesc.chance < 100
				        );
			}
			));
		}

		/*
		skillAbili.Add(new Ability(0.3f, "Fill " + (m_champ.WeaponHolder.MainWeapon.WeaponStat.skillId > 0 ? RefData.Instance.RefTexts(RefData.Instance.RefItems[m_champ.WeaponHolder.MainWeapon.WeaponStat.skillId].name) : ""), 
		                           ()=>{
			int backup = m_champ.NuclearSkillStack+3;
			return (m_champ.NuclearSkillStack) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.NuclearSkillStack += 3;
			--m_champ.RemainStatPoint;
		},
		()=>{
			return m_champ.WeaponHolder.MainWeapon.WeaponStat.skillId > 0;
		}
		));

		skillAbili.Add(new Ability(0.3f, "Fill Macho Skill", 
		                           ()=>{
			int backup = m_champ.MachoSkillStack+3;
			return (m_champ.MachoSkillStack) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.MachoSkillStack += 3;
			--m_champ.RemainStatPoint;
		}));
*/
		skillAbili.Add(new Ability(0.3f, RefData.Instance.RefTexts(RefData.Instance.RefItems[131].name), 
		                            ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(131);
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}
			
			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
				//"SP:" + Weapon.GetSP(RefData.Instance.RefItems[131], ori) + " -> " + "<color=yellow>" +Weapon.GetSP(RefData.Instance.RefItems[131], backup)+ "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(131);
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.EquipPassiveSkillWeapon(new ItemWeaponData(131), null);
			}
			
			--m_champ.RemainStatPoint;
		},
		()=>{
			switch(m_champ.WeaponHolder.MainWeapon.RefItem.id)
			{
			case Const.ChampGunRefItemId:			
				Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(131);
				if (weapon != null)
				{
					if (weapon.Level >= weapon.RefItem.maxLevel)
						return false;
				}
				return true;
			case Const.ChampLightningLauncherRefItemId:
			case Const.ChampFiregunRefItemId:
			case Const.ChampGuidedRocketLauncherRefItemId:
			case Const.ChampRocketLauncherRefItemId:
			case Const.ChampBoomerangLauncherRefItemId:
				break;
			}
			
			return false;
		}
		));

		skillAbili.Add(new Ability(0.3f, RefData.Instance.RefTexts(RefData.Instance.RefItems[134].name), 
		                           ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(134);
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}
			
			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(134);
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.EquipPassiveSkillWeapon(new ItemWeaponData(134), null);
			}
			
			--m_champ.RemainStatPoint;
		},
		()=>{
			switch(m_champ.WeaponHolder.MainWeapon.RefItem.id)
			{
			case Const.ChampBoomerangLauncherRefItemId:			
				Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(134);
				if (weapon != null)
				{
					if (weapon.Level >= weapon.RefItem.maxLevel)
						return false;
				}
				return true;
			case Const.ChampGunRefItemId:
			case Const.ChampLightningLauncherRefItemId:
			case Const.ChampFiregunRefItemId:
			case Const.ChampGuidedRocketLauncherRefItemId:
			case Const.ChampRocketLauncherRefItemId:
				break;
			}
			
			return false;
		}
		));

		skillAbili.Add(new Ability(0.3f, RefData.Instance.RefTexts(RefData.Instance.RefItems[129].name), 
		                           ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(129);
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}
			
			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
				//"SP:" + Weapon.GetSP(RefData.Instance.RefItems[129], ori) + " -> " + "<color=yellow>" +Weapon.GetSP(RefData.Instance.RefItems[129], backup)+ "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(129);
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.EquipPassiveSkillWeapon(new ItemWeaponData(129), null);
			}
			
			--m_champ.RemainStatPoint;
		},
		()=>{
			switch(m_champ.WeaponHolder.MainWeapon.RefItem.id)
			{
			case Const.ChampLightningLauncherRefItemId:			
				Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(129);
				if (weapon != null)
				{
					if (weapon.Level >= weapon.RefItem.maxLevel)
						return false;
				}
				return true;
			case Const.ChampFiregunRefItemId:
			case Const.ChampGunRefItemId:			
			case Const.ChampBoomerangLauncherRefItemId:
			case Const.ChampGuidedRocketLauncherRefItemId:
			case Const.ChampRocketLauncherRefItemId:
				break;
			}
			
			return false;
		}
		));

		skillAbili.Add(new Ability(0.3f, RefData.Instance.RefTexts(RefData.Instance.RefItems[135].name), 
		                           ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(135);
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}
			
			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(135);
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.EquipPassiveSkillWeapon(new ItemWeaponData(135), null);
			}
			
			--m_champ.RemainStatPoint;
		},
		()=>{
			switch(m_champ.WeaponHolder.MainWeapon.RefItem.id)
			{
			case Const.ChampFiregunRefItemId:
			
				Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(135);
				if (weapon != null)
				{
					if (weapon.Level >= weapon.RefItem.maxLevel)
						return false;
				}
				return true;
			case Const.ChampGunRefItemId:
			case Const.ChampBoomerangLauncherRefItemId:
			case Const.ChampLightningLauncherRefItemId:
			case Const.ChampGuidedRocketLauncherRefItemId:
			case Const.ChampRocketLauncherRefItemId:
				return false;
			}
			
			return false;

		}
		));
		/*
		skillAbili.Add(new Ability(0.3f, "Charge to Nuclear Skill", 
		                           ()=>{
			return (m_champ.NuclearSkillStack) + " -> " + "<color=yellow>" + (m_champ.NuclearSkillStack+1) + "</color>" + "\n" +
					"SP:" + Weapon.GetSP(RefData.Instance.RefItems[133], 1);
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetActiveWeapon(133);
			if (weapon != null)
			{
				//weapon.LevelUp();
			}
			else
			{
				m_champ.EquipActiveWeapon(new ItemWeaponData(133, null));
			}
			++m_champ.NuclearSkillStack;
			--m_champ.RemainStatPoint;
		}));
*/
		skillAbili.Add(new Ability(0.3f, "Fill " + RefData.Instance.RefItems[130].name, 
		                           ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(130);
			m_backup.Shield += 10;
			return (m_champ.m_creatureProperty.Shield) + " -> " + "<color=yellow>" + (m_backup.Shield) + "</color>";
				//"SP:" + Weapon.GetSP(RefData.Instance.RefItems[130], 1);
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveSkillWeapon(130);
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.EquipPassiveSkillWeapon(new ItemWeaponData(130), null);
			}
			
			--m_champ.RemainStatPoint;
		}));


		utilAbili.Add(new Ability(0.3f, "Splash Radius", 
		                          ()=>{
			
			m_backup.SplashRadius+=1;
			
			return (m_champ.m_creatureProperty.SplashRadius) + " -> " + "<color=yellow>" + (m_backup.SplashRadius) + "m</color>";
		},
		()=>{
			m_champ.m_creatureProperty.SplashRadius+=1;
			
			--m_champ.RemainStatPoint;
		},
		()=>{
			switch(m_champ.WeaponHolder.MainWeapon.RefItem.id)
			{
			case Const.ChampLightningLauncherRefItemId:
			case Const.ChampFiregunRefItemId:
			case Const.ChampGunRefItemId:			
			case Const.ChampBoomerangLauncherRefItemId:
				return false;
			case Const.ChampGuidedRocketLauncherRefItemId:
			case Const.ChampRocketLauncherRefItemId:
				return true;
			}
			
			return false;
		}
		));

		utilAbili.Add(new Ability(0.3f, "Flamethrower Length", 
		                          ()=>{
			
			m_backup.BulletLength+=0.25f;
			
			return (m_champ.m_creatureProperty.BulletLength) + " -> " + "<color=yellow>" + (m_backup.BulletLength) + "m</color>";
		},
		()=>{
			m_champ.m_creatureProperty.BulletLength+=0.25f;
			
			--m_champ.RemainStatPoint;
		},
		()=>{
			if (m_champ.m_creatureProperty.BulletLength >= 1f)
				return false;

			switch(m_champ.WeaponHolder.MainWeapon.RefItem.id)
			{
			case Const.ChampFiregunRefItemId:
				return true;
			case Const.ChampLightningLauncherRefItemId:			
			case Const.ChampGunRefItemId:			
			case Const.ChampBoomerangLauncherRefItemId:
			case Const.ChampGuidedRocketLauncherRefItemId:
			case Const.ChampRocketLauncherRefItemId:
				return false;
			}
			
			return false;
		}
		));
		
		utilAbili.Add(new Ability(0.3f, "Followers On Summoning", 
		                          ()=>{
			
			m_backup.CallableFollowers+=1;
			
			return (m_champ.m_creatureProperty.CallableFollowers) + " -> " + "<color=yellow>" + (m_backup.CallableFollowers) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.CallableFollowers+=1;
			
			--m_champ.RemainStatPoint;
		},
		()=>{
			for(int i = 0; i < m_champ.AccessoryItems.Length; ++i)
			{
				if (m_champ.AccessoryItems[i] != null && 
				    m_champ.AccessoryItems[i].Item.RefItem.type == ItemData.Type.Follower &&
				    m_champ.m_creatureProperty.CallableFollowers < Const.MaxCallableFollowers
				    )
					return true;
			}
			return false;
		}
		));
		
		utilAbili.Add(new Ability(0.3f, "Gain Extra XP", 
		                          ()=>{
			m_backup.GainExtraExp += 0.5f;
			return (m_champ.m_creatureProperty.GainExtraExp*100) + " -> " + "<color=yellow>" + (m_backup.GainExtraExp*100) + "%</color>";
		},
		()=>{
			m_champ.m_creatureProperty.GainExtraExp += 0.5f;
			--m_champ.RemainStatPoint;
		}));

		utilAbili.Add(new Ability(0.3f, "Gain Extra Gold", 
		                          ()=>{
			m_backup.GainExtraGold += 0.5f;
			return (m_champ.m_creatureProperty.GainExtraGold*100) + " -> " + "<color=yellow>" + (m_backup.GainExtraGold*100) + "%</color>";
		},
		()=>{
			m_champ.m_creatureProperty.GainExtraGold += 0.5f;
			--m_champ.RemainStatPoint;
		}));
		
		m_abilities.Add(AbilityCategory.ChampStat, basicAbili);
		m_abilities.Add(AbilityCategory.Skill, skillAbili);
		m_abilities.Add(AbilityCategory.Weapon, utilAbili);

		for(int i = 0; i < m_statButtons.Length; ++i)
			m_statButtons[i] = new YGUISystem.GUIButton(transform.Find("StatButton"+i).gameObject, ()=>{return true;});


		m_remainPointText = new YGUISystem.GUILable(transform.Find("Text/RemainPointText").gameObject);

		m_rollButton = new YGUISystem.GUIPriceButton(transform.Find("RollingButton").gameObject, Const.StartPosYOfPriceButtonImage, ()=>{
			return m_champ.RemainStatPoint > 0;
		});
		m_rollButton.Prices = RefData.Instance.RefItems[Const.RandomAbilityRefItemId].levelup.conds;

		transform.Find("RollingButton").gameObject.SetActive(Cheat.EnableAbilityRollButton);
#if UNITY_EDITOR
		if (Const.CHEAT_MODE)
		{
			transform.Find("RollingButton").gameObject.SetActive(true);
		}
#endif

		RandomAbility(!AutoAssigned);

		AutoAssignedAbillity();
	}

	public void StartSpinButton(YGUISystem.GUIButton button)
	{
		button.Button.enabled = false;
		button.Lable.Text.enabled = false;
		button.Button.animator.SetBool("Spin", true);
		button.Button.audio.Play();
	}

	public void StopSpinButton(int slot)
	{
		m_statButtons[slot].Button.enabled = true;
		m_statButtons[slot].Lable.Text.enabled = true;
	}

	void RandomAbility(bool ani)
	{
		int selectCount = 0;
		while(selectCount < Const.AbilitySlots)
		{
			List<Ability> abilis = m_abilities[(AbilityCategory)Random.Range(0, Const.AbilitySlots)];
			Ability ability = abilis[Random.Range(0, abilis.Count)];

			bool alreadyExist = false;
			for(int i = 0; i < selectCount; ++i)
			{
				if (m_abilitySlots[i] == ability)
				{
					alreadyExist = true;
					break;
				}
			}

			if (alreadyExist == true)
				continue;

			float ratio = Random.Range(0f, 1f);
			if (ability.m_availableFunctor() == true && ratio < ability.m_chance)
			{
				m_abilitySlots[selectCount] = ability;
				++selectCount;
			}

		}

		if (ani == true)
		{
			for(selectCount = 0; selectCount < Const.AbilitySlots; ++selectCount)
				StartSpinButton(m_statButtons[selectCount]);
		}

	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();


		GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>().ShowBanner(true);

		AutoAssignedAbillity();
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	void AutoAssignedAbillity()
	{
		if (AutoAssigned & m_champ != null)
		{
			while(0 < m_champ.RemainStatPoint)
			{
				Ability ability = m_abilitySlots[Random.Range(0, Const.AbilitySlots)];
				
				ability.m_functor();
				RandomAbility(false);
			}

			OnClickOK();
		}
	}

	public void OnClickStat(int slot)
	{
		if (m_champ.RemainStatPoint == 0)
			return;

		Ability ability = m_abilitySlots[slot];

		ability.m_functor();
		RandomAbility(true);
	
	}

	public void OnClickOK()
	{

		for(int i = 0; i < m_statButtons.Length; ++i)
		{
			if (m_statButtons[i].Button.GetComponent<SpinButtonGUI>().IsSpining())
				return;
		}

		gameObject.SetActive(false);
		GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>().ShowBanner(false);

		m_statusGUI.SlidingNormalAccessoryBoard();

	}

	public void OnClickRoll()
	{
		if (true == m_rollButton.TryToPay())
		{
			RandomAbility(true);
			++m_usedCountOfRandomAbilityItem;

			m_rollButton.NormalWorth = 1f+m_usedCountOfRandomAbilityItem;

			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Ability", "Roll"+m_usedCountOfRandomAbilityItem, 0);
		}
	}

	public bool AutoAssigned
	{
		set {m_autoAssigned = value;}
		get {return m_autoAssigned;}
	}

	void Update()
	{

		m_champ.m_creatureProperty.CopyTo(m_backup);

		int statSlot = 0;
		foreach(YGUISystem.GUIButton button in m_statButtons)
		{
			button.Lable.Text.text = m_abilitySlots[statSlot].m_name + "\n" + m_abilitySlots[statSlot].m_compare();
			++statSlot;
		}

		string color = "<color=lime>";
		if (m_champ.RemainStatPoint == 0)
			color = "<color=red>";

		m_remainPointText.Text.text = color + m_champ.RemainStatPoint + "</color>";


		m_rollButton.Update();

	}

}
