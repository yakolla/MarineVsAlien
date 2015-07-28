using UnityEngine;
using System.Collections;

public class Dungeon : MonoBehaviour {

	[SerializeField]
	int				m_dungeonId;

	RefWorldMap		m_refWorldMap;


	// Use this for initialization
	void Start () {	
		
		m_refWorldMap = RefData.Instance.RefWorldMaps[m_dungeonId];


	}

	public int DungeonId
	{
		get{return m_dungeonId;}
	}
}
