using UnityEngine;

namespace FPS.EnemyComponent
{
    public class EnemyAnimationController : MonoBehaviour
    {
        private static readonly int IsAttack = Animator.StringToHash("IsAttack");
        private static readonly int IsRun = Animator.StringToHash("IsRun");
        private static readonly int IsHide = Animator.StringToHash("IsHide");

        [SerializeField]
        private Animator _animator;
        
        public void Attack()
        {
            _animator.SetBool(IsAttack, true);
        }

        public void Run()
        {
            _animator.SetBool(IsRun, true);
        }

        public void ResetAttack()
        {
            _animator.SetBool(IsAttack, false);
        }
        
        public void ResetRun()
        {
            _animator.SetBool(IsRun, false);
        }

        public void Reset()
        {
            _animator.SetBool(IsRun, false);
            _animator.SetBool(IsAttack, false);
            _animator.SetBool(IsHide, false);
        }

        public bool IsBoolAttack()
        {
            return _animator.GetBool(IsAttack);
        }

        public bool IsActiveAttack()
        {
            return _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        }

        public void Hide()
        {
            _animator.SetBool(IsHide, true);
        }
        
        public bool IsActiveHide()
        {
            return _animator.GetBool(IsHide);
        }
    }
}