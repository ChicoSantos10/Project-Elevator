using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SSSTools.FunText.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Robot
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class RobotMovement : MonoBehaviour
    {
        static HashSet<Vector3> usedPositions = new HashSet<Vector3>();

        [SerializeField] GameObject positions;
        [SerializeField] float scanTime = 1;

        public NavMeshAgent _agent;
        WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        CustomWaitForSeconds scanTimer;
        readonly List<Vector3> _positions = new List<Vector3>();
        public bool isScanning = false;
        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        void Start()
        {
            foreach (Transform child in positions.transform)
            {
                _positions.Add(child.position);
            }

            scanTimer = new CustomWaitForSeconds(scanTime);
            
            StartPath();
        }

        void StartPath()
        {
            Vector3 nextDest = GetNextDestination();
            _agent.SetDestination(nextDest);
            StartCoroutine(WaitForPosition());
            StartCoroutine(CheckIfArrived());

            IEnumerator WaitForPosition()
            {
                while (IsPositionAvailable(nextDest))
                {
                    yield return null;
                }

                usedPositions.Add(nextDest);
            }
        }

        IEnumerator CheckIfArrived()
        {
            do
            {
                isScanning = false;
                yield return null;
            } while (_agent.hasPath);

            StartCoroutine(StartScanning());
        }

        IEnumerator StartScanning()
        {
            isScanning = true;
            transform.DORotate(transform.rotation.eulerAngles + Vector3.up * 360, scanTime, RotateMode.FastBeyond360);
            yield return scanTimer.Wait(scanTime);
            
            usedPositions.Remove(_agent.destination);
            StartPath();
        }

        Vector3 GetNextDestination()
        {
            return _positions[Random.Range(0, _positions.Count)];
        }

        bool IsPositionAvailable(Vector3 pos) => !usedPositions.Contains(pos);


        public static void ClosestRobotToTarget(GameObject target)
        {
            GameObject[] robots;
            robots = GameObject.FindGameObjectsWithTag("Robot");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 targetPos = target.transform.position;

            foreach (var rbt in robots)
            {
                Vector3 diff = rbt.transform.position - targetPos;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = rbt;
                    distance = curDistance;
                }
            }

            if (closest == null)
                return;
            
            closest.GetComponent<Robot.RobotMovement>()._agent.isStopped = true;
            closest.GetComponent<Robot.RobotMovement>()._agent.SetDestination(target.transform.position);
            closest.GetComponent<Robot.RobotMovement>()._agent.isStopped = false;
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnSceneLoaderListener()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            usedPositions.Clear();
        }
    }
}
