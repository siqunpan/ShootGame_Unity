using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Collections;

[XmlRoot("LevelDescription")]
[XmlType("LevelDescription")]
public class LevelDescription{

	[XmlAttribute]
	public float Duration
	{
		get;
		set;
	}

	[XmlElement("EnemyDescription", typeof(EnemyDescription))]
	public EnemyDescription[] Enemies
	{
		get;
		private set;
	}
		
	[XmlAttribute]
	public string Name
	{
		get;
		set;
	}
	
}
