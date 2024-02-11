using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class BrainStayOnPlatform : MonoBehaviour
{
    public int dnaLength=2;
    public float timeAlive;
    public float timeWalked=0;
    public DNAStayOnPlatform dna;
    private bool alive = true;
    private StarterAssetsInputs _input;
    public GameObject eyes;
    public bool seeGround=false;

    // private void OnCollisionEnter(Collision other) {
    //     // Debug.Log(other);
    //     if(other.gameObject.CompareTag("dead")){
    //         Debug.Log("dead");
    //         alive=false;
    //         timeAlive=0;
    //         timeWalked=0;
    //         gameObject.SetActive(false);
    //     }
    // }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        GameObject body = hit.collider.gameObject;
        if(body.tag == "dead"){
            alive=false;
            // Debug.Log("dead");
            timeWalked=0;
            timeAlive=0;
            gameObject.SetActive(false);
        }
    }

    private void Awake(){
        // 0->forward
        // 1->left
        // 2->right
        // two genes for when platform can be seen and when it cannot be seen.
        dna = new DNAStayOnPlatform(dnaLength,3);
        _input = GetComponent<StarterAssetsInputs>();
        timeAlive=0;
        timeWalked=0;
        alive=true;
    }

    private void Update(){
        if(!alive)return;

        Debug.DrawRay(eyes.transform.position,eyes.transform.forward * 10, Color.red, 10);
        RaycastHit hit;
        seeGround=false;
        if(Physics.Raycast(eyes.transform.position,eyes.transform.forward * 10,out hit)){
            // Debug.Log(hit.collider.gameObject.tag);
            if(hit.collider.gameObject.tag == "platform" || hit.collider.gameObject.tag == "Player"){
                seeGround=true;
            }
        }
        timeAlive=PopulationManagerStayOnPlatform.elapsed;
        float move,turn;
        move=0;
        turn=0;
        if(seeGround){
            if(dna.GetGene(0)==0){move=1;}
            if(dna.GetGene(0)==1)turn=-90;
            if(dna.GetGene(0)==2)turn =90;
        } else {
            if(dna.GetGene(1)==0){move=1;}
            if(dna.GetGene(1)==1)turn=-90;
            if(dna.GetGene(1)==2)turn =90;

        }
        if(turn!=0){
            transform.Rotate(0,turn,0);
        }
        if(move==1){
            _input.MoveInput(new Vector2(transform.forward.x,transform.forward.z));
            timeWalked++;
        }
    }

    public void Die(){
        Destroy(gameObject);
    }
}
