using System.Collections.Generic;
using System.Text;
using UnityEngine.Experimental.LowLevel;

namespace Wokarol
{
    public static class PlayerLoopUtils
    {
        /// <summary>
        /// Populates StringBuilder with all systems in a PlayerLoopSystem
        /// </summary>
        /// <param name="system">System to search through</param>
        /// <param name="sb">StringBuilder to populate</param>
        /// <param name="depth">starting indention depth, defaulted to 0</param>
        public static void RecursivePlayerLoopPrint(PlayerLoopSystem system, StringBuilder sb, int depth = 0) {
            if (depth == 0) {
                sb.AppendLine("ROOT NODE");
            } else if (system.type != null) {
                for (int i = 0; i < depth; i++) {
                    sb.Append("\t");
                }
                sb.AppendLine(system.type.Name);
            }
            if (system.subSystemList != null) {
                depth++;
                foreach (var s in system.subSystemList) {
                    RecursivePlayerLoopPrint(s, sb, depth);
                }
                depth--;
            }
        }

        /// <summary>
        /// Add system in front of subSystem list of T
        /// </summary>
        /// <typeparam name="T">type of system to search for</typeparam>
        /// <param name="system">system to search through</param>
        /// <param name="addition">system ti insert</param>
        /// <returns>returns false if system T was not found</returns>
        public static bool AddToLoop<T>(ref PlayerLoopSystem system, PlayerLoopSystem addition) {
            if (system.type == typeof(T)) {
                List<PlayerLoopSystem> systems = new List<PlayerLoopSystem>(system.subSystemList);
                systems.Insert(0, addition);
                system.subSystemList = systems.ToArray();
                return true;
            }
            if (system.subSystemList != null) {
                for (int i = 0; i < system.subSystemList.Length; i++) {
                    if (AddToLoop<T>(ref system.subSystemList[i], addition)) {
                        return true;
                    }
                }
            }
            return false;
        }
    } 
}
