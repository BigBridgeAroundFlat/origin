using System;
using UnityEngine;

namespace Utility
{
    /*
     * エディタ拡張
     * ボタン押した時のコールバックにenum設定可能
     */
    [AttributeUsage(AttributeTargets.Method)]
    public class EnumActionAttribute : PropertyAttribute
    {
        public Type EnumType;

        public EnumActionAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}
