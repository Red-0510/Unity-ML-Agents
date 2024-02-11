using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceBallLayer {

	public int numNeurons;
	public List<BalanceBallNeuron> neurons = new List<BalanceBallNeuron>();

	public BalanceBallLayer(int nNeurons, int numNeuronInputs)
	{
		numNeurons = nNeurons;
		for(int i = 0; i < nNeurons; i++)
		{
			neurons.Add(new BalanceBallNeuron(numNeuronInputs));
		}
	}
}
