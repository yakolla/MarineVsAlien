using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GoMainTitleGUI : MonoBehaviour {

	ADMob					m_admob;
	YGUISystem.GUILable		m_title;
	YGUISystem.GUILable		m_yes;
	YGUISystem.GUILable		m_no;

	void Start () {

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();
		m_title = new YGUISystem.GUILable(transform.Find("Image/Text").gameObject);
		m_title.Text.text = RefData.Instance.RefTexts(MultiLang.ID.GoToTheMainTitle);

		m_yes = new YGUISystem.GUILable(transform.Find("YesButton/Text").gameObject);
		m_yes.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Yes);

		m_no = new YGUISystem.GUILable(transform.Find("NoButton/Text").gameObject);
		m_no.Text.text = RefData.Instance.RefTexts(MultiLang.ID.No);
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
		Const.ShowLoadingGUI("Save...");
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
