using UnityEngine;
using System.Collections;

public class MoveEngine : MonoBehaviour {
	
	public float f_maxSpeed = -1;
	public float MaxSpeed
	{
		get
		{
			return f_maxSpeed;
		}
		set
		{
			f_maxSpeed = value;
		}
	}

	private Vector2 v2_speed;
	public Vector2 Speed
	{
		get
		{
			return v2_speed;
		}
		set
		{
			if(value.x >= 1)
			{
				v2_speed.x = 1;
			}
			else if(value.x <= -1)
			{
				v2_speed.x = -1;
			}
			else
			{
				v2_speed.x = value.x;
			}

			if(value.y >= 1)
			{
				v2_speed.y = 1;
			}
			else if(value.y <= -1)
			{
				v2_speed.y = -1;
			}
			else
			{
				v2_speed.y = value.y;
			}
		}
	}

	private Vector2 v2_position;
	public Vector2 Position
	{
		get
		{
			return this.transform.position;
		}
		set
		{
			this.transform.position = value;

		}
	}

	private Vector2 v2_newPosition;

	private float f_distanceCamera = -1f; 
	private float f_leftBorderCamera = -1f;
	private float f_rightBorderCamera = -1f;
	private float f_upBorderCamera = -1f;
	private float f_downBorderCamera = -1f;
	private float f_leftWorldColliderPlayer = -1f;
	private float f_rightWorldColliderPlayer = -1f;
	private float f_downWorldColliderPlayer = -1f;
	private float f_upWorldColliderPlayer = -1f;
	private Vector2 v2_sizeWorldTriggerColliderPlayer = Vector2.zero;

	void Start()
	{
		f_distanceCamera = (this.transform.position - Camera.main.transform.position).z;
		f_leftBorderCamera = Camera.main.ViewportToWorldPoint (new Vector3 (0f, 0f, f_distanceCamera)).x;
		f_rightBorderCamera = Camera.main.ViewportToWorldPoint (new Vector3 (1f, 0f, f_distanceCamera)).x;
		f_downBorderCamera = Camera.main.ViewportToWorldPoint (new Vector3 (0f, 0f, f_distanceCamera)).y;
		f_upBorderCamera = Camera.main.ViewportToWorldPoint (new Vector3 (0f, 1f, f_distanceCamera)).y;

		BoxCollider2D collider = this.gameObject.GetComponent<BoxCollider2D> ();

		f_leftWorldColliderPlayer = collider.transform.TransformPoint (
										collider.center - new Vector2 (collider.size.x * 0.5f, 0f)).x;
		f_rightWorldColliderPlayer = collider.transform.TransformPoint (
										collider.center + new Vector2 (collider.size.x * 0.5f, 0f)).x;

		f_downWorldColliderPlayer = collider.transform.TransformPoint (
										collider.center - new Vector2 (0f, collider.size.y * 0.5f)).y;

		f_upWorldColliderPlayer = collider.transform.TransformPoint (
										collider.center + new Vector2 (0f, collider.size.y * 0.5f)).y;

		v2_sizeWorldTriggerColliderPlayer = new Vector2 (f_rightWorldColliderPlayer - f_leftWorldColliderPlayer,
		                                                f_upWorldColliderPlayer - f_downWorldColliderPlayer);


		f_leftBorderCamera += v2_sizeWorldTriggerColliderPlayer.x * 0.5f;
		f_rightBorderCamera -= v2_sizeWorldTriggerColliderPlayer.x * 0.5f;
		f_downBorderCamera += v2_sizeWorldTriggerColliderPlayer.y * 0.5f;
		f_upBorderCamera -= v2_sizeWorldTriggerColliderPlayer.y * 0.5f;
	}

	public Vector2 setPositionInCameraView (Vector2 _v2_position)
	{
		_v2_position.x = Mathf.Clamp (_v2_position.x, f_leftBorderCamera, f_rightBorderCamera);
		_v2_position.y = Mathf.Clamp (_v2_position.y, f_downBorderCamera, f_upBorderCamera);

		return _v2_position;
	}

	public void doOrderMove(Enum_moveMode _moveMode)
	{
		Vector3 v3_newPosition = Vector3.zero;

		switch (_moveMode) 
		{
		case Enum_moveMode.NormalMove:
			v2_newPosition = Position + Speed * Time.deltaTime * MaxSpeed;

			v2_newPosition = setPositionInCameraView(v2_newPosition);

			Position = v2_newPosition;

			if(Network.isServer || Network.isClient)
			{
				v3_newPosition.x = v2_newPosition.x;
				v3_newPosition.y = v2_newPosition.y;
				v3_newPosition.z = 0;

				networkView.RPC ("updatePositionThisPlayerRPC", RPCMode.Others, v3_newPosition);
			}

			break;
		case Enum_moveMode.DoubleClique:
//			if(v2_speed.x > 0)
//				v2_newPosition = Position + new Vector2(3, 0);
//			else if(v2_speed.x < 0)
//				v2_newPosition = Position + new Vector2(-3, 0);
//
//			if(v2_speed.y > 0)
//				v2_newPosition += new Vector2(0, 3);
//			else if(v2_speed.y < 0)
//			{
//				v2_newPosition += new Vector2(0, -3);
//			}
//			Position = v2_newPosition;
			break;
		default:
			break;
		}
	}

	[RPC]
	public void updatePositionThisPlayerRPC(Vector3 _v3_newPosition)
	{
		updatePositionThisPlayerLocal (_v3_newPosition);
	}

	public void updatePositionThisPlayerLocal(Vector3 _v3_newPosition)
	{
		Position = _v3_newPosition;
	}
}














