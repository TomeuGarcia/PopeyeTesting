using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurtleDashing : MonoBehaviour
{
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashDistance;
    [SerializeField] private float timeUntilDash;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float cooldownTime;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _knockbackForce =3;

    private Vector3 _dashDir = Vector3.zero;
    private float _dashTimer = 0f;
    private float _feedbackTimer = 0f;
    private Coroutine dashResetingCoroutine;
    
    private bool _currentlyDashing;
    private bool _dashEnded = false;
    private bool _stunned = false;
    public bool DashEnded => _dashEnded;
    public bool Stunned => _stunned;
    
    [SerializeField] private float smoothing = 0.1f;
    private Quaternion _targetRotation = new Quaternion();
    // Start is called before the first frame update
    void Start()
    {
       // Physics.IgnoreCollision(playerTransform.GetComponent<Collider>(), GetComponent<Collider>());
        ResetTimers();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentlyDashing)
        {
            if (_feedbackTimer < timeUntilDash)
            {
                _targetRotation = Quaternion.LookRotation(playerTransform.position-transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * smoothing);
                
                _feedbackTimer += Time.deltaTime;
            }
            else if (_dashTimer < dashDuration && !_stunned)
            {
                if(_dashDir == Vector3.zero) {_dashDir = (playerTransform.position - transform.position).normalized;}
                PerformDash();

                _dashTimer += Time.deltaTime;
            }
            else if(!_stunned)
            {
                _dashEnded = true;
                _currentlyDashing = false;
                dashResetingCoroutine = StartCoroutine(ResetDash());
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Floor") && !other.gameObject.CompareTag("Anchor"))
        {
            _rigidbody.AddForce(other.contacts[0].normal * _knockbackForce,ForceMode.Impulse);
            _stunned = true;
        }
    }

    public void StartDashing()
    {
        _dashEnded = false;
        _stunned = false;
        _currentlyDashing = true;
        ResetTimers();
        _dashDir = Vector3.zero;
        navMeshAgent.enabled = false;
    }

    private void ResetTimers()
    {
        _dashTimer = 0;
        _feedbackTimer = 0;
    }
    
    void PerformDash()
    {
        transform.position +=  Time.deltaTime * dashDistance * _dashDir ;
    }

    IEnumerator ResetDash()
    {
        yield return new WaitForSeconds(cooldownTime);
        StartDashing();
    }

    public void StopCurrentCoroutines()
    {
        _stunned = false;
        if(dashResetingCoroutine!= null)
        StopCoroutine(dashResetingCoroutine);
    }
}
