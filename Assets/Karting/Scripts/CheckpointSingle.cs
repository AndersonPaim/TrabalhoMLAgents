using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartGame.KartSystems
{
    public class CheckpointSingle : MonoBehaviour
    {

        private int _checkpointNumber;

        private void OnTriggerEnter(Collider other)
        {
            CarControllerAgent car = other.GetComponent<CarControllerAgent>();
            if(car != null)
            {
                car.ReachCheckpoint(_checkpointNumber);
            }
        }

        public void SetCheckpointNumber(int number)
        {
            _checkpointNumber = number;
        }
    }
}