using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartGame.KartSystems
{
    public class TrackCheckpoints : MonoBehaviour
    {
        [SerializeField] private List<CheckpointSingle> _checkpointsList;

        public List<CheckpointSingle> CheckpointsList => _checkpointsList;

        private void Awake()
        {
            int i = 0;
            foreach (CheckpointSingle checkpointSingle in _checkpointsList)
            {
                checkpointSingle.SetCheckpointNumber(i);
                i++;
            }
        }
    }
}