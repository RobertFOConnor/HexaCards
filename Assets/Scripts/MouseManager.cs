using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class MouseManager : MonoBehaviour
{

    public GameObject cardPrefab;
    public RectTransform ParentPanel;
    public Text playerName, resourcesText;
    public Button sacResButton, sacCardButton, endTurnButton;
    public Map map;

    private List<GameObject> cards = new List<GameObject>();
    private float cardSelectOffset = 70f;
    private GameObject selectedCard;
    private GameObject ourHex, selectedHex;

    Color hoverColor = Color.white;
    Color selectedColor = Color.cyan * Mathf.LinearToGammaSpace(400f);
    Color unselectedColor = new Color(1f, 1f, 1f, 50f/255f);

    private string STATE = "";

    private Player player;

    void Start()
    {
        player = new Player("Bobby", Card.getTestCards(), true);

        hoverColor.a = 0.5f;
        selectedColor.a = 0.5f;

        for (int i = 0; i < player.getHand().Count; i++)
        {
            Card card = player.getHand()[i];
            addCardGUI(card);
        }


        playerName.text = player.getName();
        resourcesText.text = player.getBaseRes()+"/"+player.getCurrRes();

        Button sacRes = sacResButton.GetComponent<Button>();
        sacRes.onClick.AddListener(sacForResClicked);

        Button sacCards = sacCardButton.GetComponent<Button>();
        sacCards.onClick.AddListener(sacForCardsClicked);

        Button endTurn = endTurnButton.GetComponent<Button>();
        endTurn.onClick.AddListener(endTurnClicked);

    }


    void addCardGUI(Card card) {
        GameObject goButton = Instantiate(cardPrefab);
        goButton.transform.SetParent(ParentPanel, false);
        goButton.transform.localScale = new Vector3(1, 1, 1);
        goButton.GetComponent<CardUI>().title.text = card.getName();
        goButton.GetComponent<CardUI>().resources.text = card.getCost() + "";

        Button tempButton = goButton.GetComponent<Button>();
        cards.Add(goButton);

        tempButton.onClick.AddListener(() => cardClicked(goButton));
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
            try
            {
                GameObject ourHitObject = hitInfo.collider.transform.parent.gameObject;

                if (ourHitObject.GetComponent<Hex>() != null)
                {
                    MouseOver_Hex(ourHitObject);
                }
            }
            catch (NullReferenceException e) {
                if (ourHex != null)
                {
                    hoverHex(false);
                }
            }
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
                else if (!isEnemyHex()) //Moving unit
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

                if (ourHex.GetComponent<Hex>().unit == null)
                {
                    if (!isEnemyHex() && getSelCard().getCost() <= player.getCurrRes())
                    {
                        summonUnit(ourHex);
                    }

                    print(getSelCard().getCost());
                }
                else {
                    selectUnit(unit);
                }


            } else {
                if (selectedHex == null)
                {
                    selectUnit(unit);
                }
            }
        }
    }

    void selectUnit(Unit unit) {
        if (unit != null)
        {
            STATE = "UNIT_SELECTED";
            selectHex();
            setCardSelected(false);
            selectedCard = null;
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
            changeColor(selectedHex, unselectedColor);
            selectedHex.GetComponent<Hex>().isSelected = false;
            selectedHex = null;
        }
    }

    

    void hoverHex(bool selected)
    {
        Color c = hoverColor;
        if (!selected)
        {
            c = unselectedColor;
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

        print(cards.IndexOf(selectedCard));

        setCardSelected(true);

        if (ourHex != null)
        {
            hoverHex(false);
        }
    }

    void setCardSelected(bool selected)
    {
        if (selectedCard != null)
        {
            Vector3 pos = selectedCard.transform.position;
            if (selected)
            {
                pos.y += cardSelectOffset;
                if (!player.hasSacrificed())
                {
                    sacCardButton.enabled = true;
                    sacCardButton.enabled = true;
                }
            }
            else
            {
                pos.y -= cardSelectOffset;
                sacCardButton.enabled = false;
                sacCardButton.enabled = false;
            }
            selectedCard.transform.position = pos;
        }
    }

    void removeCard()
    {
        Destroy(selectedCard);
        cards.Remove(selectedCard);
        selectedCard = null;
    }

    public void summonUnit(GameObject hex)
    {
        if (selectedCard != null)
        {
            Vector3 pos = hex.transform.position;

            Card c = getSelCard();
            GameObject variableForPrefab = (GameObject)Resources.Load("Prefabs/Unit_" + c.getId(), typeof(GameObject));
            GameObject unit_go = Instantiate(variableForPrefab, pos, Quaternion.identity);
            hex.GetComponent<Hex>().unit = unit_go.GetComponent<Unit>();
            hex.GetComponent<Hex>().unit.initUnit(c.getId(), c.getName(), c.getAttack(), c.getHealth(), c.getCountdown(), 1);
            hex.GetComponent<Hex>().unit.faceEnemyDirection(true);

            pos = hex.transform.position;

            player.consumeCard(cards.IndexOf(selectedCard));
            updateResUI();
            removeCard();

            STATE = "";
        }
    }

    void sacForResClicked() {
        if (canSacrifice()) {
            player.sacrificeResource(cards.IndexOf(selectedCard));
            updateResUI();
            removeCard();
            printHand();
        }
    }

    void updateResUI() {
        resourcesText.text = player.getCurrRes() + "/" + player.getBaseRes();
    }

    void sacForCardsClicked()
    {
        if (canSacrifice())
        {
            player.sacrificeCards(cards.IndexOf(selectedCard));
            addCardGUI(player.getHand()[player.getHand().Count-2]);
            addCardGUI(player.getHand()[player.getHand().Count - 1]);
            removeCard();

            printHand();
        }
    }

    void printHand() {
        string hand = "";
        for (int i = 0; i < player.getHand().Count; i++)
        {
            hand += player.getHand()[i].getCost() + ",";
        }
        print(hand);
    }

    void endTurnClicked() {
        setCardSelected(false);
        selectedCard = null;
        player.endTurn();

        //ATTACK PHASE WOULD GO HERE...

        for (int i = 0; i < map.userGrid.Count; i++)
        {
            Unit unit = map.userGrid[i].GetComponent<Hex>().unit;
            if (unit != null)
            {
                unit.currCountdown--;
                if (unit.currCountdown <= 0) {
                    unit.isAttacking = true;


                    int y = map.userGrid[i].GetComponent<Hex>().y;
                    bool hasEnemyTarget = false;

                    for (int x = 0; x < 3; x++) {//FIND PREY
                        Hex target = GameObject.Find("Hex_e_" + x + "_" + y).GetComponent<Hex>();
                        if (target.unit != null) {
                            unit.destination = target.transform.position;
                            hasEnemyTarget = true;
                            break;
                        }
                    }

                    if (!hasEnemyTarget) {//NO ENEMY,..ATTACK THEIR SHIP!!
                        Ship targetship = GameObject.Find("Ship_e_"+y).GetComponent<Ship>();
                        unit.destination = targetship.transform.position;
                    }
                   
                }

            }
        }

        player.startTurn();
        addCardGUI(player.getHand()[player.getHand().Count - 1]);
        updateResUI();
    }

    bool canSacrifice() {
        return selectedCard != null;
    }

    Card getSelCard() {
        return player.getHand()[cards.IndexOf(selectedCard)];
    }
}
