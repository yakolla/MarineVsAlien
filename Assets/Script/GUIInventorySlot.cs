using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIInventorySlot : MonoBehaviour {



	public class GUIPriceGemButton
	{
		public YGUISystem.GUIPriceButton	m_priceButton;
		public YGUISystem.GUIPriceButton	m_gemButton;

		public GUIPriceGemButton(Transform transform, string buttonPath, System.Func<bool>			enableChecker)
		{
			m_priceButton = new YGUISystem.GUIPriceButton(transform.Find(buttonPath).gameObject, Const.StartPosYOfPriceButtonImage, enableChecker);
			m_gemButton = new YGUISystem.GUIPriceButton(transform.Find(buttonPath + "/GemButton").gameObject, Const.StartPosYOfGemPriceButtonImage, enableChecker);
		}

		public void Update()
		{
			m_priceButton.Update();
			m_gemButton.Update();
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
		
		public void SetPrices(RefPrice[] normal, RefPrice[] gem)
		{
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
	GUIPriceGemButton	m_priceButton1;
	ItemObject 	m_itemObjOfWarehouse;
	public void Init(ItemObject itemObj)
	{
		m_itemObjOfWarehouse = itemObj;
		m_priceButton0 = new GUIPriceGemButton(transform, "GUIPriceButton0", ()=>{return true;});
		m_priceButton1 = new GUIPriceGemButton(transform, "GUIPriceButton1", ()=>{return true;});

		m_item = new YGUISystem.GUIButton(transform.Find("PictureButton").gameObject, m_priceButton0.EnableChecker);
		m_item.Icon.Lable.Text.text = itemObj.Item.Description();
		m_item.Icon.Image = itemObj.ItemIcon;

		m_checkImage = transform.Find("PictureButton/Check").gameObject;
	}

	public GUIPriceGemButton PriceButton0
	{
		get{return m_priceButton0;}
	}

	public string ItemDesc
	{
		set{m_item.Icon.Lable.Text.text = value;}
	}

	public GUIPriceGemButton PriceButton1
	{
		get{return m_priceButton1;}
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
		m_priceButton0.Update();
		m_priceButton1.Update();

	}

	public void Check(bool check)
	{
		m_checkImage.SetActive(check);
	}
	
	public void StopSpinButton(int slot)
	{
		m_priceButton0.m_priceButton.GUIImageButton.Button.enabled = true;
		m_priceButton0.m_priceButton.GUIImageButton.Lable.Text.enabled = true;

		m_priceButton1.m_priceButton.GUIImageButton.Button.enabled = true;
		m_priceButton1.m_priceButton.GUIImageButton.Lable.Text.enabled = true;
	}
}



