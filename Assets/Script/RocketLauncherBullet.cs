using UnityEngine;
using System.Collections;

public class RocketLauncherBullet : Bullet {

	[SerializeField]
	protected float m_speed = 1f;
	protected float	m_accel = 0f;

	[SerializeField]
	protected GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	protected float			m_bombRange = 5f;

	GameObject			m_efGun;

	// Use this for initialization
	void Start () {

	}

	protected void OnEnable()
	{
		m_isDestroying = false;
		m_accel = 0f;

		m_efGun = transform.Find("Body/ef gun").gameObject;
		m_efGun.SetActive(true);


	}


	// Update is called once per frame
	void Update () {
		if (m_isDestroying == true)
			return;

		transform.Translate(Mathf.Clamp(m_accel, 0, 0.1f), 0, 0, transform);
		m_accel += Time.deltaTime*0.1f*m_speed;


	}

	protected void Bomb()
	{
		m_efGun.SetActive(false);
		bomb(m_bombRange, m_prefBombEffect);
	}

	public void OnTriggerEnter(Collider other) {
		if (m_isDestroying == true)
			return;

		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			Bomb();
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			GameObjectPool.Instance.Free(this.gameObject);
		}
	}
}
