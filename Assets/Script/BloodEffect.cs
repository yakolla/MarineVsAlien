using UnityEngine;
using System.Collections;

public class BloodEffect : MonoBehaviour {


	float		m_elapsed;

	void Start()
	{
		m_elapsed = Time.time + 3f;
	}

	void Update()
	{
		if (m_elapsed < Time.time)
		{
			DestroyObject(gameObject);
		}
	}

}
