using UnityEngine;

public class EffectTargetingCircle : EffectTargeting{
	

	public override void	Init(Vector3 targetPos)
	{
		m_prefName = "Pref/ef_targeting";
		base.Init(targetPos);

		ParticleSystem particle = m_effectTargetPoint.GetComponent<ParticleSystem>();
		particle.Play();


	}


}
