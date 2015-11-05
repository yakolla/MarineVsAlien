/*******************************************************************************
 * Copyright 2012-2014 One Platform Foundation
 *
 *       Licensed under the Apache License, Version 2.0 (the "License");
 *       you may not use this file except in compliance with the License.
 *       You may obtain a copy of the License at
 *
 *           http://www.apache.org/licenses/LICENSE-2.0
 *
 *       Unless required by applicable law or agreed to in writing, software
 *       distributed under the License is distributed on an "AS IS" BASIS,
 *       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *       See the License for the specific language governing permissions and
 *       limitations under the License.
 ******************************************************************************/

using UnityEngine;
using OnePF;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

/**
 * Example of OpenIAB usage
 */ 
public class ShopIAB : MonoBehaviour
{
	class PaidItem
	{
		SecuredType.XInt		m_gem;

		public PaidItem(int gem)
		{
			m_gem = gem;
		}

		public int Gem
		{
			get{return m_gem.Value;}
		}
	}

	YGUISystem.GUIButton[]	m_piadItemButtons = new YGUISystem.GUIButton[3];
	YGUISystem.GUIButton	m_closeButton;
	Dictionary<string, PaidItem>	m_paidItems = new Dictionary<string, PaidItem>();
	YGUISystem.GUILable	m_needGems;

    string _label = "";
    bool _isInitialized = false;
	bool m_progressing = false;
	int	m_needTotalGems = 0;
	int	m_tryToSaveCount = 0;

	private void Awake()
	{

		// Listen to all events for illustration purposes
		OpenIABEventManager.billingSupportedEvent += billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent += purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;

	}

	private void OnDestroy()
	{
		// Listen to all events for illustration purposes
		OpenIABEventManager.billingSupportedEvent -= billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}

    private void Start()
    {
		m_needGems = new YGUISystem.GUILable(transform.Find("NeedGems/Text").gameObject);

        Init();
    }

	void OnEnable() {
		TimeEffector.Instance.StopTime();
	}
	
	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	void Init()
	{
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Shop", "OpenShop", 0);
		m_paidItems.Add("gem.1000", new PaidItem(100000));
		m_paidItems.Add("gem.3000", new PaidItem(350000));
		m_paidItems.Add("gem.5000", new PaidItem(600000));

		m_closeButton = new YGUISystem.GUIButton(transform.Find("CloseButton").gameObject, ()=>{return true;});
		m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.CheckingYourInventory);

		m_piadItemButtons[0] = new YGUISystem.GUIButton(transform.Find("PaidItemButton0").gameObject, ()=>{
			return _isInitialized && m_progressing == false;
		});
		m_piadItemButtons[0].Lable.Text.text = m_paidItems["gem.1000"].Gem.ToString();

		m_piadItemButtons[1] = new YGUISystem.GUIButton(transform.Find("PaidItemButton1").gameObject, ()=>{
			return _isInitialized && m_progressing == false;
		});
		m_piadItemButtons[1].Lable.Text.text = m_paidItems["gem.3000"].Gem.ToString();

		m_piadItemButtons[2] = new YGUISystem.GUIButton(transform.Find("PaidItemButton2").gameObject, ()=>{
			return _isInitialized && m_progressing == false;
		});
		m_piadItemButtons[2].Lable.Text.text = m_paidItems["gem.5000"].Gem.ToString();

		foreach(KeyValuePair<string, PaidItem> pair in m_paidItems)
		{
			OpenIAB.mapSku(pair.Key, OpenIAB_Android.STORE_GOOGLE, pair.Key);
		}
							  
		var googlePublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyJoNHt3k2PMI0zVAgcQK6QARbGDJQ2KtuD+0JJuVqlaWqi5dv5Gn7zyhaTlwb9w7S65E9ZmnJDgmwhDIROSBDNFqcYxL9dYp8PDj7zhxyVz5fmgTZKMvXp9a2qo84mQ0RTlplhK32GMGCrlzGdh1cYu4Z07da6mEzBjTyGKss/E44QJNVfI982TznB7qXfIgMWp6ZAzLZOsc7CmzKd1qTyKYWu6iZl3BB3bHKRHK6vvHTyxHaf48mjwRAFrkaYESsq5oAm+hKTwR0vrN374mmGG/E5B8ZGI5xBNkmCV9bG2wc6Uqb++uf//T1hM5qcs8LvBlr2iOSGHucWo8mb+o6QIDAQAB";

		var options = new Options();
		options.checkInventoryTimeoutMs = Options.INVENTORY_CHECK_TIMEOUT_MS * 2;
		options.discoveryTimeoutMs = Options.DISCOVER_TIMEOUT_MS * 2;
		options.checkInventory = true;
		options.verifyMode = OptionsVerifyMode.VERIFY_ONLY_KNOWN;
		options.prefferedStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE };
		options.availableStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE };
		options.storeKeys = new Dictionary<string, string> { {OpenIAB_Android.STORE_GOOGLE, googlePublicKey} };
		options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;


		m_progressing = true;
		// Transmit options and start the service
		OpenIAB.init(options);
	}

	public void OnClickClose()
	{
		m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Close);
		gameObject.SetActive(false);
	}

	public void OnClickPurchase(string sku)
	{
		if (_isInitialized == false)
			return;

		if (m_needTotalGems - m_paidItems[sku].Gem <= -m_paidItems["gem.1000"].Gem)
		{
			m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.PlzBuyasYouNeed);
			return;
		}

		m_tryToSaveCount = 0;
		m_progressing = true;
		m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.ItsPurchasing);
		OpenIAB.purchaseProduct(sku, "ok marine");
	}

	void Update()
	{
		if (m_progressing == true)
			Const.ShowLoadingGUI(m_closeButton.Lable.Text.text);
		else
			Const.HideLoadingGUI();

		for(int i = 0; i < m_piadItemButtons.Length; ++i)
			m_piadItemButtons[i].Update();

		m_needTotalGems = Warehouse.Instance.NeedTotalGem;
		m_needGems.Text.text = m_needTotalGems.ToString();
	}

    private void billingSupportedEvent()
    {
		m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.CheckingYourInventory);
#if UNITY_EDITOR
		queryInventorySucceededEvent(null);
#else

		OpenIAB.queryInventory();
#endif
        
    }
    
	private void billingNotSupportedEvent(string error)
    {
		m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Error) + ":" + error;
		m_progressing = false;
    }

    private void queryInventorySucceededEvent(Inventory inventory)
    {
		_isInitialized = true;

		if (inventory != null)
        {           
			m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.CheckingYourInventory);
			foreach(Purchase purchase in inventory.GetAllPurchases())
			{
				OpenIAB.consumeProduct(purchase);
			}

			if (inventory.GetAllPurchases().Count > 0)
				return;
        }

		m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Close);
		m_progressing = false;
    }
    
	private void queryInventoryFailedEvent(string error)
    {
		m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Error) + ":" + error;
		m_progressing = false;
    }
    
	private void purchaseSucceededEvent(Purchase purchase)
    {
		_label = "PURCHASED:" + purchase.ToString();

		OpenIAB.consumeProduct(purchase);
    }
	private void purchaseFailedEvent(int errorCode, string error)
    {
		m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Error) + ":" + error;
		
		m_progressing = false;
    }
    private void consumePurchaseSucceededEvent(Purchase purchase)
    {
		_label = "CONSUMED: " + purchase.ToString();

		Const.SaveGame((SavedGameRequestStatus status)=>{

			OnSaveGame(status,  purchase);

		});
    }

    private void consumePurchaseFailedEvent(string error)
    {
		m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.Error) + ":" + error;
		m_progressing = false;
    }

	void OnSaveGame(SavedGameRequestStatus status, Purchase purchase)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			m_closeButton.Lable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.ThanksForYourPurchase) + m_paidItems[purchase.Sku].Gem;
			
			PaidItem paidItem = null;
			if (true == m_paidItems.TryGetValue(purchase.Sku, out paidItem))
			{
				Warehouse.Instance.Gem.Item.Count += paidItem.Gem;
			}
			
			m_progressing = false;
			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Shop", "Purchase:" + purchase.Sku, 0);
		}
		else
		{
			if (m_tryToSaveCount < 5)
			{
				++m_tryToSaveCount;
				m_closeButton.Lable.Text.text = "Sorry, " + "Try to save " + m_tryToSaveCount;
				consumePurchaseSucceededEvent(purchase);
			}
		}
	}
}