using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurtleMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Transform playerTransform;
    private bool _followingPlayer;
    
   // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (_followingPlayer)
        {
            navMeshAgent.SetDestination(playerTransform.position);
        }
    }

    public void StartFollowingPlayer()
    {
        _followingPlayer = true;
        navMeshAgent.enabled =true;
    }

    public void StopFollowingPlayer()
    {
        _followingPlayer = false;
        navMeshAgent.enabled =false;
    }


}
