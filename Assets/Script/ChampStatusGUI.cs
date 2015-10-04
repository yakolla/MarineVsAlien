using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChampStatusGUI : MonoBehaviour {

	Champ	m_champ;

	int			m_oldMobKills;
	int			m_oldGold;
	int			m_oldGoldMedal;
	int			m_oldGem;
	int			m_oldDNA;

	GameObject		m_accessoryBoard;


	YGUISystem.GUIChargeButton[]	m_specialButtons = new YGUISystem.GUIChargeButton[Const.SpecialButtons];
	YGUISystem.GUIChargeButton[]	m_accessoryButtons = new YGUISystem.GUIChargeButton[Const.AccessoriesSlots];
	YGUISystem.GUIGuage[] m_guages = new YGUISystem.GUIGuage[Const.Guages];
	YGUISystem.GUILable m_level;
	YGUISystem.GUIButton			m_autoEarnButton;
	ComboGUIShake	m_gold;
	ComboGUIShake	m_mobKills;
	ComboGUIShake	m_goldMedal;
	ComboGUIShake	m_gem;
	ComboGUIShake	m_dna;


	void Start () {

		m_level = new YGUISystem.GUILable(transform.Find("Level/Text").gameObject);
		m_gold = transform.Find("Gold/RawImage/Text").gameObject.GetComponent<ComboGUIShake>();
		m_goldMedal = transform.Find("GoldMedal/RawImage/Text").gameObject.GetComponent<ComboGUIShake>();
		m_mobKills = transform.Find("Kills/RawImage/Text").gameObject.GetComponent<ComboGUIShake>();
		m_gem = transform.Find("Gem/RawImage/Text").gameObject.GetComponent<ComboGUIShake>();
		m_dna = transform.Find("DNA/RawImage/Text").gameObject.GetComponent<ComboGUIShake>();


		m_accessoryBoard = transform.Find("Accessory").gameObject;

		m_autoEarnButton = new YGUISystem.GUIButton(transform.Find("Special/AutoEarnButton").gameObject, ()=>{
			m_autoEarnButton.Lable.Text.text = Warehouse.Instance.AutoEarnGold.ToString();
			return Warehouse.Instance.AutoEarnGold > 0;
		});


		assignSkillButton(0, Warehouse.Instance.FindItem(21), ()=>{
			Warehouse.Instance.FindItem(21).Item.Use(m_champ);
			return true;
		});

		assignSkillButton(1, Warehouse.Instance.FindItem(22), ()=>{
			Warehouse.Instance.FindItem(22).Item.Use(m_champ);
			m_champ.Followers.ForEach((Creature follower)=>{
				Warehouse.Instance.FindItem(22).Item.Use(follower);
			});
			return true;
		});

		assignSkillButton(2, Warehouse.Instance.FindItem(23), ()=>{			
			Warehouse.Instance.FindItem(23).Item.Use(m_champ);

			return true;
		});

		assignSkillButton(3, Warehouse.Instance.FindItem(24), ()=>{	
			Warehouse.Instance.FindItem(24).Item.Use(m_champ);
			m_champ.Followers.ForEach((Creature follower)=>{
				Warehouse.Instance.FindItem(24).Item.Use(follower);
			});
			return true;
		});

		assignSkillButton(4, Warehouse.Instance.FindItem(25), ()=>{	
			Warehouse.Instance.FindItem(25).Item.Use(m_champ);
			m_champ.Followers.ForEach((Creature follower)=>{
				Warehouse.Instance.FindItem(25).Item.Use(follower);
			});
			return true;
		});

			for(int i = 0; i < m_accessoryButtons.Length; ++i)
		{
			m_accessoryButtons[i] = new YGUISystem.GUIChargeButton(transform.Find("Accessory/Button"+i).gameObject, ()=>{
				return true;
			});
		}

		m_guages[0] = new YGUISystem.GUIGuage(transform.Find("Guage/HP").gameObject, 
			()=>{return m_champ.m_creatureProperty.getHPRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxHP).ToString(); 
			}
		);

		m_guages[1] = new YGUISystem.GUIGuage(transform.Find("Guage/XP").gameObject, 
			()=>{return m_champ.m_creatureProperty.getExpRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxExp).ToString();
			}
		);

		m_guages[2] = new YGUISystem.GUIGuage(transform.Find("Guage/SP").gameObject, 
		    ()=>{return m_champ.m_creatureProperty.getSPRemainRatio();}, 
			()=>{return Mathf.FloorToInt(m_champ.m_creatureProperty.SP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxSP).ToString();
			}
		);
	}

	void assignSkillButton(int slot, ItemObject itemObj, System.Func<bool> doFunctor)
	{
		m_specialButtons[slot] = new YGUISystem.GUIChargeButton(transform.Find("Special/Button"+slot).gameObject, ()=>{
			if (m_specialButtons[slot].MaxChargingPoint != itemObj.Item.Level)
			{
				m_specialButtons[slot].MaxChargingPoint = itemObj.Item.Level;
				m_specialButtons[slot].ChargingPoint++;
			}

			if (m_champ.SkillStacks[itemObj.Item.RefItem.id-21] > 0)
			{
				m_specialButtons[slot].ChargingPoint++;
				m_champ.SkillStacks[itemObj.Item.RefItem.id-21] = 0;
			}
			return itemObj.Item.Level > 0 || m_specialButtons[slot].ChargingPoint > 0;
		});
		
		m_specialButtons[slot].Icon.Image = itemObj.ItemIcon;
		
		m_specialButtons[slot].DoFunctor = doFunctor;
		m_specialButtons[slot].CoolDownTime = itemObj.Item.RefItem.weaponStat.coolTime;

	}

	public void OnClickLevelUp(bool autoAssigned)
	{
		if (m_champ.RemainStatPoint == 0)
			return;

		ChampAbilityGUI abilityGui = GameObject.Find("HudGUI/AbilityGUI").transform.Find("Panel").gameObject.GetComponent<ChampAbilityGUI>();
		abilityGui.AutoAssigned = autoAssigned;
		abilityGui.gameObject.SetActive(true);

		m_accessoryBoard.GetComponent<Animator>().SetTrigger("SlidingDown");
	}

	public void SlidingNormalAccessoryBoard()
	{
		m_accessoryBoard.GetComponent<Animator>().SetTrigger("SlidingNormal");
	}

	public void OnClickOption()
	{
		GameObject.Find("HudGUI/OptionGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	public void OnClickSkill(int slot)
	{
		if (m_champ.SkillStacks[slot] > 0)
		{
			if (m_specialButtons[slot].DoFunctor.Invoke() == false)
				return;

			--m_champ.SkillStacks[slot];
			return;
		}
		if (m_specialButtons[slot].ChargingPoint == 0)
			return;

		if (m_specialButtons[slot].DoFunctor.Invoke() == false)
			return;

		--m_specialButtons[slot].ChargingPoint;
	}

	public void OnClickAutoEarnGold()
	{
		if (Warehouse.Instance.AutoEarnGold == 0)
			return;

		Const.GetSpawn().SharePotinsChamps(m_champ, ItemData.Type.Gold, Warehouse.Instance.AutoEarnGold, true);

		Warehouse.Instance.AutoEarnGold = 0;
	}

	public void OnClickAccessory(int slot)
	{
		if (m_champ.AccessoryItems[slot] == null)
			return;

		if (m_accessoryButtons[slot].ChargingPoint == 0)
			return;

		if (m_champ.AccessoryItems[slot].Item.Usable(m_champ) == false)
			return;

		--m_accessoryButtons[slot].ChargingPoint;

		for(int i = 0; i < m_champ.m_creatureProperty.CallableFollowers; ++i)
			m_champ.AccessoryItems[slot].Item.Use(m_champ);

	}



	void SetActiveGUI(bool active)
	{
		transform.Find("Guage").gameObject.SetActive(active);
		transform.Find("Accessory").gameObject.SetActive(active);
		transform.Find("Special").gameObject.SetActive(active);
		transform.Find("Option").gameObject.SetActive(active);
		transform.Find("Gold").gameObject.SetActive(active);
		transform.Find("GoldMedal").gameObject.SetActive(active);
		transform.Find("Kills").gameObject.SetActive(active);
		transform.Find("Gem").gameObject.SetActive(active);
		transform.Find("DNA").gameObject.SetActive(active);
		transform.Find("Level").gameObject.SetActive(active);
	}

	void Update()
	{		
		if (m_champ == null)
		{
			GameObject obj = GameObject.Find("Champ");
			if (obj == null)
			{
				SetActiveGUI(false);
				return;
			}

			m_champ = obj.GetComponent<Champ>();

			SetActiveGUI(true);
		}

		m_level.Text.text = m_champ.GoldLevel.ToString();

		if (m_oldMobKills != Warehouse.Instance.AlienEssence.Item.Count)
		{
			m_oldMobKills = Warehouse.Instance.AlienEssence.Item.Count;
			m_mobKills.enabled = true;
			m_mobKills.shake = 2f;
			m_mobKills.Text = Warehouse.Instance.AlienEssence.Item.Count.ToString();
		}

		//m_mobKills.Text = System.String.Format("{0}/{1:00}:{2:00}", m_champ.MobKills, Warehouse.Instance.PlayTime / 60, Warehouse.Instance.PlayTime % 60);

		if (m_oldGold != Warehouse.Instance.Gold.Item.Count)
		{
			m_oldGold = Warehouse.Instance.Gold.Item.Count;
			m_gold.enabled = true;
			m_gold.shake = 2f;
			m_gold.Text = Warehouse.Instance.Gold.Item.Count.ToString();
		}
		if (m_oldGoldMedal != Warehouse.Instance.GoldMedal.Item.Count)
		{
			m_oldGoldMedal = Warehouse.Instance.GoldMedal.Item.Count;
			m_goldMedal.enabled = true;
			m_goldMedal.shake = 2f;
			m_goldMedal.Text = Warehouse.Instance.GoldMedal.Item.Count.ToString();
		}
		if (m_oldDNA != Warehouse.Instance.WeaponDNA.Item.Count)
		{
			m_oldDNA = Warehouse.Instance.WeaponDNA.Item.Count;
			m_dna.enabled = true;
			m_dna.shake = 2f;
			m_dna.Text = Warehouse.Instance.WeaponDNA.Item.Count.ToString();
		}
		if (m_oldGem != Warehouse.Instance.Gem.Item.Count)
		{
			m_oldGem = Warehouse.Instance.Gem.Item.Count;
			m_gem.enabled = true;
			m_gem.shake = 2f;
			m_gem.Text = Warehouse.Instance.Gem.Item.Count.ToString();
		}

		foreach(YGUISystem.GUIButton button in m_specialButtons)
		{
			button.Update();
		}

		foreach(YGUISystem.GUIButton button in m_accessoryButtons)
		{
			button.Update();
		}

		foreach(YGUISystem.GUIGuage guage in m_guages)
		{
			guage.Update();
		}

		m_autoEarnButton.Update();
	}

}
