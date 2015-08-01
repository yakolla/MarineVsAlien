using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemFollowerData : ItemData{

	public int		m_refMobId;

	[JsonIgnore]
	public Follower	m_follower;

	[JsonConstructor]
	private ItemFollowerData()
	{
	}

	public ItemFollowerData(int refMobId) : base(refMobId-30001+1001, 1)
	{
		RefMob refMob = RefData.Instance.RefMobs[refMobId];
		m_refMobId = refMobId;
	}

	override public void Equip(Creature obj)
	{
		m_follower = InstanceFollower(obj);
	}

	override public void Use(Creature obj)
	{
		ApplyOptions(m_follower, Level);
	}

	Follower	InstanceFollower(Creature obj)
	{
		RefMob refMob = RefData.Instance.RefMobs[m_refMobId];
		
		Vector3 enemyPos = obj.transform.position;
		float angle = Random.Range(-3.14f, 3.14f);
		enemyPos.x += Mathf.Cos(angle) * 1f;
		enemyPos.z += Mathf.Sin(angle) * 1f;
		GameObject followerObj = Creature.InstanceCreature(Resources.Load<GameObject>("Pref/mon/"+refMob.prefHead), Resources.Load<GameObject>("Pref/mon_skin/" + refMob.prefBody), enemyPos, obj.transform.rotation);
		followerObj.transform.localScale = new Vector3(refMob.scale, refMob.scale, refMob.scale);

		Follower follower = (Follower)followerObj.GetComponent<Follower>();
		follower.Init(obj, refMob, Level);
		
		foreach(RefMob.WeaponDesc weaponDesc in refMob.refWeaponItems)
		{
			ItemWeaponData itemWeaponData = new ItemWeaponData(weaponDesc.refItemId);
			itemWeaponData.Level = Level;
			follower.EquipWeapon(itemWeaponData, weaponDesc.weaponStat);
		}
		
		ApplyOptions(follower, 0);

		return follower;
	}

	override public void NoUse(Creature obj)
	{
	}
	
	override public string Description()
	{
		return "<color=white>" + RefData.Instance.RefMobs[m_refMobId].name + "</color>" + "\n" +  base.Description();
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Follower)
			return false;

		ItemFollowerData itemFollowerData = item as ItemFollowerData;
		return m_refMobId == itemFollowerData.m_refMobId;
	}

}
