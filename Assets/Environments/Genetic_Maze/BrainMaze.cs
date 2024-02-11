using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class BrainMaze : MonoBehaviour
{
    public int dnaLength=2;
    public float distanceTravelled=0;
    public DNA_Maze dna;
    private bool alive = true;
    private bool seeWall;
    public GameObject eyes;
    Vector3 startPosition;
    bool hasChilds=false;

    private void Awake(){
        dna = new DNA_Maze(dnaLength,360);
        startPosition = this.transform.position;
    }

    public void OnCollisionEnter(Collision obj){
        if(obj.gameObject.CompareTag("dead")){
            distanceTravelled=0;
            alive=false;
        }
    }

    void Update(){
        if(!alive) return;
        seeWall=false;
        RaycastHit hit;
        Debug.DrawRay(eyes.transform.position,eyes.transform.forward*0.5f,Color.red);
        if(Physics.SphereCast(eyes.transform.position,0.1f,eyes.transform.forward,out hit, 0.5f)){
            if(hit.collider.gameObject.CompareTag("wall")){
                seeWall=true;
            }
        }
    }

    private void FixedUpdate() {
        if(!alive) return;
        float move=dna.GetGene(0);
        float turn=0;
        if(seeWall){
            turn = dna.GetGene(1);
        }
        this.transform.Translate(0,0,move*0.001f);
        this.transform.Rotate(0,turn,0);
        distanceTravelled = Vector3.Distance(startPosition,transform.position);
        if(!hasChilds && distanceTravelled>30.0f){
            PopulationManager_Maze.Instance.spawnImmediateChild(this);
        }
    }

    public void Die(){
        Destroy(gameObject);
    }
}
