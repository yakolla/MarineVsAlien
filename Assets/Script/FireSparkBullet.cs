using UnityEngine;
using System.Collections;

public class FireSparkBullet : Bullet {

	[SerializeField]
	GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	float			m_bombRange = 5f;
	
	ParticleSystem	m_particleSystem;
	BoxCollider		m_boxCollider;
	Parabola	m_parabola;
	
	protected enum Status
	{
		Dropping,
		Dropped,
		Charging,
		Firing,
	}
	
	protected Status			m_status = Status.Dropped;
	
	float			m_bombTime = 0f;
	
	[SerializeField]
	float			m_duration = 10f;
	
	[SerializeField]
	float			m_damageOnTime = 1f;
	
	float			m_dropTime = 0f;

	[SerializeField]
	float			m_charingTime = 1f;


	// Use this for initialization
	void Start () {
		

		m_damageType = DamageDesc.Type.Fire;	

	}

	void OnEnable()
	{
		m_boxCollider = GetComponent<BoxCollider>();
		m_particleSystem = m_prefBombEffect.particleSystem;
		m_boxCollider.enabled = false;
		m_particleSystem.enableEmission = false;
		int[] angles = {0, 90};
		transform.eulerAngles = new Vector3(0, angles[Random.Range(0, angles.Length)], 0);

		m_dropTime = 0f;
		m_bombTime = 0f;

		m_status = Status.Dropped;
	}
	
	// Update is called once per frame
	protected void Update () {
		
		switch(m_status)
		{
		case Status.Dropping:
			break;
		case Status.Dropped:
			{
				m_dropTime = Time.time+m_charingTime;
				Vector3 scale = Vector3.one;
				scale.x = m_bombRange;
				transform.localScale = scale;
				
				m_particleSystem.enableEmission = true;
				m_status = Status.Charging;				
			}
			break;
		case Status.Charging:
			if (m_dropTime < Time.time)
			{
				m_status = Status.Firing;
				
				m_boxCollider.enabled = true;
				m_bombTime = Time.time + m_damageOnTime;
				Vector3 rotation = m_prefBombEffect.transform.eulerAngles;
				rotation.y = transform.eulerAngles.y;
				
				GameObject bombEffect = (GameObject)Instantiate(m_prefBombEffect, transform.position, Quaternion.Euler(rotation));
				Vector3 scale = Vector3.one;
				scale.x = m_bombRange*3f;
				bombEffect.transform.localScale = scale;
				
				StartCoroutine(destoryObject(bombEffect));
			}
			break;
		case Status.Firing:
			if (m_bombTime < Time.time)
			{
				m_bombTime = Time.time + m_damageOnTime;
				m_boxCollider.enabled = false;
			}
			else
			{
				m_boxCollider.enabled = true;
			}
			break;
		}
		
		
	}
	
	IEnumerator destoryObject(GameObject bombEffect)
	{
		yield return new WaitForSeconds (m_duration);
		GameObjectPool.Instance.Free(this.gameObject);
		DestroyObject(bombEffect);
	}
	
	void OnTriggerEnter(Collider other) 
	{
		
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
		}
	}
}
