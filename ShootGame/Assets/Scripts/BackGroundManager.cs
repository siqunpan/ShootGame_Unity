using UnityEngine;
using System.Collections;

public class BackGroundManager : MonoBehaviour {

	public Transform t_backgrounds;
	public Transform t_background1;
	public Transform t_background2;

	public Transform t_platforms;
	public Transform t_platform1_1;
	public Transform t_platform1_2;

	public float f_scrollSpeedBackground = 0f;
	public float f_scrollSpeedPlatForm = 0f;

	private float f_distanceTwoBackgrounds = 0f;
	private float f_distanceTwoPlatforms = 0f;

	private Vector3 v3_startPositionBackgounds = Vector3.zero;
	private Vector3 v3_startPositionPlatforms = Vector3.zero;

	// Use this for initialization
	void Start () {
		f_distanceTwoBackgrounds = t_background1.position.x - t_background2.position.x;
		f_distanceTwoPlatforms = t_platform1_1.position.x - t_platform1_2.position.x;

		v3_startPositionBackgounds = t_backgrounds.position;
		v3_startPositionPlatforms = t_platforms.position;
	}
	
	// Update is called once per frame
	void Update () {
		float f_newPositionBackground = 
			Mathf.Repeat (Time.time * f_scrollSpeedBackground, f_distanceTwoBackgrounds);
		float f_newPositionPlatform =
			Mathf.Repeat (Time.time * f_scrollSpeedPlatForm, f_distanceTwoPlatforms);

		t_backgrounds.position = v3_startPositionBackgounds - Vector3.right * f_newPositionBackground;
		t_platforms.position = v3_startPositionPlatforms - Vector3.right * f_newPositionPlatform;
	}
}
