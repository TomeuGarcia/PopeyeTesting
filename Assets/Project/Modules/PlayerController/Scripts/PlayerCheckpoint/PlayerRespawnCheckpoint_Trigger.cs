using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnCheckpoint_Trigger : MonoBehaviour
{
    [SerializeField] private Transform _respawnPoint;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagUtilities.PLAYER_TAG))
        {
            other.gameObject.GetComponent<Player>()._respawnPosition = _respawnPoint.position;
            
            gameObject.SetActive(false); // disable self
        }    
    }

}
