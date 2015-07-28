using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class KeyEventMgr : MonoBehaviour {

	GameObject	m_settings;
	GameObject	m_shop;
	GameObject	m_option;
	GameObject	m_goMainTitle;
	GameObject	m_exit;
	GameObject	m_credits;


	void Start()
	{
		m_settings = transform.Find("SettingGUI/Panel").gameObject;
		m_shop = transform.Find("ShopGUI/Panel").gameObject;
		m_option = transform.Find("OptionGUI/Panel").gameObject;
		m_goMainTitle = transform.Find("GoMainTitleGUI/Panel").gameObject;
		m_exit = transform.Find("ExitGUI/Panel").gameObject;
		m_credits = transform.Find("OptionGUI/CreditsPanel").gameObject;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			if (m_shop.activeSelf)
				return;

			if (m_credits.activeSelf)
			{
				m_credits.SetActive(false);
				return;
			}
			if (m_option.activeSelf)
			{
				m_option.SetActive(false);
				return;
			}
			if (m_settings.activeSelf)
			{
				m_goMainTitle.SetActive(true);
				return;
			}

			m_exit.SetActive(true);
		}
	}
}

