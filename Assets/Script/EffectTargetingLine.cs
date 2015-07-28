using UnityEngine;

public class EffectTargetingLine : EffectTargeting {
	

	public void	Init(Vector3 srcPos, Vector3 targetPos)
	{
		m_prefName = "Pref/ef laser point _enemy";
		base.Init(srcPos);

		float targetHorAngle = Mathf.Atan2(targetPos.z-srcPos.z, targetPos.x-srcPos.x) * Mathf.Rad2Deg;
		m_effectTargetPoint.transform.eulerAngles = new Vector3(0, -targetHorAngle, 0);

		m_effectTargetPoint.transform.Translate(new Vector3(m_effectTargetPoint.transform.localScale.x, 0, 0));

	}
}
