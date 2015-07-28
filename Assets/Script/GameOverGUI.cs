using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameOverGUI : MonoBehaviour {

	ADMob					m_admob;

	YGUISystem.GUIGuage[] 	m_guages = new YGUISystem.GUIGuage[1];
	int						m_restartCount = 0;
	string[]				m_leaderBoards = {Const.LEADERBOARD_KILLED_MOBS};

	void Start () {



		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();
		/*
		m_guages[0] = new YGUISystem.GUIGuage(transform.Find("Gained Gold/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.GameBestStats.GainedGold == 0)
				return 1f;
			return (float)Warehouse.Instance.NewGameStats.GainedGold/Warehouse.Instance.GameBestStats.GainedGold;
		}, 
		()=>{
			if (Warehouse.Instance.GameBestStats.GainedGold == 0)
				return Warehouse.Instance.NewGameStats.GainedGold.ToString() + " / " + Warehouse.Instance.NewGameStats.GainedGold.ToString();

			return Warehouse.Instance.NewGameStats.GainedGold.ToString() + " / " + Warehouse.Instance.GameBestStats.GainedGold.ToString(); 
		}
		);

		m_guages[1] = new YGUISystem.GUIGuage(transform.Find("Gained XP/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.GameBestStats.GainedXP == 0)
				return 1f;
			return (float)Warehouse.Instance.NewGameStats.GainedXP/Warehouse.Instance.GameBestStats.GainedXP;
		}, 
		()=>{
			if (Warehouse.Instance.GameBestStats.GainedXP == 0)
				return Warehouse.Instance.NewGameStats.GainedXP.ToString() + " / " + Warehouse.Instance.NewGameStats.GainedXP.ToString();

			return Warehouse.Instance.NewGameStats.GainedXP.ToString() + " / " + Warehouse.Instance.GameBestStats.GainedXP.ToString(); 
		}
		);

		m_guages[2] = new YGUISystem.GUIGuage(transform.Find("Survival Time/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.GameBestStats.SurvivalTime == 0)
				return 1f;
			return Warehouse.Instance.NewGameStats.SurvivalTime/Warehouse.Instance.GameBestStats.SurvivalTime;
		}, 
		()=>{
			if (Warehouse.Instance.GameBestStats.SurvivalTime == 0)
				return Warehouse.Instance.NewGameStats.SurvivalTime.ToString() + " / " + Warehouse.Instance.NewGameStats.SurvivalTime.ToString();
			return Warehouse.Instance.NewGameStats.SurvivalTime.ToString() + " / " + Warehouse.Instance.GameBestStats.SurvivalTime.ToString(); 
		}
		);
*/
		m_guages[0] = new YGUISystem.GUIGuage(transform.Find("Killed Mobs/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.GameBestStats.KilledMobs == 0)
			    return 1f;
			return (float)Warehouse.Instance.NewGameStats.KilledMobs/Warehouse.Instance.GameBestStats.KilledMobs;
		}, 
		()=>{
			if (Warehouse.Instance.GameBestStats.KilledMobs == 0)
				return Warehouse.Instance.NewGameStats.KilledMobs.ToString() + " / " + Warehouse.Instance.NewGameStats.KilledMobs.ToString();
			return Warehouse.Instance.NewGameStats.KilledMobs.ToString() + " / " + Warehouse.Instance.GameBestStats.KilledMobs.ToString(); 
		}
		);


		for(int i = 0; i < m_guages.Length; ++i)
		{
			m_guages[i].Update();
		}

		m_admob.ShowInterstitial();
		m_admob.ShowBanner(true);

	}


	void OnEnable() {
		TimeEffector.Instance.StopTime();
		GPlusPlatform.Instance.AnalyticsTrackScreen("GameOverGUI");
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	public void OnClickRestart()
	{
		m_admob.ShowBanner(false);
		++m_restartCount;
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Restart"+m_restartCount, 0);
		Application.LoadLevel("Basic Dungeon");
	}

	public void OnClickTitle()
	{
		m_admob.ShowBanner(false);
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Title", 0);
		Const.ShowLoadingGUI("Loading...");
		Application.LoadLevel("Worldmap");
	}

	public void OnClickLeaderBoard(int slot)
	{
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Leaderboard"+slot, 0);
		GPlusPlatform.Instance.ShowLeaderboardUI(m_leaderBoards[slot]);
	}

}
