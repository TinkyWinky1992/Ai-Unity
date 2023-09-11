using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform Target;
    [SerializeField] private Transform respawn_point;
    [SerializeField] private Material win_material;
    [SerializeField] private Material lose_material;
    [SerializeField] private Material lose_timer_material;
    [SerializeField] private MeshRenderer floor_meshRenderer;
    [SerializeField] private Transform spawn_area;

    private TimerBounty timer;


    public void Awake()
    {
        timer = transform.parent.Find("TimerBounty").GetComponent<TimerBounty>();
        if (timer != null)
        {
            Debug.Log("TimerBounty found!");
        }
        else
        {
            Debug.LogError("TimerBounty not found!");
        }
    }


    public override void OnEpisodeBegin()
    {
        timer.onStop();
               
        float minX = spawn_area.position.x - (spawn_area.localScale.x / 2);
        float maxX = spawn_area.position.x + (spawn_area.localScale.x / 2);
        float minZ = spawn_area.position.z - (spawn_area.localScale.z / 2);
        float maxZ = spawn_area.position.z + (spawn_area.localScale.z / 2);

        Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
        Vector3 targetPosition = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));

        transform.position = spawnPosition;
        Target.position = targetPosition;
        timer.onResume();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector3)transform.position);
        sensor.AddObservation((Vector3)Target.position);
    }

    public override void OnActionReceived(ActionBuffers action)
    {
        float moveX = action.ContinuousActions[0];
        float moveZ = action.ContinuousActions[1];

        float movement_speed = 5f;
        Vector3 Position_v = new Vector3(moveX, 0, moveZ);
        transform.position += Vector3.Normalize(Position_v) * Time.deltaTime * movement_speed;
    }

    public override void Heuristic(in ActionBuffers actionBuffers)
    {
        ActionSegment<float> continuousActions = actionBuffers.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(10f);
            floor_meshRenderer.material = win_material;
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-10f);
            floor_meshRenderer.material = lose_material;
            EndEpisode();
        }

        
        if(timer != null)
            if(timer.isTimerAbove())
            {
                SetReward(-5f);
                floor_meshRenderer.material = lose_timer_material;
                EndEpisode();
            }

    }


}
