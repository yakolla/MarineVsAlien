using UnityEngine;
using System.Collections;

public class CreatureDeathEffect : MonoBehaviour {

	ParticleSystem m_ps;

	void Start () {

	}

	void OnEnable()
	{
		m_ps = transform.Find("ef death").GetComponent<ParticleSystem>();
		Vector3 pos = m_ps.transform.position;
		pos.y = 0f;
		m_ps.transform.position = pos;
	}

	void Update()
	{
		if (m_ps.IsAlive() == false)
		{
			GameObject.DestroyObject(gameObject);
		}
	}
}
