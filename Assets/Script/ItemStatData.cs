using UnityEngine;
using System.Collections;


public class ItemStatData : ItemData{


	public ItemStatData(int refItemId) : base(refItemId, 1)
	{

	}

	override public void Use(Creature obj)
	{	
		ApplyOptions(obj);
	}


	override public void NoUse(Creature obj)
	{

	}

	override public string Description()
	{
		return OptionsDescription();
	}

}
