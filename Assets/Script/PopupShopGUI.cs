using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PopupShopGUI : MonoBehaviour {
	

	Rect 		m_statusWindowRect;
	Rect 		m_skillWindowRect;
	
	[SerializeField]
	GUISkin		m_guiSkin = null;
	
	float 		m_slotWidth = Screen.width * (1/5f);
	float 		m_slotHeight = Screen.height * (1/8f);
		
	void OnEnable() {
		TimeEffector.Instance.StopTime();
		
		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);		

	}
	
	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}
	
	void OnGUI()
	{
		GUI.skin = m_guiSkin;
		
		m_statusWindowRect = GUI.Window ((int)Const.GUI_WindowID.PopupShop, m_statusWindowRect, DisplayStatusWindow, "");	
	}


	
	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_slotHeight;
		
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
	}

}
