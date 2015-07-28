using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class ExitGUI : MonoBehaviour {


	void Start () {

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
