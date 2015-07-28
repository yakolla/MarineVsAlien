using UnityEngine;
using System.Collections;

public class BillBoard : MonoBehaviour {
	[SerializeField]
	Renderer _renderer;
	
	[SerializeField]
	bool _lockX = false;
	
	[SerializeField]
	bool _lockY = false;
	
	[SerializeField]
	bool _lockZ = false;
	
	void Awake () {
		if (_renderer == null) {
			_renderer = renderer;
		}
	}

	void Update () {
		Vector3 oldAngles = transform.localEulerAngles;
		transform.LookAt(Camera.main.transform);
		Vector3 newAngles = transform.localEulerAngles;
		if (_lockX) {
			newAngles.x = oldAngles.x;
		}
		if (_lockY) {
			newAngles.y = oldAngles.y;
		}
		if (_lockZ) {
			newAngles.z = oldAngles.z;
		}
		transform.localEulerAngles = newAngles;
	}
	
	public void Show () {
		_renderer.enabled = true;
	}
	
	public void Hide () {
		_renderer.enabled = false;
	}
	
	public void ShowAndHide (float timer) {
		CancelInvoke();
		
		Show();
		Invoke("Hide", timer);
	}
}
