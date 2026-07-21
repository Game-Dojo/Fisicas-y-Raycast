using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private float jumpForce = 2.0f;
    [SerializeField] private LayerMask groundMask;

    [Header("Animation")]
    [SerializeField] private Sprite idleGunUp;
    [SerializeField] private Sprite idleGunDown;

    private Sprite _idleOriginal;

    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;
    private Animator _animator;

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
                _animator.SetBool("Run", true);
                _isRunning = true;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
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
                    _renderer.sprite = idleGunUp;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                _animator.SetInteger("YDirection", -1);

                if (_isRunning == false)
                {
                    print("Key Down");
                    _renderer.sprite = idleGunDown;
                }
            }

            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
                _animator.SetInteger("YDirection", 0);

                if (_isRunning == false)
                {
                    _renderer.sprite = _idleOriginal;
                }
            }

            /*if (Input.GetKeyDown(KeyCode.W) && _isGrounded && !_isJumping)
            {
                _canJump = true;
                _animator.SetTrigger("Jump");
                _isJumping = true;
            }*/

            var origin = transform.position - new Vector3(0, -0.1f, 0);
            _isGrounded = Physics2D.Raycast(origin, Vector3.down, 0.2f, groundMask);

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

    public void OnLifeLost()
    {
        _isDead = true;
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
}
