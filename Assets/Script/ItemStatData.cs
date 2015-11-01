using UnityEngine;
using System.Collections;


public class ItemStatData : ItemData{


	public ItemStatData(int refItemId) : base(refItemId, 1)
	{

	}

	override public bool Use(Creature obj)
	{	
		ApplyOptions(obj, true);
		return true;
	}


	override public void NoUse(Creature obj)
	{

	}

}
