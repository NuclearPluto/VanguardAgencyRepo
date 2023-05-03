using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    private List<PlayerController> playerList;
    private List<EnemyController> enemyList;

    private void Awake()
    {
        playerList = new List<PlayerController>();
        enemyList = new List<EnemyController>();
    }

    void OnEnable()
    {
        GameEvents.current.onEntityDeath.AddListener(RemoveEntity);
        GameEvents.current.onEntityAdded.AddListener(AddEntity);
    }

    void OnDisable() {
        GameEvents.current.onEntityDeath.RemoveListener(RemoveEntity);
        GameEvents.current.onEntityAdded.RemoveListener(AddEntity);
    }

    public void AddEntity(EntityBehavior entity)
    {
        if (entity is PlayerController)
        {
            playerList.Add(entity as PlayerController);
        }
        else if (entity is EnemyController)
        {
            enemyList.Add(entity as EnemyController);
        }
        else
        {
            Debug.LogError("Added something to GameEvents that isn't enemy or player");
        }
    }

    public void RemoveEntity(EntityBehavior entity)
    {
        if (entity is PlayerController)
        {
            playerList.Remove(entity as PlayerController);
        }
        else if (entity is EnemyController)
        {
            enemyList.Remove(entity as EnemyController);
        }
        else
        {
            Debug.LogError("Removed something in GameEvents that isn't enemy or player");
        }
    }
}
