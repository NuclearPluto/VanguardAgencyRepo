using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public UnityEvent onPlayerEnterRoom;
    public EntityEvent onEntityDeath;
    public EntityEvent onEntityAdded;

    public void PlayerEnteredRoom()
    {
        onPlayerEnterRoom?.Invoke();
    }

    public void EntityDied(EntityBehavior entity)
    {
        onEntityDeath?.Invoke(entity);
    }

    public void EntityAdded(EntityBehavior entity) {
        onEntityAdded?.Invoke(entity);
    }

}
