﻿

using UnityEngine;
using static RoboRockets.STRINGS;

namespace RoboRockets
{
    public class ConditionAiHasControl : ProcessCondition
    {
        private RocketModuleCluster module;
        public ConditionAiHasControl(RocketModuleCluster module) => this.module = module;

        public override Status EvaluateCondition()
        {
            if(module.TryGetComponent<Storage>(out var storage))
            {
                //module.TryGetComponent<KBatchedAnimController>(out var kanim);
                
                if (storage.FindFirst(ModAssets.Tags.SpaceBrain))
                {
                    //kanim.Play("grounded");
                    return ProcessCondition.Status.Ready;
                }
                else
                {
                    //kanim.Play("grounded_empty");
                    return ProcessCondition.Status.Failure;                    
                }
            }
            return ProcessCondition.Status.Ready;
        }
        public override string GetStatusMessage(Status status) => (string)UI.STARMAP.AISTATUS.NAME;
        public override string GetStatusTooltip(Status status) => (string)UI.STARMAP.AISTATUS.TOOLTIP;
        public override bool ShowInUI() => true;
    }
}
