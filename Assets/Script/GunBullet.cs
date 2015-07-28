using UnityEngine;
using System.Collections;

public class GunBullet : Bullet {

	float	m_speed = 3f;

	// Use this for initialization
	void Start () {

	}

	void OnEnable()
	{
		m_isDestroying = false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(m_speed*Time.deltaTime, 0, 0, transform);		
	}

	void OnTriggerEnter(Collider other) {
		if (m_isDestroying == true)
			return;

		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
			m_isDestroying = true;
			GameObjectPool.Instance.Free(this.gameObject);
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			m_isDestroying = true;
			GameObjectPool.Instance.Free(this.gameObject);
		}
	}

	public float	BulletSpeed
	{
		get {return m_speed;}
		set {m_speed = value;}
	}
}
