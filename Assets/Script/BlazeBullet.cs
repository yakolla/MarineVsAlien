using UnityEngine;
using System.Collections;

public class BlazeBullet : FireSparkBullet {


	Parabola	m_parabola;

	[SerializeField]
	float			m_speed = 7f;
	
	[SerializeField]
	int				m_bouncing = 1;

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		base.Init(ownerCreature, weapon, targetAngle);

		m_status = Status.Dropping;
		//m_parabola = new Parabola(gameObject, m_speed, -targetAngle.angle * Mathf.Deg2Rad, Random.Range(45f, 85f) * Mathf.Deg2Rad, m_bouncing);
		m_parabola = new Parabola(gameObject, m_speed, -ownerCreature.transform.rotation.eulerAngles.y * Mathf.Deg2Rad, Random.Range(45f, 85f) * Mathf.Deg2Rad, m_bouncing);
	}

	// Update is called once per frame
	new void Update () {

		base.Update();

		if (m_status == Status.Dropping)
		{
			if (m_parabola.Update() == false)
			{
				m_status = Status.Dropped;
			}
		}

	}

}
