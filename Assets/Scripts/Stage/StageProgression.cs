using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageProgression", menuName = "Stage/Stage Progression")]
public class StageProgression : ScriptableObject
{
    [SerializeField] private List<int> enemiesQty;
    
    public List<int> EnemiesQty => enemiesQty;
}
