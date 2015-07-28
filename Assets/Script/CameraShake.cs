using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	// How long the object should shake for.
	public float shake = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	[SerializeField]
	float shakeAmount = 0.7f;

	[SerializeField]
	float decreaseFactor = 1.0f;
	
	Vector3 originalPos;
	
	void OnEnable()
	{
		originalPos = transform.localPosition;
	}
	
	void Update()
	{
		if (shake > 0)
		{
			transform.localPosition += Random.insideUnitSphere * shakeAmount;
			
			shake -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shake = 0f;
			//transform.localPosition = originalPos;
			this.enabled = false;
		}
	}
}