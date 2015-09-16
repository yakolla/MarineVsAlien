using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class FoundItemGUI : MonoBehaviour {

	YGUISystem.GUIButton m_button;
	YGUISystem.GUILable	m_name;
	ItemObject	m_itemObj;

	void Start () {
		m_button = new YGUISystem.GUIButton(transform.Find("Button").gameObject, ()=>{
			return true;
		});

		m_name = new YGUISystem.GUILable(transform.Find("Text").gameObject);

		m_button.Icon.Image = m_itemObj.ItemIcon;
		m_button.Lable.Text.text = RefData.Instance.RefTexts(m_itemObj.Item.RefItem.name);
		m_name.Text.text = RefData.Instance.RefTexts(m_itemObj.Item.RefItem.desc);
	}


	void OnEnable() {
		TimeEffector.Instance.StopTime();
	}

	void OnDisable() {

		TimeEffector.Instance.StartTime();

	}

	public void SetItemObj(ItemObject itemObj)
	{
		m_itemObj = itemObj;

		if (m_button != null)
		{
			m_button.Icon.Image = m_itemObj.ItemIcon;
			m_button.Lable.Text.text = RefData.Instance.RefTexts(m_itemObj.Item.RefItem.name);
			m_name.Text.text = RefData.Instance.RefTexts(m_itemObj.Item.RefItem.desc);
		}
	}

	public void OnFinishAni()
	{
		gameObject.SetActive(false);
	}

}
