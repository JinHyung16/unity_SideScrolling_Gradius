using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    int HP { get; set; }
    void Damaged(int damage);

}

