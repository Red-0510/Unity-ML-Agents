using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManagerCamouflage : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform personPrefab;
    public int populationSize = 10;
    private int trainTime = 10;
    private int generation=1;
    public List<Person> populationList = new List<Person>();
    public static float elapsed;

    // private void Awake(){
    //     Vector3 lowerBound = Camera.main.ViewportToWorldPoint(new Vector3(0,0,0)); Vector3 upperBound = Camera.main.ViewportToWorldPoint(new Vector3(1,1,0));
    //     Debug.Log("lowerBound"+lowerBound);
    //     Debug.Log("upperBound"+upperBound);
    // }

    private void Start(){
        for (int i = 0; i < populationSize; i++){
            Vector3 pos = new Vector3(Random.Range(-8.0f,8.0f),Random.Range(4.0f,-4.0f),0);
            Transform personTransform= Instantiate(personPrefab);
            personTransform.parent=transform;
            personTransform.localPosition = pos;
            Person person = personTransform.GetComponent<Person>();
            float r=Random.Range(0.0f,1.0f);
            float g=Random.Range(0.0f,1.0f);
            float b=Random.Range(0.0f,1.0f);
            person.SetRGB(r,g,b);
            populationList.Add(person);
        }
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Loader.Load(Loader.Scene.MainMenuScene);
        }
        elapsed +=Time.deltaTime;

        if(elapsed>trainTime){
            BreedNewPopulation();
            elapsed=0;
        }
    }

    private Person Breed(Person p1,Person p2){
        float r,g,b;
        if(Random.Range(0,100)<5){
            r=Random.Range(0.0f,1.0f);
            g=Random.Range(0.0f,1.0f);
            b=Random.Range(0.0f,1.0f);
        } else {
            r = Random.Range(0,10) < 5 ? p1.r : p2.r;
            g = Random.Range(0,10) < 5 ? p1.g : p2.g;
            b = Random.Range(0,10) < 5 ? p1.b : p2.b;
        }

        Vector3 pos = new Vector3(Random.Range(-8.0f,8.0f),Random.Range(4.0f,-4.0f),0);
        Transform personTransform= Instantiate(personPrefab);
        personTransform.parent=transform;
        personTransform.localPosition = pos;
        Person person = personTransform.GetComponent<Person>();
        person.SetRGB(r,g,b);
        return person;
    }

    private void BreedNewPopulation(){
        List<Person> personListOrdered = populationList.OrderByDescending(p=>p.timeToDie).ToList();

        populationList.Clear();

        int candidate = (int)(personListOrdered.Count * 0.4f);
        for (int i = candidate; i < personListOrdered.Count-1; i++){
            populationList.Add(Breed(personListOrdered[i],personListOrdered[i+1]));
            populationList.Add(Breed(personListOrdered[i+1],personListOrdered[i]));
        }

        for (int i = 0; i < personListOrdered.Count; i++){
            personListOrdered[i].ShootEmCJ();
        }

        generation++;
    }


}
