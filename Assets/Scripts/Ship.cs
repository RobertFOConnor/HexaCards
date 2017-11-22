using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

    private static int SHIP_HEALTH = 10;

    private int baseHealth;
    private int currHealth;
    private bool destroyed = false;

	// Use this for initialization
	void Start () {
        baseHealth = SHIP_HEALTH;
        currHealth = baseHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.textArea);
        // Modify to your needs...
        style.normal.textColor = Color.white;
        style.fontSize = 14;
        style.alignment = TextAnchor.MiddleCenter;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        screenPos.y = Screen.height - screenPos.y;
        float yPos = 10;
        GUI.Label(new Rect(screenPos.x - 60, screenPos.y - yPos, 100, 30), "HP: "+currHealth+"/"+baseHealth, style);    
    }

    public int getBaseHealth() {
        return baseHealth;
    }

    public int getCurrHealth()
    {
        return currHealth;
    }

    public void setCurrHealth(int currHealth)
    {
        this.currHealth = currHealth;
        if (this.currHealth <= 0) {
            this.currHealth = 0;
        }
    }

    public bool isDestroyed() {
        return destroyed;
    }

    public void setDestroyed(bool destroyed) {
        this.destroyed = destroyed;
    }
}
