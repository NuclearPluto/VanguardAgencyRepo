using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStateManager : MonoBehaviour
{
    private List<PlayerController> playerList;
    private List<EnemyController> enemyList;
    // Start is called before the first frame update
    void Awake()
    {
        playerList = new List<PlayerController>();
        enemyList = new List<EnemyController>();
    }

    public void PlayerMoved() {

    }

    public void EnemyMoved() {

    }

    public void addEntity(EntityBehavior entity) {
        if (entity is PlayerController) {
            playerList.Add(entity as PlayerController);
        }
        else if (entity is EnemyController) {
            enemyList.Add(entity as EnemyController);
        }
        else {
            Debug.LogError("added something to mapstate that isn't enemy or player");
        }
    }

    public void removeEntity(EntityBehavior entity) {
        if (entity is PlayerController) {
            playerList.Remove(entity as PlayerController);
        }
        else if (entity is EnemyController) {
            enemyList.Remove(entity as EnemyController);
        }
        else {
            Debug.LogError("removed something in mapstate that isn't enemy or player");
        }
    }
}
