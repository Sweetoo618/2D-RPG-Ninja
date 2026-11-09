using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// 按 GreenNinja 的同构 grid 布局，为 Characters 下所有角色批量生成四方向动画、
/// AnimatorOverrideController 与 CharacterDefinition 数据资产。
/// </summary>
public static class CharacterAnimGenerator
{
    private const string CharactersRoot = "Assets/Sprites/NinjaAdventure/Actor/Characters";
    private const string BaseControllerPath = "Assets/Animations/Player/Player_Ninja.controller";
    private const string OutRoot = "Assets/Animations/Characters";
    private const string DefinitionRoot = "Assets/SO/Character";

    private struct FrameSet
    {
        public Sprite[] idleDown, idleUp, idleLeft, idleRight;
        public Sprite[] walkDown, walkUp, walkLeft, walkRight;
        public Sprite[] attackDown, attackUp, attackLeft, attackRight;
        public Sprite dead;
    }

    [MenuItem("Tools/Generate Character Animations")]
    public static void GenerateAll()
    {
        Debug.Log("CharacterAnimGenerator: start");
        EnsureFolder(OutRoot);
        EnsureFolder(DefinitionRoot);

        var baseClips = LoadBaseClips();
        string[] dirs = Directory.GetDirectories(CharactersRoot);
        foreach (string dir in dirs)
        {
            string name = Path.GetFileName(dir);
            string sheetPath = dir.Replace("\\", "/") + "/SpriteSheet.png";
            Sprite[] sprites = LoadSprites(sheetPath);
            if (sprites == null || sprites.Length < 16) { Debug.LogWarning("Skip (no sheet): " + name); continue; }

            bool full = sprites.Length >= 28;
            FrameSet set = BuildFrames(sprites, full);

            string charDir = OutRoot + "/" + name;
            EnsureFolder(charDir);
            CreateClips(charDir, set);

            var charClips = LoadCharClips(charDir);
            RuntimeAnimatorController controller = name == "GreenNinja"
                ? AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(BaseControllerPath)
                : CreateOverride(charDir, name, baseClips, charClips);

            Sprite[] faces = LoadSprites(dir.Replace("\\", "/") + "/Faceset.png");
            Sprite portrait = faces != null && faces.Length > 0 ? faces[0] : null;
            CreateDefinition(name, set.idleDown[0], set.walkDown, portrait, controller);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("CharacterAnimGenerator: done");
    }

    private static Dictionary<string, AnimationClip> LoadBaseClips()
    {
        var result = new Dictionary<string, AnimationClip>();
        string[] paths = Directory.GetFiles("Assets/Animations/Player", "*.anim");
        foreach (string p in paths)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(p.Replace("\\", "/"));
            if (clip != null) result[clip.name] = clip;
        }
        return result;
    }

    private static Dictionary<string, AnimationClip> LoadCharClips(string charDir)
    {
        var result = new Dictionary<string, AnimationClip>();
        string[] paths = Directory.GetFiles(charDir.Replace("/", "\\"), "*.anim");
        foreach (string p in paths)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(p.Replace("\\", "/"));
            if (clip != null) result[clip.name] = clip;
        }
        return result;
    }

    private static Sprite[] LoadSprites(string sheetPath)
    {
        var list = new List<Sprite>();
        foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(sheetPath))
        {
            var sp = o as Sprite;
            if (sp != null) list.Add(sp);
        }
        if (list.Count == 0) return null;
        list.Sort((a, b) =>
        {
            int ay = -(int)a.rect.y, by = -(int)b.rect.y;
            return ay != by ? ay.CompareTo(by) : ((int)a.rect.x).CompareTo((int)b.rect.x);
        });
        return list.ToArray();
    }

    private static FrameSet BuildFrames(Sprite[] s, bool full)
    {
        var set = new FrameSet();
        set.idleDown = new[] { s[0] };
        set.idleUp = new[] { s[1] };
        set.idleLeft = new[] { s[2] };
        set.idleRight = new[] { s[3] };
        set.walkDown = new[] { s[4], s[8], s[12] };
        set.walkUp = new[] { s[5], s[9], s[13] };
        set.walkLeft = new[] { s[6], s[10], s[14] };
        set.walkRight = new[] { s[7], s[11], s[15] };
        if (full)
        {
            set.attackDown = new[] { s[16], s[20] };
            set.attackUp = new[] { s[17], s[21] };
            set.attackLeft = new[] { s[18], s[22] };
            set.attackRight = new[] { s[19], s[23] };
            set.dead = s[24];
        }
        else
        {
            set.attackDown = set.idleDown;
            set.attackUp = set.idleUp;
            set.attackLeft = set.idleLeft;
            set.attackRight = set.idleRight;
            set.dead = set.idleDown[0];
        }
        return set;
    }

    private static void CreateClips(string dir, FrameSet set)
    {
        CreateClip(dir + "/Idle_Down.anim", set.idleDown, 1f, false);
        CreateClip(dir + "/Idle_Up.anim", set.idleUp, 1f, false);
        CreateClip(dir + "/Idle_Left.anim", set.idleLeft, 1f, false);
        CreateClip(dir + "/Idle_Right.anim", set.idleRight, 1f, false);
        CreateClip(dir + "/Walk_Down.anim", set.walkDown, 6f, true);
        CreateClip(dir + "/Walk_Up.anim", set.walkUp, 6f, true);
        CreateClip(dir + "/Walk_Left.anim", set.walkLeft, 6f, true);
        CreateClip(dir + "/Walk_Right.anim", set.walkRight, 6f, true);
        CreateClip(dir + "/Attack_Down.anim", set.attackDown, 6f, false);
        CreateClip(dir + "/Attack_Up.anim", set.attackUp, 6f, false);
        CreateClip(dir + "/Attack_Left.anim", set.attackLeft, 6f, false);
        CreateClip(dir + "/Attack_Right.anim", set.attackRight, 6f, false);
        CreateClip(dir + "/Dead.anim", new[] { set.dead }, 1f, false);
    }

    private static void CreateClip(string path, Sprite[] frames, float fps, bool loop)
    {
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(path) != null) AssetDatabase.DeleteAsset(path);
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 60f;
        var keys = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < frames.Length; i++)
            keys[i] = new ObjectReferenceKeyframe { time = i / fps, value = frames[i] };
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        AssetDatabase.CreateAsset(clip, path);
    }

    private static RuntimeAnimatorController CreateOverride(string dir, string name, Dictionary<string, AnimationClip> baseClips, Dictionary<string, AnimationClip> charClips)
    {
        var baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(BaseControllerPath);
        if (baseController == null) return null;

        var overrideController = new AnimatorOverrideController(baseController);
        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(pairs);
        for (int i = 0; i < pairs.Count; i++)
        {
            AnimationClip baseClip = pairs[i].Key;
            if (baseClip == null) continue;
            if (charClips.TryGetValue(baseClip.name, out AnimationClip replacement))
                pairs[i] = new KeyValuePair<AnimationClip, AnimationClip>(baseClip, replacement);
        }
        overrideController.ApplyOverrides(pairs);

        string path = dir + "/" + name + ".overrideController";
        if (AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(path) != null) AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(overrideController, path);
        return overrideController;
    }

    private static void CreateDefinition(string name, Sprite preview, Sprite[] previewFrames, Sprite portrait, RuntimeAnimatorController controller)
    {
        string path = DefinitionRoot + "/Character_" + name + ".asset";
        var def = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(path);
        if (def == null)
        {
            def = ScriptableObject.CreateInstance<CharacterDefinition>();
            AssetDatabase.CreateAsset(def, path);
        }
        def.displayName = name;
        def.previewSprite = preview;
        def.previewFrames = previewFrames;
        def.portrait = portrait;
        def.animatorController = controller;
        EditorUtility.SetDirty(def);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string leaf = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, leaf);
    }
}