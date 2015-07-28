using UnityEngine;

public class Cheat {

	public static bool EnableAbilityRollButton
	{
		get{return Warehouse.Instance.FindItem(Const.EngineeringBayRefItemId).Item.Level >= 1;}
	}



	public static int HowManyAbilityPointOnStart
	{
		get{
			if (Warehouse.Instance.FindItem(Const.EngineeringBayRefItemId).Item.Level >= 9)
				return 9;
			else if (Warehouse.Instance.FindItem(Const.EngineeringBayRefItemId).Item.Level >= 7)
				return 6;
			else if (Warehouse.Instance.FindItem(Const.EngineeringBayRefItemId).Item.Level >= 5)
				return 3;

			return 0;
		}
	}

	public static int HowManyAccessorySlot
	{
		get{
			if (Warehouse.Instance.FindItem(Const.EngineeringBayRefItemId).Item.Level >= 3)
				return Const.AccessoriesSlots;

			return Const.HalfAccessoriesSlots;
		}
	}

	public static int HowManyAbilityPointRatioOnLevelUp
	{
		get{

			return 1;
		}
	}

	public static bool PlayWithPet
	{
		//get{return Warehouse.Instance.FindItem(Const.AcademyRefItemId).Item.Level >= 1;}
		get{return true;}
	}

	public static bool AutoAssignedAbility
	{
		get{return Warehouse.Instance.FindItem(Const.AcademyRefItemId).Item.Level >= 5;}
	}

	public static bool AutoMove
	{
		get{return Warehouse.Instance.FindItem(Const.AcademyRefItemId).Item.Level >= 9;}
	}
}
