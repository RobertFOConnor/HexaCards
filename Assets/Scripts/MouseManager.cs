using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class MouseManager : MonoBehaviour
{

    public Text playerName, resourcesText;
    public Text enemyName, enemyResourcesText;
    public Button endTurnButton;
    public Map map;
    public SacrificeMenu sacMenu;
    public HandCardsUI handUI;

    private Hex ourHex, selectedHex;
    public Camera cam;
    public AttackManager attackManager;

    private static string STATE = "";

    private Player player, enemyPlayer;
    private Context context = new Context();

    void Start()
    {
        List<Card> deck = new List<Card>();
        List<Card> edeck = new List<Card>();

        for (int i = 1; i <= 10; i++) {
            deck.Add(CardFactory.buildCard(i));
            edeck.Add(CardFactory.buildCard(i));
        }


        player = new Player("Bobby", deck, true);
        enemyPlayer = new Player("Jack", edeck, false);

        player.setHandUI(handUI);
        sacMenu.initVars(player);

        playerName.text = player.getName();
        enemyName.text = enemyPlayer.getName();

        Button endTurn = endTurnButton.GetComponent<Button>();
        endTurn.onClick.AddListener(endTurnClicked);

        context.setGameState(new NullState(context));
    }

    void Update()
    {
        if (STATE == "ATTACK_PHASE" || STATE == "ENEMY_ATTACK_PHASE")
        {
            if (attackManager.currAttackerIsFinished())
            {              
                bool myAttackPhase = (STATE == "ATTACK_PHASE");
                if (!attackManager.isThereNextAttacker(myAttackPhase))
                {
                    STATE = "";
                    if (!myAttackPhase)
                    {
                        startPlayerTurn();
                    }
                    else
                    {
                        startEnemyTurn();
                    }                  
                }
            }
        }


        // Is the mouse over a Unity UI Element?
        if (EventSystem.current.IsPointerOverGameObject())
        {
            unHoverHex();
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

                Hex hex = ourHitObject.GetComponent<Hex>();
                if (hex != null)
                {
                    MouseOver_Hex(hex);
                }
            }
            catch (NullReferenceException e)
            {
                unHoverHex();
            }
        }
        else
        {
            unHoverHex();
        }

        // If this were an FPS, what you'd do is probably something like this:
        // (This fires a ray out from the center of the camera's view.)
        //Ray fpsRay = new Ray( Camera.main.transform.position, Camera.main.transform.forward );
    }

    void unHoverHex() {
        if (ourHex != null)
        {
            ourHex.seHovering(false);
        }
    }

    void MouseOver_Hex(Hex ourHitObject)
    {
        //Debug.Log("Raycast hit: " + ourHitObject.name);

        if (ourHex == null)
        {
            ourHex = ourHitObject;
            ourHex.seHovering(true);
        }

        if (!ourHex.Equals(ourHitObject)) //If we're not still hovering on the same hex
        {
            ourHex.seHovering(false);
            ourHex = ourHitObject;
            ourHex.seHovering(true);
        }


        if (Input.GetMouseButtonDown(0))
        {

            // We have clicked on a hex.  Do something about it!

            Unit unit = ourHex.getUnit();

            if (unit != null)
            {
                context.getGameState().onUnitClicked(ourHex);
            }
            else {
                context.getGameState().onHexClicked(ourHex);
            }
        }
    }

    public Context getContext() {
        return context;
    }



    public Player getPlayer()
    {
        return player;
    }

    void printHand()
    {
        string hand = "";
        for (int i = 0; i < player.getHand().Count; i++)
        {
            hand += player.getHand()[i].getCost() + ",";
        }
        print(hand);
    }

    void endTurnClicked()
    {
        player.endTurn();

        //ATTACK PHASE WOULD GO HERE...

        if (attackManager.isThereAnyAttackingUnits(map.userGrid, true))
        {
            STATE = "ATTACK_PHASE";
        }
        else
        {
            startEnemyTurn();
        }
    }

    bool testPlace = false;

    void startEnemyTurn()
    {
        //ENEMY TURN 
        enemyPlayer.startTurn();
        startTurnForUnits(map.enemyGrid);
        enemyPlayer.sacrificeResource(0);
        Card c = enemyPlayer.getHand()[0];
        if (c.getCost() <= enemyPlayer.getCurrRes() && testPlace == false)
        {
            //placeUnit(c, GameObject.Find("Hex_e_1_3").GetComponent<Hex>(), enemyPlayer);
            enemyPlayer.consumeCard(0);
            testPlace = true;
        }
        enemyPlayer.endTurn();

        if (attackManager.isThereAnyAttackingUnits(map.enemyGrid, false))
        {
            STATE = "ENEMY_ATTACK_PHASE";
        }
        else
        {
            startPlayerTurn();
        }
    }

    void startPlayerTurn()
    {
        player.startTurn();
        startTurnForUnits(map.userGrid);
    }

    void startTurnForUnits(List<Hex> grid)
    {
        for (int i = 0; i < grid.Count; i++)
        {
            Unit unit = grid[i].getUnit();
            if (unit != null)
            {
                unit.onStartTurn();
            }
        }
    }

    public static string getTurnState() {
        return STATE;
    }

    void OnGUI()
    {
        resourcesText.text = player.getCurrRes() + "/" + player.getBaseRes();
        enemyResourcesText.text = enemyPlayer.getBaseRes() + "/" + enemyPlayer.getCurrRes();
    }
}

public interface GameState {

    void onHexClicked(Hex h);
    void onUnitClicked(Hex h);
    void onCardClicked(Card c, Player player);
    void setContext(GameState state);
}
