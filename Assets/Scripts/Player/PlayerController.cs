using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] private float speed = 8.0f;
	[SerializeField] private float jumpForce = 2.0f;
	[SerializeField] private LayerMask groundMask;

	[Header("Laser")]
	[SerializeField] private LineRenderer laser;
	[SerializeField] private Transform laserPositions;

	[Header("Animation")]
	[SerializeField] private Sprite idleGunUp;
	[SerializeField] private Sprite idleGunDown;

	private Sprite _idleOriginal;

	private Rigidbody2D _rb;
	private SpriteRenderer _renderer;
	private Animator _animator;

	private int _currentLaserPosition = 1;

	#region Private members
	private float _horizontalMovement;

	private bool _canJump = false;
	private bool _isGrounded = false;
	private bool _isJumping = false;
	private bool _isFalling = true;
	private bool _isRunning = false;
	private bool _isDead = false;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_renderer = GetComponentInChildren<SpriteRenderer>();
		_animator = GetComponentInChildren<Animator>();

		_idleOriginal = _renderer.sprite;
	}

	private void Update()
	{
		if (!_isDead)
		{
			HandleMovement();

			/*if (Input.GetKeyDown(KeyCode.W) && _isGrounded && !_isJumping)
            {
                _canJump = true;
                _animator.SetTrigger("Jump");
                _isJumping = true;
            }*/

			CheckGround();
			CheckLaserCollision();
			/*
            if (!_isJumping && _isGrounded)
            {
                _isJumping = false;
                _isFalling = false;
                _rb.gravityScale = 1;
            }

            if (_isJumping && _rb.linearVelocityY < 0)
            {
                _isJumping = false;
                _isFalling = true;
                _rb.gravityScale = 2;
            }*/
		}
	}

	private void FixedUpdate()
	{
		_rb.linearVelocityX = _horizontalMovement * speed;

		if (_canJump)
		{
			_rb.linearVelocityY = 0;
			_rb.linearVelocityY = jumpForce;
			_canJump = false;
		}
	}

	private void OnDrawGizmos()
	{
		if (_isGrounded == true)
		{
			Gizmos.color = Color.yellow;
		}
		else
		{
			Gizmos.color = Color.purple;
		}

		//Gizmos.color = (_isGrounded) ? Color.yellow : Color.purple;
		Gizmos.DrawLine(transform.position - new Vector3(0, -0.1f, 0), (transform.position) + (Vector3.down * 0.2f));
	}

	#endregion

	private void HandleMovement()
	{
		_horizontalMovement = Input.GetAxisRaw("Horizontal");

		if (Mathf.Approximately(_horizontalMovement, 0))
		{
			_animator.SetBool("Run", false);
			_animator.SetInteger("YDirection", 0);
			_isRunning = false;
		}

		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
		{
			_renderer.flipX = true;
			laserPositions.transform.localScale = new Vector3(-1, 1, 1);

			var laserPosition = laserPositions.GetChild(_currentLaserPosition).transform;
			laserPosition.right = Vector3.right * -1;

			_animator.SetBool("Run", true);
			_isRunning = true;
		}
		else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			laserPositions.transform.localScale = Vector3.one;

			var laserPosition = laserPositions.GetChild(_currentLaserPosition).transform;
			laserPosition.right = Vector3.right;

			_renderer.flipX = false;
			_animator.SetBool("Run", true);
			_isRunning = true;
		}

		if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
		{
			_canJump = true;
			_animator.SetTrigger("Jump");
			//_isJumping = true;
		}

		if (Input.GetKeyDown(KeyCode.W))
		{
			_animator.SetInteger("YDirection", 1);

			if (_isRunning == false)
			{
				print("Key Up");
				_animator.enabled = false;
				_renderer.sprite = idleGunUp;

				ChangeLaserPosition(0);
			}
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			_animator.SetInteger("YDirection", -1);

			if (_isRunning == false)
			{
				print("Key Down");
				_animator.enabled = false;
				_renderer.sprite = idleGunDown;

				ChangeLaserPosition(2);
			}
		}

		if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
		{
			if (_isRunning == false)
			{
				_renderer.sprite = _idleOriginal;
			}

			_animator.enabled = true;
			_animator.SetInteger("YDirection", 0);

			ChangeLaserPosition(1);
		}
	}

	private void CheckGround()
	{
		var origin = transform.position - new Vector3(0, -0.1f, 0);
		_isGrounded = Physics2D.Raycast(origin, Vector3.down, 0.2f, groundMask);
	}

	private void CheckLaserCollision()
	{
		var laserPosition = laserPositions.GetChild(_currentLaserPosition).transform;
		RaycastHit2D hit = Physics2D.Raycast(laserPosition.position, laserPosition.right, 15f, groundMask);

		if (hit)
			laser.SetPosition(1, laser.transform.InverseTransformPoint(hit.point));
		else
			laser.SetPosition(1, new Vector3((_renderer.flipX == false) ? 15 : -15, 0, 0));

		//Debug.DrawRay(laserPosition.position, laserPosition.right * 15f);
	}

	private void ChangeLaserPosition(int childIndex = 0)
	{
		laser.transform.position = laserPositions.GetChild(childIndex).transform.position;
		laser.transform.right = laserPositions.GetChild(childIndex).transform.right;

		_currentLaserPosition = childIndex;
	}

	public void OnLifeLost()
	{
		_isDead = true;
	}

}
