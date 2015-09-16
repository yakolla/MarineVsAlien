/*
	This script is placed in public domain. The author takes no responsibility for any possible harm.
	Contributed by Jonathan Czeck
*/
using UnityEngine;
using System.Collections;

public class LightningBullet : Bullet
{
	[SerializeField]
	int zigs = 100;

	[SerializeField]
	float speed = 1f;

	[SerializeField]
	float scale = 1f;

	[SerializeField]
	float m_length = 10f;

	[SerializeField]
	float	m_damageOnTime = 1f;

	[SerializeField]
	Color	m_color = Color.white;

	float			m_lastDamageTime = 0f;

	[SerializeField]
	int	m_maxChaining = 5;

	[SerializeField]
	bool m_beamMode = false;

	Perlin noise;
	float oneOverZigs;

	Weapon		m_weapon;
	
	private Particle[] particles;

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		Vector3 scale = transform.localScale;

		base.Init(ownerCreature, weapon, targetAngle);

		m_weapon = weapon;
		transform.parent = ownerCreature.WeaponHolder.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(new Vector3(0, targetAngle.angle, 0));
		transform.localScale = scale;
	}
	
	void Start()
	{
		m_damageType = DamageDesc.Type.Lightining;

		oneOverZigs = 1f / (float)zigs;
		particleEmitter.emit = false;

		particleEmitter.Emit(zigs);
		particles = particleEmitter.particles;
		noise = new Perlin();

		for(int i = 0; i < particles.Length; ++i)
		{
			particles[i].position = Vector3.zero;
			particles[i].color = m_color;
			particles[i].energy = 1f;
		}
		particleEmitter.particles = particles;
	}

	float BulletLength()
	{
		return m_length+m_ownerCreature.m_creatureProperty.BulletLength;
	}
	
	void Update ()
	{

		bool mobHitted = false;

		Creature[] targets = null;
		int hittedTargetCount = 0;
		if (m_beamMode == false)
		{
			targets = new Creature[m_maxChaining];
			Creature creature = null;
			RaycastHit hit;
			Vector3 fwd = transform.TransformDirection(Vector3.right);
			Vector3 pos = transform.position;
			pos.y=0;
			if (Physics.Raycast(pos, fwd, out hit, BulletLength(), 1<<9))
				creature = hit.transform.GetComponent<Creature>();

/*
			if (m_ownerCreature.Targetting == null)
			{
				Creature[] hitted = Bullet.SearchTarget(transform.position, m_ownerCreature.GetMyEnemyType(), BulletLength());
				if (hitted != null)
				{
					creature = hitted[0];
				}
				Debug.Log("Lightning NULL");
			}
			else
			{
				creature = m_ownerCreature.Targetting;
				Debug.Log("Lightning Not NULL");
			}*/

			if (creature != null)
			{
				if (creature && Creature.IsEnemy(creature, m_ownerCreature))
				{				
					m_ownerCreature.SetTarget(creature);
					m_ownerCreature.RotateToTarget(creature.transform.position);

					targets[0] = creature;
					mobHitted = true;
					hittedTargetCount = 1;
					
					for(int i = 1; i < m_maxChaining; ++i)
					{
						Creature[] chaningTargets = Bullet.SearchTarget(targets[i-1].transform.position, m_ownerCreature.GetMyEnemyType(), 3f, targets);
						if (chaningTargets == null)
							break;

						targets[i] = chaningTargets[0];
						hittedTargetCount++;
					}
					
					int perParticles = particles.Length/hittedTargetCount;
					Transform old = targets[0].transform.Find("Body/Aimpoint");
					if (old != null)
					{
						Vector3 oldAimpoint = old.position;
						createChanningParticle(transform.position, oldAimpoint, 0, perParticles);
						for(int i = 1; i < hittedTargetCount; ++i)
						{
							Transform t  = targets[i].transform.Find("Body/Aimpoint");
							if (t != null)
							{
								Vector3 aimpoint = t.position;
								createChanningParticle(oldAimpoint, aimpoint, perParticles*(i), perParticles*(i)+perParticles);
								oldAimpoint = aimpoint;
							}
						}
					}
					
					particleEmitter.particles = particles;
				}
			}
		}
		else
		{
			RaycastHit[] hit;
			Vector3 fwd = transform.TransformDirection(Vector3.right);
			hit = Physics.RaycastAll(transform.position, fwd, BulletLength(), 1<<9);
			if (hit.Length > 0)
			{
				targets = new Creature[hit.Length];
				for(int i = 0; i < hit.Length; ++i)
				{
					Creature creature = hit[i].transform.gameObject.GetComponent<Creature>();
					if (creature && Creature.IsEnemy(creature, m_ownerCreature))
					{				
						targets[hittedTargetCount] = creature;
						hittedTargetCount += 1;
					}
				}

			}
		}

		if (hittedTargetCount > 0)
		{
			TryToSetDamageBuffType(m_weapon);

			if (m_lastDamageTime+(m_damageOnTime*m_ownerCreature.m_creatureProperty.AttackCoolTime)<Time.time)
			{
				for(int i = 0; i < hittedTargetCount; ++i)
				{
					GiveDamage(targets[i]);
				}
				
				m_lastDamageTime = Time.time;
			}
		}
		else
		{
			m_lastDamageTime = Time.time;

			Vector3 targetPos = new Vector3();
			targetPos.x = Mathf.Cos(transform.rotation.eulerAngles.y*Mathf.Deg2Rad)*BulletLength();
			targetPos.z = Mathf.Sin(transform.rotation.eulerAngles.y*Mathf.Deg2Rad)*-BulletLength();
			targetPos.x += transform.position.x;
			targetPos.z += transform.position.z;
			targetPos.y = m_ownerCreature.transform.localPosition.y;
			
			createChanningParticle(transform.position, targetPos, 0, particles.Length);
			
			particleEmitter.particles = particles;

			m_ownerCreature.SetTarget(null);
		}

		if (mobHitted == false)
		{

		}
	}	

	void createChanningParticle(Vector3 start, Vector3 dest, int particleStartIndex, int particleFinishIndex)
	{
		float timex = Time.time * speed * 0.1365143f;
		float timey = Time.time * speed * 1.21688f;
		float timez = Time.time * speed * 2.5564f;


		int length = particleFinishIndex-particleStartIndex;
		for (int i=0; i < length; i++)
		{
			float lerpProgress = oneOverZigs * (float)i * (particles.Length/length);
			Vector3 position = Vector3.Lerp(start, dest, lerpProgress);
			Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
			                             noise.Noise(timey + position.x, timey + position.y, timey + position.z),
			                             noise.Noise(timez + position.x, timez + position.y, timez + position.z));
			position += offset * scale * lerpProgress;
			
			particles[particleStartIndex+i].position = position;
			particles[particleStartIndex+i].color = m_color;
			particles[particleStartIndex+i].energy = 1f;
		}
	}

	public int MaxChaining
	{
		get { return m_maxChaining; }
		set { m_maxChaining = value;}
	}

	public bool BeamMode
	{
		set {m_beamMode = value;}
	}
}