using UnityEngine;
using System.Reflection;

public class Unit : MonoBehaviour
{
    int y;

    private Animator anim;
    private int id;
    private string name;
    private int baseAttack, currAttack, tempAttack;
    private int baseHealth, currHealth, tempHealth;
    private int baseCountdown, currCountdown;
    private int baseMove, currMove;
    private bool owner;
    
    private bool relentless = false;
    public Hex attackTarget;
    public Ship attackShip;

    public Vector3 hexPos;
    public Vector3 destination;
    public float speed = 4;
    public float retreatSpeed = 10;
    private CardEffects[] effects;
    private string[] specials;

    private Map map;
    private Player player;
    private Context context;


    public void initUnit(int id, string name, int baseAttack, int baseHealth, int baseCountdown, int baseMove, Player player, CardEffects[] effects, int y)
    {
        this.id = id;
        this.name = name;
        this.baseAttack = baseAttack;
        this.baseHealth = baseHealth;
        this.baseCountdown = baseCountdown;
        this.baseMove = baseMove;
        this.player = player;
        this.owner = player.isPlayer();
        this.effects = effects;

        tempAttack = currAttack = baseAttack;
        tempHealth = currHealth = baseHealth;
        currCountdown = baseCountdown;
        currMove = baseMove;

        relentless = hasEffect("relentless");

        if (hasEffect("haste")) {
            currCountdown = 0;
        }
        onSummon();

        context = new Context();
        context.setState(new IdleState());
    }

    // Use this for initialization
    void Start()
    {
        destination = transform.position;
        hexPos = destination;
        anim = GetComponent<Animator>();
    }

    public void faceEnemyDirection(bool isPlayer1)
    {
        int rotation = 90;
        if (!isPlayer1)
        {
            rotation *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);//Reverse enemy direction
        }
        transform.Rotate(new Vector3(0, rotation, 0), Space.World);//Face enemy
    }

    public void onStartTurn()
    {
        setCurrCountdown(getCurrCountdown() - 1);
        tempAttack = currAttack;
        tempHealth = currHealth;
        currMove = baseMove;
    }

    void Update()
    {
        Vector3 dir = destination - transform.position;
        Vector3 velocity = dir.normalized * speed * Time.deltaTime;

        // Make sure the velocity doesn't actually exceed the distance we want.
        velocity = Vector3.ClampMagnitude(velocity, dir.magnitude);

        transform.Translate(velocity, Space.World);

        context.getState().update(this);
    }

    class Context {

        private State state;

        public State getState() {
            return state;
        }

        public void setState(State state) {
            this.state = state;
        }

    }

    private interface State {
        void update(Unit unit);
    }

    private class IdleState: State {

        public void update(Unit unit) {
            if (unit.hasReachedDestination())
            {
                unit.hexPos = unit.transform.position;
            }
        }
    }

    class RunningState : State
    {

        public void update(Unit unit)
        {
            if (unit.hasReachedDestination())
            {
                unit.context.setState(new StrikingState());                
            }
        }
    }

    class StrikingState : State
    {

        public void update(Unit unit)
        {
            unit.anim.SetBool("isAttacking", false);
            unit.onAttack();
        }
    }

    class RetreatingState : State
    {

        public void update(Unit unit)
        {
            unit.speed = unit.retreatSpeed;
            if (unit.hasReachedDestination())
            {
                unit.speed = 4;
                unit.context.setState(new IdleState());
            }
        }
    }

    bool hasReachedDestination()
    {
        return transform.position == destination;
    }

    void onSummon()
    {
        applyEffects(MethodBase.GetCurrentMethod().Name);
    }

    public void doAttack()
    {

        bool hasEnemyTarget = false;

        for (int x = 0; x < 3; x++)
        {//FIND PREY
            Hex target;
            string objectName = "Hex_";
            if (owner)
            {
                objectName = "Hex_e_";
            }
            objectName += (2 - x) + "_" + y;
            target = GameObject.Find(objectName).GetComponent<Hex>();

            if (target.unit != null)
            {
                destination = target.transform.position;
                attackTarget = target;
                hasEnemyTarget = true;
                break;
            }

        }

        if (!hasEnemyTarget)
        {//NO ENEMY,..ATTACK THEIR SHIP!!
            Ship targetship;
            if (owner)
            {
                targetship = GameObject.Find("Ship_e_" + y).GetComponent<Ship>();
            }
            else
            {
                targetship = GameObject.Find("Ship_" + y).GetComponent<Ship>();
            }
            destination = targetship.transform.position;
            attackShip = targetship;
        }
    }

    void onAttack()
    {
        destination = hexPos;
        currCountdown = baseCountdown;

        if (attackTarget != null)
        {
            attackTarget.getUnit().onHit(tempAttack);
            if (attackTarget.getUnit().currHealth <= 0)
            {
                onKill();
            }
        }
        else
        {
            onAttackShip();
        }
        applyEffects(MethodBase.GetCurrentMethod().Name);
        attackTarget = null;
        attackShip = null;
        context.setState(new RetreatingState());
    }

    void onAttackShip()
    {
        attackShip.setCurrHealth(attackShip.getCurrHealth() - currAttack);
        applyEffects(MethodBase.GetCurrentMethod().Name);
        if (attackShip.getCurrHealth() <= 0 && !attackShip.isDestroyed())
        {
            onKillShip();
        }
    }

    void onKill()
    {
        Destroy(attackTarget.unit); //DESTROY UNIT
        applyEffects(MethodBase.GetCurrentMethod().Name);
    }

    void onKillShip()
    {
        attackShip.setDestroyed(true);
        applyEffects(MethodBase.GetCurrentMethod().Name);
    }

    void onHit(int damage)
    {
        applyEffects(MethodBase.GetCurrentMethod().Name);
        currHealth -= damage;
    }

    public void onDestroyed()
    {
        applyEffects(MethodBase.GetCurrentMethod().Name);
    }

    public void onMove() {
        currMove--;
        applyEffects(MethodBase.GetCurrentMethod().Name);
    }

    public bool isAttacking() {
        return !(context.getState() is IdleState);
    }

    void applyEffects(string trigger)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            CardEffects effect = effects[i];

            if (effect.trigger.Equals(trigger))
            {
                string stat = effect.stat;
                int amount = effect.amount;

                switch (stat)
                {
                    case "attack":
                        currAttack += amount;
                        break;
                    case "health":
                        currHealth += amount;
                        break;
                    case "countdown":
                        currCountdown += amount;
                        break;
                    case "draw":
                        player.drawCard();
                        break;
                    case "resources":
                        player.addResources(amount);
                        break;
                }
            }
        }
    }

    bool hasEffect(string effectName)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            CardEffects effect = effects[i];

            if (effect.trigger.Equals(effectName))
            {
                return true;
            }
        }
        return false;
    }


    public void startAttack(Hex target) {
        destination = target.transform.position;
        attackTarget = target;
        anim.SetBool("isAttacking", true);
        context.setState(new RunningState());
    }

    public void startAttack(Ship target)
    {
        destination = target.transform.position;
        attackShip = target;
        context.setState(new RunningState());
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
        GUI.Label(new Rect(screenPos.x - 60, screenPos.y - yPos, 100, 30), "A: " + tempAttack + " " + "H: " + tempHealth, style);
        GUI.Label(new Rect(screenPos.x - 60, screenPos.y - yPos + 30, 100, 30), "C: " + currCountdown, style);
        //GUI.Label(new Rect(screenPos.x - 80, screenPos.y + 3 + 20, 300, 50), "Health: "+currHealth, style);
    }

    public int getId()
    {
        return id;
    }

    public string getName()
    {
        return name;
    }

    public int getBaseAttack()
    {
        return baseAttack;
    }

    public int getCurrAttack()
    {
        return currAttack;
    }

    public int getBaseHealth()
    {
        return baseHealth;
    }

    public int getCurrHealth()
    {
        return currAttack;
    }

    public void setCurrHealth(int currHealth)
    {
        this.currHealth = currHealth;
    }

    public int getBaseCountdown()
    {
        return baseCountdown;
    }

    public int getCurrCountdown()
    {
        return currCountdown;
    }

    public void setCurrCountdown(int currCountdown)
    {
        this.currCountdown = currCountdown;
        if (currCountdown < 0) {
            currCountdown = 0;
        }
    }

    public int getBaseMove()
    {
        return baseMove;
    }

    public int getCurrMove()
    {
        return currMove;
    }

    public void setCurrMove(int currMove)
    {
        this.currMove = currMove;
    }

    public bool getOwner()
    {
        return owner;
    }

    public void onTurnOver()
    {

    }

    public virtual void onUnitDestroyed()
    {

    }
}
