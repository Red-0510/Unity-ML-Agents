using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ANNDrive : MonoBehaviour
{
    DriveANN ann;
    public float visibleDistance=50.0f;
    public int epochs=1000;
    public float speed=50.0f;
    public float rotationSpeed=100.0f;
    bool traningDone = false;
    float trainingProgress=0;
    double sse=0;
    double lastSSE=1;
    public float translation;
    public float rotation;
    public bool loadFromFile=false;

    void Start(){
        ann = new DriveANN(5,2,2,10,0.5);
        if(loadFromFile){
            LoadWeightsFromFile();
            traningDone=true;
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
        string path = Application.dataPath + "/Environments/Imitation_Drive/trainingData.txt";
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
                    string[] data = line.Split(",");
                    float thisError=0;
                    if(System.Convert.ToDouble(data[5])!=0 && System.Convert.ToDouble(data[6])!=0){
                        inputs.Clear();
                        outputs.Clear();
                        inputs.Add(System.Convert.ToDouble(data[0]));
                        inputs.Add(System.Convert.ToDouble(data[1]));
                        inputs.Add(System.Convert.ToDouble(data[2]));
                        inputs.Add(System.Convert.ToDouble(data[3]));
                        inputs.Add(System.Convert.ToDouble(data[4]));

                        double o1=Map(0,1,-1,1,System.Convert.ToSingle(data[5]));
                        outputs.Add(o1);
                        double o2=Map(0,1,-1,1,System.Convert.ToSingle(data[6]));
                        outputs.Add(o2);

                        calcOutputs = ann.Train(inputs,outputs);
                        thisError = (Mathf.Pow((float)(outputs[0] - calcOutputs[0]),2) + Mathf.Pow((float)(outputs[1] - calcOutputs[1]),2))/2.0f;
                    }
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
        traningDone=true;
        SaveWeightsToFile();
    }

    void SaveWeightsToFile(){
        string path = Application.dataPath + "/Environments/Imitation_Drive/weights.txt";
        // Debug.Log(path);
        StreamWriter wf = File.CreateText(path);
        wf.WriteLine(ann.PrintWeights());
        wf.Close();
    }

    void LoadWeightsFromFile(){
        string path = Application.dataPath + "/Environments/Imitation_Drive/weights.txt";
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

    float Round(float x){
        return (float) System.Math.Round(x,System.MidpointRounding.AwayFromZero);
    }

    public void GetData(List<double>inputs, List<double> outputs){
        List<double> calcOutputs;
        calcOutputs = ann.Train(inputs,outputs);
        float err = (Mathf.Pow((float)(outputs[0] - calcOutputs[0]),2) + Mathf.Pow((float)(outputs[1] - calcOutputs[1]),2))/2.0f;
        if(err<lastSSE){
            lastSSE=err;
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Loader.Load(Loader.Scene.MainMenuScene);
        }
        if(!traningDone) return;
        List<double> calcOutputs = new List<double>();
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        Debug.DrawRay(transform.position,this.transform.forward * visibleDistance, Color.red);

        Debug.DrawRay(transform.position,this.transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position,-this.transform.right * visibleDistance, Color.red);

        Debug.DrawRay(transform.position,Quaternion.AngleAxis(-45,Vector3.up) * this.transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position,Quaternion.AngleAxis(45,Vector3.up) * -this.transform.right * visibleDistance, Color.red);

        RaycastHit hit;
        float fdist = 0, rdist = 0, ldist=0, r45dist=0, l45dist=0;

        if(Physics.Raycast(transform.position,this.transform.forward,out hit, visibleDistance)){
            fdist =1 - Round(hit.distance/visibleDistance);
        }

        if(Physics.Raycast(transform.position,this.transform.right,out hit, visibleDistance)){
            rdist =1 - Round(hit.distance/visibleDistance);
        }
        
        if(Physics.Raycast(transform.position, -this.transform.right,out hit, visibleDistance)){
            ldist =1 - Round(hit.distance/visibleDistance);
        }
        
        if(Physics.Raycast(transform.position,Quaternion.AngleAxis(-45,Vector3.up) * this.transform.right,out hit, visibleDistance)){
            r45dist =1 - Round(hit.distance/visibleDistance);
        }
        
        if(Physics.Raycast(transform.position,Quaternion.AngleAxis(45,Vector3.up) * -this.transform.right,out hit, visibleDistance)){
            l45dist =1 - Round(hit.distance/visibleDistance);
        }

        inputs.Add(fdist);
        inputs.Add(rdist);
        inputs.Add(ldist);
        inputs.Add(r45dist);
        inputs.Add(l45dist);

        outputs.Add(0);
        outputs.Add(0);
        calcOutputs = ann.CalcOutput(inputs,outputs);
        float translationInput = Map(-1,1,0,1,(float)calcOutputs[0]);
        float rotationInput = Map(-1,1,0,1,(float)calcOutputs[1]);

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation =translationInput * speed * Time.deltaTime;
        rotation =rotationInput * rotationSpeed * Time.deltaTime;

        // Move translation along the object's z-axis
        this.transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        this.transform.Rotate(0, rotation, 0);
    }
}
