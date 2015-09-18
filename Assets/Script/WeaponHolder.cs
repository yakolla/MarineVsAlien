using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHolder : MonoBehaviour {

	List<Weapon>					m_weapons = new List<Weapon>();
	Dictionary<int, Weapon>		m_passiveWeapons = new Dictionary<int, Weapon>();
	Dictionary<int, Weapon>		m_activeWeapons = new Dictionary<int, Weapon>();

	float					m_weaponChangeCoolTime = 15f;
	float					m_weaponChangedTime = 0f;
	int						m_curWeaponIndex = 0;

	[SerializeField]
	bool					m_multipleWeapon = false;

	Creature				m_creature;

	bool					m_firing = false;

	void Update()
	{
		if (m_weapons.Count == 0)
			return;

		if (m_weaponChangedTime+m_weaponChangeCoolTime < Time.time)
		{
			m_weaponChangedTime = Time.time;
			m_curWeaponIndex = (m_curWeaponIndex + 1) % m_weapons.Count;
			StopFiring();
		}

		foreach(KeyValuePair<int, Weapon> pair in m_passiveWeapons)
		{
			pair.Value.StartFiring(-transform.rotation.eulerAngles.y);
		}
	}

	public void Init()
	{
		m_curWeaponIndex = 0;
		m_weaponChangedTime = 0;
		m_weapons.Clear();
		m_firing = false;

		m_creature = transform.parent.GetComponent<Creature>();
	}

	public void EquipWeapon(Weapon weapon)
	{
		m_weapons.Add(weapon);
	}

	public void UnequipWeapon(Weapon weapon)
	{
		m_weapons.Remove(weapon);
	}

	public void EquipActiveSkillWeapon(Weapon weapon)
	{
		m_activeWeapons.Add(weapon.RefItem.id, weapon);
	}


	public void EquipPassiveSkillWeapon(Weapon weapon)
	{
		m_passiveWeapons.Add(weapon.RefItem.id, weapon);
	}

	public Weapon GetPassiveSkillWeapon(int refId)
	{
		Weapon weapon = null;
		m_passiveWeapons.TryGetValue(refId, out weapon);

		return weapon;
	}

	public Weapon GetActiveSkillWeapon(int refId)
	{
		Weapon weapon = null;
		m_activeWeapons.TryGetValue(refId, out weapon);
		
		return weapon;
	}

	public void StartFiring(float targetAngle)
	{

		if (m_multipleWeapon == false)
		{

			m_weapons[m_curWeaponIndex].StartFiring(targetAngle);
		}
		else
		{
			foreach(Weapon weapon in m_weapons)
			{
				weapon.StartFiring(targetAngle);
			}
		}

		m_firing = true;
	}

	public void StopFiring()
	{
		foreach(Weapon weapon in m_weapons)
		{
			weapon.StopFiring();
		}

		m_firing = false;
	}

	public void ActiveWeaponSkillFire(int refId, float targetAngle)
	{
		Weapon weapon = GetActiveSkillWeapon(refId);
		if (weapon != null)
			weapon.StartFiring(targetAngle);
	}


	public void MoreFire()
	{
		foreach(Weapon weapon in m_weapons)
		{
			weapon.MoreFire();
		}
	}

	public void LevelUp(int refWeaponID)
	{
		foreach(Weapon weapon in m_weapons)
		{
			if (refWeaponID > 0 && refWeaponID == weapon.RefItem.id)
				weapon.LevelUp();
		}
	}

	public float AttackRange()
	{
		if (m_weapons.Count == 0)
			return 0f;

		return m_weapons[m_curWeaponIndex].WeaponStat.range + m_creature.m_creatureProperty.AttackRange;
	}

	public Weapon MainWeapon
	{
		get {
			if (m_weapons.Count == 0)
				return null;

			return m_weapons[0];
		}
	}

	public List<Weapon> Weapons
	{
		get{
			return m_weapons;
		}
	}

}
