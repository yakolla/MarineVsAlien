using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class CreditsGUI : MonoBehaviour {

	YGUISystem.GUILable	m_close;

	void Start()
	{
		m_close = new YGUISystem.GUILable(transform.Find("CloseButton/Text").gameObject);
		m_close.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Close);
	}

	public void OnClickOk()
	{
		gameObject.SetActive(false);
	}

}
