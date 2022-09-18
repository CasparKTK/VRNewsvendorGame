using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;
using Random = System.Random;


public class GameManager : MonoBehaviour
{
    //teleport
    public GameObject rig;
    
    //ui panel
    public GameObject Web;
    public GameObject panelStart;
    public GameObject panelPlay;
    public GameObject panelResult;
    public GameObject panelEnd;
    public GameObject panelChart;
    public GameObject panelCalc;
    //start panel
    public Text playDescription;
    //play panel
    public Text orderText;
    //result panel
    public Text orderedText;
    public Text demandText;
    public Text profitText;
    //end panel
    public Text averageOrderText;
    public Text averageProfitText; 
    public Text optimalProfitText;
    //record panel
    public Text pastOrderText;
    public Text pastDemandText;
    public Text roundText;
    //calculator panel
    public Text averageDemandText;
    public Text sdText;
    public Text optimalOrderText;
    public Text serviceLevelText;
    
    //variables in main game
    public int order;
    public int ordered;
    public int demand;
    public float profit;
    public int optimalOrder;
    public float optimalProfit;
    public List<int> numPad;
    //variables for calculator
    public int average;
    public int standardDeviation;
    public float serviceLevel;
    public int optimalOrderQuantity;
    public float totalProfit;
    public int totalOrdered;
    public int calmode;
    public Text calMode;
    public Text calInput;
    public List<int> calPad;

    //variables for record
    public int[] pastOrdered;
    public int[] pastDemand;
    

    //level and scenario control
    public int level;
    public int levelcap = 10;
    public List<int> progress;
    public int scenario;
    
    
    //audio variables
    public AudioClip clip;
    public AudioClip newspaper_clip;
    public AudioClip magazine_clip;
    public AudioClip instruction_clip;
    private AudioSource source;
    //online mode
    public bool onlineMode = true;
    public string user;
    public enum State{START, INIT, PLAY, RESULT , END}

    State _state;
    //Generate Gaussian Distribution for Demand
    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;
 
        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);
 
        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
 
        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }
    //Generate Random demand for each round
    public int RandomDemand(int mean, int sd)
    {
        float norm = RandomGaussian();
        int norm2;
         norm2= (int)(norm * sd + mean);
         return norm2;
    }
    //Calculate Profit after each round
    public float ProfitCalc(float cost, float price)
    {
        if (ordered >= demand)
        {
            profit = demand * price - cost * ordered;
        }
        else
        {
            profit = ordered * (price - cost);
        }
        return profit;
    }
    //Optimal Profit for given round
    public float OptimalProfitCalc(float cost, float price)
    {
        if (optimalOrder >= demand)
        {
            profit = demand * price - cost * optimalOrder;
        }
        else
        {
            profit = optimalOrder * (price - cost);
        }
        return profit;
    }
    //Calculator Buttons
    public void OnSLClicked()
    {
        calmode = 0;
        calMode.text = "Input Mode: Service Level";
    }
    public void OnAvrgClicked()
    {
        calmode = 1;
        calMode.text = "Input Mode: Average Demand";
    }
    public void OnSDClicked()
    {
        calmode = 2;
        calMode.text = "Input Mode: Standard Deviation";
    }
    
    public void ONnCal1Clicked()
    {
        
        calPad.Add(1);
        calInput.text = calInput.text+ 1  ;
    }
    public void ONnCal2Clicked()
    {
        
        calPad.Add(2);
        calInput.text = calInput.text+ 2  ;
    }
    public void ONnCal3Clicked()
    {
        
        calPad.Add(3);
        calInput.text = calInput.text+ 3  ;
    }
    public void ONnCal4Clicked()
    {
        
        calPad.Add(4);
        calInput.text = calInput.text+ 4  ;
    }
    public void ONnCal5Clicked()
    {
        
        calPad.Add(5);
        calInput.text = calInput.text+ 5  ;
    }
    public void ONnCal6Clicked()
    {
        
        calPad.Add(6);
        calInput.text = calInput.text+ 6  ;
    }
    
    public void ONnCal7Clicked()
        {
            
            calPad.Add(7);
            calInput.text = calInput.text+ 7  ;
        }
    public void ONnCal8Clicked()
    {
        
        calPad.Add(8);
        calInput.text = calInput.text+ 8  ;
    }
    public void ONnCal9Clicked()
    {
        
        calPad.Add(9);
        calInput.text = calInput.text+ 9  ;
    }
    public void ONnCal0Clicked()
    {
        if (calPad.Count != 0)
        {
            calPad.Add(0);
            calInput.text = calInput.text+ 0  ;
        }
            
    }

    public void OnCalEnterClicked()
    {
        int sum = 0;
        int multiplier = 1;
        for (int i = calPad.Count-1; i >= 0; i--)
        {
            sum += calPad[i] * multiplier;
            multiplier= multiplier * 10;
        }

        if (calmode ==0)
        {
            if (sum<=100)
            {
                serviceLevelText.text = sum.ToString() + "%";
                            serviceLevel = sum;
            }
            else
            {
                serviceLevelText.text = "Please Enter Value between 0-100";
            }
            
        }
        else if (calmode == 1)
        {
            averageDemandText.text = sum.ToString();
            average = sum;
        }
        else
        {
            sdText.text = sum.ToString();
            standardDeviation = sum;
        }
        calPad.Clear();
        calInput.text = "";
    }

    public void OnCalDelClicked()
    {
        calPad.Clear();
        calInput.text = "";
    } 
    
    public void OnCalClicked()
    {
        var curve = new MathNet.Numerics.Distributions.Normal();
        var zValue = curve.InverseCumulativeDistribution(serviceLevel/100);
        var ans = zValue * standardDeviation + average;
        optimalOrderQuantity = (int) Math.Round(ans,0);
        optimalOrderText.text = optimalOrderQuantity + "" ;
    }

    public void OnResetClicked()
    {
        serviceLevel = 0;
        average = 0;
        optimalOrderQuantity = 0;
        standardDeviation = 0;
        sdText.text = "0";
        serviceLevelText.text = "0%";
        averageDemandText.text = "0";
        optimalOrderText.text = "Optimal Order Quantity:";

    }
    //Leave result screen and start new round
    public void OnContinueClicked()
    {
        pastDemand = new int[levelcap];
            pastOrdered = new int[levelcap];
            SwitchState(State.INIT);
    }
    //quit game
    public void Exit()
    {
        StartCoroutine(Web.GetComponent<Web>().ReturnUser(user));
        Application.Quit();
    }
    //restart game
    public void RestartGame()
    {
        SceneManager.LoadScene("Newspaper Office");
    }
    //start game and teleport player into scene
    public void ONPlayClicked()
    {
        user = Web.GetComponent<Web>().user;
        teleport();
        SwitchState(State.INIT);
        
    }

    public  void teleport()
    {
        rig.transform.position = new Vector3(0.1f, 0, 0.1f);
    }
    //button for input order 
    public void ONEnterClicked()
    {
        int multiplier = 1;
        for (int i = numPad.Count-1; i >= 0; i--)
        {
            order += numPad[i] * multiplier;
            multiplier= multiplier * 10;
        }
        ordered = order;
        SwitchState(State.RESULT);
    }

    public void ONNextClicked()
    {
        totalOrdered += ordered;
        totalProfit += profit;
        pastDemand[level] = demand;
        pastOrdered[level] = ordered;
        pastDemandText.text += demand + "\n";
        pastOrderText.text += ordered + "\n";
        roundText.text += level + 1 + "\n";
        if (onlineMode == true)
        {
            StartCoroutine(Web.GetComponent<Web>().SaveData(user,scenario,level + 1,demand,ordered));
        }
        if (level == levelcap-1) 
        {
                SwitchState(State.END);
        }
        else
        {
                SwitchState(State.INIT);
                level++;
        }
    }
    
    
    public void ONn1Clicked()
    {
        
        numPad.Add(1);
        orderText.text = orderText.text+ 1  ;
    }
    public void ONn2Clicked()
    {
        numPad.Add(2);
        orderText.text = orderText.text+ 2  ;
    }
    public void ONn3Clicked()
    {
        numPad.Add(3);
        orderText.text = orderText.text+3  ;
    }
    public void ONn4Clicked()
    {
        numPad.Add(4);
        orderText.text = orderText.text+4  ;
    }
    public void ONn5Clicked()
    {
        numPad.Add(5);
        orderText.text = orderText.text+5  ;
    }
    public void ONn6Clicked()
    {
        numPad.Add(6);
        orderText.text = orderText.text+6  ;
    }
    public void ONn0Clicked()
    {
        if (numPad.Count != 0)
        {
            numPad.Add(0);
            orderText.text = orderText.text+ 0  ;
        }
        
    }
    public void ONn7Clicked()
    {
        numPad.Add(7);
        orderText.text = orderText.text+7  ;
    }
    public void ONn8Clicked()
    {
        numPad.Add(8);
        orderText.text = orderText.text+ 8  ;
    }
    public void ONn9Clicked()
    {
        numPad.Add(9);
        orderText.text = orderText.text+9  ;
    }

    public void NumpadClear()
    {
        numPad.Clear();
        orderText.text = "";
    }

    public void OnContClicked()
    {
        if (progress.Count != 1) return;
        level = 0;
        pastDemand = new int[levelcap];
        pastOrdered = new int[levelcap];
        pastDemandText.text = String.Empty + "Actual Demand\n";
        pastOrderText.text = String.Empty + "Items Ordered\n";
        roundText.text = String.Empty +"Round\n";
        totalOrdered = 0;
        totalProfit = 0;
        optimalProfit = 0;
        if (scenario==1)
        {
            scenario = 0;
            optimalOrder = 4107;
        }
        else
        {
            scenario = 1;
            optimalOrder = 9784;
        }
        progress.Add(scenario);
        SwitchState(State.INIT);

    }

    public void onScenarioClicked()
    {
        if (scenario == 0)
        {
            source.clip = newspaper_clip;
            source.Play();
        }
        else
        {
            source.clip = magazine_clip;
            source.Play();
        }
    }
    public void onInstructionClicked()
    {
        source.clip = instruction_clip;
        source.Play();
    }

    public void ToggleOnlineMode(bool value)
    {
        onlineMode = value;
    }

    public void DisplayResult()
    {
        orderedText.text = "Yesterday you ordered " + ordered + " copies.";
        if (ordered >= demand)
        {
            demandText.text = "A total of " + demand + " copies were sold.";
        }
        else
        {
            demandText.text = "A total of " + ordered + " copies were sold.";
        }
        profitText.text = "You made a profit of " + profit + ".";
    }

    public void DisplayDescription()
    {
        if (scenario == 0)
        {
            playDescription.text = "One of the most popular local newspapers is the Daily News." +
                                   " Each paper is sold for $1.5 and costs $1 from the publisher." +
                                   " If you order too much, each unsold unit will cost you $1;" +
                                   " if too little is ordered, you suffer a lost profit of $0.5 per unit." +
                                   " Please decide how many units you will like to purchase from your publisher," +
                                   " such that you can maximize your profit.";
            
        }

        if (scenario == 1)
        {
            playDescription.text = "Apart from newspapers, magazines are available at your stores." +
                                   " The best seller is the Weekly Magazine, which is priced at $6 while" +
                                   " purchasing price from publisher is $2. Similar to the Daily News," +
                                   " costs are incurred whenever you order too much or too little," +
                                   " as leftover products after one week will be discarded. " +
                                   "Please determine the optimal order quantity.";
            
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        scenario = UnityEngine.Random.Range(0,2);
        SwitchState(State.START);

        pastDemand = new int[levelcap];
        pastOrdered = new int[levelcap];
        progress.Add(scenario);
        if (scenario == 0)
        {
            optimalOrder = 9784;
        }
        else
        {
            optimalOrder = 4107;
        }

        
        pastDemandText.text = "Actual Demand\n";
        pastOrderText.text = "Items Ordered\n";
        roundText.text = "Round\n";
        source = GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
    }

    public void SwitchState(State newState)
    {
        EndState();
        _state = newState;
        BeginState(newState);
    }

    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.START:
                panelStart.SetActive(true);
                
                break;
            case State.INIT:
                order = 0;
                ordered = 0;
                demand = 0;
                orderText.text = "";
                numPad.Clear();
                panelPlay.SetActive(true);
                panelChart.SetActive(true);
                panelCalc.SetActive(true);
                SwitchState(State.PLAY);
                if (scenario == 0)
                {
                    demand = RandomDemand(10000, 500);
                    optimalProfit += OptimalProfitCalc(1, 1.5f);
                }
                else
                {
                    demand = RandomDemand(4000, 250);
                    optimalProfit += OptimalProfitCalc(2, 6.0f);
                }
                
                break;
            case State.PLAY:
                DisplayDescription();
                break;
            case State.RESULT:
                if (scenario == 0)
                {
                    profit = ProfitCalc(1, 1.5f);
                }
                else
                {
                    profit = ProfitCalc(2, 6);
                }
                
                panelResult.SetActive(true);
                DisplayResult();
                
                break;
            case State.END:
               panelEnd.SetActive(true);
               averageOrderText.text = "Average Order:" + totalOrdered / (level+1) +"\n Record";
               averageProfitText.text = "Average Profit:" + totalProfit / (level+1);
               optimalProfitText.text = "Average Optimal Profit:" + optimalProfit / (level+1);
               for (int i = 0; i <= level; i++)
               {
                   averageOrderText.text += "\n" + pastOrdered[i];
               }
               

               break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case State.START:
                break;
            case State.INIT:
                break;
            case State.PLAY:
                break;
            case State.RESULT:
                break;
            case State.END:
                break;
        }
    }

    void EndState()
    {
        switch (_state)
        {
            case State.START:
                panelStart.SetActive(false);
                break;
            case State.INIT:
                break;
            case State.PLAY:
                panelPlay.SetActive(false);
                break;
            case State.RESULT:
                panelResult.SetActive(false);
                break;
            case State.END:
                panelEnd.SetActive(false);
                break;
        }
    }
}
