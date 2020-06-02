using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayManager
{
    void EatIt();
    void AddFoodForEat(GameObject foodObj);
    void RemoveFoodForEat(GameObject foodObj);
}
