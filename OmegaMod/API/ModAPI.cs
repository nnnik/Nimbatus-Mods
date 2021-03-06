﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Nimbatus.GUI.MainMenu.Scripts;
using Assets.Nimbatus.Scripts.Persistence;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.SensorParts;
using MonoMod;
using MonoMod.ModInterop;
using OmegaMods;
using On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts;
using Partiality.Modloader;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NGenerics.Extensions;
using Partiality;

#pragma warning disable CS0626
namespace API
{
    //public class OmegaModLoader
    //{
    //    //FourLogic ForLogic = new FourLogic();
    //    public NimbatusMod[] modList;
    //    public string ModInfo;
    //    public string modInfo;
    //    string modInfo2;
    //    public void Startup()
    //    {
    //        InitializeMod(modList);
    //    }

    //    public void InitializeMod(params NimbatusMod[] Modds)
    //    {
    //        for (int i = 0; i < Modds.Length; i++)
    //        {
    //            modInfo2 = modInfo;
    //            modList[i].Load(this);
    //            modInfo = modInfo2 + modList[i].ToString() + " ";
    //        }
    //        ModInfo = " Modded Loaded Mods: " + modInfo;
    //    }

    //}
    public class ModAPI : PartialityMod
    {
       public override void Init()
       {
           base.Init();
            ModID = "API";
            author = "OmegaRogue";
            Version = new Version(1, 0, 0, 0).ToString();
           loadPriority = 0;
       }

    }
    [MonoModPatch("global::Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.BindableDronePart")]
    abstract class patch_BindableDronePart : BindableDronePart
    {
        [MonoMod.MonoModIgnore]
        internal List<KeyBinding> KeyBindings;
        public virtual void AddKeyBindings(params KeyBinding[] keys)
        {
            this.KeyBindings.AddRange(keys);
        }
        public virtual void RemoveKeyBindings(params KeyBinding[] keys)
        {
            foreach (KeyBinding key in keys)
            {
                this.KeyBindings.Remove(key);
            }
        }
    }
    [MonoModPatch("global::Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.SensorParts.SensorPart")]
    abstract class patch_SensorPart : SensorPart
    {
        [MonoMod.MonoModIgnore]
        internal List<KeyBinding> EventBindings;
        public virtual void AddEventBindings(params KeyBinding[] keys)
        {
            this.EventBindings.AddRange(keys);
        }
        public virtual void RemoveEventBindings(params KeyBinding[] keys)
        {
            foreach (KeyBinding key in keys)
            {
                this.EventBindings.Remove(key);
            }
        }

    }
    public class LoadFromFile : MonoBehaviour
    {
        public void Start()
        {
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "myassetBundle"));
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }

            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("MyObject");
            Instantiate(prefab);

            myLoadedAssetBundle.Unload(false);
        }
        
    }

    public class ConfigManager
    {
        private string _folder;
        public string Folder => Path.Combine(FolderStructure.ConfigFolder, _folder);
        public string FileExtension => ".json";

        public ConfigManager(string folder)
        {
            _folder = folder;
        }

        public JObject LoadModConfig(PartialityMod mod)
        {
            if(mod == null) throw new ArgumentNullException(nameof(mod));
            string path = GetModConfigFilePath(mod);
            string json = File.ReadAllText(path);
            return JObject.Parse(json);
            
        }

        private string GetModConfigFilePath(PartialityMod mod)
        {
            string configName = mod.ModID;
            if(configName == null) throw new ArgumentException("NimbatusMod.Name is null", nameof(mod));
            configName = configName.ToLower();
            string path = Path.Combine(Folder, configName) + FileExtension;
            return path;
        }

    }
    public static class AssetBundleImport
    {
        public static readonly string AssetFilename = "mod-nimbatus";
        
    }
    public static class FolderStructure
    {
        public static readonly string RootFolder = Application.dataPath;
        public static readonly string DataFolder = Path.Combine(RootFolder, "OmegaData");
        public static readonly string ModsFolder = Path.Combine(DataFolder, "Mods");
        public static readonly string ConfigFolder = Path.Combine(DataFolder, "Config");
        public static readonly string AssetsFolder = Path.Combine(DataFolder, "Assets");
    }
    //public abstract class NimbatusMod : PartialityMod
    //{
    //    public OmegaModLoader Mods;
    //    public abstract string Name { get; }
    //    public abstract string Description { get; }
    //    public virtual Version modVersion => new Version(0, 0, 1);

    //    public abstract string Author { get; }

    //    //public JObject Config { get; private set; }
    //    public virtual void Load(OmegaModLoader Mods)
    //    {
    //        this.Mods = Mods;
    //        string name = Name;
    //        if (name == null) throw new InvalidOperationException("NimbatusMod.Name returned null");

    //    }
    //    public override string ToString() => Name + " " + Version;

    //    public override void Init()
    //    {
    //        base.Init();
    //        Version = modVersion.ToString();
    //        author = Author;
    //        ModID = Name;
    //    }


    //    //public virtual JObject CreateDefaultConfiguration()
    //    //{
    //    //    return new JObject();
    //    //}

    //}
    [MonoModPatch("global::Assets.Nimbatus.GUI.MainMenu.Scripts.ShowVersionNumber")]
    class patch_ShowVersionNumber : ShowVersionNumber
    {

        public int labelSizeAdd;
       // public OmegaModLoader Mod;
        public extern void orig_Update();
        public void Update()
        {
            this.labelSizeAdd = 0;
            this.Label.SetDimensions(this.Label.width + this.labelSizeAdd, this.Label.height + this.labelSizeAdd);
            this.Label.text = "Version " + SaveGameManager.CurrentGameVersion + " Closed Alpha " + "Modded using OmegaMod";//+ this.Mod.ModInfo;
        }
        public void Start()
        {
            //this.Mod = new OmegaModLoader();
            Debug.Log("Running OmegaMod");
           // this.Mod.Startup();
        }
    }
}
