/*
 * VirtualLoveEditor
 * 三点だいしゅきツールの簡易設定用ツール
 * 
 * Copyright(c) 2024 UniSakiStudio
 * Released under the MIT license
 * https://opensource.org/licenses/mit-license.php
 */

using System.Linq;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using System.IO;
using Microsoft.Win32;

namespace jp.unisakistudio.virtualloveeditor
{
    public class VirtualLoveMenuItems : EditorWindow
    {
        [MenuItem("GameObject/ゆにさきスタジオ/三点でいいよツール/三点でいいよツール(男)追加", false, 22)]
        static public void AddPrefab01() { AddPrefab("三点でいいよ(男)"); }
        [MenuItem("GameObject/ゆにさきスタジオ/三点でいいよツール/三点でいいよツール(女)追加", false, 23)]
        static public void AddPrefab02() { AddPrefab("三点でいいよ(女)"); }

        static public void AddPrefab(string name)
        {
            if (Selection.activeGameObject)
            {
#if !VIRTUAL_LOVE
                EditorUtility.DisplayDialog("必要なツールが不足しています", "このプロジェクトにはVCC版の三点だいしゅきツールがインポートされていません。Boothのショップから三点だいしゅきツールのVCC版をダウンロードし、VCCかALCOMから三点だいしゅきツールをプロジェクトに追加してください", "閉じる");
#endif

#if !NDMF
                EditorUtility.DisplayDialog("必要なツールが不足しています", "このプロジェクトには「Non-Destructive Modular Framework」がインポートされていません。VCCかALCOMからNon-Destructive Modular Frameworkをプロジェクトに追加してください", "閉じる");
#endif

#if !MODULAR_AVATAR
                EditorUtility.DisplayDialog("必要なツールが不足しています", "このプロジェクトには「Modular Avatar」がインポートされていません。VCCかALCOMからModular Avatarをプロジェクトに追加してください", "閉じる");
#endif

                var avatar = Selection.activeGameObject.GetComponent<VRCAvatarDescriptor>();

                var guids = AssetDatabase.FindAssets("t:prefab " + name)
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(path => string.Equals(Path.GetFileNameWithoutExtension(path), name, System.StringComparison.CurrentCulture));
                if (guids.Count() == 0)
                {
                    EditorUtility.DisplayDialog("エラー", "Prefabsが見つかりません。ツールを再度インポートしなおしてください", "閉じる");
                    return;
                }

                var prefabs = AssetDatabase.LoadAssetAtPath<GameObject>(guids.First());

                PrefabUtility.InstantiatePrefab(prefabs, avatar.transform);
            }
       }

        [MenuItem("GameObject/ゆにさきスタジオ/三点でいいよツール/三点でいいよツール(男)追加", true)]
        private static bool ValidateBoy()
        {
            if (!Selection.activeGameObject)
            {
                return false;
            }
            var regKey = Registry.CurrentUser.CreateSubKey(VirtualBedEditor.REGKEY);

            var regValueBed = (string)regKey.GetValue(VirtualBedEditor.APPKEY_VIRTUALBED);
            if (regValueBed != "licensed")
            {
                return false;
            }

            var regValueBoy = (string)regKey.GetValue(VirtualBedEditor.APPKEY_BOY);
            if (regValueBoy != "licensed")
            {
                return false;
            }
            
            var avatar = Selection.activeGameObject.GetComponent<VRCAvatarDescriptor>();
            return avatar != null;
        }

        [MenuItem("GameObject/ゆにさきスタジオ/三点でいいよツール/三点でいいよツール(女)追加", true)]
        private static bool ValidateGirl()
        {
            if (!Selection.activeGameObject)
            {
                return false;
            }
            var regKey = Registry.CurrentUser.CreateSubKey(VirtualBedEditor.REGKEY);

            var regValueBed = (string)regKey.GetValue(VirtualBedEditor.APPKEY_VIRTUALBED);
            if (regValueBed != "licensed")
            {
                return false;
            }

            var regValueGirl = (string)regKey.GetValue(VirtualBedEditor.APPKEY_GIRL);
            if (regValueGirl != "licensed")
            {
                return false;
            }

            var avatar = Selection.activeGameObject.GetComponent<VRCAvatarDescriptor>();
            return avatar != null;
        }
    }
}
