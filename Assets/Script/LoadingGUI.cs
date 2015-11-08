using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class LoadingGUI : MonoBehaviour {

	ADMob					m_admob;
	Text					m_lable;

	void Awake()
	{
		
		m_lable = transform.Find("Panel/Image/Text").GetComponent<Text>();
	}

	public void SetActive(bool act) 
	{
		if (m_admob == null)
		{
			GameObject obj = GameObject.Instantiate(Resources.Load("Pref/ADMob")) as GameObject;
			m_admob = obj.GetComponent<ADMob>();
		}
		m_admob.ShowBanner(act);

		gameObject.SetActive(act);
	}

	public void SetText(string text)
	{
		m_lable.text = text;
	}

}
