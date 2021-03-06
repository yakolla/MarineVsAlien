﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;


public class Worldmap : MonoBehaviour {

	string		log = "log";
	GameObject	m_selectedMap;
	YGUISystem.GUILable		m_start;
	YGUISystem.GUILable		m_leadderBoard;

	enum LoginWith
	{
		Start,
		LeaderBoard,
		Achievement,
	}

	void Start()
	{
		m_start = new YGUISystem.GUILable(transform.Find("MainPanel/ButtonStart/Text").gameObject);
		m_leadderBoard = new YGUISystem.GUILable(transform.Find("MainPanel/ButtonLeaderBoard/Text").gameObject);

		m_start.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Start);
		m_leadderBoard.Text.text = RefData.Instance.RefTexts(MultiLang.ID.LeaderBoard);

		Const.HideLoadingGUI();
	}

	IEnumerator DelayMessage (string function, float delay)
	{
		
		yield return new WaitForSeconds(delay);
		
		SendMessage(function);
	}

	int m_try = 0;
	void OnOpenSavedGameForLoading(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			Warehouse.Instance.FileName = game.Filename;
			GPlusPlatform.Instance.LoadGame(game, OnReadGame);
		} else {
			if (m_try < 3)
			{
				++m_try;
				log = "OnOpenSavedGameForLoading:" + status + m_try;
				Const.ShowLoadingGUI(log);

				StartCoroutine(DelayMessage("OpenGame", 1f));
				return;
			}

			Const.HideLoadingGUI();
		}


		log = "OnOpenSavedGameForLoading:" + status + game;
	}

	void OnOpenSavedGameForSaving(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {

			System.TimeSpan totalPlayingTime = new System.TimeSpan(System.TimeSpan.TicksPerSecond*0);
			Warehouse.Instance.Reset();
			Warehouse.Instance.FileName = game.Filename;
			GPlusPlatform.Instance.SaveGame(game, Warehouse.Instance.Serialize(), totalPlayingTime, null, OnWriteGame);
		} else {
			// handle error
		}
		log = "OnSavedGameOpened:" + status + game;
	}

	void OnWriteGame (SavedGameRequestStatus status, ISavedGameMetadata game) {

		if (status == SavedGameRequestStatus.Success) {
			Application.LoadLevel("Basic Dungeon");
			Const.HideLoadingGUI();
		} else {

		}

		log = "OnSavedGameWritten:" + status + game;
	}

	void OnReadGame (SavedGameRequestStatus status, byte[] data) {
		log = "OnSavedGameDataRead:" + status;

		if (status == SavedGameRequestStatus.Success) {
			if (data.Length > 0)
			{
				Warehouse.Instance.Deserialize(data);

			}

			Application.LoadLevel("Basic Dungeon");
			//Const.HideLoadingGUI();
		} else {

		}


	}

	void Login(LoginWith loginWith)
	{

		if (Application.platform == RuntimePlatform.Android)
		{
			Const.ShowLoadingGUI("Try to login");

			GPlusPlatform.Instance.Login((bool success) => {
				// handle success or failure

				
				//if (success == true)
				{
					log = "Login success";
					Const.ShowLoadingGUI(log);

					switch(loginWith)
					{
					case LoginWith.Start:
						Const.ShowLoadingGUI("Loading...");
						m_try = 0;
						OpenGame();
						break;
					case LoginWith.LeaderBoard:
						GPlusPlatform.Instance.ShowLeaderboardUI(Const.LEADERBOARD_KILLED_MOBS);
						Const.HideLoadingGUI();
						break;
					case LoginWith.Achievement:
						GPlusPlatform.Instance.ShowAchievementsUI();
						Const.HideLoadingGUI();
						break;

					}

				}

			});
		}
	}
	/*
	public void OnGUI()
	{
		if (GUI.Button(new Rect(0, 0, 300, 100), log))
		{
			GPlusPlatform.Instance.ShowSavedGameBoard(3, (SelectUIStatus status, ISavedGameMetadata game) => {
				if (status == SelectUIStatus.SavedGameSelected) {
					
					string fileName = game.Filename;
					if (fileName.Equals(""))
					{
						fileName = System.DateTime.Now.Ticks.ToString();
						GPlusPlatform.Instance.OpenGame(fileName, OnOpenSavedGameForSaving);
						
					}
					else
					{
						GPlusPlatform.Instance.OpenGame(fileName, OnOpenSavedGameForLoading);
					}
					
					
				} else {
					// handle cancel or error
					Const.HideLoadingGUI();
				}
				log = status.ToString();
			});
		}
	}
*/
	public void OnClickStart()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Login(LoginWith.Start);
		}
		else
		{
			Warehouse.Instance.Deserialize(Warehouse.Instance.Serialize());
			Application.LoadLevel("Basic Dungeon");
		}
	}

	public void OpenGame()
	{		
		GPlusPlatform.Instance.OpenGame("marineVsAlien.sav", OnOpenSavedGameForLoading);
	}

	public void OnClickLeaderBoard()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Login(LoginWith.LeaderBoard);
		}

	}

	public void OnClickAchievement()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Login(LoginWith.Achievement);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			transform.Find("ExitGUI/Panel").gameObject.SetActive(true);
		}
	}
}
