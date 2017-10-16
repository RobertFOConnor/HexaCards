using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{

    private int id;
    private string name;
    private string description;
    private int resourceCost;
    private int rarity;

    private int attack;
    private int health;
    private int countdown;


    public Card(int id, string name, string description, int resourceCost, int rarity, int attack, int health, int countdown) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.resourceCost = resourceCost;
        this.rarity = rarity;
        this.attack = attack;
        this.health = health;
        this.countdown = countdown;
    }


    private void Start()
    {

    }

    private void Update()
    {

    }

    public int getId() {
        return id;
    }

    public string getName() {
        return name;
    }

    public string getDescription() {
        return description;
    }

    public int getCost() {
        return resourceCost;
    }

    public int getRarity() {
        return rarity;
    }

    public int getCountdown()
    {
        return countdown;
    }


    public int getHealth()
    {
        return health;
    }


    public int getAttack()
    {
        return attack;
    }


    public static List<Card> getTestCards() {
        List<Card> cards = new List<Card>();
        cards.Add(new Card(1, "Monster man", "He's a monster who is also a man.", 3, 1, 3, 3, 2));
        cards.Add(new Card(2, "space man", "He's a spaceman who is also a man.", 2, 1, 1, 2, 2));
        return cards;
    }
}
