using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Parameters", menuName = "ScriptableObjects/Parameters", order = 1)]
    public class Data_Parameter : ScriptableObject
    {
        [Header("General")]
        [SerializeField, Range(0, 1)] private float _density = 0.5f;
        [SerializeField, Range(0, 1)] private float _ambientAmount = 0.2f;
        [SerializeField, Range(0, 20)] private float _lifeTime = 10;
        [SerializeField, Range(0, 1)] private float _sourceBlurRadius;

        public float Density { get => _density; }
        public float AmbientAmount { get => _ambientAmount; }
        public float LifeTime { get => _lifeTime; }
        public float SourceBlurRadius { get => _sourceBlurRadius; }


        [Header("Visual")]
        [SerializeField] private Texture2D _colorTexture;

        [SerializeField, Range(0, 1)] private float _particleFill;
        [SerializeField, Range(0, 1)] private float _brightnessRandomness = 0.25f;
        [SerializeField, Range(0, 1)] private float _brightnessLag = 0.5f;
        [SerializeField, Range(0, 1)] private float _colorLag = 0.5f;
        [SerializeField, Range(0, 1)] private float _colorOverLife;
        [SerializeField, Range(0, 1)] private float _sourceModulateColor;
        [SerializeField, Range(0, 1)] private float _randomAmbientColor;
        [SerializeField] private bool _useSpawnColor;
        [SerializeField, Range(0, 1)] private float _sourceWeight;
        [Space]
        [SerializeField, Range(0, 1)] private float _depthModulateBrightness;


        public Texture2D ColorTexture { get => _colorTexture; }
        public float ParticleFill { get => _particleFill; }
        public float BrightnessRandomness { get => _brightnessRandomness; }
        public float BrightnessLag { get => _brightnessLag; }
        public float ColorLag { get => _colorLag; }
        public float ColorOverLife { get => _colorOverLife; }
        public float SourceModulateColor { get => _sourceModulateColor; }
        public float RandomAmbientColor { get => _randomAmbientColor; }
        public bool UseSpawnColor { get => _useSpawnColor; }
        public float SourceWeight { get => _sourceWeight; }
        public float DepthModulateBrightness { get => _depthModulateBrightness; }


        [Header("Influencers")]
        [SerializeField, Range(0, 1)] private float _sourceGlow;
        [SerializeField, Range(0, 1)] private float _sourceModulateFill;
        [SerializeField, Range(0, 1)] private float _sourceModulateBrightness;
        [SerializeField, Range(0, 1)] private float _sourceModulateSize;
        [SerializeField, Range(0, 0.005f)] private float _sourceAttraction;
        [Space]
        [SerializeField, Range(0, 1)] private float _depthModulateSize;

        public float SourceGlow { get => _sourceGlow; }
        public float SourceModulateFill { get => _sourceModulateFill; }
        public float SourceModulateBrightness { get => _sourceModulateBrightness; }
        public float SourceModulateSize { get => _sourceModulateSize; }
        public float SourceAttraction { get => _sourceAttraction; }
        public float DepthModulateSize { get => _depthModulateSize; }


        [Header("Size")]
        [SerializeField, Range(0, 0.1f)] private float _size = 0.25f;
        [SerializeField, Range(0, 1)] private float _sizeLag = 0.5f;
        [Space]
        [SerializeField, Range(0, 1)] private float _sizeRandomness;
        [SerializeField, Range(0, 1)] private float _sizeRandomness2;
        [Space]
        [SerializeField, Range(0, 1)] private float _scaleInLifetimePercentage = 0.2f;
        [SerializeField, Range(0, 1)] private float _scaleOutLifetimePercentage = 0.2f;

        public float Size { get => _size; }
        public float SizeLag { get => _sizeLag; }
        public float SizeRandomness { get => _sizeRandomness; }
        public float SizeRandomness2 { get => _sizeRandomness2; }
        public float ScaleInLifetimePercentage { get => _scaleInLifetimePercentage; }
        public float ScaleOutLifetimePercentage { get => _scaleOutLifetimePercentage; }


        [Header("Movement")]
        [SerializeField, Range(0, 1)] private float _inertia;
        [Space]
        [SerializeField] private bool _useMotionVectors = true;
        [SerializeField, Range(0, 1)] private float _motionVectorRandomness;

        public float Inertia { get => _inertia; }
        public bool UseMotionVectors { get => _useMotionVectors; }
        public float MotionVectorRandomness { get => _motionVectorRandomness; }
	}
}
