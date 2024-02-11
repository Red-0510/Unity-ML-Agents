using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public float speed = 50.0f;
    public float rotationSpeed = 100.0f;
    public float visibleDistance=50.0f;
    List<string> collectedTrainingData = new List<string>();
    List<string> collectedOutData = new List<string>();
    StreamWriter tdf;
    public GameObject annDriver;
    ANNDrive driver;

    void Start(){
        string path = Application.dataPath + "/Environments/Imitation_Drive/trainingData.txt";
        tdf = File.CreateText(path);
        // driver = annDriver.GetComponent<ANNDrive>();
    }

    private void OnApplicationQuit() {
        for(int i=0;i<collectedTrainingData.Count;i++){
            string res = collectedTrainingData[i] + collectedOutData[i];
            tdf.WriteLine(res);
        }
        tdf.Close();
    }

    float Round(float x){
        return (float) System.Math.Round(x,System.MidpointRounding.AwayFromZero);
    }


    void Update()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        // Make it move 10 meters per second instead of 10 meters per frame...
        float translation =translationInput * speed * Time.deltaTime;
        float rotation =rotationInput * rotationSpeed * Time.deltaTime;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);

        // Debug.DrawRay(transform.position,Quaternion.AngleAxis(-45,Vector3.up) * this.transform.right * visibleDistance, Color.red);
        // Debug.DrawRay(transform.position,Quaternion.AngleAxis(45,Vector3.up) * -this.transform.right * visibleDistance, Color.red);

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
        
        string td = fdist + ","+rdist+","+ldist+","+r45dist+","+l45dist;
        string output= "," + Round(translationInput) + ","+Round(rotationInput);
        if(!collectedTrainingData.Contains(td)){
            collectedTrainingData.Add(td);
            collectedOutData.Add(output);
        }
    }
}
