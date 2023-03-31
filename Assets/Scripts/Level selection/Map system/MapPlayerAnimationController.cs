using UnityEngine;

namespace Level_selection.Map_system
{
    public class MapPlayerAnimationController
    {
        private static readonly int IsMove = Animator.StringToHash("IsMove");

        private readonly Animator _animator;

        public MapPlayerAnimationController(Animator animator)
        {
            _animator = animator;
        }
        
        public void Move(bool isMove)
        {
            _animator.SetBool(IsMove, isMove);
        }
    }
}