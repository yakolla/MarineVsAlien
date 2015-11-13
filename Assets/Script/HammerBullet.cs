using UnityEngine;
using System.Collections;

public class HammerBullet : Bullet {
	
	[SerializeField]
	GameObject		m_prefBombEffect;
	[SerializeField]
	float		m_bombRange = 2f;
	float		m_elapsedTime = 0f;
	// Use this for initialization
	void Start () 
	{

	}

	// Update is called once per frame
	protected void Update () {
		if (m_isDestroying == true)
			return;

		m_elapsedTime += Time.deltaTime;

		if (m_elapsedTime >= 0.1f)
		{
			bomb(m_bombRange, m_prefBombEffect);
		}
		
	}

	void OnEnable()
	{
		m_isDestroying = false;
	}

}
