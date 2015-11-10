using UnityEngine;
using System.Collections;

public class FollowingCamera : MonoBehaviour
{

	[SerializeField]
	GameObject m_target;
	GameObject m_nextTarget;
	GameObject m_mainTarget;


	[SerializeField]
	Vector3 m_cameraOffset = Vector3.zero;
	Vector3 m_cameraSide;
	Vector3	m_from;

	Rect m_cameraEdge = new Rect();

	float m_elapsedTime = 0f;
	bool m_done = false;

	public void SetTarget(GameObject target, GameObject nextTarget)
	{
		m_target = target;
		m_nextTarget = nextTarget;
		m_elapsedTime = 0f;
		m_done = false;
		m_from = Camera.main.transform.position;
	}

	public void SetMainTarget(GameObject mainTarget)
	{
		m_mainTarget = mainTarget;
		SetTarget(m_mainTarget, null);
	}

	public float SideSize
	{
		set {m_cameraSide.x = value;}
	}


	void Start()
	{
		BoxCollider edge = GameObject.Find("Dungeon/CameraEdge").gameObject.GetComponent<BoxCollider>();
		m_cameraEdge.x = edge.transform.position.x-edge.size.x/2+edge.center.x;
		m_cameraEdge.y = edge.transform.position.z-edge.center.z;
		m_cameraEdge.width = m_cameraEdge.x+edge.size.x;
		m_cameraEdge.height = m_cameraEdge.y+edge.size.z;
	}
	
	void Update()
	{
		if (m_target == null)
		{
			m_target = m_mainTarget;
			m_elapsedTime = 0f;
			m_done = false;
			m_from = Camera.main.transform.position;
			return;
		}

		m_elapsedTime += Time.deltaTime*0.6f;

		Vector3 myCharacterPosition = Vector3.Lerp(m_from, m_target.transform.position-m_cameraOffset+m_cameraSide, Mathf.Min(1f, m_elapsedTime));
		myCharacterPosition.x = Mathf.Clamp(myCharacterPosition.x, m_cameraEdge.x, m_cameraEdge.width);
		myCharacterPosition.z = Mathf.Clamp(myCharacterPosition.z, m_cameraEdge.y, m_cameraEdge.height);
		Camera.main.transform.position = myCharacterPosition;

		if (m_elapsedTime >= 2f)
		{
			if (m_nextTarget != null)
			{
				SetTarget(m_nextTarget, null);
			}
		}
	}
}