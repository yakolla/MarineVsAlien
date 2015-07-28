using UnityEngine;
using System.Collections;

public class ExplosionBullet : Bullet {

	[SerializeField]
	protected GameObject		m_prefBombEffect = null;

	protected float				m_bombRange;

	// Use this for initialization
	void Start () {
	}

	void OnEnable()
	{
		m_bombRange = 3f;
		m_isDestroying = false;
	}

	// Update is called once per frame
	protected void Update () {

		if (m_isDestroying == true)
			return;

		bomb(m_bombRange, m_prefBombEffect);
	}

	public float BombRange
	{
		set{m_bombRange = value;}
		get{return m_bombRange;}
	}

}
