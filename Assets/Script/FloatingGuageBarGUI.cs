using UnityEngine;
using System.Collections;

public class FloatingGuageBarGUI : MonoBehaviour {

	protected Creature	m_creature;


	YGUISystem.GUIGuage m_guage;
	YGUISystem.GUILable	m_level;

	// Use this for initialization
	void Start () {
		m_creature = transform.parent.gameObject.GetComponent<Creature>();
		m_guage = new YGUISystem.GUIGuage(transform.Find("Canvas/HP").gameObject, 
		                                   ()=>{return guageRemainRatio();}, 
		()=>{return ""; }
		);

		Transform trans = transform.Find("Canvas/HP/Level/Text");
		if (trans != null)
		{
			m_level = new YGUISystem.GUILable(trans.gameObject);
		}


		Vector3 pos = m_creature.HPPointTransform.localPosition;
		pos.y += 1.5f;
		m_guage.RectTransform.transform.localPosition = pos;
		m_guage.RectTransform.transform.localScale = m_creature.HPPointTransform.localScale;
	}

	// Update is called once per frame
	void Update () {
		m_guage.Update();

		if (m_level != null)
		{
			m_level.Text.text = m_creature.m_creatureProperty.Level.ToString();
		}

		transform.forward = Camera.main.transform.forward;
	}

	virtual protected float guageRemainRatio()
	{
		return 1f;
	}
}

