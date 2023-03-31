using UnityEngine;

namespace FPS.AttackSystem
{
    public class AttackAnimationController : MonoBehaviour
    {
        private static readonly int IsAttack = Animator.StringToHash("IsAttack");

        [SerializeField]
        private Animator _animator;

        public void SetBoolAttack(bool isAttack)
        {
            _animator.SetBool(IsAttack, isAttack);
        }
        
        public void Attack()
        {
            _animator.CrossFade("Attack", 0f, -1, 0f, 0f);
        }

        public void Reset()
        {
            _animator.SetBool(IsAttack, false);
        }

        public bool IsActiveAttack()
        {
            return _animator.GetBool(IsAttack);
        }
    }
}