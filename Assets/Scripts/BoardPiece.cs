using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public abstract class BoardPiece : MonoBehaviour
{
    public int attackDamage;
    public int startingHealth;
    private float health;
    public float speed;

    public virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeDamage(int damageAmount, BoardPiece attacker)
    {
        health -= damageAmount;
        print(health);
        if (health <= 0)
        {
          Death(attacker);
        }
    }

    public virtual void Death(BoardPiece attacker)
    {
        attacker.Killed(this);
    }

    public virtual void Killed(BoardPiece victim)
    {
        
    }
}