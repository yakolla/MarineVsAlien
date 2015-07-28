using UnityEngine;
using System.Collections;

public class BombFramgment : MonoBehaviour {

	Parabola	m_parabola;

	void Start()
	{
		m_parabola = new Parabola(gameObject, Random.Range(5f, 8f), Random.Range(-3.14f, 3.14f), Random.Range(1.0f, 1.57f), 3);
		rigidbody.AddTorque(Random.Range(0f, 2f), Random.Range(0f, 2f), Random.Range(0f, 2f), ForceMode.Impulse);
	}

	void Update()
	{
		if (false == m_parabola.Update())
		{
			DestroyObject(gameObject);
		}
	}

}
