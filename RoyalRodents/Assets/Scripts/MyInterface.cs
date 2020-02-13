using System.Collections;
using UnityEngine;



// This file has two diferent interfaces in it, uncommon but shows diff types

//interface 1
public interface IMyInterface 
{

    void kill();

}


//interface 2
public interface IDamageable<T>
{
    void Damage(T damageTaken);

    void SetUpHealthBar(GameObject go);

    void UpdateHealthBar();
}
