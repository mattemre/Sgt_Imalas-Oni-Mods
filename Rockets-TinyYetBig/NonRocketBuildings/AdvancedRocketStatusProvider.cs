﻿using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ProcessCondition;
using static Rendering.BlockTileRenderer;
using static STRINGS.UI.STARMAP;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
    internal class AdvancedRocketStatusProvider : KMonoBehaviour, ISim1000ms
    {
        [MyCmpGet] LaunchPad launchPad;
        [MyCmpGet] LogicPorts logicPorts;
        public HashedString rocketStateOutput;
        public HashedString ignoreWarningInput;
        const int RocketPath = 1, RocketConstruction = 2, RocketStorage = 4, RocketCrew = 8;

        [Serialize]
        int LaunchSignalValue = 0;
        [Serialize]
        bool[] LaunchSignalBits = null;


        public override void OnSpawn()
        {
            base.OnSpawn();

            this.Subscribe((int)GameHashes.LogicEvent, new System.Action<object>(this.OnLogicValueChanged));
        }

        private void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID !=ignoreWarningInput)
                return;
            this.UpdateSignalBitmap();
        }

        public void Sim1000ms(float dt)
        {
            RocketModuleCluster landedRocket = launchPad.LandedRocket;
            if(landedRocket != null)
            {
                int status = GetRocketProcessCondition(landedRocket.CraftInterface);
                logicPorts.SendSignal(this.rocketStateOutput, status);
            }
            else
            {
                logicPorts.SendSignal(this.rocketStateOutput, 0);
            }
        }

        public void ConvertWarnings(ProcessConditionType processConditionType, ref ProcessCondition.Status status)
        {
            if (status == Status.Warning 
                && LaunchSignalBits[0]
                && processConditionType == ProcessConditionType.RocketStorage 
                && LaunchSignalBits[1]
                )
            {
                status = Status.Ready;
            }
        }
        void UpdateSignalBitmap()
        {
            var inputBitsInt = logicPorts.GetInputValue(ignoreWarningInput);
            BitArray inputs = new BitArray(new int[] { inputBitsInt });
            LaunchSignalBits = new bool[inputs.Count];
            inputs.CopyTo(LaunchSignalBits, 0);
        }

        int GetRocketProcessCondition(CraftModuleInterface rocket)
        {
            int value = 0;
            if (EvaluateConditionSet(rocket, ProcessConditionType.RocketPrep) == ProcessCondition.Status.Ready)
                value += RocketConstruction;
            if (EvaluateConditionSet(rocket, ProcessConditionType.RocketStorage) == ProcessCondition.Status.Ready)
                value += RocketStorage;
            if (EvaluateConditionSet(rocket, ProcessConditionType.RocketBoard) == ProcessCondition.Status.Ready)
                value += RocketCrew;
            if (EvaluateConditionSet(rocket, ProcessConditionType.RocketFlight) == ProcessCondition.Status.Ready)
                value += RocketPath;
            return value;
        }
        private ProcessCondition.Status EvaluateConditionSet(CraftModuleInterface rocket, ProcessCondition.ProcessConditionType conditionType)
        {
            ProcessCondition.Status conditionSet = ProcessCondition.Status.Ready;
            foreach (ProcessCondition condition1 in rocket.GetConditionSet(conditionType))
            {
                ProcessCondition.Status condition2 = condition1.EvaluateCondition();
                if (condition2 < conditionSet)
                    conditionSet = condition2;
                if (conditionSet == ProcessCondition.Status.Failure)
                    break;
            }
            return conditionSet;
        }
    }
    
}
