
public class NullState : GameState
{

    Context context;

    public NullState(Context context)
    {
        this.context = context;
    }

    public void onHexClicked(Hex h)
    {
        //do nothing?
    }

    public void onUnitClicked(Hex h)
    {
        //set unit state
        setContext(new UnitState(context, h));
    }

    public void onCardClicked(Card c, Player player)
    {
        //set card state
        setContext(new CardState(context, c, player));
    }

    public void setContext(GameState state)
    {
        context.setGameState(state);
    }
}