using UnityEngine;
using System.Collections;

public class MobAIDummy : MobAI {

	override public void Update () {
		if (TimeEffector.Instance.IsStop() == true)
			return;
	}


}
