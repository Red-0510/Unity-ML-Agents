using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerPong {

	public int numNeurons;
	public List<NeuronPong> neurons = new List<NeuronPong>();

	public LayerPong(int nNeurons, int numNeuronInputs)
	{
		numNeurons = nNeurons;
		for(int i = 0; i < nNeurons; i++)
		{
			neurons.Add(new NeuronPong(numNeuronInputs));
		}
	}
}
