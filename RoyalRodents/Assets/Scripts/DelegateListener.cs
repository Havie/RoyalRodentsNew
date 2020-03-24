using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateListener : MonoBehaviour
{

    delegate float healthDelegate(PlayerStats ps);
   // delegate int intDelegate(int a);
   

    private void Start()
    {
        DelegateTutorial.DelegateExample myintDelegate1 = ScorebyNO;

        DelegateTutorial.Foobar(myintDelegate1);
        DelegateTutorial.AddDelegate(myintDelegate1);
        DelegateTutorial.playDelegate();
    }

    void onGameOver(PlayerStats[] ps)
    {
        string mostHP = getHighestStat(ps, ScorebyHP);
        string mostAttck = getHighestStat(ps, ScorebyAtack);

    }

    //Option 1 Normal
    float ScorebyHP(PlayerStats stats)
    {
        return stats.getHealth();
    }
    //Option 2 Lambda
    healthDelegate ScorebyAtack = stats => stats.getAttackDamage();

    void ScorebyNO(int  no )
    {
        Debug.Log( no  + " From function in another class");
    }


    string getHighestStat(PlayerStats[] ps, healthDelegate statCalculator)
    {
        float highestStat = 0;

        foreach(PlayerStats stats in ps)
        {
            float hp = statCalculator(stats);
            if(hp>highestStat)
            {
                highestStat = hp;
            }
        }

        return highestStat.ToString();
    }
}
