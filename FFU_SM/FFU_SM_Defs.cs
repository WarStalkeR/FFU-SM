#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using BepInEx;
using UnityEngine;
using BepInEx.Configuration;
using FFU_Shattered_Magic;
using MonoMod;
using System.IO;
using TheLastStand.View.Menus;

namespace FFU_Shattered_Magic {
    public class FFU_SM_Defs {
        public static readonly string modName = "Fight for Universe: Shattered Magic";
        public static readonly string modVersion = "0.1";
        public static ConfigFile modDefsCfg;
        public static ConfigEntry<string> debug_ListPerks;
        public static ConfigEntry<string> debug_ListTraits;
        public static void InitCfg() {
            ModLog.Init();
            modDefsCfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "mod_FFU_SM_Defs.cfg"), true);
            debug_ListPerks = modDefsCfg.Bind("Debug", "ListPerks", "False");
            debug_ListTraits = modDefsCfg.Bind("Debug", "ListTraits", "False");
            ModLog.Message("Finished loading configuration!");
        }
    }
}

namespace TheLastStand.View.Menus {
    public class patch_MainMenuView : MainMenuView {
        private extern void orig_Start();
        private void Start() {
            orig_Start();
            FFU_SM_Defs.InitCfg();
        }
    }
}

/*namespace PatchTargetNamespace {
    public class TargetClassName {
        // ----------------------------------------------------------- //
        // Class for example purpose only. Doesn't exist in your code. //
        // Only in targeted namespace of DLL you want to modify.       //
        // If target is in global namespace (-) in ILSpy/DnSpy then,   //
        // patch_Target should be outside all namespaces in the code.  //
        // ----------------------------------------------------------- //
        public int a;
        public string b;
        public TargetClassName(int refA, string refB) {
            a = refA;
            b = refB;
        }
    }
    public class patch_TargetClassName : TargetClassName {
        // ------------------------------------------------------ //
        // Example: a way to access private variable or function. //
        // ------------------------------------------------------ //
        [MonoModIgnore] private bool privateVar;
        [MonoModIgnore] private bool privateFunc() { return true; }
        // ------------------------------------------- //
        // Example: a way to modify existing function. //
        // ------------------------------------------- //
        public extern bool orig_TargetFunctionToModify();
        public bool TargetFunctionToModify() {
            // The code you run before original function.
            var moddedResult = orig_TargetFunctionToModify();
            // The code you run after original function.
            return moddedResult;
        }
        // ---------------------------------------------- //
        // Example: a way to overwrite existing function. //
        // ---------------------------------------------- //
        [MonoModReplace] public bool FunctionToReplace() {
            // Careful, if it has multiple constructors, you might need to give up on MonoModReplace.
            return true;
        }
        // ---------------------------------------------- //
        // Example: a way to modify existing constructor. //
        // ---------------------------------------------- //
        [MonoModIgnore] public patch_TargetClassName(int refC, string refD) : base(refC, refD) { }
        // You can remove [MonoModIgnore] row, if compiler doesn't throw error without it.
        [MonoModOriginal] public extern void orig_TargetConstructor(int refA, string refB);
        [MonoModConstructor] public void TargetConstructor(int refA, string refB) {
            orig_TargetConstructor(refA, refB);
            // Here you can initialize variables you've added.
            // You also can do it before original constructor initialized.
            // Don't forget to use it in exactly same namespace as original.
        }
        // ------------------------------------------------ //
        // Example: a way to overwrite existing enumerator. //
        // ------------------------------------------------ //
        [MonoModEnumReplace] public enum patch_TargetEnum {
            // Don't forget to add original entires used by the code.
            // Or you might end up with whole bag of errors that make no sense.
            origEnumA,
            origEnumB,
            origEnumC,
            modEnumD
        }
        // ------------------------------------------------ //
        // Example: add new entries to existing enumerator. //
        // ------------------------------------------------ //
        public enum patch_TargetEnum {
            // Ensure that new entries use numbers that way above original.
            // In case, if original enumerator will get more entries.
            modEnumX = 1337,
            modEnumY = 1338,
            modEnumZ = 1339
        }
    }
}*/