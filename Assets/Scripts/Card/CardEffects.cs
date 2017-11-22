using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEffects {

    public string trigger; //Type of effect (onSummoned, onDestroyed, onUnitPlayed...).
    public string stat; //What stat is changed by the effect.
    public int amount; //How much is the stat changed.
}
