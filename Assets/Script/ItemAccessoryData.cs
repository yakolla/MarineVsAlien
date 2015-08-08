using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemAccessoryData : ItemData{


	public ItemAccessoryData(int refItemId) : base(refItemId, 1)
	{

	}


	override public void NoUse(Creature obj)
	{

	}

}
