using UnityEngine;
using System.Collections;

public class ShieldPassiveLauncher : Weapon {

	[SerializeField]
	GameObject m_prefChargingEffect;

	Bullet	m_bullet;

	new void Start()
	{
		base.Start();

		m_bullet = CreateBullet(m_firingDescs[0], m_gunPoint.transform.position) as Bullet;
		StartCoroutine(EffectShield());
	}

	override public void StartFiring(float targetAngle)
	{		
		if (isCoolTime() == true )
		{
			ConsumeSP();
			DidStartFiring(0f);
		}
		
		m_firing = true;
	}

	IEnumerator EffectShield()
	{
		DamageNumberSprite sprite = m_creature.FoceDamageText("", Vector3.one, Color.white, DamageNumberSprite.MovementType.FloatingUpAlways);		


		GameObject obj = Instantiate (m_prefChargingEffect, Vector3.zero, transform.rotation) as GameObject;		
		obj.transform.parent = m_creature.transform;
		obj.transform.localPosition = m_prefChargingEffect.transform.localPosition;
		obj.transform.localScale = m_prefChargingEffect.transform.localScale;
		obj.transform.localRotation = m_prefChargingEffect.transform.localRotation;

		while(m_creature != null)
		{
			bool shield = m_creature.m_creatureProperty.Shield > 0;
			if (shield == true)
			{
				sprite.Text = m_creature.m_creatureProperty.Shield.ToString();
			}
			
			sprite.gameObject.SetActive(shield);
			obj.SetActive(shield);
			
			yield return null;
		}

	}

	override public void LevelUp()
	{
		m_level = Mathf.Min(m_level+1, 1);

		m_creature.m_creatureProperty.Shield += 10;
	}

}
