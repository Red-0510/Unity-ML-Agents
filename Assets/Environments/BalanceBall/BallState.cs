using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallState : MonoBehaviour
{
    public bool dropped=false;
    Vector3 ballStartPos;

    private void Awake(){
        ballStartPos = this.transform.position;
        // ResetBall();
        GetComponent<Rigidbody>().isKinematic=true;
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag =="drop"){
            dropped=true;
        }
    }

    public void ResetBall(){
        transform.position = ballStartPos;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<BallState>().dropped = false;
        int ff;
        if(Random.Range(0,2)==1)ff=80;
        else ff=-80;
        // GetComponent<Rigidbody>().AddForce(0,0,ff);
    }
}
