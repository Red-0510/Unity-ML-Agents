using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveLayer {

	public int numNeurons;
	public List<DriveNeuron> neurons = new List<DriveNeuron>();

	public DriveLayer(int nNeurons, int numNeuronInputs)
	{
		numNeurons = nNeurons;
		for(int i = 0; i < nNeurons; i++)
		{
			neurons.Add(new DriveNeuron(numNeuronInputs));
		}
	}
}
