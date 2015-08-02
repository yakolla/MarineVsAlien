using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GameOverGUI : MonoBehaviour {

	ADMob					m_admob;

	YGUISystem.GUIPriceButton	m_continueButton;
	YGUISystem.GUIGuage[] 	m_guages = new YGUISystem.GUIGuage[1];
	YGUISystem.GUILable	m_restartText;
	string[]				m_leaderBoards = {Const.LEADERBOARD_KILLED_MOBS};

	void Start () {


		m_restartText = new YGUISystem.GUILable(transform.Find("RestartButton/Text").gameObject);
		m_restartText.Text.text = "Continue at " + (Warehouse.Instance.WaveIndex/2+1) + " wave";

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

		for(int i = 0; i < m_guages.Length; ++i)
		{
			m_guages[i].Update();
		}


		m_continueButton = new YGUISystem.GUIPriceButton(transform.Find("ContinueButton").gameObject, Const.StartPosYOfPriceButtonImage, ()=>{
			return true;
		});


		m_continueButton.NormalWorth = Warehouse.Instance.WaveIndex;
		m_continueButton.Prices = RefData.Instance.RefItems[Const.RandomAbilityRefItemId].levelup.conds;

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
		m_continueButton.Update();
	}

	public void OnClickRestart()
	{
		m_admob.ShowBanner(false);

		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Restart", 0);

		Warehouse.Instance.WaveIndex /= 2;
		Warehouse.Instance.KilledMobs /= 2;


		SaveGame(false);
	}

	public void OnClickContinue()
	{
		if (true == m_continueButton.TryToPay())
		{
			m_admob.ShowBanner(false);
			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Continue", 0);
			
			SaveGame(false);
		}
	}

	public void OnClickTitle()
	{
		m_admob.ShowBanner(false);
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Title", 0);


		Warehouse.Instance.WaveIndex /= 2;
		Warehouse.Instance.KilledMobs /= 2;

		SaveGame(true);
	}

	public void OnClickLeaderBoard(int slot)
	{
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Leaderboard"+slot, 0);
		GPlusPlatform.Instance.ShowLeaderboardUI(m_leaderBoards[slot]);
	}

}
