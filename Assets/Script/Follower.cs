using UnityEngine;
using System.Collections;

public class Follower : Creature {


	Creature	m_owner;
	MobAI		m_ai;
	bool		m_updatable;
	int			m_consumedSP;
	int SP
	{
		get {return m_consumedSP*m_creatureProperty.Level;}
	}
	// Update is called once per frame
	new void Update () {

		if (m_owner == null)
			return;

		base.Update();

		m_ai.Update();
		
	}

	override public bool AutoAttack() {
		
		
		if (m_updatable == true && HasCrowdControl() == false)
		{
			if (Targetting == null)
			{
				SetTarget(m_owner.SearchTarget(GetMyEnemyType(), null, 5f));
			}
			
			if (Targetting != null)
			{
				if (true == inAttackRange(Targetting, 0f))
				{
					m_weaponHolder.StartFiring(RotateToTarget(Targetting.transform.position));
					return true;
				}

				m_weaponHolder.StopFiring();
				return false;
			}

		}
		
		SetTarget(null);
		m_weaponHolder.StopFiring();
		return false;
	}

	override public Creature GetOwner()
	{
		return m_owner;
	}

	public override void SetTarget(Creature target)
	{
		base.SetTarget(target);
		m_ai.SetTarget(target);
		
	}

	IEnumerator DecHpEffect()
	{
		while(m_creatureProperty.HP > 0)
		{
			yield return new WaitForSeconds(1f);
			m_creatureProperty.HP -= 1;
		}

		Death();
	}

	IEnumerator DecSpEffect()
	{
		while(m_behaviourType == BehaviourType.ALive)
		{
			yield return new WaitForSeconds(1f);
			if (m_owner == null)
				break;

			if (m_owner.m_creatureProperty.SP > SP)
			{
				m_owner.m_creatureProperty.SP -= SP;
				m_updatable = true;
			}
			else
			{
				m_updatable = false;
			}
		}		

	}

	public void Init(Creature owner, RefMob refMob, RefItem refItem, int level)
	{
		base.Init(refMob, level);

		m_owner = owner;
		CreatureType = m_owner.CreatureType;
		m_consumedSP = refItem.consumedSP;

		if (m_creatureProperty.MoveSpeed == 0f)
		{
			EnableNavMeshObstacleAvoidance(false);
		}

		switch(refMob.mobAI)
		{
		case MobAIType.Normal:
			m_ai = new MobAINormal();
			break;
		case MobAIType.Rotation:
			m_ai = new MobAIRotation();
			break;
		case MobAIType.Revolution:
			m_ai = new MobAIRevolution();
			break;
		case MobAIType.ItemShuttle:
			m_ai = new MobAIItemShuttle();
			break;
		case MobAIType.Pickup:
			m_ai = new MobAIPickup();
			((MobAIPickup)(m_ai)).SetOwner(m_owner);
			CreatureType = Type.Npc;	
			gameObject.layer = 0;
			break;
		case MobAIType.Follow:
			m_ai = new MobAIFollow();
			((MobAIFollow)(m_ai)).SetOwner(m_owner);
			CreatureType = Type.ChampNpc;	
			gameObject.layer = 0;
			StartCoroutine(DecSpEffect());
			break;
		}
		
		m_ai.Init(this);
	
	}
	
	public void LevelUp()
	{
		++m_creatureProperty.Level;
		WeaponHolder.LevelUp();
	}


}
