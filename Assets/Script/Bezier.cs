using UnityEngine;

public class Bezier {

	Vector3 m_start;
	Vector3 m_end;
	Vector3 m_handle1;
	Vector3 m_handle2;
	float	m_step = 0.07f;
	GameObject m_owner;
	GameObject m_target;
	float c = 0;

	public Bezier(GameObject obj, GameObject target, Vector3 handle1, Vector3 handle2, float step)
	{
		m_owner = obj;
		m_start = obj.transform.position;
		m_end = target.transform.position;
		m_handle1 = handle1;
		m_handle2 = handle2;
		m_step = step;
		m_target = target;
	}

	public void Destroy()
	{
		MonoBehaviour.DestroyObject(m_owner);
	}
	
	Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float u = 1 - t;
		float tt = t*t;
		float uu = u*u;
		float uuu = uu * u;
		float ttt = tt * t;
		
		Vector3 p = uuu * p0; //first term
		p += 3 * uu * t * p1; //second term
		p += 3 * u * tt * p2; //third term
		p += ttt * p3; //fourth term
		
		return p;
	}

	public bool Update()
	{
		if (m_target != null)
		{
			m_end = m_target.transform.position;
			
		}
		if (c < 1) //we defined 100 steps to draw the curve
		{
			c += m_step; //100 steps to draw each bezier curve
			c = Mathf.Min(1, c);
			m_owner.transform.position = CalculateBezierPoint(c, m_start, m_handle1, m_handle2, m_end);
			return true;
		}

		m_owner.transform.position = m_end;
		return false;
	}

}
