
public class CardState : GameState
{

    Context context;
    Card c;
    Player player;

    public CardState(Context context, Card c, Player player)
    {
        this.context = context;
        this.c = c;
        this.player = player;
    }

    public void onHexClicked(Hex h)
    {
        //try playing card
        if (!h.isEnemyHex() && c.getCost() <= player.getCurrRes())
        {
            player.placeCard(c, h);
        }
        setContext(new NullState(context));
    }

    public void onUnitClicked(Hex h)
    {
        //set unit state
        setContext(new UnitState(context, h));
    }

    public void onCardClicked(Card c, Player player)
    {
        //set card state
        if (c != null) { 
            setContext(new CardState(context, c, player));
        } else {
            setContext(new NullState(context));
        }
    }

    public void setContext(GameState state)
    {
        context.setGameState(state);
    }
}