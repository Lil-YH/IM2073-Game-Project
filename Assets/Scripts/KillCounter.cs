using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour
{
    public GameManager gameManager;
    
    public int currKillCount = 0;
    private int numZombie = 10; 
    
    [SerializeField] Text killCount;

    // Start is called before the first frame update
    private void Start()
    {
        currKillCount = 0;
        UpdateKillCounterUI();
    }

    public void AddCount()
    {
        currKillCount++;
        UpdateKillCounterUI();
    }

    // Update is called once per frame
    private void UpdateKillCounterUI()
    {
        killCount.text = currKillCount.ToString() + " / " + numZombie.ToString();
        
        //Game Complete trigger
        if (currKillCount == numZombie && !GameManager.GameIsComplete)
        {
            gameManager.GameComplete();
        }
    }

    
}
