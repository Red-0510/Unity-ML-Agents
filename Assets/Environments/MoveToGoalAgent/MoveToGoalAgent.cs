using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent{

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial,loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public float moveSpeed=1f;
    Vector3 startPos;

    // private void Awake(){
    //     startPos = Vector;
    // }
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Loader.Load(Loader.Scene.MainMenuScene);
        }
    }

    public override void OnEpisodeBegin(){
        transform.localPosition = new Vector3(Random.Range(-22f,22f),0,Random.Range(-14f,14f));
        targetTransform.localPosition = new Vector3(Random.Range(-22f,22f),-1f,Random.Range(-14f,14f));
        // floorMeshRenderer.material = defaultlit;
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions){
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition+=new Vector3(moveX,0,moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag=="goal"){
            SetReward(1f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        else if(other.gameObject.tag =="Wall"){
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
}

