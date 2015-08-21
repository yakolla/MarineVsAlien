using UnityEngine;
using System.Collections;

public class GuidedRocketLauncherBullet : RocketLauncherBullet {

	[SerializeField]
	float m_searchCoolTime = 0.3f;
	float m_lastSearchTime = 0f;
	Creature	m_target = null;

	float	m_selfDestoryTime;

	[SerializeField]
	float	m_searchRange = 3f;

	[SerializeField]
	int maxSearch = 3;

	float	m_refreshAngleCoolTime = 0f;
	Bezier 	m_bezier;

	GuidedRocketLauncher	m_weapon;
	// Use this for initialization
	void Start () {

	}

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		base.Init(ownerCreature, weapon, targetAngle);

		m_weapon = weapon as GuidedRocketLauncher;
		m_selfDestoryTime = Time.time + 5f;


	}

	new void OnEnable()
	{
		base.OnEnable();

		m_lastSearchTime = 0f;
		m_target = null;
	}

	// Update is called once per frame
	void Update () {

		if (m_isDestroying == true)
			return;

		if (m_selfDestoryTime < Time.time)
		{
			if (m_weapon != null)
				m_weapon.OnDestroyBullet();
			Bomb();
			return;
		}

		if (m_target == null && m_lastSearchTime <= Time.time)
		{
			m_lastSearchTime = Time.time + m_searchCoolTime;

			Creature[] searchedTargets = Bullet.SearchTarget(transform.position, m_ownerCreature.GetMyEnemyType(), m_searchRange);
			if (searchedTargets != null)
			{
				m_target = searchedTargets[Random.Range(0, searchedTargets.Length)];
				Vector3 handle1 = transform.position;
				handle1 += (transform.forward+transform.up)*(Vector3.Distance(transform.position, m_target.transform.position)*0.5f);
				Vector3 handle2 = m_target.transform.position;
				handle2.y = 3f;
				m_bezier = new Bezier(gameObject, m_target.gameObject, handle1, handle2, 0.01f);
			}

		}

		if (m_bezier != null)
		{
			if (m_bezier.Update() == false)
			{
				if (m_weapon != null)
					m_weapon.OnDestroyBullet();
				Bomb();
				m_bezier = null;
			}
		}
		/*
		float destAngle = transform.eulerAngles.y;
		if (m_target != null)
		{
			destAngle = Mathf.Atan2(m_target.transform.position.z-transform.position.z, m_target.transform.position.x-transform.position.x) * Mathf.Rad2Deg;
		}
		transform.eulerAngles = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, -destAngle, 0)), 300f*Time.deltaTime).eulerAngles;

		transform.Translate(Mathf.Clamp(m_accel, 0, 0.2f), 0, 0, transform);
		m_accel += Time.fixedDeltaTime*Time.fixedDeltaTime*m_speed;
*/
	}

	new void OnTriggerEnter(Collider other) {
		return;

		if (m_isDestroying == true)
			return;

		if (other.tag.CompareTo("Wall") == 0)
		{
			if (m_weapon != null)
				m_weapon.OnDestroyBullet();

			GameObjectPool.Instance.Free(this.gameObject);
			return;
		}

		Creature target = other.gameObject.GetComponent<Creature>();
		if (m_target != null && m_target == target)
		{
			if (m_weapon != null)
				m_weapon.OnDestroyBullet();
			Bomb();
		}
	}

}
