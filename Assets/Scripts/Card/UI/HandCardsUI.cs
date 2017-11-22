using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HandCardsUI : MonoBehaviour {


    public RectTransform ParentPanel;
    public GameObject cardPrefab;
    public List<GameObject> cards = new List<GameObject>();
    private float cardSelectOffset = 70f;
    public GameObject selectedCard;
    public MouseManager gameManager;
    private Player player;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addCardGUI(Card card)
    {
        GameObject goButton = Instantiate(cardPrefab);
        goButton.transform.SetParent(ParentPanel, false);
        goButton.transform.localScale = new Vector3(1, 1, 1);
        CardUI cui = goButton.GetComponent<CardUI>();
        cui.title.text = card.getName();
        cui.resources.text = card.getCost() + "";
        cui.stats.text = "A:" + card.attack + " C:" + card.countdown + " H:" + card.health;
        Button tempButton = goButton.GetComponent<Button>();
        cards.Add(goButton);
        tempButton.onClick.AddListener(() => cardClicked(goButton));
    }

    public void cardClicked(GameObject goButton)
    {
        if (selectedCard != null)
        {
            setCardSelected(false);
        }
       
        selectedCard = goButton;
        setCardSelected(true);
        gameManager.getContext().getGameState().onCardClicked(getSelCard(), player);
    }

    public void setCardSelected(bool selected)
    {
        if (selectedCard != null)
        {
            Vector3 pos = selectedCard.transform.position;
            if (selected)
            {
                pos.y += cardSelectOffset;
            }
            else
            {
                pos.y -= cardSelectOffset;
            }
            selectedCard.transform.position = pos;
        }
    }

    public void removeCard()
    {
        Destroy(selectedCard);
        cards.Remove(selectedCard);
        selectedCard = null;
        gameManager.getContext().getGameState().onCardClicked(null, player);
    }

    public Player getPlayer() {
        return player;
    }

    public void setPlayer(Player player)
    {
        this.player = player;
    }

    public Card getSelCard()
    {
        return player.getHand()[cards.IndexOf(selectedCard)];
    }
}
