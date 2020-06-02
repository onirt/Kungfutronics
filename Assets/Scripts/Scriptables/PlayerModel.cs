using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "playerModel", menuName = "Player")]
public class PlayerModel : ScriptableObject
{
    public AudioClip fireUp;
    public AudioClip damage;
    public AudioClip overload;
    public GameObject powerPrefab;

    public Transform InstantiatePowerPrefab(Vector3 position, Vector3 direction, int power)
    {
        Transform powerTransform = GameObject.Instantiate(powerPrefab, position, Quaternion.identity).transform;
        powerTransform.forward = direction;
        powerTransform.GetComponent<PowerBehaviour>().power = power;
        return powerTransform;
    }
}
