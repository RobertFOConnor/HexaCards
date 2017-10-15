using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

	public GameObject hexPrefab;

	// Size of the map in terms of number of hex tiles
	// This is NOT representative of the amount of 
	// world space that we're going to take up.
	// (i.e. our tiles might be more or less than 1 Unity World Unit)
	int width = 3;
	int height = 5;

	float xOffset = 0.882f;
	float zOffset = 0.764f;

	// Use this for initialization
	void Start () {


        setupPlayerTiles(true);
        setupPlayerTiles(false);
    }

    void setupPlayerTiles(bool isPlayer1) {

        int startX = 0;
        if (!isPlayer1) {
            startX += 4;
        }

        for (int x = startX; x < width+startX; x++)
        {
            for (int y = 0; y < height; y++)
            {

                float xPos = x * xOffset;
                string name = "";
                int xCoord = 0;

                // Are we on an even row?
                if (isPlayer1)
                {
                    if (y % 2 == 0)
                    {
                        xPos += xOffset / 2f;
                    }
                    xCoord = x;
                    name = "Hex_" + xCoord + "_" + y;
                }
                else {
                    if (y % 2 == 1)
                    {
                        xPos += xOffset / 2f;
                    }
                    xCoord = x - startX;
                    xCoord = width-1-xCoord;
                    name = "Hex_e_" + xCoord + "_" + y;
                    print(name);
                }

                GameObject hex_go = Instantiate(hexPrefab, new Vector3(xPos, 0, y * zOffset), Quaternion.identity);

                // Name the gameobject something sensible.
                hex_go.name = name;
                hex_go.layer = LayerMask.NameToLayer("Hex");

                // Make sure the hex is aware of its place on the map
                hex_go.GetComponent<Hex>().x = xCoord;
                hex_go.GetComponent<Hex>().y = y;

                // For a cleaner hierachy, parent this hex to the map
                hex_go.transform.SetParent(this.transform);

                // TODO: Quill needs to explain different optimization later...
                hex_go.isStatic = true;

            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
