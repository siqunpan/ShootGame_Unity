using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class MyButton
{
	public string s_buttonName;
	public float f_timeClick;
}

public class ComboManager : MonoBehaviour {

	public delegate bool newButtonHandler (MyButton myButton);
	public event newButtonHandler onNewButtonClick;

	private List<MyButton> list_buttonStream = new List<MyButton>();

//	private List<string> list_buttonStream = new List<string>();
	
//	Dictionary<string, float> dic_buttonStream = new Dictionary<string, float>();

	public void initComboManager()
	{
		onNewButtonClick += testCombo;
	}

	public bool testCombo(MyButton _newButton)
	{
		return false;
	}

	public bool onClick(MyButton _newButton)
	{
		bool b_result;

		if(onNewButtonClick != null)
		{
			b_result = onNewButtonClick(_newButton);
		}
		else
		{
			b_result = false;
		}

		return b_result;
	}

	public void addCurrentButtonToStream(string _s_button)
	{
//		list_buttonStream.Add(_s_button);
	}
	
	public void clearButtonStream()
	{
		
	}
	
	// Use this for initialization
	void Start () {
		
	}
}
