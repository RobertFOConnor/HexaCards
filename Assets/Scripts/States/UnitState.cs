
public class UnitState : GameState
{

    Context context;
    Hex unitHex;


    public UnitState(Context context, Hex unitHex)
    {
        this.context = context;
        this.unitHex = unitHex;
    }

    public void onHexClicked(Hex h)
    {
        //move unit
        if (!h.isEnemyHex()) //Moving unit
        {
            if (unitHex.isAdjacent(h.y, h.x) && unitHex.getUnit().getCurrMove() > 0)
            {
                h.unit = unitHex.unit;
                unitHex.unit = null;

                // If we have a unit selected, let's move it to this tile!
                h.getUnit().destination = h.transform.position;
                h.getUnit().onMove();
            }
        }
        unitHex.setSelected(false);
        setContext(new NullState(context));
    }


    public void onUnitClicked(Hex h)
    {
        setContext(new UnitState(context, h)); //set unit state
    }

    public void onCardClicked(Card c, Player player)
    {
        //set card state
        setContext(new CardState(context, c, player)); //set unit state
    }

    public void setContext(GameState state)
    {
        context.setGameState(state);
    }
}