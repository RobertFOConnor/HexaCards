using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{

    public GameObject cardPrefab;
    public RectTransform ParentPanel;
    public GameObject unitPrefab;

    private List<GameObject> cards = new List<GameObject>();
    private float cardSelectOffset = 70f;
    private GameObject selectedCard;
    private GameObject ourHex, selectedHex;

    Color hoverColor = Color.grey;
    Color selectedColor = Color.yellow;

    private string STATE = "";

    void Start() {
        for (int i = 0; i < 3; i++)
        {
            GameObject goButton = Instantiate(cardPrefab);
            goButton.transform.SetParent(ParentPanel, false);
            goButton.transform.localScale = new Vector3(1, 1, 1);
            goButton.GetComponent<CardUI>().title.text = "Card " + i;

            Button tempButton = goButton.GetComponent<Button>();
            cards.Add(goButton);

            tempButton.onClick.AddListener(() => cardClicked(goButton));
        }
    }

    void Update()
    {

        // Is the mouse over a Unity UI Element?
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        // could also check if game is paused?
        // if main menu is open?

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = LayerMask.NameToLayer("Hex");
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject ourHitObject = hitInfo.collider.transform.parent.gameObject;

            //Debug.Log("Clicked On: " + ourHitObject.name);

            // So...what kind of object are we over?
            if (ourHitObject.GetComponent<Hex>() != null)
            {
                // Ah! We are over a hex!
                MouseOver_Hex(ourHitObject);
            }
            
            //else if (ourHitObject.GetComponent<Unit>() != null) {
            // We are over a unit!
            //MouseOver_Unit(ourHitObject);

            //}


        }
        else
        {
            if (ourHex != null)
            {
                hoverHex(false);
            }
        }

        // If this were an FPS, what you'd do is probably something like this:
        // (This fires a ray out from the center of the camera's view.)
        //Ray fpsRay = new Ray( Camera.main.transform.position, Camera.main.transform.forward );
    }

    void MouseOver_Hex(GameObject ourHitObject)
    {
        Debug.Log("Raycast hit: " + ourHitObject.name);
        
        if (ourHex == null) {
            ourHex = ourHitObject;
            hoverHex(true);
               
        }

        if (!ourHex.Equals(ourHitObject)) //If we're not still hovering on the same hex
        {
            hoverHex(false);
            ourHex = ourHitObject;
            hoverHex(true);
        }


        if (Input.GetMouseButtonDown(0))
        {

            // We have clicked on a hex.  Do something about it!
            // This might involve calling a bunch of other functions
            // depending on what mode you happen to be in, in your game.
            Unit unit = ourHex.GetComponent<Hex>().unit;

            if (STATE.Equals("UNIT_SELECTED"))
            {
                if (ourHex.Equals(selectedHex))
                {
                    STATE = "";
                    deselectHex();
                }
                else if (unit != null)
                {
                    deselectHex();
                    selectHex();
                }
                else if (!isEnemyHex())
                {
                    STATE = "";
                    ourHex.GetComponent<Hex>().unit = selectedHex.GetComponent<Hex>().unit;
                    selectedHex.GetComponent<Hex>().unit = null;
                    // If we have a unit selected, let's move it to this tile!
                    ourHex.GetComponent<Hex>().unit.destination = ourHitObject.transform.position;
                    deselectHex();
                }
            }
            else if (STATE.Equals("CARD_SELECTED")) {

                if (ourHex.GetComponent<Hex>().unit == null) {
                    if (!isEnemyHex())
                    {
                        STATE = "";
                        summonUnit(ourHex, unitPrefab);
                    }
                }


            } else {
                if (selectedHex == null)
                {
                    if (unit != null)
                    {
                        STATE = "UNIT_SELECTED";
                        selectHex();
                    }
                }
            }
        }
    }

    bool isEnemyHex() {
        return ourHex.name.StartsWith("Hex_e_");
    }

    void selectHex() {
        selectedHex = ourHex;
        selectedHex.GetComponent<Hex>().isSelected = true;
        changeColor(selectedHex, selectedColor);
    }

    void deselectHex() {
        if (selectedHex != null)
        {
            changeColor(selectedHex, Color.white);
            selectedHex.GetComponent<Hex>().isSelected = false;
            selectedHex = null;
        }
    }

    

    void hoverHex(bool selected)
    {
        Color c = hoverColor;
        if (!selected)
        {
            c = Color.white;
        }

        if (!ourHex.GetComponent<Hex>().isSelected)
        {
            changeColor(ourHex, c);
        }
        for (int i = 0; i < 3; i++)
        {
            GameObject enemyHex = GameObject.Find("Hex_e_" + i + "_" + ourHex.GetComponent<Hex>().y);
            if (enemyHex != null)
            {
                changeColor(enemyHex, c);
            }
        }
    }

    void changeColor(GameObject go, Color c)
    {
        MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();
        mr.material.color = c;
    }

    void cardClicked(GameObject goButton)
    {
        if (selectedCard != null)
        {
            setCardSelected(false);
        }
        deselectHex();
        STATE = "CARD_SELECTED";
        selectedCard = goButton;
        setCardSelected(true);
    }

    void setCardSelected(bool selected)
    {
        Vector3 pos = selectedCard.transform.position;
        if (selected)
        {
            pos.y += cardSelectOffset;
        }
        else
        {
            pos.y -= cardSelectOffset;
        }
        selectedCard.transform.position = pos;
    }

    void removeCard()
    {
        Destroy(selectedCard);
        cards.Remove(selectedCard);
        selectedCard = null;
    }

    public void summonUnit(GameObject hex, GameObject unitPrefab)
    {
        if (selectedCard != null)
        {
            Vector3 pos = hex.transform.position;
            GameObject unit_go = Instantiate(unitPrefab, pos, Quaternion.identity);
            //unit_go.transform.Rotate(new Vector3(0, 90, 0), Space.World);
            hex.GetComponent<Hex>().unit = unit_go.GetComponent<Unit>();
            removeCard();
        }
    }
}
