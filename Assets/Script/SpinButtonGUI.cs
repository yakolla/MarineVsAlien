using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpinButtonGUI : MonoBehaviour {


	[SerializeField]
	UnityEngine.Events.UnityEvent	m_onStop;


	public void StopSpin()
	{
		this.gameObject.GetComponent<Animator>().SetBool("Spin", false);

		m_onStop.Invoke();
	}

	public bool IsSpining()
	{
		return gameObject.GetComponent<Animator>().GetBool("Spin");
	}
}
