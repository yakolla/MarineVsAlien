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
	YGUISystem.GUILable 	m_tapDamage;
	YGUISystem.GUILable 	m_lifeSteal;

	YGUISystem.GUILable 	m_statisticsDealDmgPS;
	YGUISystem.GUILable 	m_statisticsTakenDmgPS;
	YGUISystem.GUILable 	m_statisticsKillPS;

	void Start()
	{
		m_strength =  new YGUISystem.GUILable(transform.Find("ScrollView/Contents/Strength/Text").gameObject);
		m_regenSP = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/RegenSP/Text").gameObject);
		m_gainExtraGold = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/GainExtraGold/Text").gameObject);
		m_damageReduction = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/DamageReduction/Text").gameObject);
		m_damageMultiplier = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/DamageMultiplier/Text").gameObject);
		m_criticalChance = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/CriticalChance/Text").gameObject);
		m_criticalDamage = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/CriticalDamage/Text").gameObject);
		m_tapDamage = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/TapDamage/Text").gameObject);
		m_lifeSteal = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/LifeSteal/Text").gameObject);
		m_statisticsDealDmgPS = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/StatisticsDealDmgPS/Text").gameObject);
		m_statisticsTakenDmgPS = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/StatisticsTakenDmgPS/Text").gameObject);
		m_statisticsKillPS = new YGUISystem.GUILable(transform.Find("ScrollView/Contents/StatisticsKillPS/Text").gameObject);
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
		m_tapDamage.Text.text = m_champ.m_creatureProperty.TapDamage.ToString();
		m_gainExtraGold.Text.text = (m_champ.m_creatureProperty.GainExtraGold*100f).ToString() + "%";
		m_damageReduction.Text.text = (m_champ.m_creatureProperty.DamageReduction*100f).ToString() + "%";
		m_damageMultiplier.Text.text = (m_champ.m_creatureProperty.DamageRatio*100f).ToString() + "%";
		m_criticalChance.Text.text = (m_champ.m_creatureProperty.CriticalChance*100f).ToString() + "%";
		m_criticalDamage.Text.text = (m_champ.m_creatureProperty.CriticalDamage*100f).ToString() + "%";
		m_lifeSteal.Text.text = (m_champ.m_creatureProperty.LifeSteal*100f).ToString() + "%";


		Warehouse.Instance.UpdateGameStats.Update();
		m_statisticsKillPS.Text.text = System.String.Format("{0:F2}",Warehouse.Instance.UpdateGameStats.KillPerSec);
		m_statisticsDealDmgPS.Text.text = System.String.Format("{0:F2}",Warehouse.Instance.UpdateGameStats.DealDamagePerSec);
		m_statisticsTakenDmgPS.Text.text = System.String.Format("{0:F2}",Warehouse.Instance.UpdateGameStats.TakenDamagePerSec);

	}
}


