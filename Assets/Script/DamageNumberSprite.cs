using UnityEngine;
using System.Collections;

public class DamageNumberSprite : MonoBehaviour {

	public enum MovementType
	{
		Parabola,
		RisingUp,
		FloatingUp,
		FloatingUpAlways,
	}

	[SerializeField]
	string	m_damageNumber;

	MovementType	m_movementType = MovementType.Parabola;

	Parabola	m_parabola;

	Creature	m_target;
	Vector3		m_targetPos;

	[SerializeField]
	float		m_duration = 1.5f;
	float		m_startTime = 0f;

	float		m_posY = 0f;
	Vector3		m_finishScale = Vector3.one*2f;

	TypogenicText	m_text;
	// Use this for initialization
	void Start () {

	}
	
	public void Init(Creature obj, string damage, Color color, MovementType movementType)
	{
		m_movementType = movementType;
		m_target = obj;
		m_targetPos = obj.transform.position+m_target.AimpointLocalPos;
		m_targetPos.y += 1;
		transform.position = m_targetPos;
		m_startTime = Time.time;
		m_posY = 0f;

		m_text = GetComponent<TypogenicText>();
		m_text.Text = damage;
		m_text.ColorTopLeft = color;
		//transform.localScale = Vector3.one/2f;


		if (movementType == MovementType.Parabola)
		{
			m_parabola = new Parabola(gameObject, 5f, 0f, 90*Mathf.Deg2Rad, 1);
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {

		switch(m_movementType)
		{
		case MovementType.Parabola:

			if (false == m_parabola.Update())
			{
				DestroyObject();
			}
			else
			{
				//transform.localScale = Vector3.Lerp(transform.localScale, m_finishScale, Time.deltaTime);
			}
			break;
		case MovementType.RisingUp:
			if (m_target)
			{
				m_targetPos = m_target.transform.position+m_target.AimpointLocalPos;
			}
			m_posY += 2f*Time.deltaTime;

			m_targetPos.y += m_posY;
			transform.position = m_targetPos;

			if (m_startTime+m_duration < Time.time)
			{
				DestroyObject();
			}
			break;
		case MovementType.FloatingUp:
			if (m_target)
			{
				m_targetPos = m_target.transform.position+m_target.AimpointLocalPos;
				m_targetPos.y += 3f;
			}

			transform.position = m_targetPos;

			if (m_startTime+m_duration < Time.time)
			{
				DestroyObject();
			}

			break;
		case MovementType.FloatingUpAlways:
			if (m_target)
			{
				m_targetPos = m_target.transform.position+m_target.AimpointLocalPos;
				m_targetPos.y += 3f;
			}
			
			transform.position = m_targetPos;
			
			if (m_target == null)
			{
				DestroyObject();
			}
			
			break;
		}
	}

	public string Text
	{
		set {m_text.Text = value;}
	}

	public void DestroyObject()
	{
		GameObjectPool.Instance.Free(gameObject);
	}
}

