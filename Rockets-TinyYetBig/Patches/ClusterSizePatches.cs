﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    class ClusterSizePatches
    {
        [HarmonyPatch(typeof(ClusterManager), "OnSpawn")]
        public static class DoubleAsteroidsTest
        {
            public static void Prefix(ref int ___m_numRings)
            {
                return; ///till l8er
                SgtLogger.debuglog("Rings On Spawn: "+ ___m_numRings);
                ModAssets.InnerLimit = ___m_numRings;
                ___m_numRings +=15;
                SgtLogger.debuglog("Rings extended: " + ___m_numRings);
                ModAssets.Rings = ___m_numRings;
            }

        }
    }
}
