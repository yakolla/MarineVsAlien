using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class SlotMachineGUI : MonoBehaviour {


	YGUISystem.GUIButton[]	m_statButtons = new YGUISystem.GUIButton[Const.AbilitySlots];
	YGUISystem.GUILable		m_remainPointText;
	YGUISystem.GUIPriceButton	m_rollButton;
	YGUISystem.GUIImageStatic	m_gold;
	YGUISystem.GUIImageStatic	m_goldMedal;
	YGUISystem.GUIImageStatic	m_gem;

	GameObject			m_rewardPanel;
	YGUISystem.GUIButton	m_rewardButton;

	int			m_usedCountOfRandomAbilityItem = 0;

	ADMob		m_adMob;

	enum SlotType
	{
		Gold,
		Gem,
		GoldMedal,
		Count
	}

	class Ability
	{
		public float		m_chance;
		public SlotType		m_name;
		public SecuredType.XInt			m_count;


		public Ability(float chance, SlotType name, int count)
		{
			m_chance = chance;
			m_name = name;
			m_count = count;

		}
	}

	Texture[]	m_icons = new Texture[(int)SlotType.Count];

	List<Ability>	m_abilities = new List<Ability>();
	Ability[]	m_abilitySlots = new Ability[Const.AbilitySlots];

	bool		m_rollable = true;
	bool		m_savable = false;

	void Start ()
	{
		m_rewardPanel = transform.Find("RewardPanel").gameObject;
		m_rewardPanel.SetActive(false);

		m_rewardButton = new YGUISystem.GUIButton(m_rewardPanel.transform.Find("OKButton").gameObject, ()=>{return true;});

		m_gold = new YGUISystem.GUIImageStatic(transform.Find("GoldImage").gameObject, Warehouse.Instance.Gold.ItemIcon);
		m_goldMedal = new YGUISystem.GUIImageStatic(transform.Find("GoldMedalImage").gameObject, Warehouse.Instance.GoldMedal.ItemIcon);
		m_gem = new YGUISystem.GUIImageStatic(transform.Find("GemImage").gameObject, Warehouse.Instance.Gem.ItemIcon);


		m_icons[(int)SlotType.Gold] = Resources.Load("Sprites/Gold") as Texture;
		m_icons[(int)SlotType.Gem] = Resources.Load("Sprites/Gem") as Texture;
		m_icons[(int)SlotType.GoldMedal] = Resources.Load("Sprites/GoldMedal") as Texture;

		m_abilities.Add(new Ability(0.05f, SlotType.Gold, 1000));
		m_abilities.Add(new Ability(0.1f, SlotType.Gold, 500));
		m_abilities.Add(new Ability(0.3f, SlotType.Gold, 100));
		m_abilities.Add(new Ability(0.01f, SlotType.Gem, 10));
		m_abilities.Add(new Ability(0.1f, SlotType.Gem, 5));
		m_abilities.Add(new Ability(0.3f, SlotType.Gem, 1));
		m_abilities.Add(new Ability(0.2f, SlotType.GoldMedal, 10));
		m_abilities.Add(new Ability(0.3f, SlotType.GoldMedal, 5));
		m_abilities.Add(new Ability(0.5f, SlotType.GoldMedal, 1));



		for(int i = 0; i < m_statButtons.Length; ++i)
			m_statButtons[i] = new YGUISystem.GUIButton(transform.Find("StatButton"+i).gameObject, ()=>{return true;});

		m_rollButton = new YGUISystem.GUIPriceButton(transform.Find("RollingButton").gameObject, Const.StartPosYOfPriceButtonImage, ()=>{
			return m_rollable && m_rewardPanel.activeSelf == false;
		});
		m_rollButton.Prices = RefData.Instance.RefItems[Const.SlotMachineRollRefItemId].levelup.conds;
		
		transform.Find("RollingButton").gameObject.SetActive(true);

		RandomAbility(false);
	}

	public void StartSpinButton(YGUISystem.GUIButton button)
	{
		button.Button.enabled = false;
		button.Lable.Text.enabled = false;
		button.Button.animator.SetBool("Spin", true);
		button.Button.audio.Play();

		m_rollable = false;
	}

	public void StopSpinButton(int slot)
	{
		m_statButtons[slot].Button.enabled = true;
		m_statButtons[slot].Lable.Text.enabled = true;

		if (slot == 2)
		{
			bool equal = true;
			for(int i = 1; i < m_abilitySlots.Length; ++i)
			{
				if (m_abilitySlots[0] != m_abilitySlots[i])
				{
					equal = false;
					break;
				}
			}
			
			if (equal == true)
			{
				m_rewardButton.Icon.Image = m_icons[(int)m_abilitySlots[0].m_name];
				m_rewardButton.Icon.Lable.Text.text = m_abilitySlots[0].m_count.Value.ToString();
				m_rewardPanel.SetActive(true);
			}

			if (m_savable == false)
			{
				m_savable = true;
				Const.SaveGame((SavedGameRequestStatus status, ISavedGameMetadata game) => {
					if (status == SavedGameRequestStatus.Success) {
						// handle reading or writing of saved game.
					} else {
						// handle error
					}

					m_savable = false;
				});
			}
			
			m_rollable = true;
		}
	}

	public void OnClickRewardOk()
	{
		switch(m_abilitySlots[0].m_name)
		{
		case SlotType.Gold:
			Warehouse.Instance.Gold.Item.Count += m_abilitySlots[0].m_count.Value;
			break;
		case SlotType.Gem:
			Warehouse.Instance.Gem.Item.Count += m_abilitySlots[0].m_count.Value;
			break;
		case SlotType.GoldMedal:
			Warehouse.Instance.GoldMedal.Item.Count += m_abilitySlots[0].m_count.Value;
			break;
		}
		

		m_rewardPanel.SetActive(false);
	}
	
	void RandomAbility(bool ani)
	{
		int selectCount = 0;
		while(selectCount < Const.AbilitySlots)
		{
			List<Ability> abilis = m_abilities;
			Ability ability = abilis[Random.Range(0, abilis.Count)];

			while(selectCount < Const.AbilitySlots)
			{
				float ratio = Random.Range(0f, 1f);
				if (ratio < ability.m_chance)
				{
					m_abilitySlots[selectCount] = ability;
					m_statButtons[selectCount].Icon.Image = m_icons[(int)ability.m_name];
					++selectCount;
					continue;
				}

				break;
			}

		}

		if (ani == true)
		{
			for(selectCount = 0; selectCount < Const.AbilitySlots; ++selectCount)
				StartSpinButton(m_statButtons[selectCount]);
		}

	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();


		GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>().ShowBanner(true);

	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	public void OnClickOK()
	{
		if (m_rewardPanel.activeSelf == true)
			return;

		for(int i = 0; i < m_statButtons.Length; ++i)
		{
			if (m_statButtons[i].Button.GetComponent<SpinButtonGUI>().IsSpining())
				return;
		}

		gameObject.SetActive(false);
		GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>().ShowBanner(false);


	}

	public void OnClickRoll()
	{
		if (true == m_rollButton.TryToPay())
		{
			RandomAbility(true);



			++m_usedCountOfRandomAbilityItem;

			m_rollButton.NormalWorth = 1f;

			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "SlotMachine", "SlotMachine"+m_usedCountOfRandomAbilityItem, 0);
		}
	}


	void Update()
	{
		int statSlot = 0;
		foreach(YGUISystem.GUIButton button in m_statButtons)
		{
			button.Lable.Text.text = m_abilitySlots[statSlot].m_count.Value.ToString();
			++statSlot;
		}


		m_gold.Lable.Text.text = Warehouse.Instance.Gold.Item.Count.ToString();
		m_goldMedal.Lable.Text.text = Warehouse.Instance.GoldMedal.Item.Count.ToString();
		m_gem.Lable.Text.text = Warehouse.Instance.Gem.Item.Count.ToString();

		m_rollButton.Update();
	}

}
