using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class BalanceBallBrain : MonoBehaviour
{
    public float tiltSpeed=60f;
    public GameObject ball;
    List<string> collectedTrainingData = new List<string>();
    List<string> collectedOutput = new List<string>();
    StreamWriter tdf;

    void Start(){
        string path = Application.dataPath + "/Environments/BalanceBall/trainingData.txt";
        tdf = File.CreateText(path);
        ball.GetComponent<BallState>().ResetBall();
    }

    private void OnApplicationQuit() {
        for(int i=0;i<collectedTrainingData.Count;i++){
            string res = collectedTrainingData[i] + collectedOutput[i];
            tdf.WriteLine(res);
        }
        tdf.Close();
    }

    private void Update(){
        if(Input.GetKeyDown("space") || (ball.GetComponent<BallState>().dropped)){
            ball.GetComponent<BallState>().ResetBall();
            // collectedTrainingData.Clear();
            // collectedOutput.Clear();
            return;
        }
        float rotationInput = Input.GetAxis("Horizontal");
        float rotation =rotationInput * tiltSpeed * Time.deltaTime;
        this.transform.Rotate(Vector3.right,rotation);

        float sz = GetComponent<Renderer>().bounds.size.z;
        double rx = Round(this.transform.rotation.x);
        double z = Round(ball.transform.position.z/sz);
        double avx = Round(ball.GetComponent<Rigidbody>().angularVelocity.x);

        string td = rx.ToString("F2")+","+z.ToString("F2")+","+avx.ToString("F2");
        string output = ","+Round(rotationInput).ToString("F2");
        if(!collectedTrainingData.Contains(td)){
            collectedTrainingData.Add(td);
            collectedOutput.Add(output);
        }

    }

    float Round(float x){
        int temp = (int)(10*x);
        float ret = (float)(temp)/10.0f;
        if(x!=ret && ret<0){
            ret-=0.1f;
        }
        return ret;
    }

}

