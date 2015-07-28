using UnityEngine;
using System.Collections;

public class LeapStrikeBullet : Bullet {

	[SerializeField]
	GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	float			m_bombRange = 5f;


	void OnEnable()
	{
		m_isDestroying = false;
	}

	// Update is called once per frame
	protected void Update () {
		if (m_isDestroying == true)
			return;
		
		bomb(m_bombRange, m_prefBombEffect);
		
	}
}
