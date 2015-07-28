using UnityEngine;
using System.Collections;

public class FloatingHealthBarGUI : FloatingGuageBarGUI {

	override protected float guageRemainRatio()
	{
		return m_creature.m_creatureProperty.getHPRemainRatio();
	}
}

