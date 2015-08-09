using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GoMainTitleGUI : MonoBehaviour {

	ADMob					m_admob;

	void Start () {

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();

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

	public void OnClickYes()
	{
		Const.ShowLoadingGUI("Loading...");
		Const.SaveGame((SavedGameRequestStatus status, ISavedGameMetadata game)=>{			

			Application.LoadLevel("Worldmap");
			TimeEffector.Instance.StartTime();
		});

	}

	public void OnClickNo()
	{
		gameObject.SetActive(false);
	}

}
