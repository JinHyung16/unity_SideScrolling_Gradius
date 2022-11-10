using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    /// <summary>
    /// Enemy���� IDamage�� ��ӹ����� �����
    /// �Ѿ� �������� ���� Enemy �� HP ����
    /// </summary>

    int HP { get; set; }
    void Damaged(int damage);

}

