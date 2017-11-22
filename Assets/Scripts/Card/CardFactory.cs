using UnityEngine;
using System.IO;

public class CardFactory
{
    public static Card buildCard(int id)
    {
        string cardPath = Application.streamingAssetsPath + "/card_" + id + ".json";
        string jsonString = File.ReadAllText(cardPath);
        Card card = JsonUtility.FromJson<Card>(jsonString);
        CardEffects[] effects = JsonHelper.FromJson<CardEffects>(jsonString);
        card.effects = effects;

        return card;
    }

}
