﻿using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;
using UtilLibs;
using static SetStartDupes.ModAssets;

namespace SetStartDupes
{
    class Patches
    {

        //[HarmonyPatch(typeof(CryoTank))]
        //[HarmonyPatch(nameof(CryoTank.DropContents))]
        //public class AddToCryoTank
        //{
        //    public static void Prefix()
        //    {
        //        ModAssets.EditingSingleDupe = true;
        //        ImmigrantScreen.InitializeImmigrantScreen(null);
        //        //SingleDupeImmigrandScreen.InitializeSingleImmigrantScreen(null);
        //    }
        //    public static void Postfix(CryoTank.StatesInstance __instance)
        //    {
        //        Debug.Log("asssaaaaaaats " + _TargetStats);
        //        if (ModAssets._TargetStats != null)
        //        {
        //            var dupe = __instance.sm.defrostedDuplicant.Get(__instance);
        //            ModAssets._TargetStats.Apply(dupe);
        //            Debug.Log("Dupee " + dupe);

        //            dupe.GetComponent<MinionIdentity>().arrivalTime = UnityEngine.Random.Range(-2000, -1000);
        //            MinionResume component = dupe.GetComponent<MinionResume>();
        //            int num = 3;
        //            for (int i = 0; i < num; i++)
        //            {
        //                component.ForceAddSkillPoint();
        //            }


        //            ModAssets._TargetStats = null;
        //        }
        //    }
        //}
        //[HarmonyPatch(typeof(CharacterContainer))]
        //[HarmonyPatch(nameof(CharacterContainer.GenerateCharacter))]
        //public class OverwriteRngGeneration
        //{
        //    public static bool Prefix(CharacterContainer __instance, KButton ___selectButton)
        //    {
        //        if (ModAssets.EditingSingleDupe)
        //        {

        //            if (ModAssets._TargetStats != null)
        //            {
        //                __instance.stats = ModAssets._TargetStats;
        //            }
        //            else
        //            {
        //                __instance.stats = new MinionStartingStats(is_starter_minion: false, null, "AncientKnowledge");
        //            }

        //            __instance.SetAnimator();
        //            __instance.SetInfoText();
        //            __instance.StartCoroutine(__instance.SetAttributes());
        //            ___selectButton.ClearOnClick();
        //            ___selectButton.enabled = true;
        //            ___selectButton.onClick += delegate
        //            {
        //                __instance.SelectDeliverable();
        //            };



        //            ModAssets._TargetStats = __instance.stats;
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        //[HarmonyPatch(typeof(ImmigrantScreen))]
        //[HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
        //public class CustomSingleForNoTelepad
        //{
        //    public static bool Prefix(Telepad telepad, ImmigrantScreen __instance)
        //    {
        //        if (telepad == null && EditingSingleDupe)
        //        {
        //            __instance.DisableProceedButton();

        //            if (__instance.containers != null && __instance.containers.Count > 1)
        //            {
        //                foreach(var container in __instance.containers)
        //                {
        //                    UnityEngine.Object.Destroy(container.GetGameObject());
        //                }
        //                __instance.containers.Clear();
        //            }

        //            __instance.containers = new List<ITelepadDeliverableContainer>();

        //            CharacterContainer characterContainerZZZ = Util.KInstantiateUI<CharacterContainer>(__instance.containerPrefab.gameObject, __instance.containerParent);
        //            characterContainerZZZ.SetController(__instance);

        //            __instance.containers.Add(characterContainerZZZ);
        //            __instance.selectedDeliverables = new List<ITelepadDeliverable>();
        //            __instance.AddDeliverable(characterContainerZZZ.stats);

        //            foreach (ITelepadDeliverableContainer container in __instance.containers)
        //            {
        //                CharacterContainer characterContainer = container as CharacterContainer;
        //                if ((UnityEngine.Object)characterContainer != (UnityEngine.Object)null)
        //                    characterContainer.SetReshufflingState(false);
        //            }
        //            __instance.EnableProceedButton();
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        //[HarmonyPatch(typeof(ImmigrantScreen), "OnProceed")]
        //public class SkipTelepadStuff
        //{
        //    public static bool Prefix(Telepad ___telepad, ImmigrantScreen __instance)
        //    {
        //        if (___telepad == null && EditingSingleDupe)
        //        {
        //            var containerField = AccessTools.Field(typeof(CharacterSelectionController), "containers");
        //            var __containers = (List<ITelepadDeliverableContainer>)containerField.GetValue(__instance);
        //            var deliverablesField = AccessTools.Field(typeof(CharacterSelectionController), "selectedDeliverables");

        //            var DupeToDeliver = (MinionStartingStats)((List<ITelepadDeliverable>)deliverablesField.GetValue(__instance)).First();
        //            Debug.Log("AAAAAASSSSSSS " + DupeToDeliver);
        //            ModAssets._TargetStats = DupeToDeliver;

        //            __instance.Show(false);
        //            if (__containers != null)
        //            {
        //                __containers.ForEach((System.Action<ITelepadDeliverableContainer>)(cc => UnityEngine.Object.Destroy((UnityEngine.Object)cc.GetGameObject())));
        //                __containers.Clear();
        //            }
        //            AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
        //            AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
        //            MusicManager.instance.PlaySong("Stinger_NewDuplicant");
        //            EditingSingleDupe = false;
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        [HarmonyPatch(typeof(ImmigrantScreen))]
        [HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
        public class AddRerollButtonIfEnabled
        {
            public static void Postfix(Telepad telepad, ImmigrantScreen __instance)
            {
                if (StartDupeConfig.Instance.RerollDuringGame)
                {
                    foreach (ITelepadDeliverableContainer container in __instance.containers)
                    {
                        CharacterContainer characterContainer = container as CharacterContainer;
                        if (characterContainer != null)
                        {
                            characterContainer.SetReshufflingState(true);
                        }
                    }
                }
            }
        }





        [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
        public class GrabButtpnPrefab2
        {
            public static void Postfix(KButton ___proceedButton)
            {
                //Debug.Log("Creating PREFAB2");
                NextButtonPrefab = Util.KInstantiateUI(___proceedButton.gameObject);
                //UIUtils.ListAllChildren(NextButtonPrefab.transform);
                NextButtonPrefab.name = "CycleButtonPrefab";
            }
        }



        [HarmonyPatch(typeof(WattsonMessage))]
        [HarmonyPatch(nameof(WattsonMessage.OnActivate))]
        public class DupeSpawnAdjustmentNo2BecauseKleiIsKlei
        {
            const float OxilitePerDupePerDay = 0.1f * 600f; //in KG
            const float FoodBarsPerDupePerDay = 1000 / 800f; //in Units
            static void Postfix()
            {
                if (StartDupeConfig.Instance.StartupResources && StartDupeConfig.Instance.DuplicantStartAmount > 3)
                {
                    GameObject telepad = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
                    float dupeCount = StartDupeConfig.Instance.DuplicantStartAmount;

                    float OxiliteNeeded = OxilitePerDupePerDay * StartDupeConfig.Instance.SupportedDays * (dupeCount - 3);
                    float FoodeNeeded = FoodBarsPerDupePerDay * StartDupeConfig.Instance.SupportedDays * (dupeCount - 3);
                    Vector3 SpawnPos = telepad.transform.position;

                    while (OxiliteNeeded > 0)
                    {
                        var SpawnAmount = Math.Min(OxiliteNeeded, 25000f);
                        OxiliteNeeded -= SpawnAmount;
                        ElementLoader.FindElementByHash(SimHashes.OxyRock).substance.SpawnResource(SpawnPos, SpawnAmount, UtilLibs.UtilMethods.GetKelvinFromC(20f), byte.MaxValue, 0, false);
                    }

                    GameObject go = Util.KInstantiate(Assets.GetPrefab(FieldRationConfig.ID));
                    go.transform.SetPosition(SpawnPos);
                    PrimaryElement component2 = go.GetComponent<PrimaryElement>();
                    component2.Units = FoodeNeeded;
                    go.SetActive(true);
                }
            }

            static void YeetOxilite(GameObject originGo, float amount)
            {

                GameObject go = Util.KInstantiate(Assets.GetPrefab(FieldRationConfig.ID));
                go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(originGo), Grid.SceneLayer.Ore));
                PrimaryElement component2 = go.GetComponent<PrimaryElement>();
                component2.Units = amount;
                go.SetActive(true);


                Vector2 initial_velocity = new Vector2(UnityEngine.Random.Range(-2f, 2f) * 1f, (float)((double)UnityEngine.Random.value * 2.0 + 4.0));
                if (GameComps.Fallers.Has((object)go))
                    GameComps.Fallers.Remove(go);
                GameComps.Fallers.Add(go, initial_velocity);
            }


            public static float AdjustCellX(float OldX, GameObject printingPod, int index) ///int requirement to consume previous "3" on stack
            {
                int newCell = Grid.PosToCell(printingPod) + ((index + 1) % 4 - 1);
                //Debug.Log("Old CellPosX: " + OldX + ", New CellPos: " + Grid.CellToXY(newCell));
                //YeetOxilite(printingPod, 150f);
                return (float)Grid.CellToXY(newCell).x + 0.5f;
            }

            public static readonly MethodInfo NewCellX = AccessTools.Method(
               typeof(DupeSpawnAdjustmentNo2BecauseKleiIsKlei),
               nameof(DupeSpawnAdjustmentNo2BecauseKleiIsKlei.AdjustCellX));

            public static readonly MethodInfo GetPrintingPodInfo = AccessTools.Method(
               typeof(GameUtil),
               nameof(GameUtil.GetTelepad));

            public static readonly MethodInfo GetDupeFromComponentInfo = AccessTools.Method(
               typeof(Components.Cmps<MinionIdentity>),
               ("get_Item"));


            [HarmonyPriority(Priority.Last)]
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Sub);
                var insertionIndexPrintingPodInfo = code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == GetPrintingPodInfo);
                var minionGetterIndexInfo = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == GetDupeFromComponentInfo);

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                    int printingPodIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, insertionIndexPrintingPodInfo, false);
                    int IDXIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, minionGetterIndexInfo);

                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, printingPodIndex));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, IDXIndex));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, NewCellX));
                    // TranspilerHelper.PrintInstructions(code);
                }
                //foreach (var v in code) { Console.WriteLine(v.opcode + (v.operand != null ? ": " + v.operand : "")); };
                return code;
            }
        }

        [HarmonyPatch(typeof(MinionStartingStats), "GenerateTraits")]
        [HarmonyPatch(nameof(MinionStartingStats.GenerateTraits))]
        public class AllowCustomTraitAllignment
        {
            public static bool VariableTraits(bool isStarterMinion) ///int requirement to consume previous "3" on stack
            {
                return false;
            }

            public static readonly MethodInfo overrideStarterGeneration = AccessTools.Method(
               typeof(AllowCustomTraitAllignment),
               nameof(AllowCustomTraitAllignment.VariableTraits));

            public static readonly MethodInfo PreviousCellXY = AccessTools.Method(
               typeof(Grid),
               nameof(Grid.XYToCell));

            [HarmonyPriority(Priority.Last)]
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Ldfld && ci.operand.ToString().Contains("is_starter_minion"));

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                    //++insertionIndex;
                    //code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, 7));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, overrideStarterGeneration));
                    //code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Stloc_S,  7));
                    //code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, 7));
                }
                //foreach (var v in code) { Console.WriteLine(v.opcode + (v.operand != null ? ": " + v.operand : "")); };
                return code;
            }
        }

        [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
        public class CharacterSelectionController_Patch
        {
            public static int CustomStartingDupeCount(int dupeCount) ///int requirement to consume previous "3" on stack
            {
                if (dupeCount == 3)
                    return StartDupeConfig.Instance.DuplicantStartAmount; ///push new value to the stack
                else return dupeCount;
            }

            public static readonly MethodInfo AdjustNumber = AccessTools.Method(
               typeof(CharacterSelectionController_Patch),
               nameof(CustomStartingDupeCount));

            [HarmonyPriority(Priority.Last)]
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Ldc_I4_3);

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, AdjustNumber));
                }
                return code;
            }

            /// <summary>
            /// Size Adjustment
            /// </summary>
            /// <param name="__instance"></param>
            public static void Prefix(CharacterSelectionController __instance)
            {

                Debug.Log(__instance.GetType());
                GameObject parentToScale = (GameObject)typeof(CharacterSelectionController).GetField("containerParent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                CharacterContainer prefabToScale = (CharacterContainer)typeof(CharacterSelectionController).GetField("containerPrefab", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                if (__instance.GetType() == typeof(MinionSelectScreen))
                {
#if DEBUG
                    Debug.Log("Manipulating Instance: " + __instance.GetType());

                    //UIUtils.ListAllChildren(__instance.transform);
                    // UIUtils.ListAllChildrenWithComponents(__instance.transform);

#endif

                    GridLayoutGroup[] objectsOfType2 = UnityEngine.Object.FindObjectsOfType<GridLayoutGroup>();
                    foreach (var layout in objectsOfType2)
                    {
                        if (layout.name == "CharacterContainers")
                        {
                            ///adding scroll
                            var scroll = layout.transform.parent.parent.FindOrAddComponent<ScrollRect>();
                            scroll.content = layout.transform.parent.rectTransform();
                            scroll.horizontal = false;
                            scroll.scrollSensitivity = 100;
                            scroll.movementType = ScrollRect.MovementType.Clamped;
                            scroll.inertia = false;
                            ///setting start pos
                            layout.transform.parent.rectTransform().pivot = new Vector2(0.5f, 0.99f);

                            ///top & bottom padding
                            layout.transform.parent.TryGetComponent<VerticalLayoutGroup>(out var verticalLayoutGroup);
                            verticalLayoutGroup.padding = new RectOffset(00, 00, 50, 50);
                            layout.childAlignment = TextAnchor.UpperCenter;
                            int countPerRow = StartDupeConfig.Instance.DuplicantStartAmount;

                            layout.constraintCount = 5;
                        }
                    }
                    __instance.transform.Find("Content/BottomContent").TryGetComponent<VerticalLayoutGroup>(out var buttonGroup);
                    buttonGroup.childAlignment = TextAnchor.LowerCenter;
                }

#if DEBUG
                //Debug.Log("PREFAB: " + size);
#endif
            }

            public static void Postfix(CharacterSelectionController __instance, CharacterContainer ___containerPrefab)
            {
                if (ModAssets.StartPrefab == null)
                {
                    StartPrefab = ___containerPrefab.transform.Find("Details").gameObject;
                    //StartPrefab.transform.Find("Top/PortraitContainer/PortraitContent").gameObject.SetActive(false);
                    //StartPrefab.transform.name = "ModifyDupeStats";

                }
                if (!__instance.IsStarterMinion)
                    return;

                LocText[] objectsOfType1 = UnityEngine.Object.FindObjectsOfType<LocText>();
                if (objectsOfType1 != null)
                {
                    foreach (LocText locText in objectsOfType1)
                    {
                        if (locText.key == "STRINGS.UI.IMMIGRANTSCREEN.SELECTYOURCREW")
                        {
                            locText.key = StartDupeConfig.Instance.DuplicantStartAmount == 1 ? "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURLONECREWMAN" : "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURCREW";
                            break;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CharacterContainer), "OnCmpDisable")]
        public static class RestoreOnCLosing
        {
            public static void Prefix(CharacterContainer __instance, Transform ___aptitudeLabel)
            {
#if DEBUG
                //Debug.Log("Closing start");
                //UIUtils.ListAllChildren(__instance.transform);
                //Debug.Log("Closing Stop");
#endif


                __instance.transform.Find("Details").gameObject.SetActive(true);

                var skillMod = __instance.transform.Find("ModifyDupeStats");

                if (skillMod == null)
                    return;
                skillMod.gameObject.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
        public static class AddChangeButtonToCharacterContainer
        {
            public static void Postfix(CharacterContainer __instance, MinionStartingStats ___stats, bool is_starter)
            {
                ///Only during startup when config is disabled

                //bool IsWhackyDupe = false;
                //Type BioInksCustomDupeType = Type.GetType("PrintingPodRecharge.Cmps.CustomDupe, PrintingPodRecharge", false, false);
                //if(BioInksCustomDupeType != null)
                //{

                //    //var obj = go.gameObject.GetComponent(VaricolouredBalloonsHelperType);
                //    ////foreach (var cmp in VaricolouredBalloonsHelperType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) 
                //    ////   SgtLogger.l(cmp.Name.ToString(),"GET Field");
                //    ////foreach (var cmp in VaricolouredBalloonsHelperType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
                //    ////    SgtLogger.l(cmp.Name.ToString(), "GET method");

                //    //var component = go.GetComponent(VaricolouredBalloonsHelperType);
                //    //var fieldInfo = (uint)Traverse.Create(component).Method("get_ArtistBalloonSymbolIdx").GetValue();
                //}

                var buttonPrefab = __instance.transform.Find("TitleBar/RenameButton").gameObject;
                var titlebar = __instance.transform.Find("TitleBar").gameObject;

                float insetDistance = (!is_starter && !StartDupeConfig.Instance.ModifyDuringGame) ? 40 : 75;
                ///Make skin button
                var skinBtn = Util.KInstantiateUI(buttonPrefab, titlebar);
                skinBtn.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, insetDistance, skinBtn.rectTransform().sizeDelta.x);
                skinBtn.name = "ChangeDupeStatButton";
                skinBtn.GetComponent<ToolTip>().toolTip = "Select dupe skin"; ///STRINGLOC!

                skinBtn.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("ic_dupe");
                //var currentlySelectedIdentity = __instance.GetComponent<MinionIdentity>();

                System.Action RebuildDupePanel = () =>
                {
                    __instance.SetInfoText();
                    __instance.SetAttributes();
                    __instance.SetAnimator();
                };

                UIUtils.AddActionToButton(skinBtn.transform, "", () => DupeSkinScreenAddon.ShowSkinScreen(__instance, ___stats));

                

                if(!(!is_starter && !StartDupeConfig.Instance.ModifyDuringGame))
                {

                    float insetDistancePresetButton =  110;
                    ///Make Preset button
                    var PresetButton = Util.KInstantiateUI(buttonPrefab, titlebar);
                    PresetButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, insetDistancePresetButton, PresetButton.rectTransform().sizeDelta.x);
                    PresetButton.name = "DupePresetButton";
                    PresetButton.GetComponent<ToolTip>().toolTip = "Load stat preset"; ///STRINGLOC!

                    PresetButton.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("iconPaste");
                    //var currentlySelectedIdentity = __instance.GetComponent<MinionIdentity>();

                    //UIUtils.AddActionToButton(PresetButton.transform, "", () => DupePresetScreenAddon.ShowPresetScreen(__instance, ___stats)); 
                    UIUtils.AddActionToButton(PresetButton.transform, "", () => UnityPresetScreen.ShowWindow(___stats, RebuildDupePanel));
                }



                if (!is_starter && !StartDupeConfig.Instance.ModifyDuringGame)
                    return;
                ///Make modify button
                var changebtn = Util.KInstantiateUI(buttonPrefab, titlebar);
                changebtn.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 40f, changebtn.rectTransform().sizeDelta.x);
                changebtn.name = "ChangeDupeStatButton";
                changebtn.GetComponent<ToolTip>().toolTip = "Adjust dupe stats";///STRINGLOC! TODO

                var img = changebtn.transform.Find("Image").GetComponent<KImage>();
                img.sprite = Assets.GetSprite("icon_gear");

                var button = __instance.transform.Find("ShuffleDupeButton").GetComponent<KButton>();
                var button2 = __instance.transform.Find("ArchetypeSelect").GetComponent<KButton>();


                ChangeButton(false, changebtn, __instance, ___stats, button, button2, RebuildDupePanel);

                CycleButtonLeftPrefab = Util.KInstantiateUI(buttonPrefab);
                CycleButtonLeftPrefab.GetComponent<ToolTip>().enabled = false;
                CycleButtonLeftPrefab.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("iconLeft");
                CycleButtonLeftPrefab.name = "PrevButton";

                CycleButtonRightPrefab = Util.KInstantiateUI(buttonPrefab);
                CycleButtonRightPrefab.GetComponent<ToolTip>().enabled = false;
                CycleButtonRightPrefab.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("iconRight");
                CycleButtonRightPrefab.name = "NextButton";
            }

            static void ChangeButton(bool isCurrentlyInEditMode, GameObject buttonGO, CharacterContainer parent, MinionStartingStats referencedStats, KButton ButtonToDisable, KButton ButtonToDisableAswell, System.Action OnClose)
            {
                buttonGO.GetComponent<ToolTip>().SetSimpleTooltip(!isCurrentlyInEditMode ? "Adjust dupe stats" : "Store Settings");
                var img = buttonGO.transform.Find("Image").GetComponent<KImage>();
                img.sprite = Assets.GetSprite(!isCurrentlyInEditMode ? "icon_gear" : "iconSave");
                var button = buttonGO.GetComponent<KButton>();
                button.ClearOnClick();
                button.onClick += () =>
                {
                    ChangeButton(!isCurrentlyInEditMode, buttonGO, parent, referencedStats, ButtonToDisable, ButtonToDisableAswell, OnClose);
                    if (isCurrentlyInEditMode)
                    {
                        InstantiateOrGetDupeModWindow(parent.gameObject, referencedStats, true);
                        OnClose.Invoke();
                    }
                    else
                    {
                        InstantiateOrGetDupeModWindow(parent.gameObject, referencedStats, false);
                    }
                    ButtonToDisable.isInteractable = isCurrentlyInEditMode;
                    ButtonToDisableAswell.isInteractable = isCurrentlyInEditMode;
                };
                parent.transform.Find("Details").gameObject.SetActive(!isCurrentlyInEditMode);
            }

            static void InstantiateOrGetDupeModWindow(GameObject parent, MinionStartingStats referencedStats, bool hide)
            {

                bool ShouldInit = true;
                var ParentContainer = parent.transform.Find("ModifyDupeStats");


                if (ParentContainer == null)
                {
                    //Debug.Log("HAD TO MAKE NEW");
                    ParentContainer = Util.KInstantiateUI(StartPrefab, parent).transform;
                    ParentContainer.gameObject.name = "ModifyDupeStats";
                }
                else
                {
                    //Debug.Log("FOUND OLD");
                    ParentContainer.name = "oldd";
                    ParentContainer.gameObject.SetActive(false);

                    ParentContainer = Util.KInstantiateUI(StartPrefab, parent).transform;
                    ParentContainer.gameObject.name = "ModifyDupeStats";
                    //ShouldInit = false;
                }


                ///Building the Button window
                if (ShouldInit)
                {

                    //Debug.Log("FindScroll");
                    //UIUtils.ListAllChildren(ParentContainer.transform);
                    //Debug.Log("endFindScroll");

                    UIUtils.FindAndDestroy(ParentContainer.transform, "Top");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "AttributeScores");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "AttributeScores");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "Scroll/Content/TraitsAndAptitudes/AptitudeContainer");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "Scroll/Content/TraitsAndAptitudes/TraitContainer");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "Scroll/Content/ExpectationsGroupAlt");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "Scroll/Content/DescriptionGroup");

                    var ContentContainer = ParentContainer.Find("Scroll/Content/TraitsAndAptitudes");
                    var overallSize = ParentContainer.Find("Scroll");
                    var SizeSetter = ParentContainer.Find("Scroll").GetComponent<LayoutElement>();
                    SizeSetter.flexibleHeight = 600;


                    //UIUtils.ListComponents(overallSize.gameObject);


                    //overallSize.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 600);
                    ///Building 3 button prefab for switching traits / interests
                    var prefabParent = NextButtonPrefab;
                    if (prefabParent.transform.Find("NextButton") == null)
                    {

                        prefabParent.GetComponent<KButton>().enabled = false;
                        var left = Util.KInstantiateUI(CycleButtonLeftPrefab, prefabParent);
                        left.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 30);
                        UIUtils.TryFindComponent<ToolTip>(left.transform).toolTip = "Cycle to previous";
                        //UIUtils.TryFindComponent<ToolTip>(left.transform, "Image").toolTip= "Cycle to previous";
                        var right = Util.KInstantiateUI(CycleButtonRightPrefab, prefabParent);
                        UIUtils.TryFindComponent<ToolTip>(right.transform).toolTip = "Cycle to next";
                        //UIUtils.TryFindComponent<ToolTip>(right.transform,"Image").toolTip="Cycle to next";
                        right.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 30);
                    }


                    var renameLabel = prefabParent.transform.Find("SelectLabel");
                    if (renameLabel != null)
                    {
                        renameLabel.name = "Label";
                    }

                    //Debug.Log(prefabParent.GetComponent<LayoutElement>().preferredHeight + "PREF HEIG");
                    //Debug.Log(prefabParent.GetComponent<LayoutElement>().minHeight + "min HEIG");
                    prefabParent.GetComponent<LayoutElement>().minHeight = 20;
                    prefabParent.GetComponent<LayoutElement>().preferredHeight = 30;
                    var spacerParent = prefabParent.transform.Find("Label").gameObject;

                    //skillMod.transform.Find("DetailsContainer").gameObject.SetActive(false);
                    var DupeTraitMng = ParentContainer.FindOrAddComponent<DupeTraitManager>();



                    var Spacer2AndInterestHolder = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);

                    UIUtils.TryChangeText(Spacer2AndInterestHolder.transform, "", "INTERESTS");


                    ///Aptitudes

                    DupeTraitMng.referencedInterests = referencedStats.skillAptitudes;
                    DupeTraitMng.dupeStatPoints = referencedStats.StartingLevels;
                    DupeTraitMng.GetInterestsWithStats();
                    DupeTraitMng.AddSkillLevels(ref referencedStats.StartingLevels);
                    int index = 0;

                    foreach (var a in DupeTraitMng.GetInterestsWithStats())
                    {
                        var AptitudeEntry = Util.KInstantiateUI(prefabParent, ContentContainer.gameObject, true);

                        var name = AptitudeEntry.AddComponent<DupeTraitHolder>();
                        name.Group = DupeTraitMng.ActiveInterests[index];
                        AptitudeEntry.GetComponent<KButton>().enabled = false;
                        ApplyDefaultStyle(AptitudeEntry.GetComponent<KImage>());
                        UIUtils.TryChangeText(AptitudeEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, name.NAME(), name.RelevantAttribute(), DupeTraitMng.GetBonusValue(index)));


                        UIUtils.AddActionToButton(AptitudeEntry.transform, "NextButton", () =>
                        {
                            int prevInd = DupeTraitMng.GetCurrentIndex(name.Group.Id);
                            DupeTraitMng.GetNextInterest(prevInd);
                            name.Group = DupeTraitMng.ActiveInterests[prevInd];
                            UIUtils.TryChangeText(AptitudeEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, name.NAME(), name.RelevantAttribute(), DupeTraitMng.GetBonusValue(prevInd)));
                        }
                        );
                        UIUtils.AddActionToButton(AptitudeEntry.transform, "PrevButton", () =>
                        {
                            int prevInd = DupeTraitMng.GetCurrentIndex(name.Group.Id);
                            DupeTraitMng.GetNextInterest(prevInd, true);
                            name.Group = DupeTraitMng.ActiveInterests[prevInd];
                            UIUtils.TryChangeText(AptitudeEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, name.NAME(), name.RelevantAttribute(), DupeTraitMng.GetBonusValue(prevInd)));
                        }
                        );
                        index++;
                    }
                    ///EndAptitudes

                    var spacer3 = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);
                    UIUtils.TryChangeText(spacer3.transform, "", "TRAITS");
                    //Db.Get().traits.TryGet();

                    var TraitsToSort = new List<Tuple<GameObject, DupeTraitManager.NextType>>();


                    foreach (Trait v in referencedStats.Traits)
                    {
                        if (v.Id == MinionConfig.MINION_BASE_TRAIT_ID)
                            continue;
                        var traitEntry = Util.KInstantiateUI(prefabParent, ContentContainer.gameObject, true);
                        DupeTraitMng.AddTrait(v.Id);
                        var TraitHolder = traitEntry.AddComponent<DupeTraitHolder>();
                        TraitHolder.CurrentTrait = v;
                        UIUtils.AddSimpleTooltipToObject(traitEntry.transform, TraitHolder.GetTraitTooltip(), true);
                        var type = DupeTraitManager.GetTraitListOfTrait(v.Id, out var list);
                        TraitsToSort.Add(new Tuple<GameObject, DupeTraitManager.NextType>(traitEntry, type));

                        ApplyTraitStyleByKey(traitEntry.GetComponent<KImage>(), type);
                        ApplyTraitStyleByKey(traitEntry.transform.Find("PrevButton").GetComponent<KImage>(), type);
                        ApplyTraitStyleByKey(traitEntry.transform.Find("NextButton").GetComponent<KImage>(), type);
                        UIUtils.TryChangeText(traitEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, v.Name));
                        UIUtils.AddActionToButton(traitEntry.transform, "NextButton", () =>
                        {

                            string nextTraitId = DupeTraitMng.GetNextTraitId(TraitHolder.CurrentTrait.Id, false);
                            Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                            DupeTraitMng.ReplaceTrait(TraitHolder.CurrentTrait.Id, nextTraitId);
                            referencedStats.Traits.Remove(TraitHolder.CurrentTrait);
                            referencedStats.Traits.Add(NextTrait);
                            TraitHolder.CurrentTrait = NextTrait;

                            UIUtils.TryChangeText(traitEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, TraitHolder.CurrentTrait.Name));
                            UIUtils.AddSimpleTooltipToObject(traitEntry.transform, TraitHolder.GetTraitTooltip(), true);
                        });
                        UIUtils.AddActionToButton(traitEntry.transform, "PrevButton", () =>
                        {

                            string nextTraitId = DupeTraitMng.GetNextTraitId(TraitHolder.CurrentTrait.Id, true);
                            Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                            DupeTraitMng.ReplaceTrait(TraitHolder.CurrentTrait.Id, nextTraitId);
                            referencedStats.Traits.Remove(TraitHolder.CurrentTrait);
                            referencedStats.Traits.Add(NextTrait);
                            TraitHolder.CurrentTrait = NextTrait;

                            UIUtils.TryChangeText(traitEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, TraitHolder.CurrentTrait.Name));
                            UIUtils.AddSimpleTooltipToObject(traitEntry.transform, TraitHolder.GetTraitTooltip(), true);
                        });
                    }

                    TraitsToSort = TraitsToSort.OrderBy(t => (int)t.second).ToList();
                    for (int i = 0; i < TraitsToSort.Count; i++)
                    {
                        TraitsToSort[i].first.transform.SetAsLastSibling();
                    }

                    var spacer = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);
                    UIUtils.TryChangeText(spacer.transform, "", "REACTIONS");

                    var JoyTrait = Util.KInstantiateUI(prefabParent, ContentContainer.gameObject, true);
                    DupeTraitMng.AddTrait(referencedStats.joyTrait.Id);


                    //var JoyType = HoldMyReferences.GetTraitListOfTrait(referencedStats.joyTrait.Name, out var list);

                    var JoyHolder = JoyTrait.AddComponent<DupeTraitHolder>();
                    JoyHolder.CurrentTrait = referencedStats.joyTrait;
                    ApplyTraitStyleByKey(JoyTrait.GetComponent<KImage>(), DupeTraitManager.NextType.joy);
                    ApplyTraitStyleByKey(JoyTrait.transform.Find("PrevButton").GetComponent<KImage>(), DupeTraitManager.NextType.joy);
                    ApplyTraitStyleByKey(JoyTrait.transform.Find("NextButton").GetComponent<KImage>(), DupeTraitManager.NextType.joy);
                    UIUtils.TryChangeText(JoyTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.JOYREACTION, referencedStats.joyTrait.Name));
                    UIUtils.AddSimpleTooltipToObject(JoyTrait.transform, JoyHolder.GetTraitTooltip(), true);

                    UIUtils.AddActionToButton(JoyTrait.transform, "NextButton", () =>
                    {
                        string nextTraitId = DupeTraitMng.GetNextTraitId(JoyHolder.CurrentTrait.Id, false);
                        Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                        DupeTraitMng.ReplaceTrait(JoyHolder.CurrentTrait.Id, nextTraitId);
                        referencedStats.joyTrait = NextTrait;
                        JoyHolder.CurrentTrait = NextTrait;

                        UIUtils.TryChangeText(JoyTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.JOYREACTION, referencedStats.joyTrait.Name));
                        UIUtils.AddSimpleTooltipToObject(JoyTrait.transform, JoyHolder.GetTraitTooltip(), true);
                    });
                    UIUtils.AddActionToButton(JoyTrait.transform, "PrevButton", () =>
                    {
                        string nextTraitId = DupeTraitMng.GetNextTraitId(JoyHolder.CurrentTrait.Id, true);
                        Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                        DupeTraitMng.ReplaceTrait(JoyHolder.CurrentTrait.Id, nextTraitId);
                        referencedStats.joyTrait = NextTrait;
                        JoyHolder.CurrentTrait = NextTrait;

                        UIUtils.TryChangeText(JoyTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.JOYREACTION, referencedStats.joyTrait.Name));
                        UIUtils.AddSimpleTooltipToObject(JoyTrait.transform, JoyHolder.GetTraitTooltip(), true);
                    }
                     );

                    var StressTrait = Util.KInstantiateUI(prefabParent, ContentContainer.gameObject, true);

                    DupeTraitMng.AddTrait(referencedStats.stressTrait.Id);

                    ApplyTraitStyleByKey(StressTrait.GetComponent<KImage>(), DupeTraitManager.NextType.stress);
                    ApplyTraitStyleByKey(StressTrait.transform.Find("PrevButton").GetComponent<KImage>(), DupeTraitManager.NextType.stress);
                    ApplyTraitStyleByKey(StressTrait.transform.Find("NextButton").GetComponent<KImage>(), DupeTraitManager.NextType.stress);

                    var StressHolder = JoyTrait.AddComponent<DupeTraitHolder>();
                    StressHolder.CurrentTrait = referencedStats.stressTrait;

                    UIUtils.AddSimpleTooltipToObject(StressTrait.transform, StressHolder.GetTraitTooltip(), true);
                    UIUtils.TryChangeText(StressTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.STRESSREACTION, referencedStats.stressTrait.Name));

                    UIUtils.AddActionToButton(StressTrait.transform, "NextButton", () =>
                    {
                        string nextTraitId = DupeTraitMng.GetNextTraitId(StressHolder.CurrentTrait.Id, false);
                        Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                        DupeTraitMng.ReplaceTrait(StressHolder.CurrentTrait.Id, nextTraitId);
                        referencedStats.stressTrait = NextTrait;
                        StressHolder.CurrentTrait = NextTrait;
                        UIUtils.TryChangeText(StressTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.STRESSREACTION, referencedStats.stressTrait.Name));
                        UIUtils.AddSimpleTooltipToObject(StressTrait.transform, StressHolder.GetTraitTooltip(), true);
                    });
                    UIUtils.AddActionToButton(StressTrait.transform, "PrevButton", () =>
                    {
                        string nextTraitId = DupeTraitMng.GetNextTraitId(StressHolder.CurrentTrait.Id, true);
                        Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                        DupeTraitMng.ReplaceTrait(StressHolder.CurrentTrait.Id, nextTraitId);
                        referencedStats.stressTrait = NextTrait;
                        StressHolder.CurrentTrait = NextTrait;
                        UIUtils.TryChangeText(StressTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.STRESSREACTION, referencedStats.stressTrait.Name));
                        UIUtils.AddSimpleTooltipToObject(StressTrait.transform, StressHolder.GetTraitTooltip(), true);
                    });
                }



                ParentContainer.gameObject.SetActive(!hide);
            }
        }


        /// <summary>
        /// /// Init. auto translation
        /// /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
    }
}
