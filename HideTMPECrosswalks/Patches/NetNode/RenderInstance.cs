using Harmony;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace HideTMPECrosswalks.Patches.NetNode {
    using Utils;

    [HarmonyPatch()]
    public static class RenderInstance {
        static void Log(string m) => Extensions.Log("NetNode_RenderInstance Transpiler: " + m);
        static MethodBase TargetMethod() {
            // RenderInstance(RenderManager.CameraInfo cameraInfo, ushort nodeID, NetInfo info, int iter, Flags flags, ref uint instanceIndex, ref RenderManager.Instance data)
            var ret = typeof(global::NetNode).GetMethod("RenderInstance", BindingFlags.NonPublic | BindingFlags.Instance);
            Extensions.Assert(ret!=null, "did not manage to find original function to patch");
            Log("aquired method " + ret);
            return ret;
        }

        //static bool Prefix(ushort nodeID){}
        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions) {
            try {
                var codes = TranspilerUtils.ToCodeList(instructions);
                CheckFlagsCommon.PatchCheckFlags(codes, 2, 13); // patch second draw mesh.
                Log("successfully patched NetNode.RenderInstance");
                return codes;
            }catch(Exception e) {
                Log(e + "\n" + Environment.StackTrace);
                throw e;
            }
        }
    } // end class
} // end name space