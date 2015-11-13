using UnityEngine;
using System.Collections;

public class ItemObject {


	Texture2D			m_icon;
	ItemData			m_item;

	public ItemObject(ItemData item)
	{
		m_item = item;
		m_icon = Resources.Load<Texture2D>(item.RefItem.icon);

	}

	public ItemData Item
	{
		get {return m_item;}
	}

	public Texture2D ItemIcon
	{
		get { return m_icon; }
	}
}
