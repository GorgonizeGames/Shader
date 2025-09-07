using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Gorgonize.ToonShader.Utilities
{
    /// <summary>
    /// Performance monitoring and optimization system for Ultimate Toon Shader
    /// Tracks frame rates, memory usage, and automatically adjusts quality settings
    /// </summary>
    public class ToonPerformanceManager : MonoBehaviour
    {
        [Header("Performance Monitoring")]
        [Tooltip("Enable automatic performance monitoring")]
        public bool enableMonitoring = true;
        
        [Tooltip("Target frame rate for quality adjustments")]
        public int targetFrameRate = 60;
        
        [Tooltip("Minimum frame rate before quality reduction")]
        public int minimumFrameRate = 30;
        
        [Tooltip("Time between performance checks (seconds)")]
        public float checkInterval = 2f;
        
        [Tooltip("Number of frames to average for performance calculation")]
        public int frameAverageCount = 30;
        
        [Header("Auto-Optimization")]
        [Tooltip("Automatically reduce quality when performance drops")]
        public bool enableAutoOptimization = true;
        
        [Tooltip("Delay before auto-optimization kicks in (seconds)")]
        public float optimizationDelay = 5f;
        
        [Tooltip("Aggressiveness of optimization (0 = conservative, 1 = aggressive)")]
        [Range(0f, 1f)]
        public float optimizationAggressiveness = 0.5f;
        
        [Header("Quality Thresholds")]
        [Tooltip("Frame rate threshold for switching to Ultra quality")]
        public int ultraQualityThreshold = 80;
        
        [Tooltip("Frame rate threshold for switching to High quality")]
        public int highQualityThreshold = 60;
        
        [Tooltip("Frame rate threshold for switching to Medium quality")]
        public int mediumQualityThreshold = 45;
        
        [Tooltip("Frame rate threshold for switching to Low quality")]
        public int lowQualityThreshold = 30;
        
        // Performance tracking
        private Queue<float> frameTimeHistory = new Queue<float>();
        private float averageFrameTime;
        private float currentFrameRate;
        private float performanceTrend;
        private float lastOptimizationTime;
        
        // Quality management
        private int currentGlobalQuality = 2; // Start with High quality
        private Dictionary<ToonMaterialController, int> controllerQualities = new Dictionary<ToonMaterialController, int>();
        private List<ToonMaterialController> managedControllers = new List<ToonMaterialController>();
        
        // Performance statistics
        private float totalRenderTime;
        private int droppedFrames;
        private float maxFrameTime;
        private float minFrameTime = float.MaxValue;
        
        // Events
        public System.Action<int> OnQualityChanged;
        public System.Action<float> OnPerformanceUpdated;
        public System.Action<PerformanceAlert> OnPerformanceAlert;
        
        public enum PerformanceAlert
        {
            None,
            LowFrameRate,
            HighMemoryUsage,
            QualityReduced,
            QualityIncreased
        }
        
        #region Singleton Pattern
        private static ToonPerformanceManager _instance;
        public static ToonPerformanceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ToonPerformanceManager");
                    _instance = go.AddComponent<ToonPerformanceManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        #endregion
        
        #region Unity Lifecycle
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializePerformanceMonitoring();
            FindAndRegisterControllers();
            
            if (enableMonitoring)
            {
                StartCoroutine(PerformanceMonitoringCoroutine());
            }
        }
        
        private void Update()
        {
            if (enableMonitoring)
            {
                UpdateFrameTimeHistory();
                CalculatePerformanceMetrics();
            }
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        #endregion
        
        #region Performance Monitoring
        private void InitializePerformanceMonitoring()
        {
            Application.targetFrameRate = targetFrameRate;
            frameTimeHistory.Clear();
            
            // Initialize quality settings based on platform
            DetermineInitialQuality();
        }
        
        private void DetermineInitialQuality()
        {
            // Base quality on platform and hardware capabilities
            if (Application.isMobilePlatform)
            {
                currentGlobalQuality = 1; // Medium quality for mobile
            }
            else if (SystemInfo.graphicsMemorySize < 2048) // Less than 2GB VRAM
            {
                currentGlobalQuality = 1; // Medium quality
            }
            else if (SystemInfo.graphicsMemorySize >= 4096) // 4GB+ VRAM
            {
                currentGlobalQuality = 3; // Ultra quality
            }
            else
            {
                currentGlobalQuality = 2; // High quality
            }
        }
        
        private void UpdateFrameTimeHistory()
        {
            float currentFrameTime = Time.unscaledDeltaTime;
            frameTimeHistory.Enqueue(currentFrameTime);
            
            if (frameTimeHistory.Count > frameAverageCount)
            {
                frameTimeHistory.Dequeue();
            }
            
            // Update min/max frame times
            if (currentFrameTime > maxFrameTime)
                maxFrameTime = currentFrameTime;
            
            if (currentFrameTime < minFrameTime)
                minFrameTime = currentFrameTime;
        }
        
        private void CalculatePerformanceMetrics()
        {
            if (frameTimeHistory.Count == 0) return;
            
            // Calculate average frame time
            float total = 0f;
            foreach (float frameTime in frameTimeHistory)
            {
                total += frameTime;
            }
            averageFrameTime = total / frameTimeHistory.Count;
            
            // Calculate current frame rate
            currentFrameRate = 1f / averageFrameTime;
            
            // Calculate performance trend
            if (frameTimeHistory.Count >= frameAverageCount)
            {
                float recentTotal = 0f;
                float olderTotal = 0f;
                int halfCount = frameAverageCount / 2;
                
                var array = frameTimeHistory.ToArray();
                
                for (int i = 0; i < halfCount; i++)
                {
                    olderTotal += array[i];
                }
                
                for (int i = halfCount; i < frameAverageCount; i++)
                {
                    recentTotal += array[i];
                }
                
                float recentAverage = recentTotal / halfCount;
                float olderAverage = olderTotal / halfCount;
                
                performanceTrend = (olderAverage - recentAverage) / olderAverage;
            }
            
            // Count dropped frames
            if (currentFrameRate < targetFrameRate * 0.9f)
            {
                droppedFrames++;
            }
            
            OnPerformanceUpdated?.Invoke(currentFrameRate);
        }
        
        private IEnumerator PerformanceMonitoringCoroutine()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(checkInterval);
                
                if (enableAutoOptimization)
                {
                    EvaluateAndOptimizePerformance();
                }
                
                CheckForPerformanceAlerts();
            }
        }
        
        private void EvaluateAndOptimizePerformance()
        {
            if (Time.time - lastOptimizationTime < optimizationDelay)
                return;
            
            int recommendedQuality = CalculateRecommendedQuality();
            
            if (recommendedQuality != currentGlobalQuality)
            {
                SetGlobalQuality(recommendedQuality);
                lastOptimizationTime = Time.time;
                
                PerformanceAlert alert = recommendedQuality < currentGlobalQuality ? 
                    PerformanceAlert.QualityReduced : PerformanceAlert.QualityIncreased;
                OnPerformanceAlert?.Invoke(alert);
            }
        }
        
        private int CalculateRecommendedQuality()
        {
            float adjustedFrameRate = currentFrameRate;
            
            // Factor in optimization aggressiveness
            float threshold = Mathf.Lerp(0.8f, 1.2f, optimizationAggressiveness);
            
            if (adjustedFrameRate >= ultraQualityThreshold * threshold)
                return 3; // Ultra
            else if (adjustedFrameRate >= highQualityThreshold * threshold)
                return 2; // High
            else if (adjustedFrameRate >= mediumQualityThreshold * threshold)
                return 1; // Medium
            else
                return 0; // Low
        }
        
        private void CheckForPerformanceAlerts()
        {
            if (currentFrameRate < minimumFrameRate)
            {
                OnPerformanceAlert?.Invoke(PerformanceAlert.LowFrameRate);
            }
            
            // Check memory usage
            if (SystemInfo.systemMemorySize > 0)
            {
                float memoryUsageRatio = (float)System.GC.GetTotalMemory(false) / (SystemInfo.systemMemorySize * 1024 * 1024);
                if (memoryUsageRatio > 0.8f) // 80% memory usage
                {
                    OnPerformanceAlert?.Invoke(PerformanceAlert.HighMemoryUsage);
                }
            }
        }
        #endregion
        
        // Note: Add the missing reference to the ToonMaterialController
        // This should be defined elsewhere in your project
        
        public void FindAndRegisterControllers()
        {
            // Implementation would go here when ToonMaterialController is available
        }
        
        public void SetGlobalQuality(int qualityLevel)
        {
            qualityLevel = Mathf.Clamp(qualityLevel, 0, 3);
            currentGlobalQuality = qualityLevel;
            OnQualityChanged?.Invoke(qualityLevel);
        }
        
        public float GetCurrentFrameRate()
        {
            return currentFrameRate;
        }
        
        public int GetCurrentQuality()
        {
            return currentGlobalQuality;
        }
    }
}