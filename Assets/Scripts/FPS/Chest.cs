using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Events;
using JetBrains.Annotations;
using Runner.ReagentSystem;
using Tools.SimpleEventBus;
using UnityEngine;
using Zenject;

namespace FPS
{
    public class Chest : MonoBehaviour
    {
        [SerializeField]
        private Transform _rewardPoint;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private float _delayOpenScreen;

        [Inject]
        private ReagentModelProvider _reagentModelProvider;
        [Inject]
        private LevelManager _levelManager;
        [Inject]
        private ReagentNameProvider _reagentNameProvider;

        private Reagent _reagent;
        private Vector3 _startPosition;
        
        private void Awake()
        {
            _startPosition = _rewardPoint.position;
        }

        private void OnEnable()
        {
            _rewardPoint.position = _startPosition;
            _animator.SetBool("IsOpen", false);
        }

        [UsedImplicitly]
        public void Open()
        {
            _reagent = _reagentModelProvider.GetReagentModel(_levelManager.CurrentRewardReagentType);
            _reagent.Rigidbody.isKinematic = true;
            _rewardPoint.localScale = Vector3.one;
            _reagent.transform.parent = _rewardPoint;
            _reagent.transform.localPosition = Vector3.zero;
            _reagent.transform.localRotation = Quaternion.Euler(Vector3.zero);

            EventStreams.UserInterface.Publish(new EventCompletedLevel());
            _animator.SetBool("IsOpen", true);

            OpenScreen(_reagent.ReagentType);
        }

        private async UniTaskVoid OpenScreen(ReagentType reagentType)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delayOpenScreen), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            EventStreams.UserInterface.Publish(new EventShowCompletedScreen(_reagentNameProvider.GetNameByReagentType(reagentType)));
        }

        private void OnDisable()
        {
            _reagent?.Release();
        }
    }
}