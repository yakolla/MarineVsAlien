using UnityEngine;
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

	}

	void OnEnable() {
		if (m_admob != null)
			m_admob.ShowBanner(true);

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

		Const.SaveGame((SavedGameRequestStatus status, ISavedGameMetadata game) => {
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
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "https://play.google.com/store/apps/details?id=com.banegole.marinevsalien");
		AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");
		currentActivity.Call ("startActivity", intentObject);
	}

	public void OnClickCredits()
	{
		transform.parent.Find("CreditsPanel").gameObject.SetActive(true);
		
	}
}
