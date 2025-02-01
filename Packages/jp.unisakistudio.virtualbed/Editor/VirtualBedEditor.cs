using UnityEngine;
using UnityEditor;
using Microsoft.Win32;
using System.Linq;
using jp.unisakistudio.virtualbed;
using System.Collections.Generic;
using UnityEditor.Animations;

namespace jp.unisakistudio.virtualloveeditor
{

    [CustomEditor(typeof(VirtualBed))]
    public class VirtualBedEditor : Editor
    {
        internal const string REGKEY = @"SOFTWARE\UnisakiStudio";
        internal const string APPKEY_VIRTUALBED = "virtualbed";
        internal const string APPKEY_BOY = "virtuallove_boy";
        internal const string APPKEY_GIRL = "virtuallove_girl";

        List<string> existProducts;
        List<string> existFolders;

        public override void OnInspectorGUI()
        {
            VirtualBed virtualBed = target as VirtualBed;

            EditorGUILayout.LabelField("三点でいいよツール", new GUIStyle() { fontStyle = FontStyle.Bold, fontSize = 20, }, GUILayout.Height(30));

#if UNISAKISTUDIO_DEBUG
            base.OnInspectorGUI();
#endif

#if !VIRTUAL_LOVE
            EditorGUILayout.HelpBox("このプロジェクトにはVCC版の三点だいしゅきツールがインポートされていません。Boothのショップから三点だいしゅきツールのVCC版をダウンロードし、VCCかALCOMから三点だいしゅきツールをプロジェクトに追加してください", MessageType.Error);
#endif

#if !NDMF
            EditorGUILayout.HelpBox("このプロジェクトには「Non-Destructive Modular Framework」がインポートされていません。VCCかALCOMからNon-Destructive Modular Frameworkをプロジェクトに追加してください", MessageType.Error);
#endif

#if !MODULAR_AVATAR
            EditorGUILayout.HelpBox("このプロジェクトには「Modular Avatar」がインポートされていません。VCCかALCOMからModular Avatarをプロジェクトに追加してください", MessageType.Error);
#endif

            /*
             * このコメント分を含むここから先の処理は三点でいいｙツールをゆにさきスタジオから購入した場合に変更することを許可します。
             * つまり購入者はライセンスにまつわるこの先のソースコードを削除して再配布を行うことができます。
             * 逆に、購入をせずにGithubなどからソースコードを取得しただけの場合、このライセンスに関するソースコードに手を加えることは許可しません。
             */
            if (virtualBed.isVirtualBedLicensed)
            {
                if (virtualBed.isBoy && virtualBed.isVirtualLoveBoyLicensed)
                {
                    RenderInspectorGUI();
                    return;
                }
                if (!virtualBed.isBoy && virtualBed.isVirtualLoveGirlLicensed)
                {
                    RenderInspectorGUI();
                    return;
                }
            }

            var regKey = Registry.CurrentUser.CreateSubKey(REGKEY);

            var regValueBed = (string)regKey.GetValue(APPKEY_VIRTUALBED);
            if (regValueBed == "licensed")
            {
                virtualBed.isVirtualBedLicensed = true;
            }

            var regValueBoy = (string)regKey.GetValue(APPKEY_BOY);
            if (regValueBoy == "licensed")
            {
                virtualBed.isVirtualLoveBoyLicensed = true;
            }

            var regValueGirl = (string)regKey.GetValue(APPKEY_GIRL);
            if (regValueGirl == "licensed")
            {
                virtualBed.isVirtualLoveGirlLicensed = true;
            }

            if (!virtualBed.isVirtualBedLicensed)
            {
                EditorGUILayout.HelpBox("このコンピュータには三点でいいよツールの使用が許諾されていません。Boothのショップから三点でいいよツールを購入して、コンピュータにライセンスをインストールしてください", MessageType.Error);
                if (EditorGUILayout.LinkButton("三点でいいよツール(Booth)"))
                {
                    Application.OpenURL("https://yunisaki.booth.pm/items/3722687");
                }
                return;
            }
            else
            {
                if (virtualBed.isBoy && !virtualBed.isVirtualLoveBoyLicensed)
                {
                    EditorGUILayout.HelpBox("このコンピュータには三点だいしゅきツール(男)の使用が許諾されていません。Boothのショップから三点だいしゅきツール(男)を購入して、コンピュータにライセンスをインストールしてください", MessageType.Error);
                    if (EditorGUILayout.LinkButton("三点だいしゅきツール(Booth)"))
                    {
                        Application.OpenURL("https://yunisaki.booth.pm/items/3641334");
                    }
                    return;
                }
                if (!virtualBed.isBoy && !virtualBed.isVirtualLoveGirlLicensed)
                {
                    EditorGUILayout.HelpBox("このコンピュータには三点だいしゅきツール(女)の使用が許諾されていません。Boothのショップから三点だいしゅきツール(女)を購入して、コンピュータにライセンスをインストールしてください", MessageType.Error);
                    if (EditorGUILayout.LinkButton("三点だいしゅきツール(Booth)"))
                    {
                        Application.OpenURL("https://yunisaki.booth.pm/items/3641334");
                    }
                    return;
                }
            }
            /*
             * ライセンス処理ここまで
             */

            RenderInspectorGUI();
        }

        private void RenderInspectorGUI()
        {
            VirtualBed virtualBed = target as VirtualBed;

            EditorGUILayout.HelpBox("このコンピュータには三点でいいよツールの使用が許諾されています", MessageType.None);

            Transform avatar = virtualBed.transform;
            VRC.SDK3.Avatars.Components.VRCAvatarDescriptor avatarDescriptor = null;
            while (avatar != null)
            {
                if (avatar.TryGetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(out avatarDescriptor))
                {
                    break;
                }
                avatar = avatar.parent;
            }
            if (avatarDescriptor == null)
            {
                EditorGUILayout.HelpBox("オブジェクトがVRC用のアバターオブジェクトの中に入っていません。このオブジェクトはVRCAvatarDescriptorコンポーネントの付いたオブジェクトの中に配置してください", MessageType.Error);
                return;
            }
            if (existProducts == null)
            {
                if (avatar != null)
                {
                    existProducts = CheckExistProduct(avatarDescriptor);
                }
            }
            if (existProducts != null)
            {
                foreach (var existProduct in existProducts)
                {
                    EditorGUILayout.HelpBox(existProduct + "の設定がアバターに残っています！不具合が発生する可能性があるので、自動で設定を取り除く機能を使用してください。また、アバター購入時にBaseLayerに設定されていたLocomotion用のAnimatorControllerがある場合は、恐れ入りますが手動で復仇してお使いください。", MessageType.Error);
                    if (GUILayout.Button(existProduct + "の設定を取り除く"))
                    {
                        RemoveExistProduct(avatarDescriptor, existProduct);
                        existProducts = null;
                    }
                }
            }

            // unitypackage版のAssets用フォルダがあったら警告を出す
            if (existFolders == null)
            {
                existFolders = CheckExistFolder();
            }
            if (existFolders != null)
            {
                foreach (var existFolder in existFolders)
                {
                    EditorGUILayout.HelpBox("unitypackage版の「" + existFolder + "」フォルダがプロジェクトに残っています！不具合が発生する可能性があるので、自動で設定を取り除く機能を使用してください。", MessageType.Error);
                    if (GUILayout.Button(existFolder + "のフォルダを取り除く"))
                    {
                        RemoveExistFolder(existFolder);
                        existFolders = null;
                    }
                }
            }
        }

        List<(string name,
            List<string> animatorControllerNames,
            List<string> layerNames,
            List<string> checkExpressionParametersNames,
            List<string> expressionParametersNames,
            List<string> expressionsMenuNames,
            List<string> prefabsNames)> productDefines = new ()
        {
            (
                "三点でいいよツール",
                new List<string>
                {
                },
                new()
                {
                    "VirtualBed_BedTypeBoy",
                    "VirtualBed_BedTypeGirl",
                    "VirtualBed_BedHeight",
                    "VirtualBed_LocomotionLock",
                },
                 new List<string>
                {
                    "USS_VB_BedType",
                    "BedHeight",
                },
                 new List<string>
                {
                    "USS_VB_BedType",
                    "BedHeight",
                    "LocomotionLock",
                },
                new List<string>
                {
                    "VirtualBed_Menu",
                },
                new List<string>
                {
                    "VirtualBed",
                }
            ),
        };

        List<string> CheckExistProduct(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor avatar)
        {
            var existProducts = new List<string>();

            foreach (var productDefine in productDefines)
            {
                bool isExistProduct = false;
                // AnimatorControllerが商品のか調べる
                var animatorController = avatar.baseAnimationLayers.First(layer => layer.type == VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.Base);
                if (animatorController.animatorController != null)
                {
                    foreach (var animatoControllerName in productDefine.animatorControllerNames)
                    {
                        if (animatorController.animatorController.name.Contains(animatoControllerName))
                        {
                            isExistProduct = true;
                        }
                    }
                }

                var fxAnimationLayer = avatar.baseAnimationLayers.First(layer => layer.type == VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX);
                var fxAnimatorController = fxAnimationLayer.animatorController as AnimatorController;
                if (fxAnimatorController != null)
                {
                    foreach (var layer in fxAnimatorController.layers)
                    {
                        if (productDefine.layerNames.IndexOf(layer.name) != -1)
                        {
                            isExistProduct = true;
                        }
                    }
                }

                var productMenuGuids = new List<string>();
                foreach (var menuName in productDefine.expressionsMenuNames)
                {
                    productMenuGuids.AddRange(AssetDatabase.FindAssets(menuName));
                }
                // 再起で商品のメニューを使用しているか調べる
                bool isKawaiiSittingMenu(VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu menu)
                {
                    if (menu == null)
                    {
                        return false;
                    }
                    foreach (var menuName in productDefine.expressionsMenuNames)
                    {
                        if (productMenuGuids.IndexOf(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(menu))) != -1)
                        {
                            return true;
                        }

                    }
                    foreach (var control in menu.controls)
                    {
                        if (control.type != VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu)
                        {
                            continue;
                        }
                        if (isKawaiiSittingMenu(control.subMenu))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                if (isKawaiiSittingMenu(avatar.expressionsMenu))
                {
                    isExistProduct = true;
                }

                // パラメータに商品のがあるか調べる
                if (avatar.expressionParameters)
                {
                    foreach (var parameter in avatar.expressionParameters.parameters)
                    {
                        if (productDefine.checkExpressionParametersNames.IndexOf(parameter.name) != -1)
                        {
                            isExistProduct = true;
                        }
                    }
                }
                foreach (Transform child in avatar.transform)
                {
                    if (productDefine.prefabsNames.IndexOf(child.name) != -1)
                    {
                        isExistProduct = true;
                    }
                }

                if (isExistProduct)
                {
                    existProducts.Add(productDefine.name);
                }
            }

            return existProducts;
        }

        void RemoveExistProduct(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor avatar, string productName)
        {
            var productDefine = productDefines.FirstOrDefault(define => define.name == productName);
            if (productDefine == default)
            {
                return;
            }

            // AnimatorControllerが商品のか調べる
            var animatorController = avatar.baseAnimationLayers.First(layer => layer.type == VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.Base);
            if (animatorController.animatorController != null)
            {
                foreach (var animatoControllerName in productDefine.animatorControllerNames)
                {
                    if (AssetDatabase.FindAssets(animatoControllerName).ToList().IndexOf(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(animatorController.animatorController))) != -1)
                    {
                        Undo.RecordObject(avatar, "RemoveExistProduct");
                        avatar.baseAnimationLayers[(int)VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.Base].animatorController = null;
                        avatar.baseAnimationLayers[(int)VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.Base].isDefault = true;
                        avatar.baseAnimationLayers[(int)VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.Base].isEnabled = true;
                        EditorUtility.SetDirty(avatar);
                        break;
                    }
                }
            }

            var fxAnimationLayer = avatar.baseAnimationLayers.First(layer => layer.type == VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX);
            var fxAnimatorController = fxAnimationLayer.animatorController as AnimatorController;
            if (fxAnimatorController != null)
            {
                fxAnimatorController.layers = fxAnimatorController.layers.Where((layer) =>
                {
                    return productDefine.layerNames.IndexOf(layer.name) == -1;
                }).ToArray();
            }
            // 再起で商品のメニューを使用しているか調べる
            void removeKawaiiSittingMenu(VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu menu)
            {
                if (menu == null)
                {
                    return;
                }
                for (var i = menu.controls.Count - 1; i >= 0; i--)
                {
                    var control = menu.controls[i];
                    if (control.type != VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu)
                    {
                        continue;
                    }
                    bool isKawaiiSittingMenu = false;
                    foreach (var menuName in productDefine.expressionsMenuNames)
                    {
                        if (AssetDatabase.FindAssets(menuName).ToList().IndexOf(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(control.subMenu))) != -1)
                        {
                            Undo.RecordObject(menu, "RemoveExistProduct");
                            menu.controls.RemoveAt(i);
                            isKawaiiSittingMenu = true;
                            EditorUtility.SetDirty(menu);
                            break;
                        }
                    }
                    if (!isKawaiiSittingMenu)
                    {
                        removeKawaiiSittingMenu(control.subMenu);
                    }
                }
            }
            removeKawaiiSittingMenu(avatar.expressionsMenu);

            // パラメータに商品のがあるか調べる
            Undo.RecordObject(avatar.expressionParameters, "RemoveExistProduct");
            avatar.expressionParameters.parameters = avatar.expressionParameters.parameters.Where((parameter) =>
            {
                return productDefine.expressionParametersNames.IndexOf(parameter.name) == -1;
            }).ToArray();
            EditorUtility.SetDirty(avatar.expressionParameters);

            // Prefabsを削除
            List<GameObject> deleteObjects = new();
            foreach (Transform child in avatar.transform)
            {
                if (productDefine.prefabsNames.IndexOf(child.name) != -1)
                {
                    deleteObjects.Add(child.gameObject);
                }
            }
            while (deleteObjects.Count > 0)
            {
                Undo.DestroyObjectImmediate(deleteObjects[0]);
                deleteObjects.RemoveAt(0);
            }


            AssetDatabase.SaveAssets();
        }

        private readonly List<string> folderDefines = new()
        {
            "Assets/UnisakiStudio/VirtualBed",
        };

        protected List<string> CheckExistFolder()
        {
            List<string> existFolders = new();
            foreach (var folderDefine in folderDefines)
            {
                if (AssetDatabase.IsValidFolder(folderDefine))
                {
                    existFolders.Add(folderDefine);
                }
            }
            return existFolders;
        }
        void RemoveExistFolder(string folder)
        {
            AssetDatabase.DeleteAsset(folder);
        }

    }
}
