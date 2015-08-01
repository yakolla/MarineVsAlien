using UnityEngine;
using System.Collections;

public class Follower : Creature {


	Creature	m_owner;
	MobAI		m_ai;
	int			m_refMobId;

	// Update is called once per frame
	new void Update () {

		base.Update();

		m_ai.Update();
		
	}

	override public bool AutoAttack() {
		
		
		if (HasCrowdControl() == false)
		{
			if (Targetting == null)
			{
				SetTarget(SearchTarget(GetMyEnemyType(), null, 50f));
			}
			
			if (Targetting != null)
			{
				if (true == inAttackRange(Targetting, 0f))
				{
					m_weaponHolder.StartFiring(RotateToTarget(Targetting.transform.position));
					return true;
				}
			}
		}
		
		SetTarget(null);
		m_weaponHolder.StopFiring();
		return false;
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

	public void Init(Creature owner, RefMob refMob, int level)
	{
		base.Init(refMob, level);

		m_owner = owner;
		CreatureType = m_owner.CreatureType;

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
			//StartCoroutine(DecHpEffect());
			break;
		}
		
		m_ai.Init(this);
	
	}

	public int FollowerID
	{
		get{return m_refMobId;}
	}

	public void LevelUp()
	{
		WeaponHolder.LevelUp();
	}


}
