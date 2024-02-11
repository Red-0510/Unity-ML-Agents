using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PopulationManagerMovement : MonoBehaviour {
    public Transform botPrefab;
    public int populationSize=50;
    List<BrainMovement> populationList;
    public static float elapsed=0;
    public float trialTime=10f;
    int generation=1;

    private void Awake(){
        populationList = new List<BrainMovement>();
    }

    private void Start(){
        for(int i=0;i<populationSize;i++){
            Vector3 pos = new Vector3(Random.Range(-3,3),0,Random.Range(-3,3));
            Transform bot= Instantiate(botPrefab);
            bot.parent = transform;
            bot.localPosition = pos;
            BrainMovement brain = bot.GetComponent<BrainMovement>();
            populationList.Add(brain);
        }
    }

    private BrainMovement Breed(BrainMovement b1,BrainMovement b2){
        Vector3 pos = new Vector3(Random.Range(-3,3),0,Random.Range(-3,3));
        Transform bot = Instantiate(botPrefab);
        bot.parent = transform;
        bot.localPosition = pos;
        BrainMovement brain = bot.GetComponent<BrainMovement>();
        brain.dna.Combine(b1.dna,b2.dna);
        if(Random.Range(0,1000)<10)brain.dna.Mutate();
        return brain;
    }

    private void BreedPopulation(){
        List<BrainMovement> orderedList = populationList.OrderBy(b=>b.timeAlive +b.distanceTravelled).ToList();
        populationList.Clear();
        int candidate = (int)(orderedList.Count*0.4f);
        float tta=0f,dist=0f;
        for(int i=candidate;i<orderedList.Count-1;i++){
            populationList.Add(Breed(orderedList[i],orderedList[i+1]));
            populationList.Add(Breed(orderedList[i+1],orderedList[i]));
            tta = Mathf.Max(tta,orderedList[i].timeAlive);
            dist = Mathf.Max(dist,orderedList[i].distanceTravelled);
        }
        Debug.Log(generation+" "+tta+" "+dist);

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