using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(CharacterController))]
public class BrainMovement : MonoBehaviour
{
    public int dnaLength=4;
    public float timeAlive;
    public float distanceTravelled;
    Vector2 startPosition;
    public DNAMovement dna;
    private StarterAssetsInputs _input;
    private Vector2 m_Move;
    private bool jump;
    private bool sprint;
    private bool alive = true;

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        GameObject body = hit.collider.gameObject;
        if(body.tag == "dead"){
            alive=false;
            gameObject.SetActive(false);
        }
    }

    public void Die(){
        Destroy(gameObject);
    }

    private void Awake(){
        dna = new DNAMovement(dnaLength,2);
        _input = GetComponent<StarterAssetsInputs>();
        timeAlive=0;
        alive=true;
        distanceTravelled=0f;
        startPosition = new Vector2(transform.position.x,transform.position.z);
    }
    private void Update(){
    }

    private void LateUpdate(){
        jump=false;
        sprint=false;
        m_Move.x = dna.GetGene(0);
        m_Move.y=dna.GetGene(1);
        if(dna.GetGene(2)==1)sprint=true;
        if(dna.GetGene(3)==1)jump=true;

        _input.SprintInput(sprint);
        _input.JumpInput(jump);
        _input.MoveInput(m_Move);

        if(alive){
            timeAlive+=Time.deltaTime;
        }
        distanceTravelled = Vector2.Distance(new Vector2(transform.position.x,transform.position.z),startPosition);
    }
}


