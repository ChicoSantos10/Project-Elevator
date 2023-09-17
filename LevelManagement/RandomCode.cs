using UnityEngine;

namespace LevelManagement
{
    [CreateAssetMenu(menuName = "Elevator Code/Random", fileName = "RandomCode", order = 0)]
    internal class RandomCode : Code
    {
        void OnEnable()
        {
            a = Random.Range(1, 9);
            b = Random.Range(1, 9);
            c = Random.Range(1, 9);
            d = Random.Range(1, 9);
        }
    }
}