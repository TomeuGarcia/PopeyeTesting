using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] private Transform _anchorTransform;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SpringJoint _springJoint;
    [SerializeField] private LineRenderer _trajectoryLine;
    [SerializeField] private LineRenderer _ownerBinderLine;
    [SerializeField] private Collider _hitTrigger;
    [SerializeField] private SphereCollider _collider;
    [SerializeField] private AnchorDamageDealer _anchorDamageDealer;
    [SerializeField] private LayerMask _obstacleLayers;

    [Header("OWNER")]
    [SerializeField] private Transform _ownerTransform;
    [SerializeField] private Vector3 _grabbedPosition = new Vector3(1.0f, 0.0f, 0.5f);
    [SerializeField] private Vector3 _aimingPosition = new Vector3(1.0f, 0.0f, 0.5f);
    [SerializeField] private Vector3 _launchDirectionOffset = new Vector3(0.0f, 1.0f, 0.0f);

    [Header("PULL")]
    [SerializeField, Range(0.0f, 20.0f)] private float _pulledTowardsOwnerAccelearation = 2.0f;

    [Header("FORCE")]
    [SerializeField] private AnimationCurve _forceCurve;
    [SerializeField, Range(0.0f, 100.0f)] private float _maxForce = 20.0f;
    private Vector3 _velocity;
    private float _throwStrength01;


    public Vector3 Position => _anchorTransform.position;
    private Vector3 _throwStartPosition;

    private bool _canMeleeAttack = true;
    public bool CanMeleeAttack
    {
        get { return _canMeleeAttack; }
        set { _canMeleeAttack = value; }
    }




    private enum AnchorStates
    {
        WithOwner,
        OnAir,
        OnGround
    }

    private AnchorStates _currentState;




    private void Awake()
    {
        InstantReturnToOwner();
        HideTrajectory();

        SetStill();
    }

    private void Update()
    {
        UpdateOwnerBinderLine();

        //DrawTrajectory(_velocity);
        //ShowTrajectory();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsOnGround()) return;

        Vector3 normal = collision.contacts[0].normal;
        if (Vector3.Dot(normal, Vector3.up) < -0.7f)
        {
            _rigidbody.AddForce((Vector3.down-normal).normalized * _rigidbody.velocity.sqrMagnitude, ForceMode.Impulse);
            return;
        }

        SetStill();
        _currentState = AnchorStates.OnGround;

        _anchorDamageDealer.DealGroundHitDamage(Position, _throwStrength01);
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (!IsOnAir()) return;

        _anchorDamageDealer.DealThrowHitDamage(otherCollider.gameObject, Position);
    }

    public void InstantReturnToOwner()
    {
        _anchorTransform.SetParent(_ownerTransform);
        _anchorTransform.localPosition = _grabbedPosition;

        _currentState = AnchorStates.WithOwner;
    }
    
    public void ReturnToOwner()
    {
        SetStill();
        _anchorTransform.SetParent(_ownerTransform);
        SetGrabbedPosition();

        _currentState = AnchorStates.WithOwner;
    }

    public void GetPulledTowardsOwner()
    {
        Vector3 direction = (_springJoint.connectedBody.position - _rigidbody.position).normalized;
        _rigidbody.AddForce(direction * _pulledTowardsOwnerAccelearation, ForceMode.Acceleration);
    }

    public void GetThrown(float strength01, Vector3 lookDirection)
    {
        ComputeTrajectory(strength01, lookDirection);

        SetAimingPositionInstantly();
        _anchorTransform.SetParent(null);

        LaunchAnchor(_velocity);


        _currentState = AnchorStates.OnAir;        
    }
    
    
    public void ComputeTrajectory(float strength01, Vector3 lookDirection)
    {
        _throwStrength01 = strength01;

        lookDirection += (_ownerTransform.rotation * _launchDirectionOffset).normalized;
        lookDirection.Normalize();

        _velocity = lookDirection * _forceCurve.Evaluate(strength01) * _maxForce;
        _throwStartPosition = Position;
        DrawTrajectory(_velocity);
    }
    


    public bool IsOnAir()
    {
        return _currentState == AnchorStates.OnAir;
    }
    public bool IsOnGround()
    {
        return _currentState == AnchorStates.OnGround;
    }
    public bool CanBeGrabbed()
    {
        return IsOnGround();
    }



    private Vector3 GetTrajectoryPosition(float time, Vector3 startVelocity, Vector3 startPosition)
    {
        return startPosition + (startVelocity * time) + 0.5f * ((Physics.gravity + _springJoint.currentForce) * _rigidbody.mass * time * time );
    }


    private async void LaunchAnchor(Vector3 startVelocity)
    {
        _currentState = AnchorStates.OnAir;

        SetMovable();
        _rigidbody.AddForce(startVelocity, ForceMode.Impulse);

        return;
        float delay = 0.2f;
        await Task.Delay((int)(delay * 1000));

        if (_currentState == AnchorStates.OnAir)
        {
            Debug.Log("DOWN");
            //Vector3 downForce = Vector3.down * startVelocity.magnitude * 3;
            Vector3 downForce = startVelocity;
            downForce.y *= -1.5f;
            _rigidbody.AddForce(downForce, ForceMode.Impulse);
        }
        
    }

    private void DrawTrajectory(Vector3 startVelocity)
    {
        int stepCount = 30;
        float time = 0.2f;
        float step = time / stepCount;


        _trajectoryLine.positionCount = stepCount;

        Vector3 position;
        Vector3 previousPosition = Position;
        for (int i = 0; i < stepCount; ++i)
        {
            float t = i * step;
            position = GetTrajectoryPosition(t, startVelocity, _throwStartPosition);
            _trajectoryLine.SetPosition(i, position);
            
            if (Physics.SphereCast(position, _collider.radius, (position - previousPosition).normalized, out RaycastHit hit, 0.1f, 
                _obstacleLayers, QueryTriggerInteraction.Ignore))
            {
                for (int  j = i+1; j < stepCount; ++j)
                {
                    _trajectoryLine.SetPosition(j, position);
                }
                return;
            }

            previousPosition = position;
        }
    }


    public async void SnapToFloorAndSetStill()
    {
        // This is kinda scuffed... need to find a better way
        if (Physics.Raycast(Position, Vector3.down, out RaycastHit hit))
        {
            if (hit.distance > _collider.radius + 0.1f)
            {
                _rigidbody.AddForce(Vector3.down *  hit.distance, ForceMode.Impulse);
                await Task.Delay(400);
                SetStill();
                return;
            }            
        }

        SetStill();
    }

    private void SetStill()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _rigidbody.interpolation = RigidbodyInterpolation.None;

        _hitTrigger.enabled = false;
    }
    private void SetMovable()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        _hitTrigger.enabled = true;
    }
    public void SetPullMovable()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }


    private void UpdateOwnerBinderLine()
    {
        _ownerBinderLine.positionCount = 2;
        _ownerBinderLine.SetPosition(0, Position);
        _ownerBinderLine.SetPosition(1, _ownerTransform.position);
    }

    public void ShowTrajectory()
    {
        _trajectoryLine.enabled = true;
    }
    
    public void HideTrajectory()
    {
        _trajectoryLine.enabled = false;
    }

    public void SetAimingPosition()
    {
        float duration = Vector3.Distance(_anchorTransform.position, _aimingPosition) * 0.02f;

        _anchorTransform.DOKill();
        _anchorTransform.DOLocalMove(_aimingPosition, duration);
    }
    public void SetAimingPositionInstantly()
    {
        _anchorTransform.DOKill();
        _anchorTransform.localPosition =_aimingPosition;
    }
    public void SetGrabbedPosition()
    {
        float duration = Vector3.Distance(_anchorTransform.position, _grabbedPosition) * 0.02f;

        _anchorTransform.DOKill();
        _anchorTransform.DOLocalMove(_grabbedPosition, duration);
    }



    
    public async Task MeleeAttack()
    {
        float prepareDuration = 0.1f;
        float recoverDuration = 0.2f;
        float duration = 0.5f;
        float halfDuration = duration / 2;

        _anchorDamageDealer.StartMeleeHitDamage(Position, prepareDuration, duration);

        _canMeleeAttack = false;


        _anchorTransform.DOComplete();

        _anchorTransform.DOBlendableLocalMoveBy(new Vector3(0.5f, 0.0f, -1.0f), prepareDuration).OnComplete(() =>
        {
            _anchorTransform.DOBlendableLocalMoveBy(new Vector3(0.0f, 0.0f, -1.2f), halfDuration).OnComplete(() =>
            {
                _anchorTransform.DOBlendableLocalMoveBy(new Vector3(0.0f, 0.0f, 1.2f), halfDuration);
            });

            _anchorTransform.DOBlendableLocalMoveBy(new Vector3(-2.0f, 0.0f, 0.0f), duration).OnComplete(() =>
            {
                _anchorTransform.DOLocalMove(_grabbedPosition, recoverDuration).OnComplete(() =>
                {
                    _canMeleeAttack = true;
                });
            });
        });

        await Task.Delay((int)((prepareDuration + duration + recoverDuration)*1000));
    }

    

    public bool IsOwnerTensionLimit()
    {
        Vector3 ownerPosition = _springJoint.connectedBody.position;

        float distance = Vector3.Distance(Position, ownerPosition);
        float thresholdDistance = _springJoint.maxDistance;

        if (distance < thresholdDistance)
        {
            return false;
        }

        if (Physics.Raycast(Position, (ownerPosition - Position).normalized, distance, _obstacleLayers))
        {
            return false;
        }

        return true;
    }

    public async Task AttractOwner(float duration)
    {
        Transform ownerTransform = _springJoint.connectedBody.transform;
        Vector3 ownerPosition = ownerTransform.position;
        Vector3 directionToAnchor = (Position - ownerPosition).normalized;
        float offsetDistance = 2.5f;
        float offsetHeight = 2.5f;

        Vector3 endPosition = Position;
        endPosition += directionToAnchor * offsetDistance;
        endPosition.y += offsetHeight;

        Vector3 directionToEndPosition = (endPosition - ownerPosition).normalized;
        if (Physics.Raycast(ownerPosition, directionToEndPosition, out RaycastHit hit, Vector3.Distance(ownerPosition, endPosition), _obstacleLayers))
        {
            endPosition = hit.point - directionToEndPosition * 0.6f;
        }


        await ownerTransform.DOMove(endPosition, duration).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
    }




}
