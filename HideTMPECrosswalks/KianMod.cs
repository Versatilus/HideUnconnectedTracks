using Harmony;
using ICities;
using JetBrains.Annotations;
using UnityEngine;
using HideTMPECrosswalks.Utils;
using HideTMPECrosswalks.Patches;
using HideTMPECrosswalks.Settings;
using System.Collections.Generic;

namespace HideTMPECrosswalks {
    public class KianModInfo : IUserMod {
        public string Name => "RM TMPE Crossings V2.4";
        public string Description => "Automatically hide crosswalk textures on segment ends when TMPE bans crosswalks";

        [UsedImplicitly]
        public void OnEnabled() {
            System.IO.File.WriteAllText("mod.debug.log", ""); // restart log.
            InstallHarmony();

            LoadingWrapperPatch.OnPostLevelLoaded += PrefabUtils.CreateNoZebraTextures;
#if DEBUG
            LoadingWrapperPatch.OnPostLevelLoaded += TestOnLoad.Test;
#endif
            try {
                AppMode mode = Extensions.currentMode;
                if (mode == AppMode.Game || mode == AppMode.AssetEditor) {
                    //PrefabUtils.CreateNoZebraTextures();
                }
            }
            catch { }
        }

        [UsedImplicitly]
        public void OnDisabled() {
            UninstallHarmony();

#if DEBUG
            LoadingWrapperPatch.OnPostLevelLoaded -= TestOnLoad.Test;
#endif
            LoadingWrapperPatch.OnPostLevelLoaded -= PrefabUtils.CreateNoZebraTextures;
            Options.instance = null;
        }


        //[UsedImplicitly]
        //public void OnSettingsUI(UIHelperBase helperBasae) {
        //    new Options(helperBasae);
        //}

        #region Harmony
        HarmonyInstance harmony = null;
        const string HarmonyId = "CS.kian.HideTMPECrosswalks";
        void InstallHarmony() {
            if (harmony == null) {
                Extensions.Log("HideTMPECrosswalks Patching...",true);
#if DEBUG
                HarmonyInstance.DEBUG = true;
#endif
                HarmonyInstance.SELF_PATCHING = false;
                harmony = HarmonyInstance.Create(HarmonyId);
                harmony.PatchAll(GetType().Assembly);
            }
        }

        void UninstallHarmony() {
            if (harmony != null) {
                harmony.UnpatchAll(HarmonyId);
                harmony = null;
                Extensions.Log("HideTMPECrosswalks patches Reverted.",true);
            }
        }
        #endregion
    }

#if DEBUG

#endif

}
