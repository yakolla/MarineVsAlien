using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	[SerializeField]
	ItemData			m_item;


	[SerializeField]
	float			m_lifeTime = 15f;
	float			m_timeToDeath = 0;

	Bezier			m_bezier;
	Parabola		m_parabola;
	Creature		m_target = null;

	[SerializeField]
	GameObject		m_prefPickupItemEffect;

	BoxCollider		m_collider;

	System.Action<Creature> m_callbackOnPickup;

	void Start () {
		m_collider = GetComponent<BoxCollider>();
		m_parabola = new Parabola(gameObject, Random.Range(5f, 8f), Random.Range(-3.14f, 3.14f), Random.Range(1.3f, 1.57f), 2);
		m_parabola.GroundY = 0.5f;
		m_parabola.TimeScale = 1.5f;
		LifeTime = m_lifeTime;
	}

	void SetTarget(Creature target)
	{
		m_target = target;
		Vector3 handle1 = transform.position;
		handle1 += (transform.forward+transform.up)*(Vector3.Distance(transform.position, target.transform.position)*0.5f);
		Vector3 handle2 = target.transform.position;
		handle2.y = 3f;
		m_bezier = new Bezier(gameObject, target.gameObject, handle1, handle2, 0.06f);
	}

	public float LifeTime
	{
		set {m_timeToDeath = Time.time + m_lifeTime;}
	}

	public void StartPickupEffect(Creature obj)
	{
		if (obj == null)
			return;

		m_collider.enabled = false;

		SetTarget(obj);

	}

	void Update()
	{

		if (m_parabola.Update() == false)
		{
			if (Time.time > m_timeToDeath)
				Death();
		}

		if (m_bezier != null && m_bezier.Update() == false)
		{
			if (m_target != null)
			{
				m_target.ApplyPickUpItemEffect(ItemType, m_prefPickupItemEffect, Item.Count);

				m_item.Pickup(m_target);
				if (m_callbackOnPickup != null)
				{
					m_callbackOnPickup(m_target);
				}
			}

			Death();
		}

		transform.forward = Camera.main.transform.forward;
	}

	void Death()
	{
		DestroyObject(this.gameObject);
	}

	public ItemData.Type ItemType
	{
		get { return m_item.RefItem.type; }
	}

	public ItemData Item
	{
		get { return m_item; }
		set {
			m_item = value;

			m_prefPickupItemEffect = Const.GetPrefItemEatEffect(m_item.RefItem);
		}
	}

	public System.Action<Creature> PickupCallback
	{
		set { m_callbackOnPickup = value;}
	}

}
