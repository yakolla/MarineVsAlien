using UnityEngine;
using System.Collections;

public class SummonMobBullet : GrenadeBullet {

	EffectTargetingCircle	m_effectTargetingPoint = null;
	Creature		m_spawnedMob = null;
	// Use this for initialization

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		m_effectTargetingPoint = new EffectTargetingCircle();
		base.Init(ownerCreature, weapon, targetAngle);

		if (weapon.WeaponStat.summonRefMobId > 0)
		{
			m_spawnedMob = Const.GetSpawn().SpawnMob(RefData.Instance.RefMobs[weapon.WeaponStat.summonRefMobId], weapon.Level, gameObject.transform.position, false, false);
			m_spawnedMob.EnableNavmesh(false);
			m_spawnedMob.CreatureType = ownerCreature.CreatureType;
			m_spawnedMob.SetTarget(null);
		}
	}

	protected override void createParabola(float targetAngle)
	{
		m_parabola = new Parabola(gameObject, m_speed, -(transform.rotation.eulerAngles.y+targetAngle) * Mathf.Deg2Rad, Random.Range(1f, 1.4f), m_bouncing);
		m_effectTargetingPoint.Init(m_parabola.DestPosition);
	}

	void OnDisable()
	{
		if (m_spawnedMob != null)
			m_spawnedMob.EnableNavmesh(true);
	}

	// Update is called once per frame
	new void Update () {
		base.Update();

		if (m_spawnedMob != null)
			m_spawnedMob.transform.position = m_parabola.Position;
	}


	protected override void bomb(float bombRange, GameObject prefBombEffect)
	{
		base.bomb(bombRange, prefBombEffect);
		m_effectTargetingPoint.Death();
	}
}
