using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GameOverGUI : MonoBehaviour {

	ADMob					m_admob;

	YGUISystem.GUIGuage[] 	m_guages = new YGUISystem.GUIGuage[2];

	string[]				m_leaderBoards = {Const.LEADERBOARD_KILLED_MOBS};

	void Start () {
	
		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();

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

		m_guages[1] = new YGUISystem.GUIGuage(transform.Find("Waves/Guage/Guage").gameObject, 
		                                      ()=>{
			if (Warehouse.Instance.GameBestStats.WaveIndex == 0)
				return 1f;
			return (float)Warehouse.Instance.NewGameStats.WaveIndex/Warehouse.Instance.GameBestStats.WaveIndex;
		}, 
		()=>{
			if (Warehouse.Instance.GameBestStats.WaveIndex == 0)
				return "1 / 1";
			return (Warehouse.Instance.NewGameStats.WaveIndex+1).ToString() + " / " + (Warehouse.Instance.GameBestStats.WaveIndex+1).ToString(); 
		}
		);


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

	void SaveGame(bool mainTitle)
	{
		Const.ShowLoadingGUI("Loading...");

		GPlusPlatform.Instance.ReportScore(Const.LEADERBOARD_KILLED_MOBS, Warehouse.Instance.NewGameStats.KilledMobs, (bool success) => {
			// handle success or failure
		});
		
		Const.SaveGame((SavedGameRequestStatus status, ISavedGameMetadata game) => {
			if (status == SavedGameRequestStatus.Success) {
				// handle reading or writing of saved game.
			} else {
				// handle error
			}
			
			TimeEffector.Instance.StartTime();

			if (mainTitle == true)
				Application.LoadLevel("Worldmap");
			else
				Application.LoadLevel("Basic Dungeon");
		});

	}

	void Update()
	{
		for(int i = 0; i < m_guages.Length; ++i)
		{
			m_guages[i].Update();
		}
	}

	public void OnClickDecWave()
	{
		Warehouse.Instance.NewGameStats.WaveIndex = Mathf.Max(0, --Warehouse.Instance.NewGameStats.WaveIndex);
	}

	public void OnClickIncWave()
	{
		Warehouse.Instance.NewGameStats.WaveIndex = Mathf.Min(Warehouse.Instance.GameBestStats.WaveIndex, ++Warehouse.Instance.NewGameStats.WaveIndex);
	}

	public void OnClickContinue()
	{

		m_admob.ShowBanner(false);
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Continue", 0);
		Warehouse.Instance.WaveIndex = Warehouse.Instance.NewGameStats.WaveIndex;
		SaveGame(false);
	}

	public void OnClickTitle()
	{
		m_admob.ShowBanner(false);
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Title", 0);

		SaveGame(true);
	}

	public void OnClickLeaderBoard(int slot)
	{
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Leaderboard"+slot, 0);
		GPlusPlatform.Instance.ShowLeaderboardUI(m_leaderBoards[slot]);
	}

}
