using UnityEngine;
using System.Collections;

public class BoomerangBullet : Bullet {

	[SerializeField]
	float	m_speed = 3f;

	[SerializeField]
	float	m_lifeTime = 4f;

	[SerializeField]
	float	m_length = 10f;

	float	m_elapsed;

	Vector3	m_start;
	Vector3	m_goal;

	bool	m_returnPhase = false;
	// Use this for initialization
	void Start () {

	}

	void OnEnable()
	{
		m_start = transform.position;
		m_goal = m_start+transform.right*m_length;
		m_elapsed = 0f;
		m_returnPhase = false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp(m_start, m_goal, m_elapsed/m_lifeTime);
		m_elapsed += Time.deltaTime*m_speed;

		if (m_elapsed/m_lifeTime > 1)
		{
			if (m_returnPhase == true)
			{
				GameObjectPool.Instance.Free(gameObject);
				return;
			}
			else
			{
				m_returnPhase = true;
				m_elapsed = 0f;
				m_start = m_goal;

				if (m_ownerCreature)
					m_goal = m_ownerCreature.WeaponHolder.transform.position;
			}

		}

		if (m_returnPhase == true)
		{
			if (m_ownerCreature)
				m_goal = m_ownerCreature.WeaponHolder.transform.position;
		}
	}

	void OnTriggerEnter(Collider other) {
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
		}

	}
}
