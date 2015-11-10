using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class LoadingGUI : MonoBehaviour {

	Text					m_lable;

	void Awake()
	{
		
		m_lable = transform.Find("Panel/Image/Text").GetComponent<Text>();
	}

	public void SetActive(bool act) 
	{
		gameObject.SetActive(act);
	}

	public void SetText(string text)
	{
		m_lable.text = text;
	}

}
