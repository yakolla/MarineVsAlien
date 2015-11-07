using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemFollowerData : ItemData{

	[JsonIgnore]
	public Follower	m_follower;

	[JsonConstructor]
	private ItemFollowerData()
	{
	}

	public ItemFollowerData(int refItemId) : base(refItemId, 1)
	{

	}

	override public void Equip(Creature obj)
	{
		m_follower = InstanceFollower(obj);
		base.Equip(m_follower);
	}

	override public bool Use(Creature obj)
	{
		ApplyOptions(m_follower, true);
		return true;
	}

	Follower	InstanceFollower(Creature obj)
	{
		RefMob refMob = RefData.Instance.RefMobs[RefItem.followerId];
		
		Vector3 enemyPos = obj.transform.position;
		float angle = Random.Range(-3.14f, 3.14f);
		enemyPos.x += Mathf.Cos(angle) * 1f;
		enemyPos.z += Mathf.Sin(angle) * 1f;
		GameObject followerObj = Creature.InstanceCreature(Resources.Load<GameObject>("Pref/mon/"+refMob.prefHead), Resources.Load<GameObject>("Pref/mon_skin/" + refMob.prefBody), enemyPos, obj.transform.rotation);
		followerObj.transform.localScale = new Vector3(refMob.scale, refMob.scale, refMob.scale);

		Follower follower = (Follower)followerObj.GetComponent<Follower>();
		follower.Init(obj, refMob, RefItem, Level);
		
		foreach(RefMob.WeaponDesc weaponDesc in refMob.refWeaponItems)
		{
			ItemWeaponData itemWeaponData = new ItemWeaponData(weaponDesc.refItemId);
			itemWeaponData.Level = Level;
			itemWeaponData.Evolution = Evolution;
			weaponDesc.maxLevel = RefItem.maxLevel;
			follower.EquipWeapon(itemWeaponData, weaponDesc);
		}

		return follower;
	}

	override public void NoUse(Creature obj)
	{
	}
	
	override protected string itemName()
	{
		return RefData.Instance.RefTexts(RefData.Instance.RefMobs[RefItem.followerId].name);
	}

	override public string Description()
	{
		string desc = base.Description();

		if (m_follower != null)
			desc += RefData.Instance.RefTexts(MultiLang.ID.Damage) + ":" + m_follower.WeaponHolder.MainWeapon.Damage;

		return desc;
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Follower)
			return false;

		ItemFollowerData itemFollowerData = item as ItemFollowerData;
		return RefItem.followerId == itemFollowerData.RefItem.followerId;
	}

}
