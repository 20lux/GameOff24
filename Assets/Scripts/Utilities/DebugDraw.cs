using UnityEngine;
using System;
using System.Collections.Generic;

namespace jazari.DebugDraw
{
    [Flags]
    public enum DebugLayers : uint
    {
        None = 0,
        Layer1 = 1 << 1,
        Layer2 = 1 << 2,
        Layer3 = 1 << 3,
        Layer4 = 1 << 4,
        Layer5 = 1 << 5,
        Layer6 = 1 << 6,
        Layer7 = 1 << 7,
        Layer8 = 1 << 8,
        All = Layer1 | Layer2 | Layer3 | Layer4 | Layer5 | Layer6 | Layer7 | Layer8
    }

    public class DebugDraw : MonoBehaviour
    {
        // Singleton instance
        private static DebugDraw _instance;
        public static DebugDraw Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Create a new GameObject and attach this script
                    GameObject go = new GameObject("DebugDraw");
                    _instance = go.AddComponent<DebugDraw>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private static DebugMeshDrawer _meshDrawer;
        private static uint _enabledLayers = (uint)DebugLayers.All;
        private static bool _doDepthTest = true;
        private static int _maxPoolSize = 1024;
        private static int _startingPoolSize = 256;

        public static Action OnDrawSettingsUpdated;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _meshDrawer = new DebugMeshDrawer(this);
            } else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            // Update and render debug shapes
            _meshDrawer.Update();
        }

        #region Drawing Methods

        public static void SetDrawingDepthTestEnabled(bool enabled)
        {
            if (enabled != _doDepthTest)
            {
                _doDepthTest = enabled;
                _meshDrawer.SetDepthTestEnabled(_doDepthTest);
                OnDrawSettingsUpdated?.Invoke();
            }
        }

        public static void SetEnabledLayers(uint layers)
        {
            _enabledLayers = layers;
            OnDrawSettingsUpdated?.Invoke();
        }

        public static void SetLayerEnabled(uint layer, bool enabled)
        {
            if (enabled)
            {
                _enabledLayers |= layer;
            } else
            {
                _enabledLayers &= ~layer;
            }
            OnDrawSettingsUpdated?.Invoke();
        }

        public static bool GetDoDepthTest()
        {
            return _doDepthTest;
        }

        public static uint GetEnabledLayers()
        {
            return _enabledLayers;
        }

        public static int GetMaxPoolSize()
        {
            return _maxPoolSize;
        }

        public static int GetStartingPoolSize()
        {
            return _startingPoolSize;
        }

        // Static methods to draw shapes
        public static void Box(Vector3 position, Vector3 size, Color? color = null, float duration = 0f, bool drawSolid = false, uint layers = (uint)DebugLayers.Layer1)
        {
            Matrix4x4 transform = Matrix4x4.TRS(position, Quaternion.identity, size);
            DebugDraw._meshDrawer.DrawBox(transform, duration, color ?? Color.white, drawSolid, layers);
        }

        public static void Box(Matrix4x4 transform, Color? color = null, float duration = 0f, bool drawSolid = false, uint layers = (uint)DebugLayers.Layer1)
        {
            DebugDraw._meshDrawer.DrawBox(transform, duration, color ?? Color.white, drawSolid, layers);
        }

        public static void Sphere(Vector3 position, float radius = 1f, Color? color = null, float duration = 0f, bool drawSolid = false, uint layers = (uint)DebugLayers.Layer1)
        {
            Matrix4x4 transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one * radius * 2f);
            DebugDraw._meshDrawer.DrawSphere(transform, duration, color ?? Color.white, drawSolid, layers);
        }

        public static void Sphere(Matrix4x4 transform, Color? color = null, float duration = 0f, bool drawSolid = false, uint layers = (uint)DebugLayers.Layer1)
        {
            DebugDraw._meshDrawer.DrawSphere(transform, duration, color ?? Color.white, drawSolid, layers);
        }

        // Implement other shapes (Cylinder, Capsule, etc.) similarly

        #endregion
    }

    public class DebugMeshDrawer
    {
        private readonly DebugDraw _parent;

        public readonly ObjectPool<DrawMeshInstance> MeshPool;
        private readonly List<DebugMeshCollection> _collections = new List<DebugMeshCollection>();

        // Collections for different shapes
        private readonly DebugMeshCollection _boxCollection;
        private readonly DebugMeshCollection _boxSolidCollection;
        private readonly DebugMeshCollection _sphereCollection;
        private readonly DebugMeshCollection _sphereSolidCollection;
        // Add other collections as needed

        public DebugMeshDrawer(DebugDraw parent)
        {
            _parent = parent;

            MeshPool = new ObjectPool<DrawMeshInstance>(DebugDraw.GetMaxPoolSize(), DebugDraw.GetStartingPoolSize());

            // Create materials
            Material wireframeMaterial = CreateDefaultMaterial(false);
            Material solidMaterial = CreateDefaultMaterial(true);

            // Create meshes
            Mesh boxMesh = DebugMeshes.Construct(DebugShape.Cube);
            Mesh sphereMesh = DebugMeshes.Construct(DebugShape.Sphere);
            // Create other meshes as needed

            // Create collections
            _boxCollection = new DebugMeshCollection(boxMesh, wireframeMaterial);
            _boxSolidCollection = new DebugMeshCollection(boxMesh, solidMaterial);
            _sphereCollection = new DebugMeshCollection(sphereMesh, wireframeMaterial);
            _sphereSolidCollection = new DebugMeshCollection(sphereMesh, solidMaterial);
            // Add other collections as needed

            _collections.Add(_boxCollection);
            _collections.Add(_boxSolidCollection);
            _collections.Add(_sphereCollection);
            _collections.Add(_sphereSolidCollection);
            // Add other collections as needed

            DebugMeshCollection.OnInstanceRemoved += inst => MeshPool.Return(inst);
        }

        private Material CreateDefaultMaterial(bool solid)
        {
            Shader shader = Shader.Find(solid ? "Standard" : "Custom/WireframeShader");
            if (shader == null)
            {
                Debug.LogError("Shader not found. Ensure the shader is included in the project.");
                shader = Shader.Find("Standard");
            }
            Material material = new Material(shader);
            material.SetColor("_Color", Color.white);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
            if (!solid)
            {
                material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                material.SetInt("_ZWrite", 1);
            }
            return material;
        }

        private DrawMeshInstance GetAMeshInstance(Matrix4x4 transform, float duration, Color color, uint layers)
        {
            DrawMeshInstance inst = MeshPool.Retrieve();
            if (inst != null)
            {
                inst.Transform = transform;
                inst.SetDuration(duration);
                inst.Color = color;
                inst.DrawLayers = layers;
            }
            return inst;
        }

        public void Update()
        {
            foreach (var collection in _collections)
            {
                collection.Update();
            }
        }

        public void SetDepthTestEnabled(bool doDepthTest)
        {
            foreach (var collection in _collections)
            {
                collection.Material.SetInt("_ZTest", doDepthTest ? (int)UnityEngine.Rendering.CompareFunction.LessEqual : (int)UnityEngine.Rendering.CompareFunction.Always);
            }
        }

        #region Drawing Methods

        public void DrawBox(Matrix4x4 transform, float duration, Color color, bool drawSolid, uint layers)
        {
            if ((DebugDraw.GetEnabledLayers() & layers) == 0)
            {
                return;
            }
            var instance = GetAMeshInstance(transform, duration, color, layers);
            if (drawSolid)
            {
                _boxSolidCollection.Add(instance);
            } else
            {
                _boxCollection.Add(instance);
            }
        }

        public void DrawSphere(Matrix4x4 transform, float duration, Color color, bool drawSolid, uint layers)
        {
            if ((DebugDraw.GetEnabledLayers() & layers) == 0)
            {
                return;
            }
            var instance = GetAMeshInstance(transform, duration, color, layers);
            if (drawSolid)
            {
                _sphereSolidCollection.Add(instance);
            } else
            {
                _sphereCollection.Add(instance);
            }
        }

        // Implement other shapes similarly

        #endregion
    }

    public class DebugMeshCollection
    {
        public Mesh Mesh { get; }
        public Material Material { get; }
        private readonly List<DrawMeshInstance> _drawInstances = new List<DrawMeshInstance>();

        public static Action<DrawMeshInstance> OnInstanceRemoved;

        public DebugMeshCollection(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
        }

        public void Update()
        {
            // Remove expired instances
            for (int i = _drawInstances.Count - 1; i >= 0; i--)
            {
                if (_drawInstances[i].IsExpired())
                {
                    OnInstanceRemoved?.Invoke(_drawInstances[i]);
                    _drawInstances.RemoveAt(i);
                }
            }

            if (_drawInstances.Count == 0)
            {
                return;
            }

            // Prepare matrices and colors
            int instanceCount = _drawInstances.Count;
            Matrix4x4[] matrices = new Matrix4x4[instanceCount];
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            Vector4[] colors = new Vector4[instanceCount];

            for (int i = 0; i < instanceCount; i++)
            {
                matrices[i] = _drawInstances[i].Transform;
                colors[i] = _drawInstances[i].Color;
                _drawInstances[i].BeenDrawn = true;
            }

            // Set per-instance colors
            propertyBlock.SetVectorArray("_Color", colors);

            // Draw instances
            Graphics.DrawMeshInstanced(Mesh, 0, Material, matrices, instanceCount, propertyBlock);
        }

        public void Add(DrawMeshInstance instance)
        {
            if (instance != null)
            {
                _drawInstances.Add(instance);
            }
        }
    }

    public interface IPoolable
    {
        void Reset();
    }

    public class ObjectPool<T> where T : IPoolable, new()
    {
        public readonly int MaxSize;
        public int CurrentSize;
        public int FreeObjects;
        private readonly Queue<T> _pool;

        public ObjectPool(int maxSize, int startingSize)
        {
            _pool = new Queue<T>();
            MaxSize = maxSize;
            ExpandPool(startingSize);
        }

        public T Retrieve()
        {
            if (FreeObjects == 0 && !ExpandPool(1))
            {
                Debug.LogWarning($"{typeof(T)} pool has no free objects, consider increasing max size");
                return default;
            }
            FreeObjects--;
            return _pool.Dequeue();
        }

        public bool ExpandPool(int expansion)
        {
            expansion = Mathf.Min(expansion, MaxSize - CurrentSize);
            if (expansion == 0)
            {
                return false;
            }
            for (int i = 0; i < expansion; i++)
            {
                _pool.Enqueue(new T());
            }
            FreeObjects += expansion;
            CurrentSize += expansion;
            return true;
        }

        public void Return(T obj)
        {
            obj.Reset();
            _pool.Enqueue(obj);
            FreeObjects++;
        }
    }

    public class DrawInstance : IPoolable
    {
        public Color Color;
        public bool BeenDrawn;
        protected float ExpirationTime;
        public uint DrawLayers;

        public void SetDuration(float duration)
        {
            ExpirationTime = Time.time + duration;
            BeenDrawn = false;
        }

        public virtual bool IsExpired()
        {
            return (Time.time > ExpirationTime && BeenDrawn);
        }

        public virtual void Reset()
        {
            BeenDrawn = false;
            Color = default;
            ExpirationTime = 0f;
            DrawLayers = 0;
        }
    }

    public class DrawMeshInstance : DrawInstance
    {
        public Matrix4x4 Transform;

        public override void Reset()
        {
            base.Reset();
            Transform = Matrix4x4.identity;
        }
    }

    public static class DebugMeshes
    {
        // Implement mesh construction methods for various shapes
        public static Mesh Construct(DebugShape shape)
        {
            Mesh mesh = new Mesh();
            switch (shape)
            {
                case DebugShape.Cube:
                    mesh = CreateCubeMesh();
                    break;
                case DebugShape.Sphere:
                    mesh = CreateSphereMesh(16, 16);
                    break;
                // Implement other shapes
                default:
                    Debug.LogError($"DebugShape {shape} not implemented");
                    break;
            }
            return mesh;
        }

        private static Mesh CreateCubeMesh()
        {
            Mesh mesh = new();

            // Define vertices and indices for a cube
            Vector3[] vertices = {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f)
        };

            int[] indices = {
            // Lines
            0,1, 1,2, 2,3, 3,0, // Bottom face
            4,5, 5,6, 6,7, 7,4, // Top face
            0,4, 1,5, 2,6, 3,7  // Sides
        };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateSphereMesh(int longitudeSegments, int latitudeSegments)
        {
            // Implement sphere mesh generation
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            float radius = 0.5f;
            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                float theta = lat * Mathf.PI / latitudeSegments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / longitudeSegments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);

                    Vector3 vertex = new Vector3(
                        cosPhi * sinTheta,
                        cosTheta,
                        sinPhi * sinTheta
                    ) * radius;
                    vertices.Add(vertex);
                }
            }

            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    int first = (lat * (longitudeSegments + 1)) + lon;
                    int second = first + longitudeSegments + 1;

                    // Lines
                    indices.Add(first);
                    indices.Add(second);

                    indices.Add(second);
                    indices.Add(second + 1);

                    indices.Add(second + 1);
                    indices.Add(first + 1);

                    indices.Add(first + 1);
                    indices.Add(first);
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }
    }

    public enum DebugShape
    {
        Cube,
        Sphere,
        // Add other shapes as needed
    }
}