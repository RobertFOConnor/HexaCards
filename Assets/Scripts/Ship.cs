using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

    public int baseHealth;
    public int currHealth;

	// Use this for initialization
	void Start () {
        baseHealth = 10;
        currHealth = baseHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
