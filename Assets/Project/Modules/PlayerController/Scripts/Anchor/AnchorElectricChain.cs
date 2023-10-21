using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorElectricChain : MonoBehaviour
{
    [SerializeField] private LineRenderer _anchorElectricChain;
    [SerializeField] private Transform _electrifiedDebug;

    private Anchor _anchor;
    [SerializeField] private BoxCollider _collider;

    private bool _isInElectricMode;

    private DamageHit _electricDamageHit;


    public void StartInit(Anchor anchor)
    {
        _anchor = anchor;
        ExitElectricMode();

        _electricDamageHit = new DamageHit(CombatManager.Instance.DamageOnlyEnemiesPreset, 10.0f, 80.0f, 2.0f);
    }


    void Update()
    {
        if (_isInElectricMode) 
        {
            UpdateElectricChainPosition();
            UpdateCollider();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _electricDamageHit.KnockbackDirection = _collider.transform.right;
        Vector3 chainRight = -_collider.transform.right;
        Vector3 towardsTarget = (_anchor.Position - other.transform.position).normalized;
        

        if (Vector3.Dot(chainRight, towardsTarget) < 0) _electricDamageHit.KnockbackDirection *= -1;

        if (CombatManager.Instance.TryDealDamage(other.gameObject, _electricDamageHit, out DamageHitResult damageHitResult))
        {
            _electrifiedDebug.transform.parent = other.transform;
            _electrifiedDebug.transform.localPosition = Vector3.zero;
        }
    }


    public void ToggleElectricMode()
    {
        if (_isInElectricMode) ExitElectricMode();
        else EnterElectricMode();
    }

    private void EnterElectricMode()
    {
        _anchorElectricChain.enabled = true;
        _collider.enabled = true;

        _isInElectricMode = true;
    }
    
    private void ExitElectricMode()
    {
        _anchorElectricChain.enabled = false;
        _collider.enabled = false;

        _isInElectricMode = false;
    }

    private void UpdateElectricChainPosition()
    {
        _anchorElectricChain.SetPosition(0, _anchor.ChainExtremes.Item1);
        _anchorElectricChain.SetPosition(1, _anchor.ChainExtremes.Item2);
    }

    private void UpdateCollider()
    {
        Vector3 anchorPosition = _anchor.ChainExtremes.Item1;
        Vector3 anchorOwnerPosition = _anchor.ChainExtremes.Item2;

        Vector3 center = Vector3.Lerp(anchorPosition, anchorOwnerPosition, 0.5f);

        Vector3 up = Vector3.up;
        Vector3 forward = (anchorOwnerPosition - anchorPosition).normalized;
        Vector3 right = Vector3.Cross(forward, up).normalized;
        up = Vector3.Cross(right, forward).normalized;

        _collider.transform.rotation = Quaternion.LookRotation(forward, up);

        float size = (anchorPosition - anchorOwnerPosition).magnitude;

        _collider.transform.localScale = new Vector3(0.3f, 0.3f, size);
        _collider.transform.position = center;

    }

}
