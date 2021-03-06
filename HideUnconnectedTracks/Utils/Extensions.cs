using ColossalFramework;
using ICities;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Linq;
using GenericGameBridge.Service;

namespace HideUnconnectedTracks.Utils {

    public static class Extensions {
        public static void CopyProperties(object target, object origin) {
            FieldInfo[] fields = origin.GetType().GetFields();
            foreach (FieldInfo fieldInfo in fields) {
                //Extensions.Log($"Copying field:<{fieldInfo.Name}> ...>");
                object value = fieldInfo.GetValue(origin);
                string strValue = value?.ToString() ?? "null";
                //Extensions.Log($"Got field value:<{strValue}> ...>");
                fieldInfo.SetValue(target, value);
                //Extensions.Log($"Copied field:<{fieldInfo.Name}> value:<{strValue}>");
            }
        }

        public static void CopyProperties<T>(object target, object origin) {
            Assert(target is T, "target is T");
            Assert(origin is T, "origin is T");
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo fieldInfo in fields) {
                //Extensions.Log($"Copying field:<{fieldInfo.Name}> ...>");
                object value = fieldInfo.GetValue(origin);
                //string strValue = value?.ToString() ?? "null";
                //Extensions.Log($"Got field value:<{strValue}> ...>");
                fieldInfo.SetValue(target, value);
                //Extensions.Log($"Copied field:<{fieldInfo.Name}> value:<{strValue}>");
            }
        }

        public static T Max<T>()
            where T : Enum =>
           System.Enum.GetValues(typeof(T)).Cast<T>().Max();

        public static void SetBit(this ref byte b, int idx) => b |= (byte)(1 << idx);
        public static void ClearBit(this ref byte b, int idx) => b &= ((byte)~(1 << idx));
        public static bool GetBit(this byte b, int idx) => (b & (byte)(1 << idx)) != 0;
        public static void SetBit(this ref byte b, int idx, bool value) {
            if (value)
                b.SetBit(idx);
            else
                b.ClearBit(idx);
        }

        internal static ref NetNode ToNode(this ushort id) => ref Singleton<NetManager>.instance.m_nodes.m_buffer[id];
        internal static ref NetSegment ToSegment(this ushort id) => ref Singleton<NetManager>.instance.m_segments.m_buffer[id];
        internal static NetLane ToLane(this uint id) => Singleton<NetManager>.instance.m_lanes.m_buffer[id];

        internal static AppMode currentMode => SimulationManager.instance.m_ManagersWrapper.loading.currentMode;
        internal static bool CheckGameMode(AppMode mode) => CheckGameMode(new[] { mode });
        internal static bool CheckGameMode(AppMode[] modes) {
            try {
                foreach (var mode in modes) {
                    if (currentMode == mode)
                        return true;
                }
            } catch { }
            return false;
        }
        internal static bool InGame => CheckGameMode(AppMode.Game);
        internal static bool InAssetEditor => CheckGameMode(AppMode.AssetEditor);


        internal static void LogLap(this Stopwatch ticks, string prefix = "") {
            ticks.Stop();
            Log(prefix + " TICKS elapsed: " + ticks.ElapsedTicks.ToString("E2"));
            ticks.Reset();
            ticks.Start();
        }
        internal static void Lap(this Stopwatch ticks) {
            ticks.Reset();
            ticks.Start();
        }

        static object LogLock = new object();
        internal static void Log(string m, bool unitylog = false) {
            lock (LogLock) {
                //var st = System.Environment.StackTrace;
                //m  = st + " : \n" + m;
#if DEBUG
                UnityEngine.Debug.Log(m);
#else
                if(unitylog)UnityEngine.Debug.Log(m);
#endif


                System.IO.File.AppendAllText("mod.debug.log", m + "\n\n");
            }
        }
        internal static void Assert(bool con, string m="") {
            if (!con) throw new System.Exception("Assertion failed: " + m);
        }

        internal static string BIG(string m) {
            string mul(string s, int i) {
                string ret_ = "";
                while (i-- > 0) ret_ += s;
                return ret_;
            }
            m = "  " + m + "  ";
            int n = 120;
            string stars1 = mul("*", n);
            string stars2 = mul("*", (n - m.Length) / 2);
            string ret = stars1 + "\n" + stars2 + m + stars2 + "\n" + stars1;
            return ret;
        }

        internal static string GetPrettyFunctionName(MethodInfo m) {
            string s = m.Name;
            string[] ss = s.Split(new[] { "g__", "|" }, System.StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length == 3)
                return ss[1];
            return s;
        }

        public static INetService netService => TrafficManager.Constants.ServiceFactory.NetService;

    }
}
