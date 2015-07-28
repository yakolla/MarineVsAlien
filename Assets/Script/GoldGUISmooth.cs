using UnityEngine;
using System.Collections;

public class GoldGUISmooth : MonoBehaviour
{
	int				m_gold = 0;
	int				m_targetGold = 0;
	TypogenicText	m_killComboGUI;



	float			m_time;

	void Start()
	{
		m_killComboGUI = GetComponent<TypogenicText>();
	}
	
	void Update()
	{
		if (m_targetGold != m_gold)
		{
			m_time = Mathf.Min(1f, m_time + Time.deltaTime);
			m_gold = (int)(m_gold * (1-m_time) + m_targetGold * m_time);
			m_killComboGUI.Text = "" + m_gold;
		}

	}

	public int Gold
	{
		set{
			if (m_targetGold != value)
			{
				m_targetGold = value;
				m_time = 0;
			}
		}
	}
}