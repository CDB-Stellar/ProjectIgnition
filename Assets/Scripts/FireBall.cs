using UnityEngine;

public class FireBall : MonoBehaviour
{
    private Rigidbody2D _rbody;
    private Animator _anim;
    
    [SerializeField] private float _decayRate;
    [SerializeField] private float _decayAmount;
    
    private bool _isLaunched;
    private float _size;
    private float _decayTimer;

    private void Start()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        // Subsribe Events       
        GameEvents.current.onLaunchFireBall += LaunchFireBall;
    }
    private void FixedUpdate()
    {
        if (_size <= 0)
            Destroy(gameObject);
        if (_isLaunched)
            Decay();
    }
    public float GetDamage()
    {
        return _size;
    }
    private void LaunchFireBall(Vector3 trajectory)
    {
        if (!_isLaunched)
        {
            transform.parent = null;
            _rbody.simulated = true;
            _isLaunched = true;
            _rbody.velocity = trajectory;
        }
    }
    public void SetSize(float size)
    {
        _size = size;
        _anim.SetFloat("fireballSize", _size);
    }
    public bool FullyGrown()
    {
        if (_size >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Decay()
    {
        if (_decayTimer > _decayRate)
        {
            _anim.SetFloat("fireballSize", _anim.GetFloat("fireballSize") - _decayAmount);
            _size -= _decayAmount;
            _decayTimer = 0.0f;
        }
        _decayTimer += Time.deltaTime;
    }
    private void Explode()
    {
        GameEvents.current.ApplyForceToPlayer(transform.position, _size);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_isLaunched)
        {
            Explode();
            Destroy(gameObject);
        }

    }

}
