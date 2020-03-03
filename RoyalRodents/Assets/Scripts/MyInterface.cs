using System.Collections;
using UnityEngine;



// This file has two diferent interfaces in it, uncommon but shows different ways to implement

//interface 1
public interface IMyInterface 
{

    void kill();

}


//interface 2 used by all characters and buildings
public interface IDamageable<T>
{
    void Damage(T damageTaken);

    void SetUpHealthBar(GameObject go);

    void UpdateHealthBar();
}

public interface DayNight
{
     void SetUpDayNight();
}
