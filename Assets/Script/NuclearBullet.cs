using UnityEngine;
using System.Collections;

public class NuclearBullet : Bullet {

	[SerializeField]
	protected GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	protected float			m_bombRange = 5f;

	void OnEnable()
	{
		m_isDestroying = false;

	}

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		base.Init(ownerCreature, weapon, targetAngle);
		Vector3 pos = transform.position;
		pos.y = 0f;

		transform.position = pos;
	}

	void Update()
	{
		if (m_isDestroying == true)
			return;

		bomb(m_bombRange, m_prefBombEffect);
	}
}
