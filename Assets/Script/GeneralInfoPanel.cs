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
	YGUISystem.GUILable 	m_dodge;

	YGUISystem.GUIGuage[] 	m_guages = new YGUISystem.GUIGuage[5];
	enum DirIndex
	{
		Strength,
		RegenSP,
		GainExtraGold,
		DamageReduction,
		DamageMultiplier,
		CriticalChance,
		CriticalDamage,
		TapDamage,
		LifeSteal,
		Dodge,
		StatisticsDealDmgPS,
		StatisticsTakenDmgPS,
		StatisticsKillPS,
		StatisticsConsumedSPPS,
		StatisticsWave,
	}
	void Start()
	{

		string[] dir = {"Contents/Strength", 
						"Contents/RegenSP", 
						"Contents/GainExtraGold", 
						"Contents/DamageReduction",
						"Contents/DamageMultiplier",
						"Contents/CriticalChance",
						"Contents/CriticalDamage",
						"Contents/TapDamage",
						"Contents/LifeSteal",
			"Contents/Dodge",
			"Contents/StatisticsDealDmgPS",
			"Contents/StatisticsTakenDmgPS",
			"Contents/StatisticsKillPS",
			"Contents/StatisticsConsumedSPPS",
			"Contents/StatisticsWave",
					};
		transform.Find(dir[(int)DirIndex.Strength]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.Strength) + ":";
		transform.Find(dir[(int)DirIndex.RegenSP]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.RegenSP) + ":";
		transform.Find(dir[(int)DirIndex.GainExtraGold]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.GainExtraGold) + ":";
		transform.Find(dir[(int)DirIndex.DamageReduction]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.Deffence) + ":";
		transform.Find(dir[(int)DirIndex.DamageMultiplier]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.DamageMultiplier) + ":";
		transform.Find(dir[(int)DirIndex.CriticalChance]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.CriticalChance) + ":";
		transform.Find(dir[(int)DirIndex.CriticalDamage]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.CriticalDamage) + ":";
		transform.Find(dir[(int)DirIndex.TapDamage]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.TapDamage) + ":";
		transform.Find(dir[(int)DirIndex.LifeSteal]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.LifeSteal) + ":";
		transform.Find(dir[(int)DirIndex.Dodge]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.Dodge) + ":";
		transform.Find(dir[(int)DirIndex.StatisticsDealDmgPS]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.DealDmgPerSec) + ":";
		transform.Find(dir[(int)DirIndex.StatisticsTakenDmgPS]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.TakenDmgPerSec) + ":";
		transform.Find(dir[(int)DirIndex.StatisticsKillPS]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.KillMobs) + ":";
		transform.Find(dir[(int)DirIndex.StatisticsConsumedSPPS]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.ConsumeSPPerSec) + ":";
		transform.Find(dir[(int)DirIndex.StatisticsWave]).GetComponent<Text>().text = RefData.Instance.RefTexts(MultiLang.ID.Wave) + ":";

		m_strength =  new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.Strength]+"/Text").gameObject);
		m_regenSP = new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.RegenSP]+"/Text").gameObject);
		m_gainExtraGold = new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.GainExtraGold]+"/Text").gameObject);
		m_damageReduction = new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.DamageReduction]+"/Text").gameObject);
		m_damageMultiplier = new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.DamageMultiplier]+"/Text").gameObject);
		m_criticalChance = new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.CriticalChance]+"/Text").gameObject);
		m_criticalDamage = new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.CriticalDamage]+"/Text").gameObject);
		m_tapDamage = new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.TapDamage]+"/Text").gameObject);
		m_lifeSteal = new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.LifeSteal]+"/Text").gameObject);
		m_dodge = new YGUISystem.GUILable(transform.Find(dir[(int)DirIndex.Dodge]+"/Text").gameObject);

		m_guages[0] = new YGUISystem.GUIGuage(transform.Find(dir[(int)DirIndex.StatisticsDealDmgPS]+"/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.NewGameStats.DealDamagePerSec == 0)
				return 0f;
			return Warehouse.Instance.NewGameStats.DealDamagePerSec/Warehouse.Instance.NewGameStats.MaxDealDamagePerSec;
		}, 
		()=>{
			if (Warehouse.Instance.NewGameStats.MaxDealDamagePerSec == 0)
				return 0 + " / " + 0;

			return System.String.Format("{0:F2} / {1}",Warehouse.Instance.NewGameStats.DealDamagePerSec, Warehouse.Instance.NewGameStats.MaxDealDamagePerSec); 
		}
		);

		m_guages[1] = new YGUISystem.GUIGuage(transform.Find(dir[(int)DirIndex.StatisticsTakenDmgPS]+"/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.NewGameStats.TakenDamagePerSec == 0)
				return 0f;
			return Warehouse.Instance.NewGameStats.TakenDamagePerSec/Warehouse.Instance.NewGameStats.MaxTakenDamagePerSec;
		}, 
		()=>{
			if (Warehouse.Instance.NewGameStats.MaxTakenDamagePerSec == 0)
				return 0 + " / " + 0;
			
			return System.String.Format("{0:F2} / {1}",Warehouse.Instance.NewGameStats.TakenDamagePerSec, Warehouse.Instance.NewGameStats.MaxTakenDamagePerSec); 
		}
		);

		m_guages[2] = new YGUISystem.GUIGuage(transform.Find(dir[(int)DirIndex.StatisticsKillPS]+"/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.GameBestStats.KilledMobs == 0)
				return 0f;
			return (float)(Warehouse.Instance.NewGameStats.KilledMobs)/Warehouse.Instance.GameBestStats.KilledMobs;
		}, 
		()=>{
			if (Warehouse.Instance.GameBestStats.KilledMobs == 0)
				return 0 + " / " + 0;
			
			return System.String.Format("{0} / {1}",Warehouse.Instance.NewGameStats.KilledMobs, Warehouse.Instance.GameBestStats.KilledMobs); 
		}
		);

		m_guages[3] = new YGUISystem.GUIGuage(transform.Find(dir[(int)DirIndex.StatisticsConsumedSPPS]+"/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.NewGameStats.ConsumedSPPerSec == 0)
				return 0f;
			return Warehouse.Instance.NewGameStats.ConsumedSPPerSec/Warehouse.Instance.NewGameStats.MaxConsumedSPPerSec;
		}, 
		()=>{
			if (Warehouse.Instance.NewGameStats.MaxConsumedSPPerSec == 0)
				return 0 + " / " + 0;
			
			return System.String.Format("{0:F2} / {1}",Warehouse.Instance.NewGameStats.ConsumedSPPerSec, Warehouse.Instance.NewGameStats.MaxConsumedSPPerSec); 
		}
		);

		m_guages[4] = new YGUISystem.GUIGuage(transform.Find(dir[(int)DirIndex.StatisticsWave]+"/Guage/Guage").gameObject, 
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
		m_regenSP.Text.text = (m_champ.m_creatureProperty.SPRegen - Warehouse.Instance.NewGameStats.ConsumedSPPerSec).ToString();
		m_tapDamage.Text.text = m_champ.m_creatureProperty.TapDamage.ToString();
		m_gainExtraGold.Text.text = System.String.Format("{0:F0}%",m_champ.m_creatureProperty.GainExtraGold*100f);
		m_damageReduction.Text.text = System.String.Format("{0:F0}%",m_champ.m_creatureProperty.DamageReduction*100f);
		m_damageMultiplier.Text.text = System.String.Format("{0:F0}%",m_champ.m_creatureProperty.DamageMultiPlier*100f);
		m_criticalChance.Text.text = System.String.Format("{0:F0}%",m_champ.m_creatureProperty.CriticalChance*100f);
		m_criticalDamage.Text.text = System.String.Format("{0:F0}%",m_champ.m_creatureProperty.CriticalDamage*100f);
		m_lifeSteal.Text.text = System.String.Format("{0}",m_champ.m_creatureProperty.LifeSteal);
		m_dodge.Text.text = System.String.Format("{0:F0}%",m_champ.m_creatureProperty.Dodge*100f);


		foreach(YGUISystem.GUIGuage guage in m_guages)
		{
			guage.Update();
		}

	}
}


