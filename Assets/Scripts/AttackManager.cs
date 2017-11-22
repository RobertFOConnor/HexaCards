using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {

    List<Hex> attackingUnits;
    public Hex currAttacker;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    List<Hex> getAttackers(List<Hex> grid)
    {
        List<Hex> attackers = new List<Hex>();

        for (int i = grid.Count - 1; i >= 0; i--)
        {
            Hex hex = grid[i];
            Unit unit = hex.getUnit();
            if (unit != null)
            {
                unit.onTurnOver();
                if (unit.getCurrCountdown() <= 0)
                {
                    attackers.Add(hex);
                }
            }
        }
        return attackers;
    }

    void doAttack(Hex hex, bool isPlayer)
    {
        Unit unit = hex.getUnit();

        int y = hex.y;
        bool hasEnemyTarget = false;

        for (int x = 0; x < 3; x++)
        {//FIND PREY
            Hex target;
            string objectName = "Hex_";
            if (isPlayer)
            {
                objectName = "Hex_e_";     
            }
            objectName += (2 - x) + "_" + y;
            target = GameObject.Find(objectName).GetComponent<Hex>();
            
            if (target.unit != null)
            {     
                hasEnemyTarget = true;
                unit.startAttack(target);
                break;
            }
           
        }

        if (!hasEnemyTarget)
        {//NO ENEMY,..ATTACK THEIR SHIP!!
            Ship targetship;
            if (isPlayer)
            {
                targetship = GameObject.Find("Ship_e_" + y).GetComponent<Ship>();
            }
            else
            {
                targetship = GameObject.Find("Ship_" + y).GetComponent<Ship>();
            }
            
            unit.startAttack(targetship);
        }        
    }

    public bool isThereAnyAttackingUnits(List<Hex> grid, bool isPlayer) {
        attackingUnits = new List<Hex>();
        attackingUnits = getAttackers(grid);
        return isThereNextAttacker(isPlayer);
    }

    public bool isThereNextAttacker(bool isPlayer) {
        if (attackingUnits.Count > 0)
        {
            currAttacker = attackingUnits[0];

            doAttack(currAttacker, isPlayer);
            attackingUnits.RemoveAt(0);
            return true;
        }
        return false;
    }

    public bool currAttackerIsFinished() {
        return !currAttacker.getUnit().isAttacking();
    }
}
