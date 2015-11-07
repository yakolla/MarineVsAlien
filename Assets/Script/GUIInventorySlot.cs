using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIInventorySlot : MonoBehaviour {



	public class GUIPriceGemButton
	{
		public YGUISystem.GUIPriceButton	m_priceButton;
		public YGUISystem.GUIPriceButton	m_gemButton;
		Const.ButtonRole m_role = Const.ButtonRole.Nothing;

		public GUIPriceGemButton(Transform transform, string buttonPath, System.Func<bool>			enableChecker)
		{
			m_priceButton = new YGUISystem.GUIPriceButton(transform.Find(buttonPath).gameObject, Const.StartPosYOfPriceButtonImage, enableChecker);
			m_gemButton = new YGUISystem.GUIPriceButton(transform.Find(buttonPath + "/GemButton").gameObject, Const.StartPosYOfGemPriceButtonImage, enableChecker);
		}

		public void Update(ItemObject itemObj)
		{
			switch(m_role)
			{
			case Const.ButtonRole.Nothing:
			case Const.ButtonRole.Unlock:
				break;
			case Const.ButtonRole.Levelup:
				m_priceButton.NormalWorth = Const.GetItemLevelupWorth(itemObj.Item.Level, itemObj.Item.RefItem.levelup);
				m_gemButton.NormalWorth = Const.GetItemLevelupWorth(itemObj.Item.Level, itemObj.Item.RefItem.levelup);
				break;
			case Const.ButtonRole.Evolution:
				m_priceButton.NormalWorth = Const.GetItemLevelupWorth(itemObj.Item.Evolution+1, itemObj.Item.RefItem.evolution);
				m_gemButton.NormalWorth = Const.GetItemLevelupWorth(itemObj.Item.Evolution+1, itemObj.Item.RefItem.evolution);
				break;			
			}
				


			m_priceButton.Update();
			m_gemButton.Update();
		}

		public bool CheckAvailableItem()
		{
			return m_priceButton.CheckAvailableItem();
		}

		public void RemoveAllListeners()
		{
			m_priceButton.GUIImageButton.Button.onClick.RemoveAllListeners();
			m_gemButton.GUIImageButton.Button.onClick.RemoveAllListeners();
		}

		public void AddListener(UnityEngine.Events.UnityAction callback, UnityEngine.Events.UnityAction gemCallback)
		{
			m_priceButton.GUIImageButton.Button.onClick.AddListener(callback);
			m_gemButton.GUIImageButton.Button.onClick.AddListener(gemCallback);
		}
		
		public void SetPrices(Const.ButtonRole role, RefItem refItem)
		{
			m_role = role;
			RefPrice[] normal = null;
			RefPrice[] gem = null;

			switch(role)
			{
			case Const.ButtonRole.Unlock:
				normal = refItem.unlock.conds;
				gem = refItem.unlock.else_conds;
				break;
			case Const.ButtonRole.Levelup:
				normal = refItem.levelup.conds;
				gem = refItem.levelup.else_conds;
				break;
			case Const.ButtonRole.Evolution:
				normal = refItem.evolution.conds;
				gem = refItem.evolution.else_conds;
				break;
			case Const.ButtonRole.Nothing:
				break;
			}

			m_priceButton.Prices = normal;
			m_gemButton.Prices = gem;
		}

		public void SetLable(string text)
		{
			m_priceButton.GUIImageButton.Lable.Text.text = text;
		}

		public System.Func<bool> EnableChecker
		{
			get{
				return m_priceButton.GUIImageButton.EnableChecker;
			}
			set{
				m_priceButton.GUIImageButton.EnableChecker = value;
				m_gemButton.GUIImageButton.EnableChecker = value;
			}
		} 

		public void SetActive(bool active)
		{
			m_priceButton.GUIImageButton.Button.gameObject.SetActive(active);
			m_gemButton.GUIImageButton.Button.gameObject.SetActive(active);
		}
	}

	YGUISystem.GUIButton	m_item;
	GameObject	m_checkImage;
	GUIPriceGemButton	m_priceButton0;
	ItemObject 	m_itemObjOfWarehouse;

	public void Init(GameObject tab, ItemObject itemObj)
	{
		m_itemObjOfWarehouse = itemObj;
		m_priceButton0 = new GUIPriceGemButton(transform, "GUIPriceButton0", ()=>{return true;});

		m_item = new YGUISystem.GUIButton(transform.Find("PictureButton").gameObject, m_priceButton0.EnableChecker);
		m_item.Icon.Lable.Text.text = itemObj.Item.Description();
		m_item.Icon.Image = itemObj.ItemIcon;
		m_item.Lable.Text.text = RefData.Instance.RefTexts(itemObj.Item.RefItem.desc);
		m_checkImage = tab.transform.Find("Checked").gameObject;
	}

	public GUIPriceGemButton PriceButton0
	{
		get{return m_priceButton0;}
	}

	public string ItemDesc
	{
		set{m_item.Icon.Lable.Text.text = value;}
	}

	public ItemObject ItemObj
	{
		get{return m_itemObjOfWarehouse;}
	}

	public void SetListener(UnityEngine.Events.UnityAction callback)
	{
		m_item.Button.onClick.RemoveAllListeners();
		m_item.Button.onClick.AddListener(callback);
	}

	public void Update()
	{

		ItemDesc = m_itemObjOfWarehouse.Item.Description();
		m_item.Update();
		m_priceButton0.Update(m_itemObjOfWarehouse);
	}

	public void Check(bool check)
	{
		m_checkImage.SetActive(check);
	}

}



