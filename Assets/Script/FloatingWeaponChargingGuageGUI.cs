using UnityEngine;
using System.Collections;

public class FloatingWeaponChargingGuageGUI : FloatingGuageBarGUI {

	override protected float guageRemainRatio()
	{
		return 1f;
	}
}

