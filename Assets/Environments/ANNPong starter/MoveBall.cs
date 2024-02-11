﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour {
	Vector3 ballStartPosition;
	Rigidbody2D rigidbody;
	public float speed = 400;
	public AudioSource blip, blop;

	void Start(){
		rigidbody = GetComponent<Rigidbody2D>();
		ballStartPosition = this.transform.position;
		ResetBall();
	}

	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.tag == "backwall"){
			blop.Play();
		} else {
			blip.Play();
		}
	}

	void ResetBall(){
		transform.position = ballStartPosition;
		rigidbody.velocity = Vector3.zero;
		Vector3 dir = new Vector3(Random.Range(100,300),Random.Range(-100,100),0).normalized;
		rigidbody.AddForce(dir*speed);
	}

	void Update(){
		if(Input.GetKeyDown("space")){
			ResetBall();
		}
	}
}