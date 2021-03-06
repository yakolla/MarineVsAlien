using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CreatureProperty {

	Creature	m_owner;

	RefCreatureBaseProperty	m_baseProperty;
	SecuredType.XInt64	m_hp = 0;

	SecuredType.XInt64		m_alphaMaxHP = 0;

	SecuredType.XInt		m_alphaPhysicalDamage = 0;

	SecuredType.XFloat	m_alphaCriticalRatio = 0f;

	SecuredType.XFloat	m_alphaCriticalDamage = 0f;

	SecuredType.XInt		m_alphaPhysicalDefencePoint = 0;
	SecuredType.XFloat	m_damageReduction = 0f;

	SecuredType.XFloat	m_alphaMoveSpeed = 0f;

	SecuredType.XFloat	m_betaMoveSpeed = 1f;

	SecuredType.XFloat	m_alphaLifeSteal = 0f;

	SecuredType.XFloat	m_gainExtraExp = 0f;
	SecuredType.XFloat	m_gainExtraGold = 0f;

	SecuredType.XFloat	m_alphaAttackCoolTime = 0f;

	SecuredType.XFloat 	m_bulletAlphaLength = 0f;

	SecuredType.XInt		m_shield = 0;

	SecuredType.XInt		m_level = 1;
	SecuredType.XInt		m_evolution = 0;

	SecuredType.XInt		m_exp = 0;

	SecuredType.XInt		m_alphaMaxSP = 0;
	SecuredType.XInt		m_sp = 0;

	SecuredType.XInt		m_callableFollowers = 1;

	SecuredType.XFloat	m_alphaSPRegen = 0f;

	SecuredType.XFloat	m_accSPRecoveryPerSec = 0f;

	SecuredType.XInt	m_bombRange = 0;
	SecuredType.XInt	m_attackRange = 0;

	SecuredType.XInt	m_tabDamage = 0;

	SecuredType.XFloat	m_dodge = 0f;

	SecuredType.XFloat	m_damageMultiPlier = 0;

	public void 	init(Creature owner, RefCreatureBaseProperty baseProperty, int level, int evolution)
	{
		m_owner = owner;
		m_baseProperty = baseProperty;
		Level = level;
		m_evolution = evolution;
		m_exp = m_baseProperty.exp;
	}

	public float getHPRemainRatio()
	{
		return (float)HP/MaxHP;
	}

	public long MaxHP
	{
		get { return (long)((m_baseProperty.maxHP+AlphaMaxHP)+(m_baseProperty.maxHP+AlphaMaxHP)*(Level-1)*m_baseProperty.hpPerLevel); }
	}

	public long AlphaMaxHP
	{
		get { return m_alphaMaxHP.Value; }
		set { m_alphaMaxHP.Value = value; }
	}

	public long HP
	{
		get { return m_hp.Value; }
		set {
			m_hp.Value = value;
			if (m_hp.Value < 0)
				m_hp.Value = 0;
			else if (m_hp.Value > MaxHP)
				m_hp.Value = MaxHP;
		}
	}

	public float getSPRemainRatio()
	{
		return (float)SP/MaxSP;
	}
	
	public int MaxSP
	{
		get { return (int)((m_baseProperty.maxSP+AlphaMaxSP)+(m_baseProperty.maxSP+AlphaMaxSP)*(Level-1)*m_baseProperty.spPerLevel); }
	}
	
	public int AlphaMaxSP
	{
		get { return m_alphaMaxSP.Value; }
		set { m_alphaMaxSP.Value = value; }
	}
	
	public int SP
	{
		get { return m_sp.Value; }
		set {
			m_sp.Value = value;
			m_sp.Value = Mathf.Clamp(m_sp.Value, 0, MaxSP);
		}

	}

	public float AlphaSPRegen
	{
		get { return m_alphaSPRegen.Value; }
		set { m_alphaSPRegen.Value = value; }
	}

	public float SPRegen
	{
		get{ return (m_baseProperty.spRegen + m_alphaSPRegen.Value);}
	}

	public int Evolution
	{
		get {return m_evolution.Value;}
		set {m_evolution = value;}
	}

	public int Level
	{
		get { return m_level.Value; }
		 set {
			m_level = value;
			m_hp = MaxHP;
			m_sp = MaxSP;
		}
	}

	public float getExpRemainRatio()
	{
		return (float)Exp/MaxExp;
	}

	public int MaxExp
	{
		get { return Mathf.FloorToInt(m_level.Value*350*1.1f); }
	}

	public int Exp
	{
		get { return m_exp.Value; }
		set { m_exp.Value = value; }
	}

	public int RewardExp
	{
		get{return (int)(Exp+Exp*m_baseProperty.expPerLevel*(Level-1));}
	}

	public void		giveExp(int exp)
	{
		m_exp.Value += exp;
		while (MaxExp <= m_exp.Value)
		{
			m_exp.Value -= MaxExp;
			++Level;

			if (m_owner != null)
			{
				m_owner.SendMessage("LevelUp");
			}

		}
	}

	public float	DamageMultiPlier
	{
		get{return m_damageMultiPlier.Value;}
		set{m_damageMultiPlier.Value = value;}
	}

	public float	Dodge
	{
		get{return m_dodge.Value;}
		set{m_dodge.Value = value;}
	}

	public int Strength
	{
		get {
			return (int)(m_baseProperty.physicalDamage + AlphaPhysicalAttackDamage + (m_baseProperty.physicalDamage + AlphaPhysicalAttackDamage)*(Level-1)*m_baseProperty.phyDamagePerLevel);

		}
	}

	public int	PhysicalAttackDamage
	{
		get {
			return Strength;
		}
	}

	public int	AlphaPhysicalAttackDamage
	{
		get {return m_alphaPhysicalDamage.Value;}
		set { m_alphaPhysicalDamage.Value = value; }
	}

	public int	PhysicalDefencePoint
	{
		get {return (int)((m_baseProperty.physicalDefence + AlphaPhysicalDefencePoint)*(Level-1)*m_baseProperty.phyDefencePerLevel);}
	}

	public int	AlphaPhysicalDefencePoint
	{
		get {return m_alphaPhysicalDefencePoint.Value;}
		set { m_alphaPhysicalDefencePoint.Value = value; }
	}

	public float	DamageReduction
	{
		get {return m_damageReduction.Value;}
		set { m_damageReduction.Value = value; }
	}

	public float	MoveSpeed
	{
		get {return (m_baseProperty.moveSpeed + AlphaMoveSpeed) * BetaMoveSpeed;}
	}

	public float	AniSpeed
	{
		get {return m_baseProperty.aniSpeedRatio * MoveSpeed;}
	}
	
	public float	AlphaMoveSpeed
	{
		get {return m_alphaMoveSpeed.Value;}
		set { m_alphaMoveSpeed.Value = Mathf.Min(value, Const.MaxAlphaMoveSpeed); }
	}

	public float	BetaMoveSpeed
	{
		get {return m_betaMoveSpeed.Value;}
		set { m_betaMoveSpeed.Value = value; }
	}


	public float LifeSteal
	{
		get{return m_baseProperty.lifeSteal + AlphaLifeSteal;}
	}

	public float AlphaLifeSteal
	{
		get { return m_alphaLifeSteal.Value; }
		set { m_alphaLifeSteal.Value = value; }
	}

	public float	CriticalChance
	{
		get {return Mathf.Min(1, m_baseProperty.criticalChance + AlphaCriticalChance);}
	}

	public float	AlphaCriticalChance
	{
		get {return m_alphaCriticalRatio.Value;}
		set { m_alphaCriticalRatio.Value = value; }
	}

	public float	CriticalDamage
	{
		get {return m_baseProperty.criticalDamage + AlphaCriticalDamage;}
	}
	
	public float	AlphaCriticalDamage
	{
		get {return m_alphaCriticalDamage.Value;}
		set { m_alphaCriticalDamage.Value = value; }
	}

	public float	GainExtraExp
	{
		get {return m_gainExtraExp.Value;}
		set { m_gainExtraExp.Value = value; }
	}	

	public float	GainExtraGold
	{
		get {return m_gainExtraGold.Value;}
		set { m_gainExtraGold.Value = value; }
	}

	public float	AttackCoolTime
	{
		get {return Mathf.Max(m_baseProperty.attackCoolTime + AlphaAttackCoolTime, 0.2f);}
	}
	
	public float	AlphaAttackCoolTime
	{
		get {return m_alphaAttackCoolTime.Value;}
		set { m_alphaAttackCoolTime.Value = value; }
	}

	public float BulletLength
	{
		get {return m_baseProperty.bulletLength+m_bulletAlphaLength.Value;}
	}

	public float BulletAlphaLength
	{
		get {return m_bulletAlphaLength.Value;}
		set { m_bulletAlphaLength.Value = value; }
	}

	public int Shield
	{
		get {return m_shield.Value;}
		set { m_shield.Value = value; }
	}

	public float RotationSpeedRatio
	{
		get {return m_baseProperty.rotationSpeedRatio;}
	}

	public bool		BackwardOnDamage
	{
		get {return m_baseProperty.backwardOnDamage;}
	}

	public int SplashRadius
	{
		set {m_bombRange.Value = value;}
		get {return m_bombRange.Value;}
	}

	public int AttackRange
	{
		set {m_attackRange.Value = value;}
		get {return m_attackRange.Value;}
	}

	public int CallableFollowers
	{
		set {m_callableFollowers.Value = value;}
		get {return m_callableFollowers.Value;}
	}

	public int TapDamage
	{
		set {m_tabDamage.Value = value;}
		get {return m_tabDamage.Value;}
	}

	public void Update()
	{
		m_accSPRecoveryPerSec.Value += (SPRegen * Time.deltaTime);
		if (m_accSPRecoveryPerSec.Value >= 1f)
		{
			SP += (int)m_accSPRecoveryPerSec.Value;
			m_accSPRecoveryPerSec.Value -= (int)m_accSPRecoveryPerSec.Value;
		}
	}

	public void CopyTo(CreatureProperty other)
	{
		other.m_owner = m_owner;
		
		other.m_baseProperty = m_baseProperty;
		other.m_hp = m_hp;
		other.m_alphaMaxHP = m_alphaMaxHP;
		other.m_alphaPhysicalDamage = m_alphaPhysicalDamage;
		other.m_alphaCriticalRatio = m_alphaCriticalRatio;
		other.m_alphaCriticalDamage = m_alphaCriticalDamage;
		other.m_alphaPhysicalDefencePoint = m_alphaPhysicalDefencePoint;
		other.m_damageReduction = m_damageReduction;
		other.m_alphaMoveSpeed = m_alphaMoveSpeed;
		other.m_betaMoveSpeed = m_betaMoveSpeed;
		other.m_alphaLifeSteal = m_alphaLifeSteal;
		other.m_gainExtraExp = m_gainExtraExp;
		other.m_gainExtraGold = m_gainExtraGold;
		other.m_alphaAttackCoolTime = m_alphaAttackCoolTime;
		other.m_level = m_level;
		other.m_exp = m_exp;
		other.m_bulletAlphaLength = m_bulletAlphaLength;
		other.m_tabDamage = m_tabDamage;

		other.m_shield = m_shield;
		other.m_bombRange = m_bombRange;
		other.m_attackRange = m_attackRange;
		other.m_alphaMaxSP = m_alphaMaxSP;
		other.m_sp = m_sp;
		other.m_alphaSPRegen = m_alphaSPRegen;
		other.m_callableFollowers = m_callableFollowers;
		other.m_damageMultiPlier = m_damageMultiPlier;
		other.m_evolution = m_evolution;
	}
}
