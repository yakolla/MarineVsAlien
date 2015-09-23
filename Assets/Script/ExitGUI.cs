using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class ExitGUI : MonoBehaviour {

	YGUISystem.GUILable		m_title;
	YGUISystem.GUILable		m_yes;
	YGUISystem.GUILable		m_no;

	void Start () {
		m_title = new YGUISystem.GUILable(transform.Find("Image/Text").gameObject);
		m_title.Text.text = RefData.Instance.RefTexts(MultiLang.ID.ExitTheGame);
		
		m_yes = new YGUISystem.GUILable(transform.Find("YesButton/Text").gameObject);
		m_yes.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Yes);
		
		m_no = new YGUISystem.GUILable(transform.Find("NoButton/Text").gameObject);
		m_no.Text.text = RefData.Instance.RefTexts(MultiLang.ID.No);
	}

	public void OnClickYes()
	{
		Application.Quit();
	}

	public void OnClickNo()
	{
		gameObject.SetActive(false);
	}

}
