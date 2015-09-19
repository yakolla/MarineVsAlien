using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GameOverGUI : MonoBehaviour {

	ADMob					m_admob;

	YGUISystem.GUIGuage[] 	m_guages = new YGUISystem.GUIGuage[2];

	string[]				m_leaderBoards = {Const.LEADERBOARD_KILLED_MOBS};
	Slider	m_waveSlider;
	Text	m_waveText;
	Text	m_continueText;
	int		m_trySave = 0;

	enum SaveWithType
	{
		MainTitle,
		Exit,
		Continue,
	}

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

		m_waveText = transform.Find("WaveGUI/Slider/Text").gameObject.GetComponent<Text>();
		m_waveText.text = (Warehouse.Instance.NewGameStats.WaveIndex+1).ToString() + " / " + (Warehouse.Instance.GameBestStats.WaveIndex+1).ToString();

		m_continueText = transform.Find("ContinueButton/Text").gameObject.GetComponent<Text>();
		m_continueText.text = "Continue at " + (Warehouse.Instance.NewGameStats.WaveIndex+1);

		m_waveSlider = transform.Find("WaveGUI/Slider").gameObject.GetComponent<Slider>();
		m_waveSlider.minValue = 0;
		m_waveSlider.maxValue = Warehouse.Instance.GameBestStats.WaveIndex+1;
		m_waveSlider.value = Warehouse.Instance.NewGameStats.WaveIndex+1;


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


		//m_admob.ShowInterstitial();
		m_admob.ShowBanner(true);


		Warehouse.Instance.NewGameStats.WaveIndex = 0;
		OnClickContinue();
	}


	void OnEnable() {
		TimeEffector.Instance.StopTime();

		GPlusPlatform.Instance.AnalyticsTrackScreen("GameOverGUI");
	}

	void OnDisable() {

		TimeEffector.Instance.StartTime();

	}

	void SaveGame(SaveWithType type)
	{
		++Warehouse.Instance.RetryCount;
		Const.ShowLoadingGUI(Warehouse.Instance.RetryCount + " Retry...");

		GPlusPlatform.Instance.ReportScore(Const.LEADERBOARD_KILLED_MOBS, Warehouse.Instance.GameBestStats.WaveIndex, (bool success) => {
			// handle success or failure
		});
		
		Const.SaveGame((SavedGameRequestStatus status, ISavedGameMetadata game) => {
			if (status == SavedGameRequestStatus.Success) {


			} else {
				// handle error
				++m_trySave;
				if (m_trySave < 3)
				{
					SaveGame(type);
					return;
				}
			}

			TimeEffector.Instance.StartTime();

			switch(type)
			{
			case SaveWithType.MainTitle:
				Application.LoadLevel("Worldmap");
				break;
			case SaveWithType.Continue:
				Application.LoadLevel("Basic Dungeon");
				break;
			case SaveWithType.Exit:
				Application.Quit();
				break;
			}
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

	public void OnWaveSliderChanged(float value)
	{
		Debug.Log("OnWaveSliderChanged:" + m_waveSlider.value);
		Warehouse.Instance.NewGameStats.WaveIndex = Mathf.Min(Warehouse.Instance.GameBestStats.WaveIndex, (int)m_waveSlider.value);
		m_waveText.text = (Warehouse.Instance.NewGameStats.WaveIndex+1).ToString() + " / " + (Warehouse.Instance.GameBestStats.WaveIndex+1).ToString();
		m_continueText.text = "Continue at " + (Warehouse.Instance.NewGameStats.WaveIndex+1);
	}

	public void OnClickContinue()
	{

		m_admob.ShowBanner(false);
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Continue", 0);
		Warehouse.Instance.WaveIndex = Warehouse.Instance.NewGameStats.WaveIndex;
		SaveGame(SaveWithType.Continue);
	}

	public void OnClickTitle()
	{
		m_admob.ShowBanner(false);
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Title", 0);

		SaveGame(SaveWithType.MainTitle);
	}

	public void OnClickExit()
	{
		SaveGame(SaveWithType.Exit);
	}

	public void OnClickLeaderBoard(int slot)
	{
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "GameOver", "Leaderboard"+slot, 0);
		GPlusPlatform.Instance.ShowLeaderboardUI(m_leaderBoards[slot]);
	}

}
