using UnityEngine;

namespace Level_selection.Bestiary_system
{
    public class TabAnimationController
    {
        private static readonly int IsSelect = Animator.StringToHash("IsSelect");
        
        private readonly Animator _animator;

        public TabAnimationController(Animator animator)
        {
            _animator = animator;
        }

        public void SetSelect(bool isSelect)
        {
            _animator.SetBool(IsSelect, isSelect);
        }
    }
}