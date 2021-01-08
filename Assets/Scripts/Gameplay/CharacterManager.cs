using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private CollisionEventLogic _footColliderLogic;
    [SerializeField]
    private CollisionEventLogic _frontColliderLogic;
    [SerializeField]
    private Collider2D _headBumperCollider;
    [Header("Physics")]
    [SerializeField]
    private float _moveSpeed = 3.5f;
    [SerializeField]
    private float _jumpSpeed = 7.5f;
    [SerializeField]
    private float _wallSlideSpeed = -1.0f;

    public bool IsGrounded { get; private set; }
    public bool IsFacingWall { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsFalling { get; private set; }

    private Rigidbody2D _rigidbody2D;
    private bool _mustJump = false;
    private Vector2 _speed = Vector2.zero;

    private ScenarioController _sceneController;
    private static string _groundedAnimKey = "Grounded";
    private static string _runAnimKey = "Run";
    private static string _jumpAnimKey = "Jump";
    private static string _wallSlideAnimKey = "WallSlide";
    private static string _fallAnimKey = "Fall";
    private static string _deathAnimKey = "Dead";

    public void Initialize(ScenarioController controller)
    {
        _sceneController = controller;

        var footData = new CollisionEventData
        {
            CollisionEnterAction = OnFootCollisionEnter,
            CollisionExitAction = OnFootCollisionExit
        };
        _footColliderLogic.Initialize(footData);
        var frontData = new CollisionEventData
        {
            CollisionEnterAction = OnFrontCollisionEnter,
            CollisionExitAction = OnFrontCollisionExit
        };
        _frontColliderLogic.Initialize(frontData);

        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void StartLevel()
    {
        _animator.SetBool(_groundedAnimKey, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Coin"))
        {
            Debug.Log(other);
            OnCoinCollected(other.transform.parent.GetComponent<CoinObjectLogic>());
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Time"))
        {
            Debug.Log(other);
            var timeLogic = other.transform.parent.GetComponent<TimeObjectLogic>();
            timeLogic.OnCollected();
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            Debug.Log("TRAP: " + other);
            SetAnimations(false, false, false, false);
            _animator.SetBool(_deathAnimKey, true);
            _sceneController.OnDeath();
            return;
        }
    }

    private void OnCoinCollected(CoinObjectLogic coinLogic)
    {
        //Debug.Log(coinLogic.gameObject.name);
        var coinsCollected = coinLogic.OnCollected(_sceneController.LevelCoins);
        _sceneController.OnCoinCollected(coinsCollected);
    }

    private void OnFootCollisionEnter(Transform t)
    {
        IsGrounded = true;
        IsFalling = false;
    }

    private void OnFootCollisionExit(Transform t)
    {
        IsGrounded = false;
        if (!IsJumping)
        {
            IsFalling = true;
        }
    }

    private void OnFrontCollisionEnter(Transform t)
    {
        IsFacingWall = true;
    }

    private void OnFrontCollisionExit(Transform t)
    {
        IsFacingWall = false;
    }

    public void OnTapDown()
    {
        if (!_sceneController.LevelStarted)
        {
            return;
        }

        if (IsGrounded)
        {
            // simple jump
            Debug.Log("Jump");
            _mustJump = true;
            return;
        }
        else
        {
            if (IsFacingWall)
            {
                // rotate and jump
                transform.right *= -1.0f;
                IsFalling = false;
                _mustJump = true;
            }
        }        
    }

    private void FixedUpdate()
    {
        if (!_sceneController.LevelStarted)
        {
            return;
        }

        var _speed = _rigidbody2D.velocity;
        if (!IsFacingWall)
        {
            _speed.x = transform.right.x * _moveSpeed;
        }

        if (_mustJump)
        {
            //_rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _speed.y = _jumpSpeed;
            IsJumping = true;
            _headBumperCollider.enabled = true;
            _animator.SetBool(_jumpAnimKey, true);
        }

        if (IsJumping && _speed.y < 0.0f)
        {
            IsJumping = false;
            _headBumperCollider.enabled = false;
            IsFalling = true;
        }

        if (IsFalling && IsFacingWall)
        {
            _speed.y = _wallSlideSpeed;            
        }

        SetAnimations(!IsFacingWall, IsJumping, IsFalling && IsFacingWall, IsFalling);

        _mustJump = false;
        _rigidbody2D.velocity = _speed;
    }

    private void SetAnimations(bool run, bool jump, bool wallSlide, bool falling)
    {
        _animator.SetBool(_jumpAnimKey, jump);
        _animator.SetBool(_wallSlideAnimKey, wallSlide);
        _animator.SetBool(_fallAnimKey, falling);
        _animator.SetBool(_groundedAnimKey, IsGrounded);
        _animator.SetBool(_runAnimKey, run && IsGrounded);
    }
}
