using UnityEngine;
using System.Collections;

public class MineBullet : GrenadeBullet {


	float			m_elapsed = 0f;

	[SerializeField]
	float			m_bombTime = 3f;

	BoxCollider		m_boxCollider;
	// Use this for initialization
	void Start () {


	}

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		base.Init(ownerCreature, weapon, targetAngle);
	
		m_elapsed = Time.time+m_bombTime;

		m_boxCollider = GetComponent<BoxCollider>();
		m_boxCollider.enabled = false;
	}

	protected override void createParabola(float targetAngle)
	{
		base.createParabola(Random.Range(0f, 360f)+targetAngle);
	}

	// Update is called once per frame
	new void Update () {
		if (m_isDestroying == true)
			return;

		if (m_parabola.Update() == false)
		{
			m_boxCollider.enabled = true;
			if (m_elapsed < Time.time)
			{
				GameObjectPool.Instance.Free(this.gameObject);
			}
		}

	}

	void OnTriggerEnter(Collider other) {
		if (m_isDestroying == true)
			return;
		
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			m_isDestroying = true;
			bomb(m_bombRange, m_prefBombEffect);
		}
	}
}
