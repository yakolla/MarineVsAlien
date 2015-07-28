using UnityEngine;

public class EffectTargeting {
	
	protected GameObject	m_effectTargetPoint;
	protected string		m_prefName;

	bool	m_death = false;

	virtual public void	Init(Vector3 targetPos)
	{
		if (m_effectTargetPoint == null)
		{
			GameObject	prefEffectTargetPoint = Resources.Load<GameObject>(m_prefName);
			m_effectTargetPoint = GameObject.Instantiate (prefEffectTargetPoint, targetPos, prefEffectTargetPoint.transform.localRotation) as GameObject;
		}

		Const.GetSpawn().StartCoroutine(AutoDeath(this));
		m_death = false;
		SetActive(true);
		m_effectTargetPoint.transform.position = targetPos;

	}

	System.Collections.IEnumerator AutoDeath(EffectTargeting obj)
	{
		yield return new WaitForSeconds(5f);

		obj.Death();
	}

	public void Death()
	{
		if (m_death == true)
			return;

		m_death = true;
		SetActive(false);
		GameObject.DestroyObject(m_effectTargetPoint);
	}

	public void SetActive(bool active)
	{
		m_effectTargetPoint.SetActive(active);
	}
}
