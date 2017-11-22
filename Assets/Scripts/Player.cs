using System.Collections.Generic;

public class Player {

    private List<Card> deck;
    private List<Card> currHand;
    private string name;
    private int baseRes, currRes;
    private bool sacrificed = false;
    private bool isUser = false;

    public static string WAITING = "WAITING";
	public static string PLACING = "PLACING";
	public static string ATTACKING = "ATTACKING";
	private string state = PLACING;

    //Where in the deck to draw the card from
    private int drawPosition = 0;
    public HandCardsUI handUI;

    public Player(string name, List<Card> deck, bool isUser)
    {
        this.name = name;
        this.isUser = isUser;
        baseRes = 0;
        currRes = 0;
        this.deck = deck;

        if (!isUser)
        {
            drawFirstCards();
        }
   
    }

    public void setHandUI(HandCardsUI handUI) {
        this.handUI = handUI;
        this.handUI.setPlayer(this);
        drawFirstCards();
    }

    void drawFirstCards() {
        currHand = new List<Card>();
        for (int i = 0; i < 4; i++)
        {
            drawCard();
        }
    }


    public void drawCard()
    {
       
        currHand.Add(deck[drawPosition]);
        if (isUser)
        {
            handUI.addCardGUI(deck[drawPosition]);
        }

        drawPosition++;

        if (drawPosition >= deck.Count)
        {
            //Full cycle of deck!
            drawPosition = 0;
        } 
    }

    public void placeCard(Card c, Hex h) {

        h.placeCard(this, c);
        consumeCard(handUI.cards.IndexOf(handUI.selectedCard));
        handUI.removeCard();
    }


    public string getName()
    {
        return name;
    }

    public void setState(string state)
    {
        this.state = state;
    }

    public List<Card> getHand()
    {
        return currHand;
    }

    public int getBaseRes()
    {
        return baseRes;
    }

    public int getCurrRes()
    {
        return currRes;
    }

    public void setCurrRes(int currRes)
    {
        this.currRes = currRes;
    }

    public bool hasSacrificed()
    {
        return sacrificed;
    }

    public bool isPlayer() {
        return isUser;
    }

    public void sacrificeResource()
    {
        currHand.RemoveAt(handUI.cards.IndexOf(handUI.selectedCard));
        sacrificed = true;
        addResources(1);
        handUI.removeCard();
    }

    public void sacrificeCards()
    {
        currHand.RemoveAt(handUI.cards.IndexOf(handUI.selectedCard));
        sacrificed = true;
        drawCard();
        drawCard();
        handUI.removeCard();
    }

    public void sacrificeResource(int index)
    {
        currHand.RemoveAt(index);
        sacrificed = true;
        addResources(1);
    }

    public void sacrificeCards(int index)
    {
        currHand.RemoveAt(index);
        sacrificed = true;
        drawCard();
        drawCard();
    }

    public void startTurn()
    {
        this.state = PLACING;
        sacrificed = false;
        currRes = baseRes;
        drawCard();
    }

    public void endTurn()
    {
        if (isUser) {
            handUI.setCardSelected(false);
            handUI.selectedCard = null;
        }

        startAttack();
    }

    public void consumeCard(int index)
    {
        setCurrRes(currRes - currHand[index].getCost());
        currHand.RemoveAt(index);
    }

    public void addResources(int amount) {
        this.baseRes += amount;
        this.currRes += amount;
    }

    public void startAttack()
    {
        this.state = ATTACKING;
    }

    public bool isAttacking()
    {
        return state.Equals(Player.ATTACKING);
    }

    public bool isWaiting()
    {
        return state.Equals(Player.WAITING);
    }

    public bool isPlacing()
    {
        return state.Equals(Player.PLACING);
    }

    public bool canSacrifice()
    {
        return handUI.selectedCard != null && !hasSacrificed() && isPlacing();
    }
}
