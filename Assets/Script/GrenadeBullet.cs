using UnityEngine;
using System.Collections;

public class GrenadeBullet : Bullet {

	[SerializeField]
	protected GameObject		m_prefBombEffect = null;

	[SerializeField]
	protected float			m_bombRange = 5f;

	[SerializeField]
	protected float			m_speed = 7f;

	[SerializeField]
	protected int				m_bouncing = 1;

	protected Parabola	m_parabola;
	// Use this for initialization
	void Start () {


	}

	void OnEnable()
	{
		m_isDestroying = false;
	}

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		base.Init(ownerCreature, weapon, targetAngle);
		createParabola(targetAngle.angle);
	}

	protected virtual void createParabola(float targetAngle)
	{
		m_parabola = new Parabola(gameObject, m_speed, -(transform.rotation.eulerAngles.y + targetAngle) * Mathf.Deg2Rad, Random.Range(55f,85f) * Mathf.Deg2Rad, m_bouncing);
	}

	// Update is called once per frame
	protected void Update () {
		if (m_isDestroying == true)
			return;

		if (m_parabola.Update() == false)
		{
			bomb(m_bombRange, m_prefBombEffect);
		}

	}

}
