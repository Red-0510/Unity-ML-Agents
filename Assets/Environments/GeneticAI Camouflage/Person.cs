using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Person : MonoBehaviour {
    public float r,g,b;
    public bool dead=false;
    public float timeToDie;
    public Image sprite;

    private void OnMouseDown(){
        dead = true;
        timeToDie=PopulationManagerCamouflage.elapsed;
        gameObject.SetActive(false);
        Debug.Log(timeToDie);
    }

    public void SetRGB(float r,float g,float b){
        this.r=r;
        this.g=g;
        this.b=b;
    }

    private void Start(){
        sprite.color = new Color(r,g,b);
    }

    public void ShootEmCJ(){
        Destroy(gameObject);
    }
}
