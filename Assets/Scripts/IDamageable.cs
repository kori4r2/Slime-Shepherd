using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int HP {get;}
    /// <summary>
    /// Função que faz a unidade tomar dano, retorna true caso a unidade fique com hp zerado ao tomar dano.
    /// </summary>
    /// <param name="dmg">Dano a ser tomado</param>
    /// <returns></returns>
    bool TakeDamage(int dmg);
    void Die();
}