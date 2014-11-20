using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseCombo : MonoBehaviour {

	private List<string> list_buttonStream = new List<string>();
	
//	Dictionary<string, float> dic_buttonStream = new Dictionary<string, float>();

	void Start()
	{

	}

	public void addCurrentButtonToStream(string _s_button)
	{
		list_buttonStream.Add(_s_button);
	}

	public void clearButtonStream()
	{

	}
}
