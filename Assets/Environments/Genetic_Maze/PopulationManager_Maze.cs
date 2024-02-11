using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager_Maze : MonoBehaviour
{
    public Transform botPrefab;
    public GameObject startingPos;
    public int populationSize=50;
    List<BrainMaze> populationList;
    public static float elapsed=0;
    public float trialTime=5f;
    int generation=1;
    GUIStyle guiStyle = new GUIStyle();

    public static PopulationManager_Maze Instance {get;private set;}

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
        Instance=this;
        populationList = new List<BrainMaze>();
    }

    private void Start(){
        for(int i=0;i<populationSize;i++){
            // Vector3 pos = new Vector3(Random.Range(-3,3),0,Random.Range(-3,3));
            Transform bot= Instantiate(botPrefab,startingPos.transform.position,transform.rotation);
            BrainMaze brain = bot.GetComponent<BrainMaze>();
            populationList.Add(brain);
        }
    }

    private BrainMaze Breed(BrainMaze b1,BrainMaze b2){
        Transform bot= Instantiate(botPrefab,startingPos.transform.position,transform.rotation);
        BrainMaze brain = bot.GetComponent<BrainMaze>();
        if(Random.Range(0,100)<1){
            brain.dna.Mutate();
        } else{
            brain.dna.Combine(b1.dna,b2.dna);
        }
        return brain;
    }

    private void BreedPopulation(){
        List<BrainMaze> orderedList = populationList.OrderBy(b=>b.distanceTravelled).ToList();
        populationList.Clear();
        int candidate = (int)(orderedList.Count*0.6f)-1;
        float dist=0f;
        for(int i=candidate;i<orderedList.Count-1;i++){
            populationList.Add(Breed(orderedList[i],orderedList[i+1]));
            populationList.Add(Breed(orderedList[i+1],orderedList[i]));
            // tta = Mathf.Max(tta,orderedList[i].timeAlive);
            dist = Mathf.Max(dist,orderedList[i].distanceTravelled);
            dist = Mathf.Max(dist,orderedList[i+1].distanceTravelled);
        }
        Debug.Log(generation+" "+dist);
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

    public void spawnImmediateChild(BrainMaze brain){
        int spawnLimit=1;
        if(populationList.Count>=50) return;
        for(int i=0;i<spawnLimit;i++){
            Transform bot= Instantiate(botPrefab,brain.transform.position,brain.transform.rotation);
            BrainMaze child = bot.GetComponent<BrainMaze>();
            child.dna.SetAllGenes(brain.dna);
            Debug.Log(child);
            Debug.Log(child.transform.position);
            populationList.Add(child);
        }
    }
}
