using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField]
    Transform pathParent;
    [SerializeField]
    GameObject[] gates;
    [SerializeField]
    GameObject cable;
    [SerializeField]
    GameObject handle;
    [SerializeField]
    GameObject handleSpawn;
    [SerializeField]
    public Vector3 spawnPosition;

    private void Start()
    {
        GameManager.obj.gameStarted += GameStarted;
    }
    private void OnDestroy() {
        GameManager.obj.gameStarted -= GameStarted;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Food")
        {
            Transform child = Instantiate(
                other.gameObject,
                new Vector3(other.transform.position.x, other.transform.position.y, spawnPosition.z),
                other.transform.rotation).transform;
            if (other.tag != "HandleSpawn" && other.tag != "Handle")
            {
                child.SetParent(pathParent);
            }

        }
        Destroy(other.gameObject);
    }
    public void GameStarted (){
        
        Transform player = GameManager.obj.player;
        foreach (GameObject gate in gates)
        {
            gate.transform.position = new Vector3(gate.transform.position.x, player.position.y + 1.8f, gate.transform.position.z);
        }

        cable.transform.position = new Vector3(cable.transform.position.x, player.position.y + 1.8f, cable.transform.position.z);

        handle.transform.position = new Vector3(handle.transform.position.x, player.position.y + 2.8f, handle.transform.position.z);
        handleSpawn.transform.position = new Vector3(handleSpawn.transform.position.x, player.position.y + 1.8f, handleSpawn.transform.position.z);
    }
}
