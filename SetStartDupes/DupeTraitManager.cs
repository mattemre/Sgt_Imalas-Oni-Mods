﻿using Database;
using Klei.AI;
using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static KInputController;

namespace SetStartDupes
{
    public class DupeTraitManager
    {
        [Serialize]
        public Dictionary<string, int> USEDSTATS = new();

        public List<string> currentTraitIds = new();
        [Serialize]
        public int strengthSkillHeightHolder = -1;


        [Serialize]
        public List<SkillGroup> ActiveInterests = new();

        MinionStartingStats ToEditMinionStats = null;

        int FallBack = -1;

        int additionalSkillPoints = 0;
        int skillPointPool = 0;
        public int SkillPointPool => skillPointPool;
        public int AdditionalSkillPoints => additionalSkillPoints;

        public int ExternalModPoints => _externalModPoints;
        int _externalModPoints = 0;

        public string PointPool => UIUtils.ColorNumber(skillPointPool);

        public enum NextType
        {
            geneShufflerTrait,
            posTrait,
            needTrait,
            negTrait,
            joy,
            stress,
            undefined,
            allTraits
        }
        internal void SetReferenceStats(MinionStartingStats referencedStats)
        {
            if (ToEditMinionStats != referencedStats)
            {
                SgtLogger.l("Redoing Reference");
                ToEditMinionStats = referencedStats;
                RecalculateAll();

            }
        }

        public void RecalculateAll()
        {
            ExternalModBonusPointCalculation();
            CalculateAdditionalSkillPoints();
        }

        public void CalculateAdditionalSkillPoints(int overridePoints = 0)
        {
            //if (overridePoints > 0)
            //{
            //    additionalSkillPoints = overridePoints;
            //    return;
            //}
            additionalSkillPoints = ModAssets.GetTraitBonus(ToEditMinionStats);
        }
        public void ExternalModBonusPointCalculation()
        {
            _externalModPoints = 0;
            int PointsPerInterest = ModAssets.MinimumPointsPerInterest(ToEditMinionStats);

            List<string> Names = new List<string>();
            foreach (var interest in ToEditMinionStats.skillAptitudes)
            {
                if (interest.Value > 0)
                {
                    foreach (var attr in interest.Key.relevantAttributes)
                        Names.Add(attr.Id);
                }
            }

            foreach (var startingLevel in ToEditMinionStats.StartingLevels)
            {
                if (Names.Contains(startingLevel.Key))
                    _externalModPoints += Math.Max(0, (startingLevel.Value - PointsPerInterest));
            }
            _externalModPoints -= ModAssets.GetTraitBonus(ToEditMinionStats);

            if (_externalModPoints != 0)
                ModAssets.OtherModBonusPoints.Add(ToEditMinionStats, _externalModPoints);
        }

        public void DeltaPointPool(int delta)
        {
            skillPointPool += delta;
        }
        public void ResetPool()
        {
            skillPointPool = 0;
        }

        public void IncreaseInterest(SkillGroup interest)
        {
            if (skillPointPool > 0 || !ModConfig.Instance.BalanceAddRemove)
            {
                foreach (var attribute in interest.relevantAttributes)
                {
                    if (ToEditMinionStats.StartingLevels.ContainsKey(attribute.Id))
                    {
                        ToEditMinionStats.StartingLevels[attribute.Id]++;
                    }
                }
                skillPointPool--;
            }
        }
        public void ReduceInterest(SkillGroup interest)
        {
            int minimumPoints = ModAssets.MinimumPointsPerInterest(ToEditMinionStats);
            foreach (var attribute in interest.relevantAttributes)
            {
                if (ToEditMinionStats.StartingLevels.ContainsKey(attribute.Id))
                {
                    if (ToEditMinionStats.StartingLevels[attribute.Id] <= minimumPoints)
                        return;

                    ToEditMinionStats.StartingLevels[attribute.Id]--;
                }
            }
            skillPointPool++;
        }


        public void ReplaceInterest(SkillGroup interestOld, SkillGroup interestNew)
        {
            int oldPoints = RemoveInterest(interestOld, false);
            AddInterest(interestNew, true, oldPoints);
        }

        public int RemoveInterest(SkillGroup interest, bool rebalanceAfter = true)
        {
            if (interest == null) return 0;

            int removedPoints = 0;

            SgtLogger.l(interest.Name, "Removing Interest");

            if (ToEditMinionStats.skillAptitudes.ContainsKey(interest))
                ToEditMinionStats.skillAptitudes.Remove(interest);

            var LevelsToRemove = new List<Klei.AI.Attribute>(interest.relevantAttributes);

            //SgtLogger.l(LevelsToRemove.Count().ToString());

            foreach (var aptitude in ToEditMinionStats.skillAptitudes.Keys)
            {
                var overlapping = aptitude.relevantAttributes.Intersect(LevelsToRemove);
                foreach (var lap in overlapping)
                {
                    SgtLogger.l(lap.Id, "not removing skillpoints for");
                    LevelsToRemove.Remove(lap);
                }
            }
            foreach (var levelToRemove in LevelsToRemove)
            {
                SgtLogger.l(levelToRemove.Name, "Removing stats for");
                if (ToEditMinionStats.StartingLevels.ContainsKey(levelToRemove.Id))
                {
                    removedPoints += ToEditMinionStats.StartingLevels[levelToRemove.Id];
                    ToEditMinionStats.StartingLevels[levelToRemove.Id] = 0;
                }
            }

            if (rebalanceAfter)
                RecalculateSkillPoints();

            if (removedPoints == 0)
                removedPoints++;

            ResetPool();

            return removedPoints;
        }
        public void AddInterest(SkillGroup interest, bool rebalanceAfter = true, int newPoints = 1)
        {
            if (interest == null) return;
            SgtLogger.l(interest.Name, "Adding Interest");


            var LevelsToAdd = new List<Klei.AI.Attribute>(interest.relevantAttributes);

            SgtLogger.l(LevelsToAdd.Count().ToString());

            foreach (var aptitude in ToEditMinionStats.skillAptitudes.Keys)
            {
                var overlapping = aptitude.relevantAttributes.Intersect(LevelsToAdd);
                foreach (var lap in overlapping)
                {
                    SgtLogger.l(lap.Name, "Overlapping");

                    if (LevelsToAdd.Contains(lap))
                    {
                        SgtLogger.l(lap.Id, "not adding skillpoints for");
                        LevelsToAdd.Remove(lap);
                    }
                }
            }

            if (!ToEditMinionStats.skillAptitudes.ContainsKey(interest))
                ToEditMinionStats.skillAptitudes.Add(interest, 1.0f);

            foreach (var LevelToAdd in LevelsToAdd)
            {
                SgtLogger.l(LevelToAdd.Name, "adding stats for");
                if (ToEditMinionStats.StartingLevels.ContainsKey(LevelToAdd.Id))
                {
                    ToEditMinionStats.StartingLevels[LevelToAdd.Id] = newPoints;
                }
            }
            if (rebalanceAfter)
                RecalculateSkillPoints();
            ResetPool();
        }

        int pointsPerInterest()
        {
            int SkillAmount = 0;
            foreach (var s in ToEditMinionStats.StartingLevels)
            {
                if (s.Value > 0)
                {
                    SkillAmount++;
                }
            }

            return ModAssets.PointsPerInterests(SkillAmount);
        }

        void RecalculateSkillPoints()
        {
            int amountToShip = AdditionalSkillPoints;

            Dictionary<string, int> newVals = new Dictionary<string, int>();

            bool reroll = false;
            do
            {
                foreach (var level in ToEditMinionStats.StartingLevels)
                {
                    if (level.Value > 0)
                    {
                        reroll = true;
                        int randomPoints = UnityEngine.Random.Range(0, amountToShip + 1);
                        amountToShip -= randomPoints;

                        if (!newVals.ContainsKey(level.Key))
                            newVals.Add(level.Key, pointsPerInterest() + randomPoints);
                        else
                            newVals[level.Key] += randomPoints;
                    }
                }
            }
            while (amountToShip > 0 && reroll);

            foreach (var newv in newVals)
            {
                ToEditMinionStats.StartingLevels[newv.Key] = newv.Value;
            }
        }


        /// <summary>
        /// no longer in use
        /// </summary>
        /// <returns></returns>
        public List<SkillGroup> GetInterestsWithStats()
        {
            ActiveInterests.Clear();
            foreach (var skillGroup in ToEditMinionStats.skillAptitudes)
            {
                ActiveInterests.Add(skillGroup.Key);
            }

            return ActiveInterests;
        }

        /// <summary>
        /// no longer in use
        /// </summary>
        /// <returns></returns>
        public void GetNextInterest(int index, bool backwards = false)
        {
            //Debug.Log(index+ ", count "+ ActiveInterests.Count());
            if (index >= ActiveInterests.Count())
                return;
            GetNextInterest(ActiveInterests[index].Id, backwards);
        }

        /// <summary>
        /// no longer in use
        /// </summary>
        /// <returns></returns>
        public void GetNextInterest(string id, bool backwards = false)
        {
            var allSkills = Db.Get().SkillGroups.resources;
            SkillGroup OldSkill = allSkills.Find(skill => skill.Id == id);
            SkillGroup newSkill = OldSkill;
            int indexInAllSkills = allSkills.FindIndex(skill => skill == OldSkill);
            var availableSkills = allSkills.Except(ToEditMinionStats.skillAptitudes.Keys);
            var dupeStatPoints = ToEditMinionStats.StartingLevels;

            if (availableSkills.Count() == 0)
                return;
            //index += backwards ? -1 : 1; 

            ///Finding the next skill in Line thats not already on the list
            for (int i = 0; i < allSkills.Count; i++)
            {
                var potentialSkill = backwards
                    ? allSkills[(allSkills.Count - i + indexInAllSkills) % allSkills.Count]
                    : allSkills[(i + indexInAllSkills) % allSkills.Count];
                if (availableSkills.Contains(potentialSkill))
                {
                    newSkill = potentialSkill;
                    break;
                }
            }

            ///removing old boni
            int dupeStatValue = FallBack;
            foreach (var relevantAttribute in OldSkill.relevantAttributes)
            {
                //dupeStatPoints
                string statId = relevantAttribute.Id;
                dupeStatValue = dupeStatPoints[statId] > dupeStatValue
                    ? dupeStatPoints[statId]
                    : dupeStatValue;
                if (DoesRemoveReduceStats(statId))
                {
                    dupeStatPoints[statId] = 0;
                }
                else
                {
                    dupeStatValue = FallBack;
                }
            }
            ToEditMinionStats.skillAptitudes.Remove(OldSkill);


            //Debug.Log("end");
            ///adding new boni
            foreach (var relevantAttribute in newSkill.relevantAttributes)
            {
                //dupeStatPoints
                string statId = relevantAttribute.Id;
                if (IsThisANewSkill(statId))
                {
                    dupeStatPoints[statId] = dupeStatValue;
                }
                else
                {
                    FallBack = dupeStatValue;
                }
            }

            ToEditMinionStats.skillAptitudes.Add(newSkill, 1);
            ActiveInterests[GetCurrentIndex(OldSkill.Id)] = newSkill;
        }

        public int GetCurrentIndex(string id)
        {
            return ActiveInterests.FindIndex(old => old.Id == id);
        }

        /// <summary>
        /// deprecated
        /// </summary>
        /// <param name="startingLevels"></param>
        internal void AddSkillLevels(ref Dictionary<string, int> startingLevels)
        {
            //Debug.Log("AAAAAAAAAAAAAAAA");
            ToEditMinionStats.StartingLevels = startingLevels;
            foreach (var skillGroup in startingLevels)
            {
                //Debug.Log(skillGroup.Key + ", " + skillGroup.Value);
                if (skillGroup.Value > 0)
                {
                    if (!IsThisANewSkill(skillGroup.Key))
                    {
                        FallBack = new KRandom().Next(1, skillGroup.Value + 1);
                    }
                }
            }
        }



        public void AddTrait(string id)
        {
            if (!currentTraitIds.Contains(id))
                currentTraitIds.Add(id);
        }
        public void ReplaceTrait(string old, string newS)
        {
            if (old == newS) return;
            if (!currentTraitIds.Contains(old) || currentTraitIds.Contains(newS))
                return;

            currentTraitIds.Remove(old);
            currentTraitIds.Add(newS);
        }
        public List<string> CurrentTraitsWithout(string thisTrait)
        {
            return currentTraitIds.Where(entry => entry != thisTrait).ToList();
        }

        public string GetNextTraitId(string currentId, bool backwards)
        {
            ModAssets.GetTraitListOfTrait(currentId, out var currentList);
            if (currentList == null)
                return currentId;
            int i = 0;

            i = currentList.FindIndex(t => t.id == currentId);
            if (i != -1)
            {
                do
                {
                    i += (backwards ? -1 : 1);
                    if (i == currentList.Count)
                        i = 0;
                    else if (i < 0)
                        i += currentList.Count;
                }
                while (CurrentTraitsWithout(currentId).Contains(currentList[i].id));
                return currentList[i].id;
            }
            return currentId;
        }


        public bool IsThisANewSkill(string stat)
        {
            if (!USEDSTATS.ContainsKey(stat))
            {
                USEDSTATS[stat] = 1;
                return true;
            }
            else
            {
                USEDSTATS[stat]++;
                return false;
            }
        }
        public bool DoesRemoveReduceStats(string stat)
        {
            if (USEDSTATS.ContainsKey(stat))
            {
                //Debug.Log(stat + ", Count " + USEDSTATS[stat]);
                if (USEDSTATS[stat] > 1)
                {
                    USEDSTATS[stat]--;
                    return false;
                }
                else
                {
                    USEDSTATS.Remove(stat);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        //public int GetBonusName(int index)
        //{
        //    Debug.Log("Index: " + index);
        //    var relevantAttribute = ActiveInterests[index].relevantAttributes.First().Id;
        //    Debug.Log(relevantAttribute);
        //    return dupeStatPoints[relevantAttribute];
        //}

        public int GetBonusValue(SkillGroup group)
        {
            //Debug.Log("Index: " + index);
            var relevantAttribute = group.relevantAttributes.First().Id;
            //Debug.Log(relevantAttribute);
            return ToEditMinionStats.StartingLevels[relevantAttribute];
        }

    }
}
