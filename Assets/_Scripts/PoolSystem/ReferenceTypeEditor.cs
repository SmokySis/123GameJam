using Sirenix.OdinInspector;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
namespace PoolSystem
{
    [CreateAssetMenu(menuName = "PoolSystem/ReferenceTypesEditor", fileName = "ReferenceTypeEditor")]
    public class ReferenceTypeEditor : ScriptableObject
    {
        [LabelText("池化数据")]
        public List<NameAndNameSpacePair> Data;
        [LabelText("数据地址")]
        public string NewFilePath;
        [Button("创建ReferenceTypes")]
        public void Generate()
        {
#if UNITY_EDITOR
            string targetName = "ReferenceTypes";
            string[] guids = AssetDatabase.FindAssets($"name:{targetName}", new[] { NewFilePath });
            var exactPaths = guids.Select(AssetDatabase.GUIDToAssetPath).Where(path => Path.GetFileNameWithoutExtension(path) == targetName).ToArray();
            foreach (var path in exactPaths)
                AssetDatabase.DeleteAsset(path);
            string code = GetNameSpace() + "namespace PoolSystem\n{\npublic static class " + $"{targetName}\n" + "{\n" + GetConstAndMap() + "        /// <summary>\r\n        /// 获取类型对应的反射\r\n        /// </summary>\r\n        /// <param name=\"referenceType\">\r\n        /// 类型\r\n        /// </param>\r\n        /// <param name=\"reference\">\r\n        /// 对应的反射类型\r\n        /// </param>\r\n        /// <returns>\r\n        /// 是否成功获取\r\n        /// </returns>\r\n        public static bool GetReference(in uint referenceType, out Type reference)\r\n        {\r\n            if (referenceType > REFERENCE_TYPE_COUNT)\r\n            {\r\n                Debug.LogWarning($\"ReferenceType GetReference Warning:{referenceType} Is Out Of Range\");\r\n                reference = null;\r\n                return false;\r\n            }\r\n            reference = types[referenceType];\r\n            return true;\r\n        }\r\n        /// <summary>\r\n        /// 获取类型在列表中的位置\r\n        /// </summary>\r\n        /// <typeparam name=\"TReference\"></typeparam>\r\n        /// <returns></returns>\r\n        public static int GetReferenceTypeIndex<TReference>() where TReference : IReference<TReference>, new()\r\n        {\r\n            return GetReferenceTypeIndex(typeof(TReference));\r\n        }\r\n        /// <summary>\r\n        /// 获取类型在列表中的位置\r\n        /// </summary>\r\n        /// <returns></returns>\r\n        public static int GetReferenceTypeIndex(Type referenceType)\r\n        {\r\n            for (int i = 0; i < REFERENCE_TYPE_COUNT; i++)\r\n            {\r\n                if (types[i] == referenceType)\r\n                    return i;\r\n            }\r\n            return -1;\r\n        }\r\n    }\r\n}";
            string fullPath = Path.Combine(NewFilePath, targetName + ".cs");
            File.WriteAllText(fullPath, code);
            AssetDatabase.Refresh();
#endif
        }
        private string GetNameSpace()
        {
            string space = "using UnityEngine;\nusing System;\n";
#if BUFF_SYSTEM
            space += "using BuffSystem;\n";
#endif
            HashSet<string> set = new();
            foreach (var v in Data)
                if (!string.IsNullOrEmpty(v.NameSpace))
                    set.Add(v.NameSpace);
            foreach (string v in set)
                space += $"using {v};\n";
            return space;
        }
        private string GetConstAndMap()
        {
            int count = 1;
            string constDefine = "public const uint GAMEOBJECTPOOL = 0;\n";
            string typesDefine = "public static readonly Type[] types = new Type[REFERENCE_TYPE_COUNT]\r\n{typeof(GameObjectPool)";
#if BUFF_SYSTEM
            count += 2;
            constDefine += "public const uint BUFFRUNTIMEDATA = 1;\npublic const uint PARALLELBUFFRUNTIMEDATA = 2;\n";
            typesDefine += ",typeof(BuffRuntimeData),typeof(ParallelBuffRunTimeData)";
#endif
            foreach (var v in Data)
            {
                if (!string.IsNullOrEmpty(v.Name))
                {
                    constDefine += $"public const uint {v.Name.ToUpper()} = {count++};\n";
                    typesDefine += string.IsNullOrEmpty(v.Parent) ? string.IsNullOrEmpty(v.NameSpace) ? $",typeof({v.Name})" : $",typeof({v.NameSpace}.{v.Name})" : $",typeof({v.Parent}.{v.Name})";
                }
            }
            typesDefine += "};\n";
            return $"public const int REFERENCE_TYPE_COUNT = {count};\n" + constDefine + typesDefine;
        }
    }
    [Serializable]
    public struct NameAndNameSpacePair
    {
        [LabelText("类名")]
        public string Name;
        [LabelText("所在命名空间")]
        public string NameSpace;
        [LabelText("所在类"), Tooltip("当该类在类中声明时填写")]
        public string Parent;
    }
}
