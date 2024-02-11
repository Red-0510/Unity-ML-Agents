using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainingSetNN{
    public double[] input;
    public double output;
}

public class PerceptronNN : MonoBehaviour
{
    public List<TrainingSetNN> ts = new List<TrainingSetNN>();
    public double[] weights={0f,0f};
    public double bias=0f;
    double totalError=0;

    public GameObject bot;

    public void sendInput(double i1,double i2,double o){
        double result = CalcOutput(i1,i2);
        Debug.Log(result);
        if(result==0){
            bot.GetComponent<Animator>().SetTrigger("Crouch");
            bot.GetComponent<Rigidbody>().isKinematic=false;
        } else {
            bot.GetComponent<Rigidbody>().isKinematic=true;
        }

        TrainingSetNN s = new TrainingSetNN();
        s.input = new double[2] {i1,i2};
        s.output=o;
        ts.Add(s);
        Train();
    }

    double DotProductBias(double[] v1,double[] v2){
        if(v1==null || v2==null)return -1;
        if(v1.Length!=v2.Length) return -1;

        double d=0;
        for(int i=0;i<v1.Length;i++){
            d+=v1[i]*v2[i];
        }
        d+=bias;
        return d;
    }

    double CalcOutput(int i){
        double output = DotProductBias(weights,ts[i].input);
        return ActivationFunction(output);
    }

    void InitialiseWeights(){
        for(int i=0;i<weights.Length;i++){
            weights[i] = Random.Range(-1.0f,1.0f);
        }
        bias = Random.Range(-1.0f,1.0f);
    }

    void UpdateWeights(int i){
        double error = ts[i].output - CalcOutput(i);
        totalError+=Mathf.Abs((float)error);
        for(int j=0;j<weights.Length;j++){
            weights[j]+=error*ts[i].input[j];
        }
        bias+=error;
    }

    double CalcOutput(double i1,double i2){
        double[] inp = new double[] {i1,i2};
        double output = DotProductBias(weights,inp);
        return ActivationFunction(output);
    }

    double ActivationFunction(double inp){
        if(inp>0) return 1;
        else return 0;
    }

    void Train(){

        for(int j=0;j<ts.Count;j++){
            UpdateWeights(j);
                // Debug.Log("W1: "+ weights[0] + " W2: "+weights[1] + " B: "+ bias);
        }
            // Debug.Log("Epoch: "+ i + "Total Error: "+totalError);
    }

    void Start(){
        InitialiseWeights();
    }

    void Test(){
        Debug.Log("0,0: "+ CalcOutput(0,0));
        Debug.Log("0,1: "+ CalcOutput(0,1));
        Debug.Log("1,0: "+ CalcOutput(1,0));
        Debug.Log("1,1: "+ CalcOutput(1,1));
    }

}
