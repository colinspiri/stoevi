// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.AI;
// using Random = UnityEngine.Random;
//
// [RequireComponent(typeof(NavMeshAgent), typeof(TorbalanSenses))]
// public class TorbalanController : MonoBehaviour {
//     // components
//     public static TorbalanController Instance;
//     private NavMeshAgent agent;
//     private TorbalanSenses senses;
//     
//     // public constants
//     // public List<Transform> passiveRoute;
//     public float passiveSpeed;
//     public float chaseSpeed;
//     [Range(0, 360)] public float searchLookAngle;
//     public float searchLookTime;
//     
//     // state
//     private enum AIState { Passive, Search, Chase }
//     private AIState state;
//     private Vector3 targetLocation;
//     // passive
//     private Crop targetCrop;
//     // search
//     private Coroutine searchCoroutine;
//     // chase
//     
//     private void Awake() {
//         Instance = this;
//         agent = GetComponent<NavMeshAgent>();
//         senses = GetComponent<TorbalanSenses>();
//     }
//
//     // Start is called before the first frame update
//     void Start() {
//         ChangeState(AIState.Passive);
//
//         senses.onPlayerEnterSight += () => {
//             if(state == AIState.Passive) ChangeState(AIState.Search);
//         };
//         senses.onHearPlayer.AddListener(() => {
//             // Debug.Log("Torbalan heard player");
//             if(state == AIState.Passive) ChangeState(AIState.Search);
//             else if (state == AIState.Search) {
//                 // update player position
//                 targetLocation = PlayerController.Instance.transform.position;
//                 // restart search coroutine
//                 if(searchCoroutine != null) StopCoroutine(searchCoroutine);
//                 searchCoroutine = StartCoroutine(SearchCoroutine());
//             }
//         });
//     }
//
//     // Update is called once per frame
//     void Update() {
//         if (Time.deltaTime == 0) return;
//         // state-specific updates
//         if (state == AIState.Passive) UpdatePassive();
//         else if (state == AIState.Search) UpdateSearch();
//         else if (state == AIState.Chase) UpdateChase();
//     }
//
//     private void UpdatePassive() {
//         targetCrop = InteractableManager.Instance.GetClosestHarvestableCropTo(transform.position);
//         if (targetCrop != null) {
//             // var distance = Vector3.Distance(targetCrop.transform.position, transform.position);
//             // Debug.Log("target crop = " + targetCrop.name + " with distance " + distance);
//             // walk to target crop
//             targetLocation = targetCrop.transform.position;
//             agent.SetDestination(targetLocation);
//             
//             // if close enough,
//             if (CloseEnoughToDestination()) {
//                 // count 
//                 TomatoCounter.Instance.TorbalanStoleTomato();
//                 // steal crop
//                 targetCrop.MakeEmpty();
//                 // walk to own location
//                 targetLocation = transform.position;
//                 agent.SetDestination(targetLocation);
//             }
//         }
//         // otherwise walk to random point
//         else if(CloseEnoughToDestination()) {
//             // pick random position on navmesh
//             float walkRadius = 60f;
//             Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
//             randomDirection += transform.position;
//             NavMesh.SamplePosition(randomDirection, out var hit, walkRadius, 1);
//             targetLocation = hit.position;
//
//             // set destination
//             agent.SetDestination(targetLocation);
//         }
//
//         // if can see player, chase
//         if(senses.CanSeePlayer()) ChangeState(AIState.Chase);
//     }
//
//     private void UpdateSearch() {
//
//         // if player noticed, chase
//         if(senses.CanSeePlayer()) ChangeState(AIState.Chase);
//     }
//
//     private IEnumerator SearchCoroutine() {
//         // go towards last known player location
//         agent.SetDestination(targetLocation);
//
//         // wait until arrived
//         while (!CloseEnoughToDestination()) {
//             yield return null;
//         }
//         
//         // look left
//         float t = 0;
//         float startingAngle = transform.rotation.eulerAngles.y;
//         float endingAngle = startingAngle + searchLookAngle;
//         while (t < searchLookTime) {
//             float angle = Mathf.Lerp(startingAngle, endingAngle, t / searchLookTime);
//             transform.rotation = Quaternion.Euler(0, angle, 0);
//
//             t += Time.deltaTime;
//             yield return null;
//         }
//         transform.rotation = Quaternion.Euler(0, endingAngle, 0);
//
//         // look right
//         t = 0;
//         startingAngle = transform.rotation.eulerAngles.y;
//         endingAngle = startingAngle - 2 * searchLookAngle;
//         while (t < searchLookTime) {
//             float angle = Mathf.Lerp(startingAngle, endingAngle, t / searchLookTime);
//             transform.rotation = Quaternion.Euler(0, angle, 0);
//
//             t += Time.deltaTime;
//             yield return null;
//         }
//         transform.rotation = Quaternion.Euler(0, endingAngle, 0);
//         
//         // if nothing found, go back to passive
//         ChangeState(AIState.Passive);
//     }
//
//     private void UpdateChase() {
//         // follow the player
//         agent.SetDestination(PlayerController.Instance.transform.position);
//         // if close enough to the player, game over
//         if (CloseEnoughToDestination()) {
//             // game over
//             MenuManager.Instance.GameOver(false);
//         }
//         
//         // if player no longer within line of sight, search for player
//         if(!senses.CanSeePlayer()) ChangeState(AIState.Search);
//     }
//
//     private void ChangeState(AIState newState) {
//         // clean up old state
//         if (state == AIState.Passive) {
//             
//         }
//         else if (state == AIState.Search) {
//             StopCoroutine(searchCoroutine);
//             searchCoroutine = null;
//         }
//         else if (state == AIState.Chase) {
//             
//         }
//         // set new state
//         // Debug.Log("Torbalan AI switched to " + newState);
//         state = newState;
//         if(state == AIState.Passive) InitializePassiveState();
//         else if(state == AIState.Search) InitializeSearchState();
//         else if (state == AIState.Chase) InitializeChaseState();
//     }
//
//     private void InitializePassiveState() {
//         // set target position to closest crop
//         targetCrop = InteractableManager.Instance.GetClosestHarvestableCropTo(transform.position);
//         if(targetCrop != null) targetLocation = targetCrop.transform.position;
//         // set speed
//         agent.speed = passiveSpeed;
//     }
//     private void InitializeSearchState() {
//         // store last known player location
//         targetLocation = PlayerController.Instance.transform.position;
//         // set speed
//         agent.speed = passiveSpeed;
//         // start coroutine
//         searchCoroutine = StartCoroutine(SearchCoroutine());
//         
//         // play stinger
//         AudioManager.Instance.PlaySearchSound();
//     }
//     private void InitializeChaseState() {
//         // set speed
//         agent.speed = chaseSpeed;
//         
//         // play stinger
//         AudioManager.Instance.PlayChaseSound();
//     }
//
//     private bool CloseEnoughToDestination(bool debug = false) {
//         Vector3 toDestination = agent.destination - transform.position;
//         // ignore vertical component
//         toDestination.y = 0;
//         float distance = toDestination.magnitude;
//         if(debug) Debug.Log("distance to destination = " + distance + ", compared to closeEnoughDistance = " + agent.stoppingDistance);
//         return distance <= agent.stoppingDistance;
//     }
// }
