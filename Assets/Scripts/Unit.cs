using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    public int id;
    public string name;
    public int baseAttack, currAttack;
    public int baseHealth, currHealth;
    public int baseCountdown, currCountdown;
    public int baseMove, currMove;

    public bool isAttacking = false;

    public Vector3 hexPos;
	public Vector3 destination;
	float speed = 6;

    public void initUnit(int id, string name, int baseAttack, int baseHealth, int baseCountdown, int baseMove) {
        this.id = id;
        this.name = name;
        this.baseAttack = baseAttack;
        this.baseHealth = baseHealth;
        this.baseCountdown = baseCountdown;
        this.baseMove = baseMove;

        currAttack = baseAttack;
        currHealth = baseHealth;
        currCountdown = baseCountdown;
        currMove = baseMove;
    }

	// Use this for initialization
	void Start () {
        destination = transform.position;
        hexPos = destination;
    }

    public void faceEnemyDirection(bool isPlayer1) {
        if (isPlayer1)
        {
            transform.Rotate(new Vector3(0, 90, 0), Space.World);
        }
        else {
            transform.Rotate(new Vector3(0, -90, 0), Space.World);
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
	
	// Update is called once per frame
	void Update () {
		// Move towards our destination

		// NOTE!  This just moves directly there, but really you'd want to feed
		// this into a pathfinding system to get a list of sub-moves or something
		// to walk a reasonable route.
		// To see how to do this, look up my TILEMAP tutorial.  It does A* pathfinding
		// and throughout the video I explain how you can apply that pathfinding
		// to hexes.

		Vector3 dir = destination - transform.position;
		Vector3 velocity = dir.normalized * speed * Time.deltaTime;

		// Make sure the velocity doesn't actually exceed the distance we want.
		velocity = Vector3.ClampMagnitude( velocity, dir.magnitude );

		transform.Translate(velocity, Space.World);

        if (transform.position == destination)
        {
            if (isAttacking)
            {
                isAttacking = false;
                destination = hexPos;
                currCountdown = baseCountdown;
            }
            else {
                hexPos = destination;
            }
        }
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
        GUI.Label(new Rect(screenPos.x-60, screenPos.y- yPos, 100, 30), "A: "+currAttack+" "+ "H: " + currHealth, style);
        GUI.Label(new Rect(screenPos.x - 60, screenPos.y - yPos+30, 100, 30), "C: " + currCountdown, style);
        //GUI.Label(new Rect(screenPos.x - 80, screenPos.y + 3 + 20, 300, 50), "Health: "+currHealth, style);
    }
}
