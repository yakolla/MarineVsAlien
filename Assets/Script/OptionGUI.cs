using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class OptionGUI : MonoBehaviour {

	ADMob					m_admob;
	Slider					m_sfxVolume;
	Slider					m_bgmVolume;

	YGUISystem.GUILable		m_close;
	YGUISystem.GUILable		m_share;
	YGUISystem.GUILable		m_credits;
	YGUISystem.GUILable		m_fanPage;

	YGUISystem.GUIGuage 	m_guages;
	Slider	m_waveSlider;
	Text	m_waveSliderText;

	YGUISystem.GUILable		m_wave;

	void Start () {

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();

		m_sfxVolume = transform.Find("SFXGUI/Slider").gameObject.GetComponent<Slider>();
		m_bgmVolume = transform.Find("BGMGUI/Slider").gameObject.GetComponent<Slider>();

		m_sfxVolume.value = Warehouse.Instance.GameOptions.m_sfxVolume;
		m_bgmVolume.value = Warehouse.Instance.GameOptions.m_bgmVolume;

		m_close = new YGUISystem.GUILable(transform.Find("CloseButton/Text").gameObject);
		m_close.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Close);

		m_share = new YGUISystem.GUILable(transform.Find("ShareButton/Text").gameObject);
		m_share.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Share);

		m_credits = new YGUISystem.GUILable(transform.Find("CreditsButton/Text").gameObject);
		m_credits.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Credits);

		m_fanPage = new YGUISystem.GUILable(transform.Find("FanPageButton/Text").gameObject);
		m_fanPage.Text.text = RefData.Instance.RefTexts(MultiLang.ID.FanPage);

		m_wave = new YGUISystem.GUILable(transform.Find("RestartWaveGUI/Slider/Lable").gameObject);
		m_wave.Text.text = RefData.Instance.RefTexts(MultiLang.ID.RestartWave);
		m_waveSliderText = transform.Find("RestartWaveGUI/Slider/Text").gameObject.GetComponent<Text>();
		m_waveSlider = transform.Find("RestartWaveGUI/Slider").gameObject.GetComponent<Slider>();
		m_waveSlider.minValue = 0;
		m_waveSlider.maxValue = Warehouse.Instance.GameBestStats.WaveIndex+1;
		m_waveSlider.value = Warehouse.Instance.GameOptions.m_reWaveIndex.Value;
	}

	void Update()
	{
		m_waveSliderText.text = (Warehouse.Instance.GameOptions.m_reWaveIndex.Value+1).ToString() + " / " + (Warehouse.Instance.GameBestStats.WaveIndex+1).ToString();
		m_waveSlider.maxValue = Warehouse.Instance.GameBestStats.WaveIndex+1;
	}

	void OnEnable() {
		if (m_admob != null)
			m_admob.ShowBanner(true);

		if (m_waveSlider != null)
			m_waveSlider.value = Warehouse.Instance.GameOptions.m_reWaveIndex.Value;

		TimeEffector.Instance.StopTime();
	}

	void OnDisable() {
		m_admob.ShowBanner(false);
		TimeEffector.Instance.StartTime();
	}

	public void OnSliderBGM()
	{
		Const.GetSpawn().audio.volume = m_bgmVolume.value;
		Warehouse.Instance.GameOptions.m_bgmVolume = m_bgmVolume.value;
	}

	public void OnSliderSFX()
	{
		m_sfxVolume.audio.Play();
		AudioListener.volume = m_sfxVolume.value;
		Warehouse.Instance.GameOptions.m_sfxVolume = m_sfxVolume.value;
	}

	public void OnClickOk()
	{
		GameObject champObj = GameObject.Find("Champ");
		if (champObj != null)
		{
			champObj.GetComponent<Champ>().ApplyGameOptions();
		}

		gameObject.SetActive(false);

	}

	public void OnClickTitle()
	{
		Const.ShowLoadingGUI("Loading...");

		Const.SaveGame((SavedGameRequestStatus status) => {
			if (status == SavedGameRequestStatus.Success) {
				// handle reading or writing of saved game.
			} else {
				// handle error
			}

			m_admob.ShowBanner(false);
			TimeEffector.Instance.StartTime();
			Application.LoadLevel("Worldmap");

			//Const.HideLoadingGUI();
		});

	}

	public void OnClickRate()
	{
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Options", "Rate", 0);
		//Application.OpenURL ("market://details?id=com.banegole.marinegrowing");


		AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");
		intentObject.Call<AndroidJavaObject> ("setAction", intentClass.GetStatic<string> ("ACTION_SEND"));
		intentObject.Call<AndroidJavaObject> ("setType", "text/plain");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Tap Marine");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "https://play.google.com/store/apps/details?id="+Const.PackageName);
		AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");
		currentActivity.Call ("startActivity", intentObject);
	}

	public void OnClickCredits()
	{
		transform.parent.Find("CreditsPanel").gameObject.SetActive(true);
	}

	public void OnClickFanPage()
	{

		Application.OpenURL ("https://www.facebook.com/%EC%9D%B8%EB%94%94-%EA%B2%8C%EC%9E%84%ED%8C%80-BaneGoleYC-842471115871641");
	}

	public void OnWaveSliderChanged(float value)
	{
		Debug.Log("OnWaveSliderChanged:" + m_waveSlider.value);
		Warehouse.Instance.GameOptions.m_reWaveIndex.Value = Mathf.Min(Warehouse.Instance.GameBestStats.WaveIndex, ((int)m_waveSlider.value/Const.GetSpawn().GetCurrentWave().mobSpawns.Length)*Const.GetSpawn().GetCurrentWave().mobSpawns.Length);

	}
}
