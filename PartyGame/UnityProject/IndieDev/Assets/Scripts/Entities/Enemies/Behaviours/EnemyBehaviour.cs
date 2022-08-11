using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public Entity target;
    public EnemyManager manager;
    public bool isMoving;
    public bool isAttacking;

    public int damage;

    public NavMeshAgent agent;
    
    public float minDistanceToAttack;

    public virtual void Target()
    {
        
    }
    
    public virtual void OnEnable()
    {
        // Quand est activé (parce que les ennemis ne sont pas destroy)
        agent.enabled = true;
    }

    public virtual void OnDisable()
    {
        agent.enabled = false;
    }

    public virtual void Attack()
    {
        target.TakeDamage(damage);
    }
    
    public PlayerManager PlayerDetected()
    {
        var reachablePlayers = new List<PlayerManager>();

        // Stocker tous les joueurs sensibles

        foreach (var player in GameManager.instance.allPlayers)
        {
            if (player != null && !player.manager.isDead)
            {
                reachablePlayers.Add(player.manager);
            }
        }

        // Sélectionner le plus proche

        var nearestDistance = Mathf.Infinity;
        PlayerManager nearest = null;
        for (int i = 0; i < reachablePlayers.Count; i++)
        {
            var reachable = reachablePlayers[i];

            // Est-ce que le chemin est accessible ?
            var path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, reachable.transform.position, NavMesh.AllAreas, path);
            if (path.status != NavMeshPathStatus.PathComplete) continue;
            
            // Est-ce que le joueur est plus proche que le précédent sélectionné ?
            var distance = Vector3.Distance(reachable.transform.position, transform.position);
            if (distance > nearestDistance) continue;
            
            nearestDistance = distance;
            nearest = reachable;
        }

        return nearest;
    }
}
