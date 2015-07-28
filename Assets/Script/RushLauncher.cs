using UnityEngine;
using System.Collections;

public class RushLauncher : Weapon {

	Vector3	m_goal;
	float	m_elapsed;
	const float DashSpeed = 9f;
	RushBullet bullet;

	enum State
	{
		Nothing,
		Ready,
		Rush,
	}

	EffectTargetingLine	m_effectTargetingPoint = null;
	State	m_state;

	new void Start()
	{
		base.Start();
		bullet = CreateBullet(m_firingDescs[0], transform.position) as RushBullet;
	}


	override public void StartFiring(float targetAngle)
	{
		if ( isCoolTime() == true )
		{
			m_goal = m_creature.transform.position+m_creature.transform.right.normalized*DashSpeed;
			m_goal.y = m_creature.transform.position.y;
			m_elapsed = 0f;
			m_state = State.Ready;

			m_effectTargetingPoint = new EffectTargetingLine();
			m_effectTargetingPoint.Init(m_creature.transform.position, m_goal);

			DidStartFiring(0f);

		}

		bullet.gameObject.SetActive(true);
		m_firing = true;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		bullet.gameObject.SetActive(false);
	}
	
	void Update()
	{
		if (m_state == State.Nothing)
			return;

		m_elapsed += Time.deltaTime;

		switch(m_state)
		{
		case State.Ready:
			if (m_elapsed >= 1f)
			{
				m_elapsed = 0f;
				m_state = State.Rush;
			}
			break;
		case State.Rush:
			m_creature.transform.transform.position = Vector3.Lerp(m_creature.transform.transform.position, m_goal, m_elapsed*0.05f);
			if (m_elapsed >= 1f)
			{
				m_state = State.Nothing;
				m_effectTargetingPoint.Death();
			}
			break;
		}


	}
}
