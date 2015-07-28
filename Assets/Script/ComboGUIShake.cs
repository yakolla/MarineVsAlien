using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ComboGUIShake : MonoBehaviour
{
	// How long the object should shake for.
	public float shake = 0f;

	[SerializeField]
	float decreaseFactor = 10.0f;
	
	Vector3 originalPos;

	Text	m_killComboGUI;

	void Start()
	{
		m_killComboGUI = GetComponent<Text>();
	}
	
	void OnEnable()
	{
		originalPos = transform.localPosition;
	}
	
	void Update()
	{
		if (shake > 1)
		{
			Vector3 scale = transform.localScale;
			scale.y = shake;
			transform.localScale = scale;

			shake -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shake = 0f;
			transform.localScale = Vector3.one;
			this.enabled = false;
		}
	}

	public string Text
	{
		get{return m_killComboGUI.text;}
		set{m_killComboGUI.text = value;}
	}
}