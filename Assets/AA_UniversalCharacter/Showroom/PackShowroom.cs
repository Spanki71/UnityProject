// Character showroom for a Codewart modular pack. Open the scene and press Play.
// Up / Down: character. Space: mix parts between all characters of the pack. Alt or Backspace: back to the character's own set.
// Ctrl: turn the whole set by 90 degrees. W / S: zoom in and back out, three steps. H: show or hide the helmet. T: show or hide the on-screen text.
// Left / Right: animation, available when the Clips list is filled in.
// One skeleton swaps its module meshes. In the T-pose the props float beside the character: in one line seen from the front and the back,
// in the corners around it seen from the sides, so they never stand behind it. In an animation they return to their bones.
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Codewart
{
    public class PackShowroom : MonoBehaviour
    {
        [Tooltip("Shown above the character name.")]
        public string packTitle = "";
        [Tooltip("Pack code in the mesh names, for example AE in Shield_M_AE_001.")]
        public string packCode = "AE";
        [Tooltip("Character prefabs in display order.")]
        public GameObject[] prefabs;
        public string[] names;
        [Tooltip("Optional. Prop prefabs of the pack: a character also shows the props numbered like its set, for example Sword_M_AC_001 with the male set 001.")]
        public GameObject[] props;
        [Tooltip("Optional. Empty: the showroom stays in the T-pose.")]
        public RuntimeAnimatorController[] clips;
        [Tooltip("Optional. A material for each character, in the order of Prefabs, shown instead of the pack material, for example a colour scheme. Empty entries keep the prefab's own material.")]
        public Material[] looks;
        public Shader floorShader, backdropShader;
        [Tooltip("Object that is visible only before Play.")]
        public GameObject editHint;
        public Color centerColor = new Color32(0x8C, 0x3E, 0xB4, 0xFF), edgeColor = new Color32(0x5E, 0x24, 0x82, 0xFF);

        const float Inner = 1.5f, Outer = 2.15f, Corner = 1.55f, Row = 1.02f, ViewTop = 2.62f;
        static readonly float[] ZoomHeight = { 0f, 1.7f, 1.05f, 0.62f }, ZoomFocus = { 1.18f, 1.15f, 1.42f, 1.62f };   // height of the view at the character and the point it looks at; step 0 fits the whole row
        static readonly Regex Module = new Regex(@"^(.+)_([A-Z]{2})_(\d{3})$");
        static readonly Regex PropName = new Regex("^(Helmet|Hat|Hood|Chaplet|Shield|Sword|Blade|Spear|Trident|Net|Bow|Arrow|Quiver|Mace|Morningstar|Axe|Hammer|Battlehammer|Halberd|Glaive|Dagger|Kunai|Cane|Lute)");
        static readonly Regex HeadGear = new Regex("^(Hat|Hood|Chaplet)$");
        static readonly Regex Numbered = new Regex(@"^[A-Za-z]+_(?:(?<sex>[MF])_(?:[LR]_)?)?[A-Z]{2}_(?<set>\d{2,3})(?:_\d+)?$");
        static readonly string[] OwnPlace = { "Helmet", "Shield", "Quiver", "Arrow" };
        static readonly string[] Mounts = { "Head", "Neck", "Hand_R", "Hand_L", "Elbow_L", "Elbow_R", "Spine_03" };
        static readonly Dictionary<string, string> Labels = new Dictionary<string, string>
        {
            ["Head_M"] = "Head", ["Hair_M"] = "Hair", ["Eyebrows"] = "Eyebrows", ["Facial_Hair"] = "Facial hair", ["Torso_M"] = "Torso", ["Pauldron"] = "Pauldrons", ["Arm_M"] = "Arms", ["Elbow"] = "Elbows",
            ["Forearm_M"] = "Forearms", ["Hand_M"] = "Hands", ["Belt"] = "Belt", ["Legs_M"] = "Legs", ["Knee"] = "Knees", ["Calf_M"] = "Calves", ["Foot_M"] = "Feet", ["Cape"] = "Cape",
        };
        static readonly (string key, string action)[] Hints = { ("↑ ↓", "Character"), ("← →", "Animation"), ("Space", "Mix parts"), ("Alt", "Own set"), ("Ctrl", "Rotate 90°"), ("W S", "Zoom"), ("H", "Helmet"), ("T", "Hide text") };

        class Prop
        {
            public GameObject go;
            public string kind, bone;
            public Vector3 pos, scale, worldScale, line, corner, center, homeLine, homeCorner;
            public Quaternion rot, floatRot;
            public float width, height;
            public bool hidden, leaned;
            public Renderer[] parts;
            public Material[][] plain;      // the prop's own materials, back when the set has no colour scheme
            public readonly Quaternion[] rots = new Quaternion[2];      // 0: on the right of the character, 1: on the left; a leaned pole tips outwards
            public readonly Vector3[] centers = new Vector3[2];
            public readonly float[] widths = new float[2];
        }

        class Hero
        {
            public string name;
            public readonly Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();
            public Prop helmet, shield;
            public readonly List<Prop> gear = new List<Prop>();   // a bow, its arrow and the quiver stay together
        }

        readonly List<Hero> heroes = new List<Hero>();
        readonly Dictionary<string, SkinnedMeshRenderer> slots = new Dictionary<string, SkinnedMeshRenderer>();
        readonly Dictionary<string, Transform> bones = new Dictionary<string, Transform>();
        readonly Dictionary<string, int> donors = new Dictionary<string, int>();      // part group -> character it comes from
        readonly List<Prop> shown = new List<Prop>();
        readonly Dictionary<Renderer, Material[]> plainBody = new Dictionary<Renderer, Material[]>();
        string packMaterial;
        Transform stage, holder;
        Animator rig;
        Camera view;
        Transform[] poseBones;
        Vector3[] posePos;
        Quaternion[] poseRot;
        Vector3 fwd, right;
        int hero, clip, turns, zoom, helmetOf, shieldOf, gearOf;
        int savedAntiAliasing, savedPixelLights, savedCascades;
        float yaw, yawSpeed, zoomNow, zoomSpeed, savedShadowDistance, rowReach = 2.72f, rowShift;
        ShadowResolution savedShadowResolution;
        bool savedRunInBackground, mixed, showText = true, showHelmet = true;
        GUIStyle small, large, plain, cap;
#if ENABLE_LEGACY_INPUT_MANAGER
        int ctrlWait;
        bool altHeld;
#else
        readonly HashSet<KeyCode> held = new HashSet<KeyCode>();
#endif

        int ClipCount => clips == null ? 0 : clips.Length;
        public int CharacterIndex => hero;
        public int AnimationIndex => clip;
        public int Turns => turns;
        public int Zoom => zoom;
        public bool ShowText => showText;
        public bool ShowHelmet => showHelmet;
        public bool Settled => Mathf.Abs(Mathf.DeltaAngle(yaw, 90f * turns)) < 0.01f;
        public string AnimationName => clip == 0 ? "T-Pose" : clips[clip - 1].name;
        public string Title => $"{heroes[hero].name}  ·  {AnimationName}{(mixed ? "  ·  Mixed set" : "")}";

        static string Group(string element) => Regex.Replace(element, "_[LR]$", "");
        static string Clean(string n) => Regex.Replace(n, @" \d+$", "");

        void Awake()
        {
            if (editHint) editHint.SetActive(false);
            savedRunInBackground = Application.runInBackground;
            Application.runInBackground = true;    // a click in another window does not stop the animation
            stage = new GameObject("Stage").transform;
            holder = new GameObject("Stored Props").transform;
            holder.gameObject.SetActive(false);
            var body = Instantiate(prefabs[0], stage);
            body.name = "Character";
            rig = body.GetComponent<Animator>();
            rig.applyRootMotion = false;   // animations stay in place
            rig.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            rig.runtimeAnimatorController = null;
            rig.enabled = false;
            foreach (var t in body.GetComponentsInChildren<Transform>(true))
                if (!t.GetComponent<Renderer>()) bones[t.name] = t;     // some module objects carry the same name as a bone
            fwd = Vector3.ProjectOnPlane(bones["Ball_L"].position - bones["Ankle_L"].position, Vector3.up).normalized;
            right = Vector3.Cross(fwd, Vector3.up);
            for (int i = 0; i < prefabs.Length; i++) heroes.Add(Read(prefabs[i], i));
            Lend();
            foreach (var root in PropRoots(body)) DestroyImmediate(root.gameObject);
            var unseen = body.GetComponentsInChildren<SkinnedMeshRenderer>(true).Where(s => !s.gameObject.activeInHierarchy).ToList();
            foreach (var s in body.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                string el = s.sharedMesh && Module.IsMatch(s.sharedMesh.name) ? Module.Match(s.sharedMesh.name).Groups[1].Value : Clean(s.gameObject.name);
                if (!slots.ContainsKey(el) && heroes.Any(h => h.meshes.ContainsKey(el))) slots[el] = s;
            }
            foreach (var s in slots.Values)      // the other body group of the first prefab has to show too
                for (var t = s.transform.parent; t != body.transform; t = t.parent) t.gameObject.SetActive(true);
            foreach (var s in unseen.Except(slots.Values)) s.gameObject.SetActive(false);
            foreach (var s in slots.Values) plainBody[s] = s.sharedMaterials;
            packMaterial = slots.Values.SelectMany(s => s.sharedMaterials).Where(m => m).GroupBy(m => m.name).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();      // a colour scheme replaces only the pack material
            foreach (var p in heroes.SelectMany(h => new[] { h.helmet, h.shield }.Concat(h.gear)).Where(p => p != null))
            {
                p.parts = p.go.GetComponentsInChildren<Renderer>(true);
                p.plain = p.parts.Select(r => r.sharedMaterials).ToArray();
            }
            poseBones = body.GetComponentsInChildren<Transform>(true);
            posePos = poseBones.Select(t => t.localPosition).ToArray();
            poseRot = poseBones.Select(t => t.localRotation).ToArray();
            var worn = heroes.Select(h => h.helmet).FirstOrDefault(p => p != null && p.bone != null) ?? heroes.Select(h => h.helmet).FirstOrDefault(p => p != null);   // a helmet worn on a bone first
            var canon = worn != null ? worn.go.transform.rotation : Quaternion.identity;   // prop meshes share the modelling axes, the helmet stands upright in them
            foreach (var h in heroes)
                foreach (var p in new[] { h.helmet, h.shield }.Concat(h.gear).Where(p => p != null)) Arrange(p, canon);
            Studio();
            NextCharacter(0);
        }

        void OnDestroy()
        {
            Application.runInBackground = savedRunInBackground;
            QualitySettings.shadowDistance = savedShadowDistance;
            QualitySettings.shadowResolution = savedShadowResolution;
            QualitySettings.shadowCascades = savedCascades;
            QualitySettings.pixelLightCount = savedPixelLights;
            QualitySettings.antiAliasing = savedAntiAliasing;
        }

        // props of the pack in a prefab: objects on a mount bone with a mesh of this pack
        List<Transform> PropRoots(GameObject root)
        {
            var found = new List<Transform>();
            foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            {
                var m = MeshOf(r);
                if (!m || !PropName.IsMatch(m.name) || !m.name.Contains($"_{packCode}_")) continue;
                var t = r.transform;
                while (t.parent && !Mounts.Contains(t.parent.name)) t = t.parent;
                if (t.parent && !found.Contains(t)) found.Add(t);
            }
            return found;
        }

        static Mesh MeshOf(Renderer r)
        {
            if (r is SkinnedMeshRenderer s) return s.sharedMesh;
            var f = r.GetComponent<MeshFilter>();
            return f ? f.sharedMesh : null;
        }

        Hero Read(GameObject prefab, int index)
        {
            var temp = Instantiate(prefab, stage);
            var h = new Hero { name = index < names.Length ? names[index] : prefab.name };
            foreach (var s in temp.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                if (s.gameObject.activeInHierarchy && s.sharedMesh && !PropName.IsMatch(s.sharedMesh.name) && Module.IsMatch(s.sharedMesh.name)) h.meshes[Module.Match(s.sharedMesh.name).Groups[1].Value] = s.sharedMesh;      // a whole body group can be switched off in a prefab
            foreach (var root in PropRoots(temp))
            {
                var mesh = root.GetComponentsInChildren<Renderer>(true).Select(MeshOf).First(m => m && PropName.IsMatch(m.name));
                string kind = PropName.Match(mesh.name).Value;
                var p = new Prop { go = root.gameObject, kind = HeadGear.IsMatch(kind) ? "Helmet" : kind, bone = root.parent.name, pos = root.localPosition, rot = root.localRotation, scale = root.localScale, worldScale = root.lossyScale, hidden = !root.gameObject.activeSelf };
                root.SetParent(holder, true);
                root.gameObject.SetActive(true);
                if (p.kind == "Helmet" && h.helmet == null) h.helmet = p;
                else if (p.kind == "Shield") h.shield = p;
                else h.gear.Add(p);
            }
            temp.SetActive(false);
            Destroy(temp);
            return h;
        }

        // props kept apart from the character prefabs, as in the older packs: Sword_M_AC_001 belongs to the male set 001, Axe_AA_001 to both sets 001.
        // They float beside the character in the T-pose; with no mount bone they stay out of the animations
        void Lend()
        {
            if (props == null) return;
            for (int i = 0; i < heroes.Count; i++)
            {
                var h = heroes[i];
                int set = SetNumber(prefabs[i]);
                if (set < 0) continue;
                var carried = new HashSet<string>(new[] { h.helmet, h.shield }.Concat(h.gear).Where(p => p != null).SelectMany(p => p.go.GetComponentsInChildren<Renderer>(true)).Select(MeshOf).Where(m => m).Select(m => m.name));
                foreach (var prefab in props.Where(p => p && PropName.IsMatch(p.name)))
                {
                    var name = Numbered.Match(prefab.name);
                    if (!name.Success || int.Parse(name.Groups["set"].Value) != set) continue;
                    if (name.Groups["sex"].Success && (name.Groups["sex"].Value == "F") != Female(h)) continue;
                    var meshes = prefab.GetComponentsInChildren<Renderer>(true).Select(MeshOf).Where(m => m).Select(m => m.name).ToList();
                    if (meshes.Count == 0 || meshes.Any(carried.Contains)) continue;
                    var go = Instantiate(prefab, holder);
                    go.name = prefab.name;
                    string kind = PropName.Match(prefab.name).Value;
                    var p = new Prop { go = go, kind = HeadGear.IsMatch(kind) ? "Helmet" : kind, scale = go.transform.localScale, worldScale = go.transform.lossyScale, rot = go.transform.localRotation };
                    if (p.kind == "Helmet" && h.helmet == null) h.helmet = p;
                    else if (p.kind == "Shield" && h.shield == null) h.shield = p;
                    else h.gear.Add(p);
                }
            }
        }

        // set number of a character: from its Modules Shaker, or from the prefab name as in M_AA_001
        static int SetNumber(GameObject prefab)
        {
            var shaker = prefab.GetComponent<ModulesShaker>();
            var digits = Regex.Match(shaker ? shaker.set_numeration : prefab.name, @"(\d{1,3})$");
            return digits.Success ? int.Parse(digits.Groups[1].Value) : -1;
        }

        // floating pose of a prop: one row across the view for the front and the back, the corners around the character for the sides
        void Arrange(Prop p, Quaternion canon)
        {
            var t = p.go.transform;
            t.SetParent(holder, false);
            t.localScale = p.worldScale;
            t.localPosition = Vector3.zero;
            var rot = canon;
            if (p.kind == "Arrow") rot = Quaternion.FromToRotation(-fwd, Vector3.up) * canon;     // an arrow lies along the model Y axis: head up
            if (p.kind == "Bow") rot = Upright(p, canon);
            var box = Box(p, rot);
            if (!OwnPlace.Contains(p.kind) && box.size.y < 0.3f * Mathf.Max(box.size.x, box.size.z))      // a net lying flat stands up and shows its face
            {
                rot = Quaternion.AngleAxis(90f, right) * rot;
                box = Box(p, rot);
            }
            float across = Mathf.Abs(right.x) * box.size.x + Mathf.Abs(right.z) * box.size.z, deep = Mathf.Abs(fwd.x) * box.size.x + Mathf.Abs(fwd.z) * box.size.z;
            if (deep > across * 1.2f && !new[] { "Helmet", "Shield", "Quiver", "Arrow", "Bow" }.Contains(p.kind))      // a blade shows its wide side, everything else faces the camera
            {
                rot = Quaternion.AngleAxis(90f, Vector3.up) * canon;
                box = Box(p, rot);
            }
            var upright = rot;
            float tilt = 0f;
            while (tilt < 60f && Box(p, Quaternion.AngleAxis(tilt, fwd) * upright).size.y + 0.12f > ViewTop) tilt += 2.5f;      // a glaive taller than the view leans, so the character keeps its size in every shot
            p.leaned = tilt > 0f;
            for (int s = 0; s < 2; s++)
            {
                var lean = Quaternion.AngleAxis(tilt, fwd);
                if ((Vector3.Dot(lean * Vector3.up, right) < 0f) == (s == 0)) lean = Quaternion.Inverse(lean);      // the top leans away from the character
                p.rots[s] = lean * upright;
                var b = Box(p, p.rots[s]);
                p.centers[s] = b.center;
                p.widths[s] = Mathf.Abs(right.x) * b.size.x + Mathf.Abs(right.z) * b.size.z;
            }
            rot = p.rots[0];
            box = Box(p, rot);
            float h = box.size.y, up = Mathf.Max(Row, 0.12f + h / 2f);
            Vector3 At(float side, float front, float y) => right * side + fwd * front + Vector3.up * y;
            switch (p.kind)
            {
                case "Shield": p.line = At(Inner, 0f, Row); p.corner = At(Corner, Corner, Row); break;
                case "Helmet": p.line = At(Outer, 0f, Row); p.corner = At(Corner, -Corner, 1.72f + h / 2f); break;
                case "Quiver": p.line = At(-2.38f, 0f, Row); p.corner = At(-Corner, Corner, 1.58f + h / 2f); break;
                case "Arrow": p.line = At(-2.02f, 0f, Row); p.corner = At(-Corner - 0.34f, -Corner, Row); break;
                default: p.line = At(-Inner, 0f, up); p.corner = At(-Corner, -Corner, up); break;
            }
            p.floatRot = rot;
            p.center = box.center;
            p.width = Mathf.Abs(right.x) * box.size.x + Mathf.Abs(right.z) * box.size.z;
            p.height = box.size.y;
            p.homeLine = p.line;
            p.homeCorner = p.corner;
        }

        // a bow mesh lies in the skeleton frame: upright from the string ends, belly away from the character
        Quaternion Upright(Prop p, Quaternion canon)
        {
            var t = p.go.transform;
            var all = p.go.GetComponentsInChildren<Transform>(true);
            if (!all.Any(b => b.name == "Bow_String_Top")) return canon;
            Vector3 At(string bone) => all.First(b => b.name == bone).position;
            t.rotation = canon;
            var q = Quaternion.FromToRotation((At("Bow_String_Top") - At("Bow_String_Bottom")).normalized, Vector3.up) * canon;
            t.rotation = q;
            var belly = Vector3.ProjectOnPlane(At("Bow_Grip") - (At("Bow_String_Top") + At("Bow_String_Bottom")) / 2f, Vector3.up);
            return Quaternion.AngleAxis(Vector3.SignedAngle(belly, -right, Vector3.up), Vector3.up) * q;
        }

        static Bounds Box(Prop p, Quaternion rot)
        {
            p.go.transform.rotation = rot;
            var box = new Bounds();
            bool first = true;
            foreach (var r in p.go.GetComponentsInChildren<Renderer>(true))
            {
                var m = MeshOf(r);
                if (!m) continue;
                var b = m.bounds;
                for (int i = 0; i < 8; i++)
                {
                    var c = r.transform.TransformPoint(b.center + Vector3.Scale(b.extents, new Vector3((i & 1) * 2 - 1, (i & 2) - 1, (i & 4) / 2 - 1)));
                    if (first) box = new Bounds(c, Vector3.zero);
                    else box.Encapsulate(c);
                    first = false;
                }
            }
            return box;
        }

        Light Lamp(string lampName, Vector3 direction, float power, Color color)
        {
            var l = new GameObject(lampName).AddComponent<Light>();
            l.type = LightType.Directional;
            l.transform.rotation = Quaternion.LookRotation(direction);
            l.intensity = power;
            l.color = color;
            l.renderMode = LightRenderMode.ForcePixel;
            l.shadows = LightShadows.None;
            return l;
        }

        // the studio: a soft pool of light on the backdrop, one shadow-casting key light, a cool fill and two rim lights
        void Studio()
        {
            savedShadowDistance = QualitySettings.shadowDistance;
            savedShadowResolution = QualitySettings.shadowResolution;
            savedCascades = QualitySettings.shadowCascades;
            savedPixelLights = QualitySettings.pixelLightCount;
            savedAntiAliasing = QualitySettings.antiAliasing;
            QualitySettings.shadowDistance = 24f;
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            QualitySettings.shadowCascades = 0;
            QualitySettings.pixelLightCount = 4;
            QualitySettings.antiAliasing = 8;
            RenderSettings.skybox = null;
            RenderSettings.fog = false;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.40f, 0.385f, 0.45f);
            RenderSettings.ambientEquatorColor = new Color(0.32f, 0.29f, 0.37f);
            RenderSettings.ambientGroundColor = new Color(0.27f, 0.18f, 0.34f);
            view = Camera.main ? Camera.main : new GameObject("Main Camera") { tag = "MainCamera" }.AddComponent<Camera>();
            view.clearFlags = CameraClearFlags.SolidColor;
            view.backgroundColor = centerColor;
            view.fieldOfView = 20f;
            view.nearClipPlane = 1f;
            view.farClipPlane = 130f;
            view.renderingPath = RenderingPath.Forward;
            view.allowMSAA = true;
            view.allowHDR = false;
            SmoothEdges(view);
            Frame();
            var key = Lamp("Key Light", -0.8f * fwd + 1.5f * Vector3.down + 0.55f * right, 0.70f, new Color(1f, 0.96f, 0.90f));
            key.shadows = LightShadows.Soft;
            key.shadowStrength = 0.7f;
            key.shadowBias = 0.03f;
            key.shadowNormalBias = 0.25f;
            Lamp("Fill Light", -fwd + 0.25f * Vector3.down - 0.85f * right, 0.17f, new Color(0.74f, 0.78f, 1f));
            Lamp("Rim Light Left", fwd + 0.55f * Vector3.down + 0.95f * right, 0.42f, new Color(1f, 0.82f, 1f));
            Lamp("Rim Light Right", fwd + 0.55f * Vector3.down - 0.95f * right, 0.42f, new Color(0.80f, 0.86f, 1f));
            Shader.SetGlobalColor("_ShowroomCenter", ShaderColor(centerColor));
            Shader.SetGlobalColor("_ShowroomEdge", ShaderColor(edgeColor));
            Shader.SetGlobalVector("_ShowroomFocus", new Vector4(0.5f, 0.46f, 1.05f, 0f));
            var backdrop = GameObject.CreatePrimitive(PrimitiveType.Quad);
            backdrop.name = "Backdrop";
            Destroy(backdrop.GetComponent<Collider>());
            backdrop.transform.SetParent(view.transform, false);
            backdrop.transform.localPosition = Vector3.forward * 120f;
            backdrop.transform.localScale = new Vector3(400f, 400f, 1f);
            var far = backdrop.GetComponent<MeshRenderer>();
            far.sharedMaterial = new Material(backdropShader);
            far.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            far.receiveShadows = false;
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            Destroy(floor.GetComponent<Collider>());
            floor.transform.localScale = new Vector3(20f, 1f, 20f);
            var ground = floor.GetComponent<MeshRenderer>();
            ground.sharedMaterial = new Material(floorShader);
            ground.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        // global shader colours arrive unconverted: a Linear project needs them in linear space
        // URP: anti-aliasing on the showroom camera only. In URP the project's pipeline asset decides MSAA (off in the Universal 3D template)
        // and QualitySettings.antiAliasing works only in the Built-in pipeline. SMAA runs in the camera's post-processing pass; nothing in the
        // project is changed. Found by name, so the script also compiles without URP.
        static void SmoothEdges(Camera cam)
        {
            if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline == null) return;
            var type = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
            if (type == null) return;
            var data = cam.GetComponent(type);
            if (!data) data = cam.gameObject.AddComponent(type);
            SetProperty(data, "renderPostProcessing", true);
            SetProperty(data, "antialiasing", "SubpixelMorphologicalAntiAliasing");
            SetProperty(data, "antialiasingQuality", "High");
        }

        static void SetProperty(Component target, string name, object value)
        {
            var property = target.GetType().GetProperty(name);
            if (property == null || !property.CanWrite) return;
            property.SetValue(target, value is string text && property.PropertyType.IsEnum ? System.Enum.Parse(property.PropertyType, text) : value);
        }

        static Color ShaderColor(Color c) => QualitySettings.activeColorSpace == ColorSpace.Linear ? c.linear : c;

        // the whole row stays in frame in any window shape
        public void Frame()
        {
            float tan = Mathf.Tan(view.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float full = 2f * tan * Mathf.Max(7.4f, rowReach / (tan * view.aspect));
            int step = Mathf.Min((int)zoomNow, ZoomHeight.Length - 2);
            float height = Mathf.Lerp(step == 0 ? full : ZoomHeight[step], ZoomHeight[step + 1], zoomNow - step);
            float focus = Mathf.Lerp(ZoomFocus[step], ZoomFocus[step + 1], zoomNow - step);
            var pan = right * (rowShift * Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Clamp01(1f - zoomNow));      // the row stays in the middle; zoomed in, the view looks at the character
            view.transform.position = fwd * (height / (2f * tan)) + Vector3.up * (focus + 0.14f * height / full) + pan;
            view.transform.LookAt(Vector3.up * focus + pan);
        }

        public void NextCharacter(int delta)
        {
            hero = (hero + delta + heroes.Count) % heroes.Count;
            clip = 0;
            turns = 0;
            yaw = yawSpeed = 0f;
            stage.rotation = Quaternion.identity;
            Pose();
            ResetSet();
        }

        public void NextAnimation(int delta)
        {
            int count = ClipCount + 1;
            clip = (clip + delta + count) % count;
            Pose();
            Dress();
        }

        public void MixParts()
        {
            var same = Enumerable.Range(0, heroes.Count).Where(i => Female(heroes[i]) == Female(heroes[hero])).ToList();   // male and female bodies do not share modules
            int Pick() => same[Random.Range(0, same.Count)];
            foreach (var g in slots.Keys.Select(Group).Distinct()) donors[g] = Pick();
            helmetOf = Pick();
            shieldOf = Pick();
            gearOf = Pick();
            mixed = true;
            Dress();
        }

        static bool Female(Hero h) => h.meshes.Keys.Any(k => k.StartsWith("Torso_F"));

        public void ResetSet()
        {
            donors.Clear();
            helmetOf = shieldOf = gearOf = hero;
            mixed = false;
            Dress();
        }

        public void Turn() => turns = (turns + 1) % 4;

        void Pose()
        {
            rig.enabled = clip > 0;
            if (clip > 0)
            {
                rig.runtimeAnimatorController = clips[clip - 1];
                rig.Rebind();
                rig.Update(0f);
                return;
            }
            rig.runtimeAnimatorController = null;
            for (int i = 0; i < poseBones.Length; i++) poseBones[i].SetLocalPositionAndRotation(posePos[i], poseRot[i]);
        }

        int Donor(string element) => donors.TryGetValue(Group(element), out var d) ? d : hero;

        // modules in their slots and props in their places, for the chosen set
        void Dress()
        {
            var look = looks != null && hero < looks.Length ? looks[hero] : null;      // mixed parts take the colour scheme of the character they are placed on
            Material[] Paint(Material[] own) => look ? own.Select(m => m && m.name == packMaterial ? look : m).ToArray() : own;
            foreach (var kv in slots)
            {
                bool has = heroes[Donor(kv.Key)].meshes.TryGetValue(kv.Key, out var mesh);
                kv.Value.gameObject.SetActive(has);
                if (has) kv.Value.sharedMesh = mesh;
                kv.Value.sharedMaterials = Paint(plainBody[kv.Value]);
            }
            foreach (var p in shown)
            {
                p.go.transform.SetParent(holder, false);
                p.go.SetActive(false);
            }
            shown.Clear();
            shown.AddRange(new[] { heroes[helmetOf].helmet, heroes[shieldOf].shield }.Concat(heroes[gearOf].gear).Where(p => p != null));
            foreach (var p in shown)
            {
                var t = p.go.transform;
                for (int i = 0; i < p.parts.Length; i++) p.parts[i].sharedMaterials = Paint(p.plain[i]);
                bool mounted = p.bone != null && bones.ContainsKey(p.bone);      // a prop kept apart from the prefab has no bone to return to
                p.go.SetActive((clip == 0 || (!p.hidden && mounted)) && (showHelmet || p.kind != "Helmet"));   // the arrow is shown in the T-pose, in animations the animator switches it on
                if (clip == 0)
                {
                    t.SetParent(stage, false);
                    t.localScale = p.worldScale;
                    t.localRotation = p.floatRot;
                }
                else if (mounted)
                {
                    t.SetParent(bones[p.bone], false);
                    t.localScale = p.scale;
                    t.SetLocalPositionAndRotation(p.pos, p.rot);
                }
            }
            if (clip == 0)
            {
                Lay();
                Spread();
            }
        }

        // the T-pose row: the shield and long weapons on the right, every other prop on the side that holds less, outward from the hands.
        // The character keeps one size in every shot; the view pans so the row stays in the middle
        void Lay()
        {
            const float gap = 0.16f, hand = 1.05f, tall = 2.25f;      // a wide net or shield keeps clear of the hand of the T-pose; a tall prop stays clear of the name in the top left corner
            var lhs = new List<Prop>();
            var rhs = new List<Prop>();
            float Width(List<Prop> side) => side.Sum(p => p.widths[side == rhs ? 0 : 1] + gap);
            List<Prop> Lighter() => Width(lhs) <= Width(rhs) ? lhs : rhs;
            var net = shown.FirstOrDefault(p => p.kind == "Net");
            var cross = net != null ? shown.Where(p => p.leaned).OrderByDescending(p => p.height).FirstOrDefault() : null;      // a long pole crosses the net instead of taking a place of its own
            var bow = shown.Where(p => p.kind == "Bow" || p.kind == "Arrow" || p.kind == "Quiver").OrderBy(p => p.kind == "Bow" ? 0 : p.kind == "Arrow" ? 1 : 2).ToList();
            rhs.AddRange(shown.Where(p => p.kind == "Shield"));
            foreach (var p in shown.Where(p => p.kind != "Shield" && p.kind != "Helmet" && p != cross && !bow.Contains(p)).OrderByDescending(p => p.height))
                (p.homeLine.y + p.height / 2f > tall || (p == net && cross != null) ? rhs : Lighter()).Add(p);
            if (bow.Count > 0) Lighter().AddRange(bow);      // a bow, its arrow and the quiver stay together
            foreach (var p in shown.Where(p => p.kind == "Helmet")) Lighter().Add(p);
            float low = -hand - 0.1f, high = hand + 0.1f;
            foreach (var (side, sign) in new[] { (lhs, -1f), (rhs, 1f) })
            {
                int s = sign > 0f ? 0 : 1;
                float edge = hand, first = 0f;
                for (int i = 0; i < side.Count; i++)
                {
                    var p = side[i];
                    Face(p, s);
                    float x = i == 0 ? Mathf.Max(Inner, hand + p.width / 2f) : edge + p.width / 2f;
                    if (i == 0) first = x;
                    var lift = Vector3.Project(p.homeLine, Vector3.up);
                    p.line = right * (sign * x) + lift;
                    p.corner = right * (sign * (Corner + x - first)) + Vector3.Project(p.homeCorner, fwd) + Vector3.Project(p.homeCorner, Vector3.up);
                    edge = x + p.width / 2f + gap;
                    low = Mathf.Min(low, sign * x - p.width / 2f);
                    high = Mathf.Max(high, sign * x + p.width / 2f);
                }
            }
            if (cross != null)
            {
                int s = rhs.Contains(net) ? 0 : 1;
                Face(cross, s);
                cross.line = Vector3.ProjectOnPlane(net.line, Vector3.up) + Vector3.Project(cross.homeLine, Vector3.up) + fwd * 0.3f;
                cross.corner = net.corner + fwd * 0.3f;
            }
            rowShift = (low + high) / 2f;
            rowReach = Mathf.Max(2.72f, (high - low) / 2f + 0.15f);      // only a row wider than the view makes the character smaller
            Frame();
        }

        void Face(Prop p, int side)
        {
            p.floatRot = p.rots[side];
            p.center = p.centers[side];
            p.width = p.widths[side];
            p.go.transform.localRotation = p.floatRot;
        }

        void Spread()
        {
            float side = Mathf.Sin(yaw * Mathf.Deg2Rad);
            foreach (var p in shown) p.go.transform.localPosition = Vector3.Lerp(p.line, p.corner, side * side) - p.center;
        }

        // how many slots and props differ from the character's own set
        public int Differences()
        {
            var h = heroes[hero];
            int bad = slots.Count(kv => h.meshes.TryGetValue(kv.Key, out var m) ? !kv.Value.gameObject.activeSelf || kv.Value.sharedMesh != m : kv.Value.gameObject.activeSelf);
            var own = new[] { h.helmet, h.shield }.Concat(h.gear).Where(p => p != null).ToList();
            return bad + shown.Except(own).Count() + own.Except(shown).Count();
        }

        public string Donors => string.Join(", ", DonorLines());

        IEnumerable<string> DonorLines()
        {
            foreach (var kv in Labels.Where(kv => slots.Keys.Any(k => Group(k) == kv.Key)))
            {
                int d = Donor(kv.Key);
                bool has = heroes[d].meshes.Keys.Any(k => Group(k) == kv.Key);
                yield return $"{kv.Value}: {(has ? heroes[d].name : "none")}";
            }
            yield return $"Helmet: {heroes[helmetOf].name}";
            yield return $"Shield: {heroes[shieldOf].name}";
            yield return $"Weapon: {heroes[gearOf].name}";
        }

        void Press(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.UpArrow: NextCharacter(-1); break;
                case KeyCode.DownArrow: NextCharacter(1); break;
                case KeyCode.LeftArrow: NextAnimation(-1); break;
                case KeyCode.RightArrow: NextAnimation(1); break;
                case KeyCode.Space: MixParts(); break;
                case KeyCode.Backspace: ResetSet(); break;
                case KeyCode.W: zoom = Mathf.Min(zoom + 1, ZoomHeight.Length - 1); break;
                case KeyCode.S: zoom = Mathf.Max(zoom - 1, 0); break;
                case KeyCode.H: showHelmet = !showHelmet; Dress(); break;
                case KeyCode.T: showText = !showText; break;
            }
        }

#if ENABLE_LEGACY_INPUT_MANAGER
#if UNITY_EDITOR_WIN
        [System.Runtime.InteropServices.DllImport("user32.dll")] static extern void keybd_event(byte key, byte scan, uint flags, System.UIntPtr extra);
        // in the Editor a lone Alt activates the Windows menu bar, which then takes the next keys; an unassigned key pressed with it prevents that
        static void MaskMenu()
        {
            keybd_event(0xE8, 0, 0, System.UIntPtr.Zero);
            keybd_event(0xE8, 0, 2, System.UIntPtr.Zero);
        }
#else
        static void MaskMenu() { }
#endif

        void ReadKeys()
        {
            foreach (var key in new[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Space, KeyCode.Backspace, KeyCode.W, KeyCode.S, KeyCode.H, KeyCode.T })
                if (Input.GetKeyDown(key)) Press(key);
            bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr);
            if (alt && !altHeld) MaskMenu();
            if (!alt && altHeld && Application.isFocused) ResetSet();   // on release, so that Alt+Tab keeps the mix
            altHeld = alt;
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) ctrlWait = 2;
            if (alt) ctrlWait = 0;     // AltGr reaches Windows as Ctrl + right Alt
            if (ctrlWait > 0 && --ctrlWait == 0) Turn();
        }
#else
        void ReadKeys() { }

        // this project has the Input Manager switched off: keys arrive through OnGUI
        void KeyEvent(Event e)
        {
            if (e.type == EventType.KeyUp) held.Remove(e.keyCode);
            if (e.type != EventType.KeyDown || !held.Add(e.keyCode)) return;
            if (e.keyCode == KeyCode.LeftAlt || e.keyCode == KeyCode.RightAlt) ResetSet();
            else if (e.keyCode == KeyCode.LeftControl || e.keyCode == KeyCode.RightControl) Turn();
            else Press(e.keyCode);
        }
#endif

        void Update()
        {
            ReadKeys();
            yaw = Mathf.SmoothDampAngle(yaw, 90f * turns, ref yawSpeed, 0.13f);
            if (Settled) yaw = 90f * turns;
            stage.rotation = Quaternion.AngleAxis(yaw, Vector3.up);
            if (clip == 0) Spread();
            zoomNow = Mathf.SmoothDamp(zoomNow, zoom, ref zoomSpeed, 0.13f);
            Frame();
        }

        void OnGUI()
        {
#if !ENABLE_LEGACY_INPUT_MANAGER
            KeyEvent(Event.current);
#endif
            if (!showText || Event.current.type != EventType.Repaint) return;
            small = small ?? new GUIStyle(GUI.skin.label) { fontSize = 16, normal = { textColor = new Color(1f, 1f, 1f, 0.72f) } };
            plain = plain ?? new GUIStyle(small) { fontSize = 19, normal = { textColor = new Color(1f, 1f, 1f, 0.92f) } };
            large = large ?? new GUIStyle(small) { fontSize = 42, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            cap = cap ?? new GUIStyle(small) { fontSize = 15, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
            float scale = Mathf.Max(0.62f, Screen.height / 1080f);
            var hints = Hints.Where(h => (h.action != "Animation" || ClipCount > 0) && (h.action != "Helmet" || heroes.Any(c => c.helmet != null))).ToArray();
            var widths = hints.Select(h => (key: cap.CalcSize(new GUIContent(h.key)).x + 22f, text: small.CalcSize(new GUIContent(h.action)).x)).ToList();
            float total = widths.Sum(w => w.key + w.text + 44f) - 34f;
            scale = Mathf.Min(scale, (Screen.width - 32f) / total);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            float width = Screen.width / scale, height = Screen.height / scale;
            GUI.Label(new Rect(36f, 26f, width - 72f, 26f), packTitle.ToUpperInvariant(), small);
            GUI.Label(new Rect(34f, 48f, width - 72f, 56f), heroes[hero].name, large);
            GUI.Label(new Rect(36f, 106f, width - 72f, 30f), $"{hero + 1} / {heroes.Count}{(ClipCount > 0 ? "     " + AnimationName : "")}{(mixed ? "     Mixed set" : "")}", plain);
            if (mixed) GUI.Label(new Rect(36f, 146f, 300f, height - 220f), string.Join("\n", DonorLines()), small);
            float x = (width - total) / 2f, y = height - 62f;
            for (int i = 0; i < hints.Length; i++)
            {
                var c = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, 0.15f);
                GUI.DrawTexture(new Rect(x, y, widths[i].key, 30f), Texture2D.whiteTexture);
                GUI.color = c;
                GUI.Label(new Rect(x, y, widths[i].key, 30f), hints[i].key, cap);
                GUI.Label(new Rect(x + widths[i].key + 10f, y + 3f, widths[i].text + 8f, 26f), hints[i].action, small);
                x += widths[i].key + widths[i].text + 44f;
            }
            GUI.matrix = Matrix4x4.identity;
        }
    }
}
