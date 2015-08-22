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
	GameObject	m_gameOver;


	void Start()
	{
		m_settings = Const.GetWindowGui(Const.WindowGUIType.SettingGUI);
		m_shop = Const.GetWindowGui(Const.WindowGUIType.ShopGUI);
		m_option = Const.GetWindowGui(Const.WindowGUIType.OptionGUI);
		m_goMainTitle = Const.GetWindowGui(Const.WindowGUIType.MainTitleGUI);
		m_exit = Const.GetWindowGui(Const.WindowGUIType.ExitGUI);
		m_credits = Const.GetWindowGui(Const.WindowGUIType.CreditsGUI);
		m_gameOver = Const.GetWindowGui(Const.WindowGUIType.GameOverGUI);
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
		{ 
			if (m_shop.activeSelf || m_gameOver.activeSelf)
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
				m_settings.SendMessage("OnEscapeKeyUp");
				return;
			}

			m_exit.SetActive(true);
		}
	}
}

