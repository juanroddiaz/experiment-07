using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private TriggerEventLogic _footColliderLogic;
    [Header("Physics")]
    [SerializeField]
    private float _moveSpeed = 3.5f;
    [SerializeField]
    private float _jumpSpeed = 7.5f;
    [SerializeField]
    private float _jumpBoostFactor = 1.5f;
    [SerializeField]
    private float _wallSlideSpeed = -1.0f;

    public bool IsGrounded { get; private set; }
    public bool IsFacingWall { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsFalling { get; private set; }

    private Rigidbody2D _rigidbody2D;
    private bool _mustJump = false;
    private bool _boostedJump = false;
    private bool _goingLeft = false;
    private bool _goingRight = false;
    private bool _moving = false;
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

        var footData = new TriggerEventData
        {
            TriggerEnterAction = null,
            TriggerExitAction = null,
        };
        _footColliderLogic.Initialize(footData);

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.simulated = false;
        _animator.SetBool(_deathAnimKey, false);
    }

    public void StartLevel()
    {
        _animator.SetBool(_groundedAnimKey, true);
        _rigidbody2D.simulated = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            OnTrapLanding();
        }
    }

    public void OnTrapLanding()
    {
        SetAnimations(false, false, false, false);
        _animator.SetBool(_deathAnimKey, true);
        _rigidbody2D.simulated = false;
        _sceneController.OnDeath();
    }

    public void OnPlatformLanding(bool boosted)
    {
        var _speed = _rigidbody2D.velocity;
        if (_speed.y < 0.0f)
        {
            _mustJump = true;
            _boostedJump = boosted;
            return;
        }
    }

    public void OnSwipe(bool goingRight)
    {
        if (!_sceneController.LevelStarted)
        {
            return;
        }

        if (goingRight)
        {
            OnRightDown();
            return;
        }

        OnLeftDown();
    }

    public void OnLeftDown()
    {
        Debug.Log("Pressing left!");
        _goingLeft = true;
        _goingRight = false;
        _moving = true;
        transform.right = Vector3.left;
    }

    public void OnRightDown()
    {
        Debug.Log("Pressing right!");
        _goingRight = true;
        _goingLeft = false;
        _moving = true;
        transform.right = Vector3.right;
    }

    public void OnButtonUp()
    {
        Debug.Log("Button Up");
        _goingRight = false;
        _goingLeft = false;
        _moving = false; 
    }


    private void FixedUpdate()
    {
        if (!_sceneController.LevelStarted)
        {
            return;
        }

        var _speed = _rigidbody2D.velocity;
        _speed.x = _moving ? transform.right.x * _moveSpeed : 0.0f;

        if (_mustJump)
        {
            _speed.y = _jumpSpeed;
            if (_boostedJump)
            {
                _speed.y *= _jumpBoostFactor;
            }
            IsJumping = true;
            IsFalling = false;
            _animator.SetBool(_jumpAnimKey, true);
        }

        if (IsJumping && _speed.y < 0.0f)
        {
            IsJumping = false;
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
