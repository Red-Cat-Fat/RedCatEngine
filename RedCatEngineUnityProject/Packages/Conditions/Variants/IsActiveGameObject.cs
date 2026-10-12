using System;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Variants
{
    [Serializable]
    [SRName("GameObject/Is Active GameObject in hierarchy")]
    public class IsActiveGameObject : ICondition
    {
        [SerializeField] private GameObject _target;

        public bool Check(IGetterApplicationContainer getter)
            => _target != null && _target.activeInHierarchy;
    }
}
