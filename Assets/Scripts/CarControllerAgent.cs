using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarControllerAgent : Agent
{
    [SerializeField] private TrackCheckpoints _trackCheckpoints;
    [SerializeField] private CarController _carController;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private bool _trainingMode;

    private float _maxTimeToReachNextCheckpoint = 50f;
    private float _timeLeft = 0;
    private int _nextCheckpoint = 0;
    private int _lastCheckpoint;

    private void Awake()
    {
        _lastCheckpoint = _trackCheckpoints.CheckpointsList.Count - 1;
        _timeLeft = _maxTimeToReachNextCheckpoint;
    }

    private void Update()
    {
        _timeLeft -= Time.deltaTime;

        AddReward(-0.01f);

        if (_timeLeft < 0f && _trainingMode)
        {
            AddReward(-1f);
            EndEpisode();
            _timeLeft = _maxTimeToReachNextCheckpoint;
            _nextCheckpoint = 0;
        }
    }

    public void ReachCheckpoint(int checkpoint)
    {
        if (checkpoint == _nextCheckpoint)
        {
            if (checkpoint != _lastCheckpoint)
            {
                AddReward((0.5f) / _lastCheckpoint + 1);
                _timeLeft = _maxTimeToReachNextCheckpoint;
                _nextCheckpoint++;
            }
            else if(_trainingMode)
            {
                AddReward(0.5f);
                EndEpisode();
                _timeLeft = _maxTimeToReachNextCheckpoint;
                _nextCheckpoint = 0;
            }
        }
        else
        {
            //AddReward(-1);
        }
    }

    public override void OnEpisodeBegin()
    {

        transform.localEulerAngles = new Vector3(0, 90, 0);
        transform.position = _spawnPosition.position;
        _nextCheckpoint = 0;
        _carController.ResetVelocity();
        _carController.ResetSteeringAngle();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 checkpointForward = _trackCheckpoints.CheckpointsList[_nextCheckpoint].gameObject.transform.forward;
        float dirDot = Vector3.Dot(transform.forward, checkpointForward);
        //Vector3 diff = _trackCheckpoints.CheckpointsList[_nextCheckpoint].transform.position - transform.position;
        sensor.AddObservation(dirDot);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int fwAmount = 0;
        int turnAmount = 0;

        switch(actions.DiscreteActions[0])
        {
            case 0:
                fwAmount = 0;
                break;
            case 1:
                fwAmount = 1;
                break;
            case 2:
                fwAmount = -1;
                break;
        }

        switch(actions.DiscreteActions[1])
        {
            case 0:
                turnAmount = 0;
                break;
            case 1:
                turnAmount = -1;
                break;
            case 2:
                turnAmount = 1;
                break;
        }

        _carController.SetInputs(fwAmount, turnAmount);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;

        if(Input.GetKey(KeyCode.W))
        {
            forwardAction = 1;
        }
        if(Input.GetKey(KeyCode.S))
        {
            forwardAction = 2;
        }

        int turnAction = 0;

        if(Input.GetKey(KeyCode.A))
        {
            turnAction = 1;
        }
        if(Input.GetKey(KeyCode.D))
        {
            turnAction = 2;
        }

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = forwardAction;
        discreteActions[1] = turnAction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Walls")
        {
            AddReward(-0.05f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Walls")
        {
            AddReward(-0.01f);
        }
    }
}