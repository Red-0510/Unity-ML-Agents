using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {
    public GameObject spherePrefab,cubePrefab;
    public Material red,green;
    PerceptronNN p;

    void Start(){
        p=GetComponent<PerceptronNN>();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Loader.Load(Loader.Scene.MainMenuScene);
        }
        if(Input.GetKeyDown("1")){
            GameObject g = Instantiate(spherePrefab,Camera.main.transform.position,Camera.main.transform.rotation);
            g.GetComponent<Renderer>().material = red;
            g.GetComponent<Rigidbody>().AddForce(0,0,-500);
            p.sendInput(0,0,0);
        }
        if(Input.GetKeyDown("2")){
            GameObject g = Instantiate(spherePrefab,Camera.main.transform.position,Camera.main.transform.rotation);
            g.GetComponent<Renderer>().material = green;
            g.GetComponent<Rigidbody>().AddForce(0,0,-500);
            p.sendInput(0,1,1);
        }
        if(Input.GetKeyDown("3")){
            GameObject g = Instantiate(cubePrefab,Camera.main.transform.position,Camera.main.transform.rotation);
            g.GetComponent<Renderer>().material = red;
            g.GetComponent<Rigidbody>().AddForce(0,0,-500);
            p.sendInput(1,0,1);
        }
        if(Input.GetKeyDown("4")){
            GameObject g = Instantiate(cubePrefab,Camera.main.transform.position,Camera.main.transform.rotation);
            g.GetComponent<Renderer>().material = green;
            g.GetComponent<Rigidbody>().AddForce(0,0,-500);
            p.sendInput(1,1,1);
        }
    }
}
