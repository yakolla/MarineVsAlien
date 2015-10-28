using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class TutorialMgr : MonoBehaviour {

	Animator	m_ani;
	string		m_tutorialName = "";
	void Start()
	{
		m_ani = GetComponent<Animator>();
	}

	void Update()
	{

	}

	public void SetTutorial(string tutorial)
	{
		m_tutorialName = tutorial;
		m_ani.SetTrigger(tutorial);
	}

	public string TutorialName
	{
		get {return m_tutorialName;}
	}
}

