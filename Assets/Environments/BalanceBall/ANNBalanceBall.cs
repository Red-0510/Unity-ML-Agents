using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.Rendering;


public class ANNBalanceBallBrain : MonoBehaviour
{
    public float tiltSpeed=60f;
    public GameObject ball;
    bool trainingDone=false;
    float trainingProgress=0;
    double sse=0;
    double lastSSE=1000;
    public int epochs = 1000;
    public BalanceBallANN ann;
    public bool loadFromFile;

    void Start(){
        ann = new BalanceBallANN(3,1,2,15,0.5);
        if(loadFromFile){
            LoadWeightsFromFile();
            trainingDone=true;
            ball.GetComponent<BallState>().ResetBall();
        } else {
            StartCoroutine(LoadTrainingSet());
        }
    }

    void OnGUI(){
        GUI.Label(new Rect(25,25,250,100),"SSE: "+lastSSE);
        GUI.Label(new Rect(25,40,250,100),"Alpha: "+ann.alpha);
        GUI.Label(new Rect(25,55,250,100),"Trained: "+trainingProgress);
    }

    IEnumerator LoadTrainingSet(){
        string path = Application.dataPath + "/Environments/BalanceBall/trainingData.txt";
        string line;
        if(File.Exists(path)){
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader tdf = File.OpenText(path);
            List<double> calcOutputs = new List<double>();
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            for(int i=0;i<epochs;i++){
                sse=0;
                tdf.BaseStream.Position=0;
                string currentWeights = ann.PrintWeights();
                while((line=tdf.ReadLine())!=null){
                    Debug.Log(line);
                    string[] data = line.Split(",");
                    float thisError=0;
                    // if(System.Convert.ToDouble(data[3])!=0){
                        inputs.Clear();
                        outputs.Clear();
                        inputs.Add(System.Convert.ToDouble(data[0]));
                        inputs.Add(System.Convert.ToDouble(data[1]));
                        inputs.Add(System.Convert.ToDouble(data[2]));

                        double o1=Map(0,1,-1,1,System.Convert.ToSingle(data[3]));
                        // double o1 = System.Convert.ToDouble(data[3]);
                        outputs.Add(o1);

                        calcOutputs = ann.Train(inputs,outputs);
                        thisError = Mathf.Pow((float)(outputs[0] - calcOutputs[0]),2);
                        // Debug.Log(inputs[0]+","+inputs[1]+","+inputs[2]+", outputs: "+calcOutputs[0]+", "+outputs[0]+", error "+thisError);
                    // }
                    sse+=thisError;
                }
                trainingProgress=(float)i/(float)epochs;
                sse/=lineCount;
                // lastSSE=sse;

                if(lastSSE < sse){
                    ann.LoadWeights(currentWeights);
                    ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f,0.01f,0.9f);
                } else {
                    ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f,0.01f,0.9f);
                    lastSSE = sse;
                }

                yield return null;
            }
        }
        trainingDone=true;
        ball.GetComponent<BallState>().ResetBall();
        SaveWeightsToFile();
    }

    float Round(float x){
        int temp = (int)(10*x);
        float ret = (float)(temp)/10.0f;
        if(x!=ret && ret<0){
            ret-=0.1f;
        }
        return ret;
    }

    void SaveWeightsToFile(){
        string path = Application.dataPath + "/Environments/BalanceBall/weights.txt";
        StreamWriter wf = File.CreateText(path);
        wf.WriteLine(ann.PrintWeights());
        wf.Close();
    }

    void LoadWeightsFromFile(){
        string path = Application.dataPath + "/Environments/BalanceBall/weights.txt";
        StreamReader wf = File.OpenText(path);
        if(File.Exists(path)){
            string line = wf.ReadLine();
            ann.LoadWeights(line);
        }
    }

    float Map(float newfrom,float newto,float origfrom, float origto,float value){
        if(value<=origfrom)return newfrom;
        else if(value>=origto) return newto;
        return (newto - newfrom) * ((value - origfrom)/(origto - origfrom)) + newfrom;
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Loader.Load(Loader.Scene.MainMenuScene);
        }
        if(!trainingDone) return;
        if(Input.GetKeyDown("space") || (ball.GetComponent<BallState>().dropped)){
            ball.GetComponent<BallState>().ResetBall();
            transform.rotation = Quaternion.AngleAxis(0,Vector3.up);
            return;
        }
        List<double> inputs = new List<double>();
        float sz = GetComponent<Renderer>().bounds.size.z;

        double rx = Round(this.transform.rotation.x);
        double z = Round(ball.transform.position.z/sz);
        double avx = Round(ball.GetComponent<Rigidbody>().angularVelocity.x);

        inputs.Add(rx);
        inputs.Add(z);
        inputs.Add(avx);

        List<double> calcOutputs = ann.CalcOutput(inputs);
        // float rotationInput = (float)calcOutputs[0];
        float rotationInput = Map(-1,1,0,1,(float)calcOutputs[0]);
        Debug.Log(rotationInput);
        float rotation =rotationInput * tiltSpeed * Time.deltaTime;
        this.transform.Rotate(Vector3.right,rotation);
    }

}


