using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemCheatData : ItemData{

	public ItemCheatData(int refItemId) : base(refItemId, 1)
	{
	}

	override public void Pickup(Creature obj)
	{
		base.Pickup(obj);
		Use (obj);
	}

	override public void Use(Creature obj)
	{

	}

	override public void NoUse(Creature obj)
	{
	}
	
	override public string Description()
	{
		string desc = base.Description();

		int level = Lock == true ? 0 : Level;

		switch(RefItemID)
		{
		case Const.EngineeringBayRefItemId:
			desc +=  "\n" + (level >= 1 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv1:Enable Roll Button of Ability</color>";
			desc +=  "\n" + (level >= 3 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv3:Unlock all follower slots</color>";
			desc +=  "\n" + (level >= 5 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv5:On starting, Give 3 Ability points</color>";
			desc +=  "\n" + (level >= 7 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv7:On starting, Give 6 Ability points</color>";
			desc +=  "\n" + (level >= 9 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv9:On starting, Give 9 Ability points</color>";
			break;
		case Const.AcademyRefItemId:
			desc +=  "\n" + (level >= 1 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv1:Play with a Pet</color>";
			desc +=  "\n" + (level >= 5 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv5:Auto assigned to the ability</color>";
			desc +=  "\n" + (level >= 9 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv9:Auto Move</color>";
			break;
		}


			
		return desc;
	}

}
