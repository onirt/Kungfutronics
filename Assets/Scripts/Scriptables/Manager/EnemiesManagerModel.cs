using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyManager", menuName = "EnemyManager")]
public class EnemiesManagerModel : ScriptableObject
{
    [SerializeField]
    private GameObject enemyPrefab;
    private string[] spots;
    private Vector3[] spawnPositions;
    private int uniqIndex;
    public void Init()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        spots = new string[enemies.Length];
        spawnPositions = new Vector3[enemies.Length];
        for (int i=0; i < enemies.Length; i++)
        {
            spots[i] = enemies[i].name;
            spawnPositions[i] = enemies[i].transform.position;
        }
        uniqIndex = spots.Length;
    }
    public IEnumerator SpawnNewEnemyAt(int delay)
    {
        Debug.Log("Spawning delay: " + delay);
        yield return new WaitForSeconds(delay);

        Transform newenemy = Instantiate(enemyPrefab, GetNewPosition(), Quaternion.identity).transform;
        Vector3 lookAt = (GameManager.obj.player.position - newenemy.position);
        newenemy.forward = new Vector3(lookAt.x, 0, lookAt.z).normalized;
        newenemy.name = "Enemy_Cristal_" + uniqIndex;
        uniqIndex++;
    }
    private Vector3 GetNewPosition()
    {
        int i;
        do
        {
            i = Random.Range(0, spots.Length);
        } while (spots[i].Length != 0);
        return spawnPositions[i];
    }
    public void NotifyDied(string name)
    {
        for (int i=0; i < spots.Length; i++)
        {
            if (name.Equals(spots[i]))
            {
                spots[i] = "";
                return;
            }
        }
    }
}
