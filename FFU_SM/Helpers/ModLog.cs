using System;

namespace FFU_Shattered_Magic {
    public class ModLog {
        public static void Init() {
            UnityEngine.Debug.LogWarning($"{FFU_SM_Defs.modName} v{FFU_SM_Defs.modVersion}, {DateTime.Now}");
        }
        internal static void Info(string logEntry = "") {
            UnityEngine.Debug.Log($"{logEntry}");
        }
        internal static void Debug(string logEntry = "") {
            UnityEngine.Debug.Log($"{logEntry}");
        }
        internal static void Message(string logEntry = "") {
            UnityEngine.Debug.LogWarning($"{logEntry}");
        }
        internal static void Warning(string logEntry = "") {
            UnityEngine.Debug.LogWarning($"{logEntry}");
        }
        internal static void Error(string logEntry = "") {
            UnityEngine.Debug.LogError($"{logEntry}");
        }
        internal static void Fatal(string logEntry = "") {
            UnityEngine.Debug.LogError($"{logEntry}");
        }
    }
}
