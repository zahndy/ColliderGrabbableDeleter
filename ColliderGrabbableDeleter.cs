using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using ResoniteModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColliderGrabbableDeleter
{
    public partial class ColliderGrabbableDeleter : ResoniteMod
    {
        public override string Name => "ColliderGrabbableDeleter";
        public override String Author => "zahndy";
        public override String Link => "https://github.com/zahndy/ColliderGrabbableDeleter";
        public override String Version => "1.0.0";

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> Enabled = new ModConfigurationKey<bool>("Enabled", "Enable this mod", () => true);

        internal static ModConfiguration Config;

        public override void OnEngineInit()
        {
            Config = GetConfiguration();
            Config.Save(true);

            Harmony harmony = new Harmony("com.zahndy.ColliderGrabbableDeleter");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(ObjectRoot), "BuildInspectorUI")]
        private class ColliderGrabbableDeleterPatch
        {
            public static void Postfix(ObjectRoot __instance, UIBuilder ui)
            {
                if (Config.GetValue(Enabled))
                {
                    ui.Style.MinHeight = 36;
                    ui.Text("Delete MeshColliders And Grabbables");
                    ui.Style.MinHeight = 24;

                    var btn = ui.Button("Delete MeshColliders And Grabbables of Child Slots");

                    btn.LocalPressed += (_btn, data) => {
                        __instance.RunSynchronously(delegate {
                            foreach (Slot _child in __instance.Slot.Children)
                            {
                                foreach (Component _Comp in _child.Components) 
                                {
                                    if (_Comp.Name == "MeshCollider" || _Comp.Name == "Grabbable")
                                    {
                                        _child.RunSynchronously(delegate {
                                            _Comp.Destroy();
                                        });
                                    }
                                }
                            }
                        });

                    };
                }
            }
        }
    }
}
