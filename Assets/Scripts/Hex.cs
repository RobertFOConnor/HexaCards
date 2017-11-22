using UnityEngine;
using System.Collections;

public class Hex : MonoBehaviour
{

    // Our coordinates in the map array
    public GameObject unit;
    public string name;
    public int x;
    public int y;
    private bool selected = false;
    private bool hovering = false;
    public Vector3 pos;
    public MeshRenderer mr;
    Color hoverColor = Color.white;
    Color selectedColor = Color.cyan * Mathf.LinearToGammaSpace(400f);
    Color unselectedColor = new Color(1f, 1f, 1f, 50f / 255f);

    public Hex[] GetNeighbours()
    {

        // So if we are at x, y -- the neighbour to our left is at x-1, y
        GameObject leftNeighbour = GameObject.Find("Hex_" + (x - 1) + "_" + y);

        // Right neighbour is also easy to find.
        GameObject right = GameObject.Find("Hex_" + (x + 1) + "_" + y);

        // The problem is that our neighbours to our upper-left and upper-right
        // might be at x-1 and x, OR they might be at x and x+1, depending
        // on whether we our on an even or odd row (i.e. if y % 2 is 0 or 1)

        return null;
    }

    void start() {
        hoverColor.a = 0.5f;
        selectedColor.a = 0.5f;
    }


    void update() {

    }


    public Unit getUnit() {
        if (unit != null)
        {
            return unit.GetComponent<Unit>();
        }
        return null;
    }

    public bool isSelected() {
        return selected;
    }

    public void setSelected(bool s) {
        selected = s;
        if (selected)
        {
            changeColor(selectedColor);
        }
        else {
            changeColor(unselectedColor);
        }
    }

    public void seHovering(bool hov)
    {
        hovering = hov;
        if (hovering)
        {
            changeColor(hoverColor);
        }
        else
        {
            changeColor(unselectedColor);
        }
    }

    public void placeCard(Player p, Card c)
    {

        GameObject variableForPrefab = (GameObject)Resources.Load("Prefabs/Unit_" + c.getId(), typeof(GameObject));
        GameObject unit_go = Instantiate(variableForPrefab, pos, Quaternion.identity);
        unit = unit_go;
        Unit u = getUnit();
        u.initUnit(c.getId(), c.getName(), c.getAttack(), c.getHealth(), c.getCountdown(), 1, p, c.effects, y);
        u.faceEnemyDirection(p.isPlayer());
       
    }


void changeColor(Color c)
    {
        mr.material.color = c;
    }

    public bool isAdjacent(int newRow, int newCol)
    {
        int row = y;
        int col = x;
        

        int rowDiff = Mathf.Abs(newRow - row);
        int colDiff = Mathf.Abs(newCol - col);


        if (rowDiff == 0)
        { //Sideways Move
            if (colDiff == 1)
            {
                return true;
            }
        }


        if (rowDiff == 1)
        { //Vertical Move
            if (colDiff == 0)
            {
                return true;
            }

            if (colDiff == 1)
            { //Diagonal

                if (row % 2 == 0)
                {
                    if (col % 2 == 0)
                    {
                        if (newCol > col)
                        {
                            return true;
                        }
                    }
                    else if (col % 2 != 0)
                    {
                        if (newCol > col)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                { //Odd Row
                    if (col % 2 != 0)
                    {
                        if (newCol < col)
                        {
                            return true;
                        }
                    }
                    else if (col % 2 == 0)
                    {
                        if (newCol < col)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool isEnemyHex()
    {
        return name.StartsWith("Hex_e_");
    }

}
