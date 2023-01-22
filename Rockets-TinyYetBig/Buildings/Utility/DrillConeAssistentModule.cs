﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI.DEVELOPMENTBUILDS.ALPHA;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Utility
{
    internal class DrillConeAssistentModule : KMonoBehaviour, ISim4000ms
    {
        [MyCmpGet] public Storage DiamondStorage;
        [MyCmpGet] RocketModuleCluster module;

        Storage TargetStorage = null;

        public void Sim4000ms(float dt)
        {
            if(module.CraftInterface.TryGetComponent<Clustercraft>(out var clustercraft) && clustercraft.Status == Clustercraft.CraftStatus.Grounded)
            {
                CheckTarget();
            }
            else if(clustercraft.Status == Clustercraft.CraftStatus.InFlight)
            {
                if (TargetStorage != null || !TargetStorage.IsNullOrDestroyed())
                {
                    TransferDiamond();
                }
            }
        }

        private void TransferDiamond()
        {
            for (int num = DiamondStorage.items.Count - 1; num >= 0; num--)
            {
                GameObject gameObject = DiamondStorage.items[num];

                float remainingCapacity = TargetStorage.RemainingCapacity();
                float currentDiamonds = DiamondStorage.MassStored();
                bool filterable = TargetStorage.storageFilters != null && TargetStorage.storageFilters.Count > 0;

                if (remainingCapacity > 0f && currentDiamonds > 0f && (filterable ? TargetStorage.storageFilters.Contains(gameObject.PrefabID()) : true))
                {
                    Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(remainingCapacity);
                    if (pickupable != null)
                    {
                        TargetStorage.Store(pickupable.gameObject,true);
                        remainingCapacity -= pickupable.PrimaryElement.Mass;
                    }
                }
            }
        }

        private void CheckTarget()
        {

            if (TargetStorage != null && TargetStorage.IsNullOrDestroyed())
                return;

            foreach (var otherModule in module.CraftInterface.ClusterModules)
            {
                if (otherModule.Get().GetDef<ResourceHarvestModule.Def>() != null)
                {
                    if (otherModule.Get().gameObject.TryGetComponent<Storage>(out var storage))
                    {
                        TargetStorage = storage;
                        return;
                    }
                }
            }
        }
    }
}
