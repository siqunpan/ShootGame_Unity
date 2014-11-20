using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public int i_damage = 0;
	public bool b_fromEnnemy = false;
	
	public Vector2 v2_speedNormal = Vector2.zero;
	
	private Vector2 v2_newPosition = Vector2.zero;
	private Vector2 v2_iniPosition = Vector2.zero;

	// Use this for initialization
	void Start () {
		if (this.gameObject.tag == "Player") 
		{
			v2_iniPosition = new Vector2 (0f, Random.Range (-5f, 5f));
			this.transform.position = v2_iniPosition;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (this.gameObject.GetComponent<SpriteRenderer> ().isVisible == false) 
		{
			Destroy(this.gameObject);		
		}

		this.gameObject.transform.Translate (v2_speedNormal.x * Time.deltaTime, v2_speedNormal.y * Time.deltaTime, 0f);
//		v2_newPosition.x = this.transform.position.x + v2_speed.x * Time.deltaTime;
//		v2_newPosition.y = this.transform.position.y + v2_speed.y * Time.deltaTime;
//		this.transform.position = v2_newPosition;
	}

//	void OnTriggerEnter2D(Collider2D other)
//	{
//		if (other.gameObject.tag == "Player") 
//		{
//			if(other.gameObject.GetComponent<BaseAvatar>().b_isEnemy
//			   ^ b_fromEnnemy)
//			{
//				Destroy(this.gameObject);
//			}
//		}
//		else if(other.gameObject.tag == "Projectile")
//		{
//			if(other.gameObject.GetComponent<Bullet>().b_fromEnnemy
//			   ^ b_fromEnnemy)
//			{
//				Destroy(this.gameObject);
//			}
//		}
//	}
}
