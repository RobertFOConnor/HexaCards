using UnityEngine;
using UnityEngine.UI;

public class SacrificeMenu : MonoBehaviour {

    public Button sacResButton, sacCardButton;
    private Player player;

    // Use this for initialization
    void Start () {
        Button sacRes = sacResButton.GetComponent<Button>();
        sacRes.onClick.AddListener(sacForResClicked);

        Button sacCards = sacCardButton.GetComponent<Button>();
        sacCards.onClick.AddListener(sacForCardsClicked);
    }

    public void initVars(Player player) {
        this.player = player;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void sacForResClicked()
    {
        if (player.canSacrifice())
        {
            player.sacrificeResource();
            
        }
    }


    void sacForCardsClicked()
    {
        if (player.canSacrifice())
        {
            player.sacrificeCards();
        }
    }

    
}
