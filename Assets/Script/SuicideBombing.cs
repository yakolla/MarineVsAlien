using UnityEngine;
using System.Collections;

public class SuicideBombing : Weapon {

	bool m_destroy = false;

	float m_destroyTime = 15f;
	float m_elapsed = 0f;

	override public void Init(Creature creature, ItemWeaponData weaponData, WeaponStat weaponStat)
	{
		base.Init(creature, weaponData, weaponStat);

		m_destroy = false;
		m_elapsed = Time.time;
		creature.EnableNavMeshObstacleAvoidance(false);
	}

	override public void StartFiring(float targetAngle)
	{
		DidStartFiring(0f);
		m_firing = true;
	}

	public void Update()
	{
		if (m_destroy == true)
		{
			return;
		}

		if (m_elapsed + m_destroyTime < Time.time)
		{
			Bomb();
		}
	}

	void Bomb()
	{
		SuicideBombingBullet bullet = CreateBullet(m_firingDescs[0], transform.position) as SuicideBombingBullet;
		bullet.BombRange += Level-1;
		m_creature.Death();
		
		m_destroy = true;
	}

	void OnTriggerEnter(Collider other) {

		if (m_destroy == true)
		{
			return;
		}

		Creature target = other.gameObject.GetComponent<Creature>();
		if (target && Creature.IsEnemy(target, m_creature))
		{
			Bomb();
		}

	}

}
