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
	SecuredType.XInt	gemReward = 0;

	void Start () {
		m_button = new YGUISystem.GUIButton(transform.Find("Button").gameObject, ()=>{
			return true;
		});

		m_name = new YGUISystem.GUILable(transform.Find("Desc").gameObject);

		SetItemDesc();
	}


	void OnEnable() {
		TimeEffector.Instance.StopTime();
	}

	void SetItemDesc()
	{
		m_button.Lable.Text.text = RefData.Instance.RefTexts(m_itemObj.Item.RefItem.name);
		m_button.Icon.Image = m_itemObj.ItemIcon;
		m_name.Text.text = RefData.Instance.RefTexts(m_itemObj.Item.RefItem.desc);

		if (m_itemObj.Item.RefItem.id == Const.GemRefItemId)
		{
			gemReward = Random.Range(500, 1500);
			m_button.Lable.Text.text = "" + gemReward.Value;
		}
	}

	void OnDisable() {

		TimeEffector.Instance.StartTime();
		if (m_itemObj.Item.RefItem.id == Const.GemRefItemId)
			Warehouse.Instance.Gem.Item.Count += gemReward.Value;

	}

	public void SetItemObj(ItemObject itemObj)
	{
		m_itemObj = itemObj;

		if (m_button != null)
		{
			SetItemDesc();
		}
	}

	public void OnFinishAni()
	{
		gameObject.SetActive(false);
	}

}
