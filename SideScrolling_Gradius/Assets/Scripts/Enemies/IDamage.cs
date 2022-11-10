using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    /// <summary>
    /// Enemy들이 IDamage도 상속받으며 사용중
    /// 총알 데미지에 따라 Enemy 별 HP 감소
    /// </summary>

    int HP { get; set; }
    void Damaged(int damage);

}

