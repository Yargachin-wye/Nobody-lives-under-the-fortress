using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "NewStipulationPool", menuName = "StipulationPool")]
public class StipulationPool : ScriptableObject
{
    [SerializeField] private string[] _pool;
    public string[] pool => _pool;
}

