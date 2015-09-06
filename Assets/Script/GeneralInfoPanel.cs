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

	YGUISystem.GUIGuage[] 	m_guages = new YGUISystem.GUIGuage[5];

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

		m_guages[0] = new YGUISystem.GUIGuage(transform.Find("ScrollView/Contents/StatisticsDealDmgPS/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.UpdateGameStats.DealDamagePerSec == 0)
				return 0f;
			return Warehouse.Instance.UpdateGameStats.DealDamagePerSec/Warehouse.Instance.UpdateGameStats.MaxDealDamagePerSec;
		}, 
		()=>{
			if (Warehouse.Instance.UpdateGameStats.MaxDealDamagePerSec == 0)
				return 0 + " / " + 0;

			return System.String.Format("{0:F2} / {1:F2}",Warehouse.Instance.UpdateGameStats.DealDamagePerSec, Warehouse.Instance.UpdateGameStats.MaxDealDamagePerSec); 
		}
		);

		m_guages[1] = new YGUISystem.GUIGuage(transform.Find("ScrollView/Contents/StatisticsTakenDmgPS/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.UpdateGameStats.TakenDamagePerSec == 0)
				return 0f;
			return Warehouse.Instance.UpdateGameStats.TakenDamagePerSec/Warehouse.Instance.UpdateGameStats.MaxTakenDamagePerSec;
		}, 
		()=>{
			if (Warehouse.Instance.UpdateGameStats.MaxTakenDamagePerSec == 0)
				return 0 + " / " + 0;
			
			return System.String.Format("{0:F2} / {1:F2}",Warehouse.Instance.UpdateGameStats.TakenDamagePerSec, Warehouse.Instance.UpdateGameStats.MaxTakenDamagePerSec); 
		}
		);

		m_guages[2] = new YGUISystem.GUIGuage(transform.Find("ScrollView/Contents/StatisticsKillPS/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.UpdateGameStats.KillPerSec == 0)
				return 0f;
			return Warehouse.Instance.UpdateGameStats.KillPerSec/Warehouse.Instance.UpdateGameStats.MaxKillPerSec;
		}, 
		()=>{
			if (Warehouse.Instance.UpdateGameStats.MaxKillPerSec == 0)
				return 0 + " / " + 0;
			
			return System.String.Format("{0:F2} / {1:F2}",Warehouse.Instance.UpdateGameStats.KillPerSec, Warehouse.Instance.UpdateGameStats.MaxKillPerSec); 
		}
		);

		m_guages[3] = new YGUISystem.GUIGuage(transform.Find("ScrollView/Contents/StatisticsConsumedSPPS/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.UpdateGameStats.ConsumedSPPerSec == 0)
				return 0f;
			return Warehouse.Instance.UpdateGameStats.ConsumedSPPerSec/Warehouse.Instance.UpdateGameStats.MaxConsumedSPPerSec;
		}, 
		()=>{
			if (Warehouse.Instance.UpdateGameStats.MaxConsumedSPPerSec == 0)
				return 0 + " / " + 0;
			
			return System.String.Format("{0:F2} / {1:F2}",Warehouse.Instance.UpdateGameStats.ConsumedSPPerSec, Warehouse.Instance.UpdateGameStats.MaxConsumedSPPerSec); 
		}
		);

		m_guages[4] = new YGUISystem.GUIGuage(transform.Find("ScrollView/Contents/StatisticsWave/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.GameBestStats.WaveIndex == 0)
				return 1f;
			return (float)(Warehouse.Instance.NewGameStats.WaveIndex+1)/(Warehouse.Instance.GameBestStats.WaveIndex+1);
		}, 
		()=>{
			if (Warehouse.Instance.GameBestStats.WaveIndex == 0)
				return "1 / 1";
			return (Warehouse.Instance.NewGameStats.WaveIndex+1).ToString() + " / " + (Warehouse.Instance.GameBestStats.WaveIndex+1).ToString(); 
		}
		);
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
		foreach(YGUISystem.GUIGuage guage in m_guages)
		{
			guage.Update();
		}

	}
}


