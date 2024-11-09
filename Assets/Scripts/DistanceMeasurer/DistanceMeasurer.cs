using Unity.VisualScripting;
using UnityEngine;

/*
This script is used to rotate the title models that make up the 
title name. When the player approaches the title models, this 
script will rotate each of the title models to 180 degrees.
*/

public class DistanceMeasurer : MonoBehaviour
{
    [Tooltip("Target to calculate distance from")]
    [SerializeField] private GameObject target;
    [Tooltip("Player object that will measure distance from target to")]
    [SerializeField] private GameObject player;
    [Tooltip("Model objects that make up the title")]
    [SerializeField] private GameObject[] textObjects;
    [Tooltip("The speed at which the text models will spin as player moves")]
    private float distance; // Distance value between target and player
    private float degrees = 0; // Degree of object rotation

    void Start()
    {
        target = gameObject;
    }

    private float CalculateDistanceInSpace()
    {
        return Vector3.Distance(target.transform.position, player.transform.position);
    }

    void Update()
    {
        distance = CalculateDistanceInSpace();

        while (distance > 0)
        {
            degrees = distance - 180;

            for (int i = 0; i < textObjects.Length; i++)
            {
                textObjects[i].transform.Rotate(0, degrees, 0);
            }
        }
    }
}
