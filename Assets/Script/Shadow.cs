using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;

public class Shadow : MonoBehaviour {

	Transform	m_parent;
	Vector3		m_oriScale;
	// Use this for initialization
	void Start () {

		m_oriScale = transform.localScale;
		m_parent = transform;
		while(m_parent.transform.parent != null)
		{
			m_parent = m_parent.transform.parent;
		}
	}

	void OnEnable()
	{
		m_parent = transform;
		while(m_parent.transform.parent != null)
		{
			m_parent = m_parent.transform.parent;
		}
	}

	// Update is called once per frame
	void LateUpdate () {
		if (m_parent == null)
		{
			return;
		}

		transform.position = new Vector3(m_parent.position.x, 0, m_parent.position.z);
		float y = Mathf.Clamp(m_parent.position.y, 0f, 10f);
		transform.localScale = m_oriScale * (1f-(y/10f));
	}

}
