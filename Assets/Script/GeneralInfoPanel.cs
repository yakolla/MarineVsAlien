using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GeneralInfoPanel : MonoBehaviour {

	Champ	m_champ;
	YGUISystem.GUILable 	m_strength;
	YGUISystem.GUILable 	m_regenSP;
	YGUISystem.GUILable 	m_gainExtraGold;
	YGUISystem.GUILable 	m_damageReduction;
	YGUISystem.GUILable 	m_damageMultiplier;
	YGUISystem.GUILable 	m_criticalChance;
	YGUISystem.GUILable 	m_criticalDamage;

	void Start()
	{
		m_strength =  new YGUISystem.GUILable(transform.Find("Strength/Text").gameObject);
		m_regenSP = new YGUISystem.GUILable(transform.Find("RegenSP/Text").gameObject);
		m_gainExtraGold = new YGUISystem.GUILable(transform.Find("GainExtraGold/Text").gameObject);
		m_damageReduction = new YGUISystem.GUILable(transform.Find("DamageReduction/Text").gameObject);
		m_damageMultiplier = new YGUISystem.GUILable(transform.Find("DamageMultiplier/Text").gameObject);
		m_criticalChance = new YGUISystem.GUILable(transform.Find("CriticalChance/Text").gameObject);
		m_criticalDamage = new YGUISystem.GUILable(transform.Find("CriticalDamage/Text").gameObject);
	}

	public void SetChamp(Champ champ)
	{
		m_champ = champ;
	}

	void Update()
	{
		if (m_champ == null)
			return;

		m_strength.Text.text = m_champ.m_creatureProperty.Strength.ToString();
		m_regenSP.Text.text = m_champ.m_creatureProperty.SPRegen.ToString();
		m_gainExtraGold.Text.text = (m_champ.m_creatureProperty.GainExtraGold*100f).ToString() + "%";
		m_damageReduction.Text.text = (m_champ.m_creatureProperty.DamageReduction*100f).ToString() + "%";
		m_damageMultiplier.Text.text = (m_champ.m_creatureProperty.DamageRatio*100f).ToString() + "%";
		m_criticalChance.Text.text = (m_champ.m_creatureProperty.CriticalChance*100f).ToString() + "%";
		m_criticalDamage.Text.text = (m_champ.m_creatureProperty.CriticalDamage*100f).ToString() + "%";
	}
}


