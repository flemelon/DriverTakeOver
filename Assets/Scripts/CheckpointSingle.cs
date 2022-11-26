using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private Track track;
    
    private void OnTriggerEnter(Collider other){

        Vector3 contact = other.gameObject.GetComponent<Collider>().ClosestPoint(transform.position);
        float sdlpDistance = Vector3.Distance(contact, transform.position);
        track.AddSdlpDistance(sdlpDistance);
    }

    public void SetTrack(Track track){
        this.track = track;
    }
}
