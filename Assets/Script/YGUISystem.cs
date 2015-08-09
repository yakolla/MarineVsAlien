using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class YGUISystem {

	public class GUIButton
	{
		protected Button			m_button;
		protected GUIImageStatic 	m_icon;
		protected GUILable			m_lable;
		protected System.Func<bool>	m_enableChecker;

		public GUIButton(GameObject obj, System.Func<bool> enableChecker)
		{
			m_button = obj.GetComponent<Button>();
			m_lable = new GUILable(obj.transform.Find("Text").gameObject);
			m_enableChecker = enableChecker;

			Transform iconTrans = obj.transform.Find("Icon");
			if (iconTrans != null)
			{
				RawImage rawImage = iconTrans.GetComponent<RawImage>();
				m_icon = new GUIImageStatic(iconTrans.gameObject, rawImage.texture);
			}
		}

		public System.Func<bool> EnableChecker
		{
			get{return m_enableChecker;}
			set{m_enableChecker = value;}
		}

		public Button Button
		{
			get{return m_button;}
		}

		public GUIImageStatic	Icon
		{
			get{return m_icon;}
		}
		
		virtual public void Update()
		{
			m_button.interactable = m_enableChecker();

			//m_button.image.color = color;
		}

		public GUILable Lable
		{
			get{return m_lable;}
		}
	}

	public class GUILockButton : GUIButton
	{
		RawImage m_lockIcon;
		bool m_lock;
		public GUILockButton(GameObject obj, System.Func<bool> enableChecker)
			: base(obj, enableChecker)
		{
			m_lockIcon = obj.transform.Find("LockIcon").GetComponent<RawImage>();
		}

		public bool Lock
		{
			set{
				m_lock = value;
				m_lockIcon.gameObject.SetActive(m_lock);
			}
			get{return m_lock;}
		}
	}

	public class GUICoolDown
	{
		float				m_startCoolDownTime;
		float				m_coolDownTime;
		Image				m_image;
		Text				m_text;
		bool				m_doing;
		System.Action		m_callback;

		public GUICoolDown(GameObject obj, System.Action		callback)
		{
			m_image = obj.GetComponent<Image>();
			m_text = obj.transform.Find("Text").GetComponent<Text>();
			m_callback = callback;

		}

		public void StartCoolDownTime(float coolDownTime)
		{
			if (m_doing == true)
				return;

			m_doing = true;
			m_startCoolDownTime = Time.time;
			m_coolDownTime = coolDownTime;
			m_image.fillAmount = 0f;
			m_text.text = coolDownTime.ToString();
		}

		void Done()
		{
			m_startCoolDownTime = 0;
			m_text.text = "";
			m_image.fillAmount = 0f;
			m_doing = false;

			m_callback();
		}

		public void Update()
		{
			if (m_startCoolDownTime == 0f)
				return;

			float elapsedRatio = (Time.time-m_startCoolDownTime)/m_coolDownTime;
			m_image.fillAmount = elapsedRatio;
			int remainTime = (int)(m_coolDownTime - (Time.time-m_startCoolDownTime));
			m_text.text = remainTime.ToString();
			if (elapsedRatio >= 1f)
			{
				Done();
			}
		}
	}

	public class GUIChargeButton : GUIButton
	{
		int m_charge;
		int m_maxCharge;
		float m_coolDownTime;

		GUICoolDown m_guiCoolDown;
		Text	m_chargingText;
		System.Action m_do;
		public GUIChargeButton(GameObject obj, System.Func<bool> enableChecker)
			: base(obj, ()=>{return m_charge > 0 && enableChecker();})
		{
			m_guiCoolDown = new GUICoolDown(obj.transform.Find("Cooldown").gameObject, ()=>{
				++ChargingPoint;
			});

			m_chargingText = obj.transform.Find("Cooldown/ChargingPoint").gameObject.GetComponent<Text>();

		}

		public System.Action DoFunctor
		{
			set{m_do = value;}
			get{return m_do;}
		}

		override public void Update()
		{
			m_guiCoolDown.Update();
			base.Update();
		}

		public int ChargingPoint
		{
			get {return m_charge;}
			set {

				m_charge = Mathf.Min(value, m_maxCharge);
				m_chargingText.text = m_charge.ToString();

				if (m_charge < m_maxCharge)
					m_guiCoolDown.StartCoolDownTime(m_coolDownTime);
			}
		}

		public int MaxChargingPoint
		{
			set {m_maxCharge = value;}
		}

		public float CoolDownTime
		{
			set {m_coolDownTime = value;}
		}
	}

	public class GUIOverlappedImageButton : GUIButton
	{
		List<GUIImageDynamic>	m_images = new List<GUIImageDynamic>();
		int						m_startImagePosY;

		public GUIOverlappedImageButton(GameObject obj, int startImagePosY, System.Func<bool> enableChecker)
			: base(obj, enableChecker)
		{
			m_startImagePosY = startImagePosY;
		}

		public void AddGUIImage(Texture icon)
		{
			m_images.Add(new GUIImageDynamic(m_button.gameObject, icon));

			Vector3 pos = Vector3.zero;
			const int gapX = 40;
			int startX = -gapX*(m_images.Count-1);
			foreach(GUIImageDynamic image in m_images)
			{
				pos.Set(startX, -m_startImagePosY, 0);
				image.Position = pos;
				startX+=gapX*2;
			}
		}

		public void RemoveAllGUIImage()
		{
			for(int i = 0; i < m_images.Count; ++i)
			{
				m_images[i].Destory();
			}
			m_images.Clear();
		}

		public List<GUIImageDynamic> GUIImages
		{
			get{return m_images;}
		}

		override public void Update()
		{
			base.Update();

			float alpha = Button.colors.normalColor.a;
			if (Button.interactable == false)
			{
				alpha = Button.colors.disabledColor.a;
			}

			Lable.Text.gameObject.SetActive(Button.interactable);

			foreach(GUIImageDynamic image in m_images)
			{
				image.Lable.Text.gameObject.SetActive(Button.interactable);

				Color color = image.Icon.color;
				color.a = alpha;
				image.Icon.color = color;

			}
		}
	}

	public class GUIPriceButton
	{
		GUIOverlappedImageButton	m_button;
		RefPrice[]		m_prices;
		float			m_normalWorth = 1f;

		public GUIPriceButton(GameObject obj, int startImagePosY, System.Func<bool> enableChecker)
		{
			m_button = new GUIOverlappedImageButton(obj, startImagePosY, enableChecker);
		}

		public RefPrice[] Prices
		{
			set{
				m_button.RemoveAllGUIImage();
				m_prices = value;

				if (m_prices == null)
				{
					return;
				}

				foreach(RefPrice price in m_prices)
				{
					RefItem condRefItem = RefData.Instance.RefItems[price.refItemId];
					
					m_button.AddGUIImage(Resources.Load<Texture>(condRefItem.icon));				
				}

			}
		}

		void displayPrice(GUIOverlappedImageButton button, RefPrice[] prices, float itemWorth)
		{
			if (prices == null)
				return;

			int priceIndex = 0;
			foreach(RefPrice price in prices)
			{
				RefItem condRefItem = RefData.Instance.RefItems[price.refItemId];
				
				string str = "<color=white>";
				int cost = (int)(price.count*itemWorth);
				
				ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId);
				int hasCount = 0;
				if (inventoryItemObj == null)
				{
					str = "<color=red>";
				}
				else if (inventoryItemObj != null)
				{
					if (inventoryItemObj.Item.Count < cost)
					{
						str = "<color=red>";
					}
					hasCount = inventoryItemObj.Item.Count;
				}
				str += hasCount;
				str += "/" + cost;
				str += "</color>";
				button.GUIImages[priceIndex].Lable.Text.text = str;
				
				++priceIndex;
			}
		}

		public GUIOverlappedImageButton GUIImageButton
		{
			get{return m_button;}
		}

		public float NormalWorth
		{
			set{m_normalWorth = value;}
		}

		public void Update()
		{
			m_button.Update();
			displayPrice(m_button, m_prices, m_normalWorth);
		}

		public bool TryToPay()
		{
			if (Const.CheckAvailableItem(m_prices, m_normalWorth))
			{
				Const.PayPriceItem(m_prices, m_normalWorth);
				return true;
			}

			return false;
		}
	}
	
	public class GUIGuage
	{
		Image	m_guage;
		GUILable	m_text;
		System.Func<float>	m_fillAmountGetter;
		System.Func<string>	m_lableGetter;

		public RectTransform RectTransform
		{
			get {return m_guage.rectTransform;}
		}
		
		public GUIGuage(GameObject obj, System.Func<float>	fillAmountGetter, System.Func<string> lableGetter)
		{
			m_guage = obj.GetComponent<Image>();
			m_text = new GUILable(obj.transform.Find("Text").gameObject);
			m_fillAmountGetter = fillAmountGetter;
			m_lableGetter = lableGetter;
		}
		
		public void Update()
		{
			m_guage.fillAmount = m_fillAmountGetter();
			m_text.Text.text = m_lableGetter();
		}
	}

	public class GUILable
	{
		Text	m_text;

		public GUILable(GameObject obj)
		{
			m_text = obj.GetComponent<Text>();
		}

		public Text Text
		{
			get{return m_text;}
		}
	}

	public class GUIImageStatic
	{
		RawImage	m_image;
		GUILable		m_lable;
		
		public GUIImageStatic(GameObject obj, Texture icon)
		{
			m_image = obj.GetComponent<RawImage>();
			m_image.texture = icon;
			
			m_lable = new GUILable(obj.transform.Find("Text").gameObject);
		}
		
		public GUILable Lable
		{
			get{return m_lable;}
		}

		public Texture Image
		{
			set{m_image.texture = value;}
		}

		public RawImage RawImage
		{
			get{return m_image;}
		}
		
		public Vector3 Position
		{
			set{m_image.gameObject.transform.localPosition = value;}
		}
	}

	public class GUIImageDynamic
	{
		RawImage	m_image;
		GUILable		m_lable;

		public GUIImageDynamic(GameObject parent, Texture icon)
		{
			GameObject guiImageObj = GameObject.Instantiate(Resources.Load<GameObject>("Pref/GUIImage")) as GameObject;
			m_image = guiImageObj.GetComponent<RawImage>();
			m_image.texture = icon;

			m_lable = new GUILable(m_image.transform.Find("Text").gameObject);

			guiImageObj.transform.parent = parent.transform;
			guiImageObj.transform.localPosition = Vector3.zero;
			guiImageObj.transform.localScale = Vector3.one;
		}
		
		public void Destory()
		{
			GameObject.DestroyObject(m_image.gameObject);
		}
		
		public GUILable Lable
		{
			get{return m_lable;}
		}

		public Vector3 Position
		{
			set{m_image.gameObject.transform.localPosition = value;}
		}

		public RawImage Icon
		{
			get{return m_image;}
		}
	}
}
