using UnityEngine;
using System;
using System.Collections;

[Serializable]
public enum Enum_moveMode
{
	Non,
	NormalMove,
	DoubleClique
}

[Serializable]
public enum Enum_shotMode
{
	Non,
	Normal,
	DeuxLine,
	TestMode,
	Spiral
}

public class InputController : MonoBehaviour
{
	private bool b_isEnemyInputController = false;

	private string s_currentMoveDirection = null;
	private string s_latestMoveDirection = null;
	private Enum_moveMode moveOrder = Enum_moveMode.Non;

	private float f_timerLatestDirectionInput = -1f;
	private float f_timerCurrentDirectionInput = -1f;
	public float f_durationDoubleClick = 0.5f;

	private MoveEngine moveEngine = null;
	private Vector2 v2_speed = Vector2.zero;

	private Weapon weapon = null;
	private float f_shotRate = 0.25f;
	private float f_timeLatestShot = -1f;
	private float f_timeCurrentShot = -1f;
	private MyButton newButton = new MyButton();
	private bool b_isCombo = false;

	private ComboManager combo;

	PlayerAvatar player = null;

	// Use this for initialization
	void Start ()
	{
		player = this.gameObject.GetComponent<PlayerAvatar>();
		b_isEnemyInputController = player.b_isEnemy;

		moveEngine = this.GetComponent<MoveEngine> ();

		weapon = this.gameObject.GetComponent<Weapon>();
		f_shotRate = weapon.f_shotRate;

		combo = this.gameObject.GetComponent<ComboManager>();
		combo.initComboManager ();
	}

	// Update is called once per frame
	void Update ()
	{
		if(player.getIdMyPlayerInGame() == player.getIdThisPlayerInGame()
		   || (!Network.isClient && !Network.isServer))
		{
			v2_speed.x = Input.GetAxis ("Horizontal");
			v2_speed.y = Input.GetAxis ("Vertical");
			moveEngine.Speed = v2_speed;
			v2_speed = Vector2.zero;

			if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0)
			{
				moveOrder = Enum_moveMode.NormalMove;		
			}

			if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
			{
				newButton.s_buttonName = "Up";
				newButton.f_timeClick = Time.time;

				b_isCombo = combo.onClick(newButton);

				if(b_isCombo == false)
				{
					s_currentMoveDirection = "Up";
				}
			}
			else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
			{
				s_currentMoveDirection = "Down";
			}
			else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
			{
				s_currentMoveDirection = "Left";
			}
			else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
			{
				s_currentMoveDirection = "Right";
			}

			if (s_currentMoveDirection != null) 
			{
				f_timerCurrentDirectionInput = Time.time;
				
				if(f_timerLatestDirectionInput == -1f)
				{
					f_timerLatestDirectionInput = f_timerCurrentDirectionInput;
				}
			}

			if (s_currentMoveDirection != null 
			    && s_latestMoveDirection != null
			    && s_latestMoveDirection == s_currentMoveDirection
			    && (f_timerCurrentDirectionInput - f_timerLatestDirectionInput) <= f_durationDoubleClick) 
			{
				moveOrder = Enum_moveMode.DoubleClique;
				s_latestMoveDirection = null;
			}

			if(moveEngine.Speed != Vector2.zero)
			{
				moveEngine.doOrderMove (moveOrder);
			}

			f_timerLatestDirectionInput = f_timerCurrentDirectionInput;
			if (s_currentMoveDirection != null && moveOrder != Enum_moveMode.DoubleClique) 
			{
				s_latestMoveDirection = s_currentMoveDirection;
			}
			s_currentMoveDirection = null;

			Enum_shotMode shotMode = Enum_shotMode.Non;

	//		if(Input.GetAxis("Fire1") != 0f)
	//		{
	//			shotMode = Enum_shotMode.Normal;
	//		}
	//		else if(Input.GetAxis("Fire2") != 0f)
	//		{
	//			shotMode = Enum_shotMode.DeuxLine;
	//
	//		}
	//		else if(Input.GetAxis("Fire3") == 1f)
	//		{
	//			shotMode = Enum_shotMode.TestMode;
	//		}

			if (Input.GetKey (KeyCode.J)) 
			{
				shotMode = Enum_shotMode.Normal;
			}
			else if(Input.GetKey(KeyCode.K))
			{
				shotMode = Enum_shotMode.DeuxLine;
			}
			else if(Input.GetKey(KeyCode.L))
			{
				shotMode = Enum_shotMode.Spiral;
			}
			else
			{
				shotMode = Enum_shotMode.Non;
			}

			if(shotMode != Enum_shotMode.Non && canShot(shotMode, b_isEnemyInputController))
			{
				weapon.Attack(shotMode, b_isEnemyInputController);
			}
		}
	}

	private bool canShot(Enum_shotMode shotMode, bool _b_isEnemy)
	{
		bool b_canshot = false;

		f_timeCurrentShot = Time.time;

		if(shotMode == Enum_shotMode.Normal)
		{
			if (f_timeLatestShot == -1f) 
			{
				f_timeLatestShot = f_timeCurrentShot;
				b_canshot = true;
			}
			else
			{
				if((f_timeCurrentShot - f_timeLatestShot) >= 1f/f_shotRate)
				{
					f_timeLatestShot = f_timeCurrentShot;
					b_canshot = true;
				}
				else
				{
					b_canshot = false;
				}
			}
		}
		else if(shotMode == Enum_shotMode.DeuxLine)
		{
			if (f_timeLatestShot == -1f) 
			{
				f_timeLatestShot = f_timeCurrentShot;
				b_canshot = true;
			}
			else
			{
				if((f_timeCurrentShot - f_timeLatestShot) >= 1f/f_shotRate)
				{
					f_timeLatestShot = f_timeCurrentShot;
					b_canshot = true;
				}
				else
				{
					b_canshot = false;
				}
			}
		}
		else if(shotMode == Enum_shotMode.TestMode)
		{
			if (f_timeLatestShot == -1f) 
			{
				f_timeLatestShot = f_timeCurrentShot;
				b_canshot = true;
			}
			else
			{
				if((f_timeCurrentShot - f_timeLatestShot) >= 1f/f_shotRate)
				{
					f_timeLatestShot = f_timeCurrentShot;
					b_canshot = true;
				}
				else
				{
					b_canshot = false;
				}
			}
		}
		else if(shotMode == Enum_shotMode.Spiral)
		{
			if (f_timeLatestShot == -1f) 
			{
				f_timeLatestShot = f_timeCurrentShot;
				b_canshot = true;
			}
			else
			{
				if((f_timeCurrentShot - f_timeLatestShot) >= 1f/f_shotRate)
				{
					f_timeLatestShot = f_timeCurrentShot;
					b_canshot = true;
				}
				else
				{
					b_canshot = false;
				}
			}
		}
		else
		{
			b_canshot = false;	
		}

		return b_canshot;
	}	
}












