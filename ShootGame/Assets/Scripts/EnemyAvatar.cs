using UnityEngine;
using System.Collections;

public class EnemyAvatar : BaseAvatar {

	// Use this for initialization
	void Start () {
		f_health = GlobalVariables.F_FULL_HEALTH_ENEMY;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Projectile") 
		{
			if(other.gameObject.GetComponent<Bullet>().b_fromEnnemy
			   ^ b_isEnemy)
			{
				f_health -= other.gameObject.GetComponent<Bullet> ().i_damage;
				Destroy(other.gameObject);
			}
		}
		else if(other.gameObject.tag == "Player"
		   && other.gameObject.GetComponent<BaseAvatar>().b_isEnemy ^ b_isEnemy)
		{
			f_health = 0f;
			Destroy(this.gameObject);
		}

		if(f_health <= 0f)
		{
			Destroy(this.gameObject);
		}
	}
}
