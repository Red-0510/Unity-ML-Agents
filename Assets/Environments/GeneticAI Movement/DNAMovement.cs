using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNAMovement {
    List<int> genes = new List<int>();
    int dnaLength=0;
    int maxValues = 0;

    public DNAMovement(int l,int v){
        dnaLength=l;
        maxValues=v;
        SetRandom();
    }

    public void SetRandom(){
        genes.Clear();
        for(int i=0;i<dnaLength;i++){
            genes.Add(Random.Range(-1,maxValues));
        }
    }

    public void Combine(DNAMovement d1,DNAMovement d2){
        for(int i=0;i<dnaLength;i++){
            if(i<dnaLength/2){
                genes[i] = d1.genes[i];
            } else {
                genes[i] = d2.genes[i];
            }
        }
    }

    public void Mutate(){
        genes[Random.Range(0,dnaLength)] = Random.Range(0,maxValues);
    }

    public void SetGene(int pos,int val){
        genes[pos] = val;
    }

    public int GetGene(int pos){
        return genes[pos];
    }
}
