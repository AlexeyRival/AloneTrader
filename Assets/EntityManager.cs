using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager only;
    public List<Entity> entities;
    private void Awake()
    {
        only = this;
        entities = new List<Entity>();
    }
    public void Add(Entity entity) { entities.Add(entity); }
    public void Die(Entity entity) { entities.Remove(entity); }
    public Entity GetNearEnemy(Vector3 position, int faction) 
    {
        int variant = -1;
        for (int i = 0; i < entities.Count; ++i) 
        {
            if (faction != entities[i].faction&&(position-entities[i].transform.position).sqrMagnitude<100) {
                if (Random.Range(0, 3) == 0) { variant = i; }
            }
        }
        if (variant != -1) return entities[variant];
        return null;
    }
}
