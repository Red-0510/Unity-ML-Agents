using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Replay{
    public List<double> states;
    public double reward;

    public Replay(double xr,double ballz,double ballvx,double r){
        states = new List<double>();
        states.Add(xr);
        states.Add(ballz);
        states.Add(ballvx);
        reward = r;
    }
}

public class DQNBallBalanceBrain : MonoBehaviour {
    public GameObject ball;
    BalanceBallANN ann;
    float reward = 0.0f;
    List<Replay> replayMemory = new List<Replay>();
    int mCapacity = 10000;
    float discount = 0.99f;
    float exploreRate = 100.0f;
    float maxExploreRate =100.0f;
    float minExploreRate = 0.01f;
    float exploreDecay = 0.001f;
    int failCount = 0;
    float tiltSpeed = 0.5f;

    float timer = 0;
    float maxBalanceTime = 0;
    Vector3 ballStartPos;

    void Start(){
        ann = new BalanceBallANN(3,2,1,6,0.2f);
        ball.GetComponent<BallState>().ResetBall();
        Time.timeScale = 10.0f;
    }

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI(){
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10,10,600,150));
        GUI.Box(new Rect(0,0,140,140),"Stats",guiStyle);
        GUI.Label(new Rect(10,25,500,30),"Fails: "+failCount,guiStyle);
        GUI.Label(new Rect(10,50,500,30),"Decay Rate: "+exploreRate,guiStyle);
        GUI.Label(new Rect(10,75,500,30),"Last Best Balance: "+maxBalanceTime,guiStyle);
        GUI.Label(new Rect(10,100,500,30),"This Balance: "+timer,guiStyle);
        GUI.EndGroup();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Loader.Load(Loader.Scene.MainMenuScene);
        }
        if(Input.GetKeyDown("space")){
            ball.GetComponent<BallState>().ResetBall();
            transform.rotation = Quaternion.identity;
            return;
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

    void FixedUpdate(){
        timer+=Time.deltaTime;
        List<double> states = new List<double>();
        List<double> qs = new List<double>();

        float sz = GetComponent<Renderer>().bounds.size.z;
        double rx = this.transform.rotation.x;
        double z = ball.transform.position.z;
        double avx = ball.GetComponent<Rigidbody>().angularVelocity.x;

        states.Add(rx);
        states.Add(z);
        states.Add(avx);

        qs = SoftMax(ann.CalcOutput(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay , minExploreRate, maxExploreRate);

        if(Random.Range(0,100)< exploreRate) maxQIndex = Random.Range(0,2);

        if(maxQIndex == 0){
            this.transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
        } else if(maxQIndex==1){
            this.transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);
        }

        if(ball.GetComponent<BallState>().dropped){
            reward = -1.0f;
        } else reward = 0.1f;

        sz = GetComponent<Renderer>().bounds.size.z;
        rx = this.transform.rotation.x;
        z = ball.transform.position.z;
        avx = ball.GetComponent<Rigidbody>().angularVelocity.x;

        Replay lastMemory = new Replay(rx,z,avx,reward);

        if(replayMemory.Count > mCapacity)replayMemory.RemoveAt(0);
        replayMemory.Add(lastMemory);

        if(ball.GetComponent<BallState>().dropped){
            for (int i=replayMemory.Count-1;i>=0;i--){
                List<double> toutputsOld = new List<double>();
                List<double> toutputsNew = new List<double>();
                toutputsOld = SoftMax(ann.CalcOutput(replayMemory[i].states));

                double maxQOld = toutputsOld.Max();
                int action = toutputsOld.ToList().IndexOf(maxQOld);

                double feedback;

                if(i==replayMemory.Count-1 || replayMemory[i].reward==-1) feedback = replayMemory[i].reward;
                else{
                    toutputsNew = SoftMax(ann.CalcOutput(replayMemory[i+1].states));
                    maxQ = toutputsNew.Max();
                    feedback = (replayMemory[i].reward + discount * maxQ);
                }
                toutputsOld[action] = feedback;
                ann.Train(replayMemory[i].states,toutputsOld);
            }

            if(timer > maxBalanceTime)maxBalanceTime = timer;
            timer=0;
            transform.rotation = Quaternion.identity;
            ball.GetComponent<BallState>().ResetBall();
            replayMemory.Clear();
            failCount++;
        }
    }

    List<double> SoftMax(List<double> values){
        double max = values.Max();

        float scale=0.0f;
        for(int i=0;i<values.Count;i++){
            scale+=Mathf.Exp((float)(values[i] - max));
        }
        List<double> result = new List<double>();
        for(int i=0;i<values.Count;i++){
            result.Add(Mathf.Exp((float)(values[i]-max))/scale);
        }

        return result;
    }
    
}
