using UnityEngine;
using System.Collections;

public class MobBossDeathEffect : MonoBehaviour {

	Animator m_ani;
	float	m_time = 0;
	enum Type
	{
		DownToGround,
		StopAtGround,
		Rotting,
	}

	Type	m_type;

	void Start () {
		m_ani = GetComponent<Animator>();

		m_type = Type.DownToGround;
	}

	void Update()
	{
		switch(m_type)
		{
		case Type.DownToGround:
		{
			Vector3	pos = transform.position;
			pos.y -= Time.deltaTime*3;
			pos.y = Mathf.Max(pos.y, 0f);
			transform.position = pos;

			if (pos.y == 0f)
			{
				m_type = Type.StopAtGround;
				m_time = Time.time + 2f;
			}

		}break;
		case Type.StopAtGround:
		{
			if (m_time < Time.time)
			{
				m_type = Type.Rotting;
				m_time = Time.time + 8f;
			}
		}break;
		case Type.Rotting:
		{
			Vector3	pos = transform.position;
			pos.y -= Time.deltaTime*0.05f;
			transform.position = pos;

			if (m_time < Time.time)
			{
				GameObject.DestroyObject(gameObject);
			}
		}break;

		}

	}

}
