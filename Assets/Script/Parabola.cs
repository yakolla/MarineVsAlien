using UnityEngine;

public class Parabola {

	float 			m_startTime;
	float			m_finishTime;
	Vector3			m_vel;
	float			m_gravity = 10f;
	Vector3			m_oriPos;
	int				m_maxBouncing = 1;
	int				m_bouncing = 0;
	bool			m_finish = false;
	float			m_groundY = 0f;
	float			m_timeScale = 1f;
	float			m_vRadian = 0f;

	Vector3			m_destPos;
	GameObject		m_obj;

	public Parabola(GameObject obj, float speed, float hRadian, float vRadian, int bouncing)
		: this(obj, speed, hRadian, vRadian, 10f, bouncing)
	{

	}

	public Parabola(GameObject obj, float speed, float hRadian, float vRadian, float gravity, int bouncing)
	{
		m_obj = obj;
		m_oriPos = obj.transform.position;
		m_maxBouncing = bouncing;
		m_vRadian = vRadian;
		
		m_vel.x = speed * Mathf.Cos(hRadian);
		m_vel.y = speed * Mathf.Sin(vRadian);
		m_vel.z = speed * Mathf.Sin(hRadian);
		
		m_startTime = Time.time;
		m_finishTime = (m_vel.y/m_gravity)*2;
		
		m_destPos.Set(m_oriPos.x+((m_vel.x*m_vel.x*Mathf.Sin(2*m_vRadian))/m_gravity*(m_vel.x >= 0 ? 1 : -1)), 0.1f, m_oriPos.z+((m_vel.z*m_vel.z*Mathf.Sin(2*m_vRadian))/m_gravity*(m_vel.z >= 0 ? 1 : -1)));
		
	}

	public float GroundY
	{
		set {m_groundY = value;}
	}

	public Vector3 Position
	{
		get {return m_obj.transform.position;}
	}

	public Vector3 DestPosition
	{
		get{return m_destPos;}
	}
	public float TimeScale
	{
		set {m_timeScale = value;}
	}

	public void Destroy()
	{
		MonoBehaviour.DestroyObject(m_obj);
	}	


	public bool Update()
	{
		if (m_finish == true)
			return false;

		float elapse = (Time.time - m_startTime)*m_timeScale;
		float t = Mathf.Min(1f, elapse/m_finishTime);
		float x = m_oriPos.x*(1-t)+m_destPos.x*t;
		float z = m_oriPos.z*(1-t)+m_destPos.z*t;
		float y = m_oriPos.y+m_vel.y*elapse -0.5f*m_gravity*(elapse*elapse);
		m_obj.transform.position = new Vector3(x, Mathf.Max(y, m_groundY), z);

		if (elapse >= m_finishTime && y <= m_groundY)
		{
			++m_bouncing;
			if (m_maxBouncing <= m_bouncing)
			{
				m_finish = true;
				return false;
			}

			// more bounce			
			m_oriPos = m_obj.transform.position;
			m_vel *= 1-(float)m_bouncing/m_maxBouncing;
			m_startTime = Time.time;
			m_finishTime = Mathf.Abs((m_vel.y/m_gravity)*2);
			m_destPos.Set(m_oriPos.x+((m_vel.x*m_vel.x*Mathf.Sin(2*m_vRadian))/m_gravity*(m_vel.x >= 0 ? 1 : -1)), 0.1f, m_oriPos.z+((m_vel.z*m_vel.z*Mathf.Sin(2*m_vRadian))/m_gravity*(m_vel.z >= 0 ? 1 : -1)));

		}

		return true;
	}

}
