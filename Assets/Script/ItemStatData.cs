using UnityEngine;
using System.Collections;


public class ItemStatData : ItemData{


	public ItemStatData(int refItemId) : base(refItemId, 1)
	{

	}

	override public void Equip(Creature obj)
	{
		for(int i = 0; i < Level; ++i)
			ApplyOptions(obj, 0);
	}

	override public void Use(Creature obj)
	{	
		ApplyOptions(obj, Level);
	}


	override public void NoUse(Creature obj)
	{
		NoApplyOptions(obj);
	}

	override public string Description()
	{
		return OptionsDescription();
	}

}
