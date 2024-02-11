using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class PopulationManagerStayOnPlatform : MonoBehaviour
{
    public Transform botPrefab;
    public int populationSize=50;
    List<BrainStayOnPlatform> populationList;
    public static float elapsed=0;
    public float trialTime=5f;
    int generation=1;
    GUIStyle guiStyle = new GUIStyle();

    void OnGUI(){
        guiStyle.fontSize=25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10,10,250,150));
        GUI.Box(new Rect(0,0,140,140),"Stats",guiStyle);
        GUI.Label(new Rect(10,25,200,30),"Gen: "+generation,guiStyle);
        GUI.Label(new Rect(10,50,200,30),string.Format("Time: {0:0.00}",
            elapsed,guiStyle));
        GUI.Label(new Rect(10,75,200,30),"Population: "+populationList.Count,guiStyle);
        GUI.EndGroup();
    }

    private void Awake(){
        populationList = new List<BrainStayOnPlatform>();
    }

    private void Start(){
        for(int i=0;i<populationSize;i++){
            Vector3 pos = new Vector3(Random.Range(-3,3),0,Random.Range(-3,3));
            Transform bot= Instantiate(botPrefab);
            bot.parent = transform;
            bot.localPosition = pos;
            BrainStayOnPlatform brain = bot.GetComponent<BrainStayOnPlatform>();
            populationList.Add(brain);
        }
    }

    private BrainStayOnPlatform Breed(BrainStayOnPlatform b1,BrainStayOnPlatform b2){
        Vector3 pos = new Vector3(Random.Range(-3,3),0,Random.Range(-3,3));
        Transform bot = Instantiate(botPrefab);
        bot.parent = transform;
        bot.localPosition = pos;
        BrainStayOnPlatform brain = bot.GetComponent<BrainStayOnPlatform>();
        if(Random.Range(0,1000)<1){
            brain.dna.Mutate();
        } else{
            brain.dna.Combine(b1.dna,b2.dna);
        }
        return brain;
    }


    private void BreedPopulation(){
        List<BrainStayOnPlatform> orderedList = populationList.OrderBy(b=>b.timeAlive + 5*b.timeWalked).ToList();
        populationList.Clear();
        int candidate = (int)(orderedList.Count*0.5f);
        // float tta=0f,dist=0f;
        for(int i=candidate;i<orderedList.Count-1;i++){
            populationList.Add(Breed(orderedList[i],orderedList[i+1]));
            populationList.Add(Breed(orderedList[i+1],orderedList[i]));
            // tta = Mathf.Max(tta,orderedList[i].timeAlive);
            // dist = Mathf.Max(dist,orderedList[i].distanceTravelled);
        }
        // Debug.Log(generation+" "+tta+" "+dist);
        Debug.Log(generation+" "+orderedList[orderedList.Count-1].timeAlive+" "+5*orderedList[orderedList.Count-1].timeWalked +" " + orderedList[orderedList.Count-1].dna.GetGene(0)+" "+ orderedList[orderedList.Count-1].dna.GetGene(1)+" "+orderedList[orderedList.Count-1].seeGround);
        for(int i=0;i<orderedList.Count;i++){
            orderedList[i].Die();
        }
        generation++;
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Loader.Load(Loader.Scene.MainMenuScene);
        }
        elapsed+=Time.deltaTime;
        if(elapsed>=trialTime){
            BreedPopulation();
            elapsed=0;
        }
    }
}
