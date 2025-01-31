﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Behaviours
{
    class RTB_PowerConsumerModule : EnergyConsumer 
    {
        private Clustercraft clustercraft;
        public override void OnPrefabInit()
        {
            this.IsVirtual= true;
            base.OnPrefabInit();
        }
        public override void OnSpawn()
        {

            CraftModuleInterface craftInterface = this.GetComponent<RocketModuleCluster>().CraftInterface;
            this.VirtualCircuitKey = (object)craftInterface;
            this.clustercraft = craftInterface.GetComponent<Clustercraft>();


            Game.Instance.energySim.AddEnergyConsumer(this);

            Game.Instance.circuitManager.Connect(this);
            Components.EnergyConsumers.Add(this);

            Game.Instance.electricalConduitSystem.AddToVirtualNetworks(this.VirtualCircuitKey, (object)this, true);
            SgtLogger.l(VirtualCircuitKey.ToString(), "VirtualKey");
            SgtLogger.l(BaseWattageRating.ToString(), "Wattage");
            operational.SetActive(true);
        }
        public override void EnergySim200ms(float dt)
        {
            CircuitID = Game.Instance.circuitManager.GetVirtualCircuitID(this);
            
            if (!IsConnected)
            {
                IsPowered = false;
            }
            circuitOverloadTime = Mathf.Max(0f, circuitOverloadTime - dt);
        }
        //public new float WattsUsed
        //{
        //    get
        //    {
        //        return BaseWattageRating;
        //    }
        //}
        public override void OnCleanUp()
        {
            Game.Instance.energySim.RemoveEnergyConsumer(this);

            Game.Instance.circuitManager.Disconnect(this, isDestroy: true);
            Components.EnergyConsumers.Remove(this);

            Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(this.VirtualCircuitKey, (object)this, true);
        }
    }
}
