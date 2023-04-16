using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TMPro.TextMeshProUGUI gText, pText, bText;
    private GameObject thermobar, genebar;
    private readonly float[] thermoY = { -3.475f, -3.195f, -2.915f, -2.635f, -2.355f, -2.075f, -1.795f,
                                         -1.515f, -1.235f, -0.955f, -0.675f, -0.395f, -0.115f, 0 }, //.28 diff
                             thermoS = { .225f, .785f, 1.345f, 1.905f, 2.465f, 3.025f, 3.585f,
                                         4.145f, 4.705f, 5.265f, 5.825f, 6.385f, 6.945f, 7.175f }, //.56 diff
                             radiY = { -3.395f, -3.075f, -2.755f, -2.435f, -2.115f, -1.795f,
                                       -1.475f, -1.155f, -0.835f, -0.515f, -0.195f}, //.32 diff
                             radiS = { .38f, 1.02f, 1.66f, 2.3f, 2.94f, 3.58f,
                                       4.22f, 4.86f, 5.5f, 6.14f, 6.78f}; //.64 diff
    private int thermoLevel = 1, radiLevel = 0, endLevel = 0;
    private PlayerController playerController;


    // Start is called before the first frame update
    void Start()
    {
        thermobar = GameObject.Find("Thermobar");
        
        gText = GameObject.Find("Glucose Text").GetComponent<TMPro.TextMeshProUGUI>();
        pText = GameObject.Find("Phosphate Text").GetComponent<TMPro.TextMeshProUGUI>();
        bText = GameObject.Find("Base Pair Text").GetComponent<TMPro.TextMeshProUGUI>();
        ResourceManager.SetUpDeck();

        playerController = GameObject.Find("Player Microbe").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHandDisplay()
    {
        List<Resource> playerHand = ResourceManager.GetPlayerHand();
        int gCount = 0, pCount = 0, bCount = 0;
        for (int i = 0; i < playerHand.Count; i++)
        {
            Resource card = playerHand[i];
            if (card.ToString().Equals(Resource.TYPES[1]))
            {
                gCount++;
            }
            else if (card.ToString().Equals(Resource.TYPES[2]))
            {
                pCount++;
            }
            else if (card.ToString().Equals(Resource.TYPES[3]))
            {
                bCount++;
            }
        }
        gText.text = "" + gCount;
        pText.text = "" + pCount;
        bText.text = "" + bCount;
    }

    /* Easy dev-test version for raising the thermorbar, proper implementation will come
     public void UpdateThermobar(int diff)
    {
        thermoLevel += diff;
        thermobar.transform.localPosition = new Vector3(thermobar.transform.localPosition.x,
                                                        thermoY[thermoLevel],
                                                        thermobar.transform.localPosition.z);
        thermobar.transform.localScale = new Vector3(thermobar.transform.localScale.x,
                                                     thermoS[thermoLevel],
                                                     thermobar.transform.localScale.z);
    }*/

    public bool CanRaiseTemp(int numSteps)
    {
        return (thermoLevel + numSteps) > 0 && (thermoLevel + numSteps) < 13;
    }
    
    public void RaiseThermobar(int numSteps)
    {
        endLevel = thermoLevel + numSteps;
        InvokeRepeating("ThermobarAnim", 0, 1/60f);
    }

    private void ThermobarAnim()
    {
        CancelInvoke();
        playerController.SetWaitingForAnim(false);
    }

    public bool CanRaiseGene(int numSteps)
    {
        return (radiLevel + numSteps) > 0 && (radiLevel + numSteps) < 11;
    }

    public void RaiseRadibar(int numSteps)
    {
        endLevel = thermoLevel + numSteps;
        InvokeRepeating("RadibarAnim", 0, 1/60f);
    }

    private void RadibarAnim()
    {
        CancelInvoke();
        playerController.SetWaitingForAnim(false);
    }
}
