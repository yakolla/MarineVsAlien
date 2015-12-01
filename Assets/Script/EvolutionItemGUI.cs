using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class EvolutionItemGUI : MonoBehaviour {

	YGUISystem.GUIButton m_button;
	YGUISystem.GUILable	m_oldEvolution;
	YGUISystem.GUILable	m_newEvolution;
	YGUISystem.GUILable	m_oldDamage;
	YGUISystem.GUILable	m_newDamage;
	Follower	m_follower;

	GameObject	m_oldFollower;
	GameObject	m_newFollower;

	YGUISystem.GUILable	m_evolutionLable;	
	YGUISystem.GUILable	m_damageLable;

	SecuredType.XInt	gemReward = 0;

	void Start () {
		m_button = new YGUISystem.GUIButton(transform.Find("Button").gameObject, ()=>{
			return true;
		});

		m_oldEvolution = new YGUISystem.GUILable(transform.Find("Button/Old/EvolutionText").gameObject);
		m_newEvolution = new YGUISystem.GUILable(transform.Find("Button/New/EvolutionText").gameObject);

		m_oldDamage = new YGUISystem.GUILable(transform.Find("Button/Old/DamageText").gameObject);
		m_newDamage = new YGUISystem.GUILable(transform.Find("Button/New/DamageText").gameObject);

		m_evolutionLable = new YGUISystem.GUILable(transform.Find("Button/EvolutionLable").gameObject);
		m_damageLable = new YGUISystem.GUILable(transform.Find("Button/DamageLable").gameObject);

		m_evolutionLable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.EvolutionLevel);
		m_damageLable.Text.text = RefData.Instance.RefTexts(MultiLang.ID.BaseDamage);
	}


	void OnEnable() {
		TimeEffector.Instance.StopTime();
	}


	void OnDisable() {

		TimeEffector.Instance.StartTime();
		m_follower = null;
		GameObject.Destroy(m_oldFollower.transform.Find("Body").gameObject);
		GameObject.Destroy(m_newFollower.transform.Find("Body").gameObject);

		m_oldFollower = null;
		m_newFollower = null;
	}

	public void Update()
	{
		if (m_follower != null)
		{
			Camera.main.gameObject.transform.position = Vector3.zero;

			CreatureProperty	backup = new CreatureProperty();

			int oldDmg = m_follower.WeaponHolder.MainWeapon.GetDamage(m_follower.m_creatureProperty, m_follower.WeaponHolder.MainWeapon.Evolution-1);
			int newDmg = m_follower.WeaponHolder.MainWeapon.Damage;

			m_oldEvolution.Text.text = (m_follower.WeaponHolder.MainWeapon.Evolution-1).ToString();
			m_newEvolution.Text.text = m_follower.WeaponHolder.MainWeapon.Evolution.ToString();
			m_oldDamage.Text.text = oldDmg.ToString();
			m_newDamage.Text.text = newDmg.ToString();

			return;
		}


	}

	public void SetItem(Follower follower)
	{

		m_follower = follower;
		m_oldFollower = Creature.InstanceCreature(Const.GetSpawn().transform.Find("OldEvolution").gameObject, Resources.Load<GameObject>("Pref/mon_skin/" + m_follower.RefMob.prefBody), Vector3.zero, Quaternion.Euler(Vector3.zero));
		m_newFollower = Creature.InstanceCreature(Const.GetSpawn().transform.Find("NewEvolution").gameObject, Resources.Load<GameObject>("Pref/mon_skin/" + m_follower.RefMob.prefBody), Vector3.zero, Quaternion.Euler(Vector3.zero));

		Follower.Transformation(m_oldFollower.transform, m_follower.WeaponHolder.MainWeapon.Evolution-1);
		Follower.Transformation(m_newFollower.transform, m_follower.WeaponHolder.MainWeapon.Evolution);
	}

	public void OnFinishAni()
	{
		gameObject.SetActive(false);
	}

}
