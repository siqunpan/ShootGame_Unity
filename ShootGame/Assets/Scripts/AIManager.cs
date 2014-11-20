using UnityEngine;
using System.Collections;


public class AIManager : MonoBehaviour {

	public Vector2 v2_speedAI = new Vector2(-0.01f, 0.01f);

	private EnemyWeapon enemyWeapon;
	private float f_shotRate = -1f;
	private float f_timerShot = 0;
	private Vector2 v2_newPosition = Vector2.zero;
	// Use this for initialization
	void Start () {
		enemyWeapon = this.gameObject.GetComponent<EnemyWeapon>();
		f_shotRate = enemyWeapon.f_shotRate;
	}
	
	// Update is called once per frame
	void Update () {

		if(canMoveAI())
		{
			this.gameObject.transform.Translate(new Vector3 (v2_speedAI.x, v2_speedAI.y * Random.Range(-1f, 1f), 0f));
		}

		if (canShotAI()) 
		{
			enemyWeapon.AttackAI();
		}
	}

	private bool canShotAI()
	{
		bool b_canshotAI = false;

		f_timerShot += Time.deltaTime;

		if(f_timerShot >= 1f/f_shotRate)
		{
			f_timerShot = 0f;
			b_canshotAI = true;
		}
		else
		{
			b_canshotAI = false;
		}

		return b_canshotAI;
	}

	public bool canMoveAI()
	{
		bool b_canMove = false;

		b_canMove = true;

		return b_canMove;
	}
}
