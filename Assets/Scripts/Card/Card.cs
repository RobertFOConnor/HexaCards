using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Card 
{
    public int id;
    public string name;
    public string description;
    public int resourceCost;
    public string type;
    public int rarity;
    public CardEffects[] effects;

    public int attack;
    public int health;
    public int countdown;


    public int getId() {
        return id;
    }

    public string getName()
    {
        return name;
    }

    public string getDescription() {
        return description;
    }

    public int getCost() {
        return resourceCost;
    }

    public string getType()
    {
        return type;
    }

    public int getRarity() {
        return rarity;
    }

    public int getAttack() {
        return attack;
    }

    public int getHealth()
    {
        return health;
    }

    public int getCountdown()
    {
        return countdown;
    }

}
