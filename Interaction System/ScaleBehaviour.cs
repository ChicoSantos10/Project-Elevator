using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using State_Machine;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interaction_System
{
    [Serializable]
    class ScaleBehaviour : InteractableBehaviour
    {
        [SerializeField] private Transform leftPlate, rightPlate;
        [SerializeField] private Transform scaleBar, leftComponents, rightComponents;
        [SerializeField, Range(0,1)] float radius = 0.1f;
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Collider[] balls;
        [SerializeField] private Transform camBoard;
        [SerializeField] private AudioClip failSound, correctSound;
        [SerializeField] private TextMeshPro texto1, texto2;
        private AudioSource audioSource;
        private Dictionary<Transform, List<Collider>> ballsPlate = new Dictionary<Transform, List<Collider>>();
        private Dictionary<Collider, Vector3> ballStartPos = new Dictionary<Collider, Vector3>();
        private Collider heavyBall;
        private int numberTries = 0;
        private bool canChoose;

        public override bool Finished { get; protected set; } = false;

        public override void StartInteraction()
        {
            inputReader.OnWeightAction += OnWeightAction;
        }

        public override void StopInteraction()
        {
            inputReader.OnWeightAction -= OnWeightAction;
        }

        private void OnWeightAction()
        {
            List<Collider> ballsLeft = ballsPlate[leftPlate];
            List<Collider> ballsRight = ballsPlate[rightPlate];
            
            if (ballsLeft.Count == ballsRight.Count)
            {
                if (ballsLeft.Contains(heavyBall))
                {
                    
                    //inclinar balança para esquerda
                    scaleBar.Rotate(0f,0f,-5f,Space.World);
                    leftComponents.Rotate(0f,0f,5f,Space.World);
                    rightComponents.Rotate(0f,0f,5f,Space.World);
                }
                else if (ballsRight.Contains(heavyBall))
                {
                    //inclinar balança para direita
                    scaleBar.Rotate(0f,0f,5f,Space.World);
                    leftComponents.Rotate(0f,0f,-5f,Space.World);
                    rightComponents.Rotate(0f,0f,-5f,Space.World);
                }
                else
                {
                    //nao rodar balança
                    //Debug.Log("where's the ball? 😂");
                }
            }
            else if (ballsLeft.Count > ballsRight.Count)
            {
                //inclinar balança para a esquerda
                scaleBar.Rotate(0f,0f,-5f,Space.World);
                leftComponents.Rotate(0f,0f,5f,Space.World);
                rightComponents.Rotate(0f,0f,5f,Space.World);
                // leftPlate.Rotate(0f,0f,5f,Space.World);
                // rightPlate.Rotate(0f,0f,5f,Space.World);
            }
            else
            {
                //inclinar balança para a direita
                scaleBar.Rotate(0f,0f,5f,Space.World);
                leftComponents.Rotate(0f,0f,-5f,Space.World);
                rightComponents.Rotate(0f,0f,-5f,Space.World);
            }
            Reset();
            numberTries++;
            if (numberTries == 2)
            {
                texto1.enabled = false;
                texto2.enabled = true;
                //metodo para mudar camara para o quadro
                OnFinishedTries();
                //desativar interacao com balanca
                StopInteraction();
            }
        }
        

        public override void OnLeftMouse(Vector2 pos)
        {
            if (numberTries == 2)
            {
                if (!canChoose)
                {
                    return;
                }
                Collider? c = SelectionManager.GetObjectScreenPoint(pos);
                if (c != null && c.CompareTag("Ball"))
                {
                    if (c.Equals(heavyBall))
                    {
                        Debug.Log("congratz, (☞ﾟヮﾟ)☞☜(ﾟヮﾟ☜)(☞ﾟヮﾟ)☞");
                        audioSource.PlayOneShot(correctSound);
                        Finished = true;
                    }
                    else
                    {
                        Debug.Log("you lose! xD noob");
                        audioSource.PlayOneShot(failSound);
                        OnFail();
                        numberTries = 0;
                        ChooseHeavyBall();   
                        StartInteraction();
                    }
                }
            }
            else
            {
                MoveBall(pos, leftPlate, rightPlate);
            }
            
            
        }

        public override void OnRightMouse(Vector2 pos)
        {
            MoveBall(pos, rightPlate, leftPlate);
        }

        private void MoveBall(Vector2 pos, Transform plate, Transform other)
        {
            Collider? ball = SelectionManager.GetObjectScreenPoint(pos);
            
            if (ball == null || !ball.CompareTag("Ball")) 
                return;

            // Remove from the other plate
            ballsPlate[other].Remove(ball);
            
            // If the ball is already on that plate remove it
            if (ballsPlate[plate].Contains(ball))
            {
                RemoveBall(ball);
                ballsPlate[plate].Remove(ball);
                return;
            }
            
            Vector2 random = Random.insideUnitCircle * radius;
            Vector3 position = new Vector3(random.x, ((SphereCollider) ball).radius * ball.transform.localScale.y, random.y) +
                               plate.position;
            Transform transform;
            (transform = ball.transform).DOMove(position, 0.5f);
            transform.parent = plate;
                
            ballsPlate[plate].Add(ball);
        }

        void OnFinishedTries()
        {
            Vector3 position = CameraTransform.position, rotation = CameraTransform.rotation.eulerAngles;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(2f);
            sequence.Append(CameraTransform.DOMove(camBoard.position, 1.0f));
            sequence.Join(CameraTransform.DORotate(camBoard.rotation.eulerAngles, 1.0f));
            sequence.AppendInterval(2f);
            sequence.Append(CameraTransform.DOMove(position, 1f));
            sequence.Join(CameraTransform.DORotate(rotation, 1f).OnComplete(()=> canChoose = true));
        }

        protected override void OnInitialize()
        {
            audioSource = Transform.GetComponent<AudioSource>();
            foreach (Collider ball in balls)
            {
                ballStartPos.Add(ball, ball.transform.position);
            }
            
            AddBallPlate(leftPlate);
            AddBallPlate(rightPlate);

            ChooseHeavyBall();
            
            void AddBallPlate(Transform plate)
            {
                if (ballsPlate.TryGetValue(plate, out List<Collider> colliders))
                {
                    colliders.Clear();
                }
                else
                {
                    ballsPlate.Add(plate, new List<Collider>());
                }
            }
        }

        void Reset()
        {
            foreach (List<Collider> balls in ballsPlate.Values)
            {
                foreach (Collider ball in balls)
                {
                    RemoveBall(ball);
                }
                balls.Clear();
            }
            //reset a rotacao dos pratos da balanca
            scaleBar.DOLocalRotate(Vector3.zero, 1f).SetDelay(2f);
            leftComponents.DOLocalRotate(Vector3.zero, 1f).SetDelay(2f);
            rightComponents.DOLocalRotate(Vector3.zero, 1f).SetDelay(2f);
        }

        void RemoveBall(Collider ball)
        {
            ball.transform.parent = null;
            ball.transform.DOMove(ballStartPos[ball], 0.5f);
        }
        
        void ChooseHeavyBall() => heavyBall = balls[Random.Range(0, balls.Length)];
    }
}