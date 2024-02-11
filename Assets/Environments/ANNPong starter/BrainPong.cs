using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainPong : MonoBehaviour{
    public GameObject paddle;
    public GameObject ball;
    public int layer;
    Rigidbody2D ballRigidbody;
    float yvel;
    public string backwallTag = "backwallr";
    float paddleMinY=8.8f;
    float paddleMaxY=17.4f;
    float paddleMaxSpeed=15f;
    public float numSaved=0;
    public float numMissed=0;

    ANNPong ann;
    void Start(){
        ann = new ANNPong(6,1,1,4,0.11);
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
    }

    List<double> Run(double bx,double by,double bvx,double bvy,double px,double py,double pv,bool train){
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();
        inputs.Add(bx);
        inputs.Add(by);
        inputs.Add(bvx);
        inputs.Add(bvy);
        inputs.Add(px);
        inputs.Add(py);
        outputs.Add(pv);
        if(train){
            return (ann.Train(inputs,outputs));
        } else {
            return ann.CalcOutput(inputs,outputs);
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Loader.Load(Loader.Scene.MainMenuScene);
        }

        float posy = Mathf.Clamp(paddle.transform.position.y + (yvel*Time.deltaTime*paddleMaxSpeed),paddleMinY,paddleMaxY);
        paddle.transform.position = new Vector3(paddle.transform.position.x,posy,paddle.transform.position.z);

        List<double> output = new List<double>();
        int layerMask = 1<<6;
        RaycastHit2D hit = Physics2D.Raycast(ball.transform.position,ballRigidbody.velocity,1000,layerMask);

        if(hit.collider!=null){
            if(hit.collider.gameObject.tag =="tops"){
                Vector3 reflection = Vector3.Reflect(ballRigidbody.velocity,hit.normal);
                hit = Physics2D.Raycast(hit.point,reflection,1000,layerMask);
            }
            if(hit.collider != null && hit.collider.gameObject.tag == backwallTag){
                float dy = (hit.point.y - paddle.transform.position.y);
                output = Run(ball.transform.position.x,ball.transform.position.y,ballRigidbody.velocity.x,ballRigidbody.velocity.y,paddle.transform.position.x,paddle.transform.position.y,dy,true);

                yvel = (float) output[0];
            }
        } else {
            yvel = 0;
        }
    }
}


