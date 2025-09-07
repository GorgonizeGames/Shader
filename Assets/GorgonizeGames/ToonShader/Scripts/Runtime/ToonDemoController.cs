using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace GorgonizeGames
{
    public class ToonDemoController : MonoBehaviour
    {
        [Header("Demo Materials")]
        public Material[] demoMaterials;
        public MeshRenderer targetRenderer;
        
        [Header("UI Controls")]
        public Slider outlineWidthSlider;
        public Slider rimPowerSlider;
        public Slider specularSizeSlider;
        public Slider rampSmoothnessSlider;
        public ColorPicker outlineColorPicker;
        public ColorPicker rimColorPicker;
        public Toggle enableOutlineToggle;
        public Toggle enableRimToggle;
        public Toggle enableSpecularToggle;
        public Toggle enableHatchingToggle;
        public Dropdown materialPresetDropdown;
        public Button resetButton;
        
        [Header("Text Displays")]
        public TextMeshProUGUI fpsText;
        public TextMeshProUGUI materialInfoText;
        
        [Header("Demo Settings")]
        public bool autoRotate = true;
        public float rotationSpeed = 30f;
        public bool showPerformanceInfo = true;
        
        private Material currentMaterial;
        private int currentMaterialIndex = 0;
        private float fpsTimer = 0f;
        private int frameCount = 0;
        private float currentFPS = 0f;
        
        // Material property IDs for optimization
        private static readonly int OutlineWidthId = Shader.PropertyToID("_OutlineWidth");
        private static readonly int RimPowerId = Shader.PropertyToID("_RimPower");
        private static readonly int SpecularSizeId = Shader.PropertyToID("_SpecularSize");
        private static readonly int RampSmoothnessId = Shader.PropertyToID("_RampSmoothness");
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int RimColorId = Shader.PropertyToID("_RimColor");
        private static readonly int EnableOutlineId = Shader.PropertyToID("_EnableOutline");
        private static readonly int EnableRimId = Shader.PropertyToID("_EnableRim");
        private static readonly int EnableSpecularId = Shader.PropertyToID("_EnableSpecular");
        private static readonly int EnableHatchingId = Shader.PropertyToID("_EnableHatching");
        
        private void Start()
        {
            InitializeDemo();
            SetupUICallbacks();
            LoadMaterialPreset(0);
        }
        
        private void Update()
        {
            if (autoRotate && targetRenderer != null)
            {
                targetRenderer.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
            
            if (showPerformanceInfo)
            {
                UpdatePerformanceInfo();
            }
            
            HandleKeyboardInput();
        }
        
        private void InitializeDemo()
        {
            if (targetRenderer != null && demoMaterials.Length > 0)
            {
                currentMaterial = Instantiate(demoMaterials[0]);
                targetRenderer.material = currentMaterial;
            }
            
            // Setup material preset dropdown
            if (materialPresetDropdown != null)
            {
                materialPresetDropdown.options.Clear();
                for (int i = 0; i < demoMaterials.Length; i++)
                {
                    materialPresetDropdown.options.Add(new Dropdown.OptionData($"Material {i + 1}"));
                }
                materialPresetDropdown.RefreshShownValue();
            }
        }
        
        private void SetupUICallbacks()
        {
            // Slider callbacks
            if (outlineWidthSlider != null)
                outlineWidthSlider.onValueChanged.AddListener(SetOutlineWidth);
            
            if (rimPowerSlider != null)
                rimPowerSlider.onValueChanged.AddListener(SetRimPower);
            
            if (specularSizeSlider != null)
                specularSizeSlider.onValueChanged.AddListener(SetSpecularSize);
            
            if (rampSmoothnessSlider != null)
                rampSmoothnessSlider.onValueChanged.AddListener(SetRampSmoothness);
            
            // Toggle callbacks
            if (enableOutlineToggle != null)
                enableOutlineToggle.onValueChanged.AddListener(SetEnableOutline);
            
            if (enableRimToggle != null)
                enableRimToggle.onValueChanged.AddListener(SetEnableRim);
            
            if (enableSpecularToggle != null)
                enableSpecularToggle.onValueChanged.AddListener(SetEnableSpecular);
            
            if (enableHatchingToggle != null)
                enableHatchingToggle.onValueChanged.AddListener(SetEnableHatching);
            
            // Color picker callbacks
            if (outlineColorPicker != null)
                outlineColorPicker.onColorChanged.AddListener(SetOutlineColor);
            
            if (rimColorPicker != null)
                rimColorPicker.onColorChanged.AddListener(SetRimColor);
            
            // Dropdown and button callbacks
            if (materialPresetDropdown != null)
                materialPresetDropdown.onValueChanged.AddListener(LoadMaterialPreset);
            
            if (resetButton != null)
                resetButton.onClick.AddListener(ResetToDefaults);
        }
        
        private void HandleKeyboardInput()
        {
            // Material switching with number keys
            for (int i = 1; i <= Mathf.Min(demoMaterials.Length, 9); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    LoadMaterialPreset(i - 1);
                }
            }
            
            // Toggle features with function keys
            if (Input.GetKeyDown(KeyCode.F1))
                ToggleOutline();
            
            if (Input.GetKeyDown(KeyCode.F2))
                ToggleRim();
            
            if (Input.GetKeyDown(KeyCode.F3))
                ToggleSpecular();
            
            if (Input.GetKeyDown(KeyCode.F4))
                ToggleHatching();
            
            // Performance info toggle
            if (Input.GetKeyDown(KeyCode.F12))
                showPerformanceInfo = !showPerformanceInfo;
            
            // Auto rotate toggle
            if (Input.GetKeyDown(KeyCode.R))
                autoRotate = !autoRotate;
        }
        
        private void UpdatePerformanceInfo()
        {
            frameCount++;
            fpsTimer += Time.deltaTime;
            
            if (fpsTimer >= 1f)
            {
                currentFPS = frameCount / fpsTimer;
                frameCount = 0;
                fpsTimer = 0f;
                
                if (fpsText != null)
                {
                    fpsText.text = $"FPS: {currentFPS:F1}";
                    
                    // Color code based on performance
                    if (currentFPS >= 60f)
                        fpsText.color = Color.green;
                    else if (currentFPS >= 30f)
                        fpsText.color = Color.yellow;
                    else
                        fpsText.color = Color.red;
                }
            }
        }
        
        // UI Callback Methods
        public void SetOutlineWidth(float value)
        {
            if (currentMaterial != null)
                currentMaterial.SetFloat(OutlineWidthId, value);
        }
        
        public void SetRimPower(float value)
        {
            if (currentMaterial != null)
                currentMaterial.SetFloat(RimPowerId, value);
        }
        
        public void SetSpecularSize(float value)
        {
            if (currentMaterial != null)
                currentMaterial.SetFloat(SpecularSizeId, value);
        }
        
        public void SetRampSmoothness(float value)
        {
            if (currentMaterial != null)
                currentMaterial.SetFloat(RampSmoothnessId, value);
        }
        
        public void SetOutlineColor(Color color)
        {
            if (currentMaterial != null)
                currentMaterial.SetColor(OutlineColorId, color);
        }
        
        public void SetRimColor(Color color)
        {
            if (currentMaterial != null)
                currentMaterial.SetColor(RimColorId, color);
        }
        
        public void SetEnableOutline(bool enabled)
        {
            if (currentMaterial != null)
            {
                currentMaterial.SetFloat(EnableOutlineId, enabled ? 1f : 0f);
                SetKeyword("ENABLE_OUTLINE", enabled);
            }
        }
        
        public void SetEnableRim(bool enabled)
        {
            if (currentMaterial != null)
            {
                currentMaterial.SetFloat(EnableRimId, enabled ? 1f : 0f);
                SetKeyword("ENABLE_RIM", enabled);
            }
        }
        
        public void SetEnableSpecular(bool enabled)
        {
            if (currentMaterial != null)
            {
                currentMaterial.SetFloat(EnableSpecularId, enabled ? 1f : 0f);
                SetKeyword("ENABLE_SPECULAR", enabled);
            }
        }
        
        public void SetEnableHatching(bool enabled)
        {
            if (currentMaterial != null)
            {
                currentMaterial.SetFloat(EnableHatchingId, enabled ? 1f : 0f);
                SetKeyword("ENABLE_HATCHING", enabled);
            }
        }
        
        public void LoadMaterialPreset(int index)
        {
            if (index < 0 || index >= demoMaterials.Length)
                return;
            
            currentMaterialIndex = index;
            
            if (currentMaterial != null)
                DestroyImmediate(currentMaterial);
            
            currentMaterial = Instantiate(demoMaterials[index]);
            
            if (targetRenderer != null)
                targetRenderer.material = currentMaterial;
            
            UpdateUIFromMaterial();
            UpdateMaterialInfo();
            
            if (materialPresetDropdown != null)
                materialPresetDropdown.value = index;
        }
        
        public void ResetToDefaults()
        {
            LoadMaterialPreset(currentMaterialIndex);
        }
        
        // Toggle methods for keyboard shortcuts
        public void ToggleOutline()
        {
            if (enableOutlineToggle != null)
            {
                enableOutlineToggle.isOn = !enableOutlineToggle.isOn;
            }
        }
        
        public void ToggleRim()
        {
            if (enableRimToggle != null)
            {
                enableRimToggle.isOn = !enableRimToggle.isOn;
            }
        }
        
        public void ToggleSpecular()
        {
            if (enableSpecularToggle != null)
            {
                enableSpecularToggle.isOn = !enableSpecularToggle.isOn;
            }
        }
        
        public void ToggleHatching()
        {
            if (enableHatchingToggle != null)
            {
                enableHatchingToggle.isOn = !enableHatchingToggle.isOn;
            }
        }
        
        private void UpdateUIFromMaterial()
        {
            if (currentMaterial == null)
                return;
            
            // Update sliders
            if (outlineWidthSlider != null)
                outlineWidthSlider.value = currentMaterial.GetFloat(OutlineWidthId);
            
            if (rimPowerSlider != null)
                rimPowerSlider.value = currentMaterial.GetFloat(RimPowerId);
            
            if (specularSizeSlider != null)
                specularSizeSlider.value = currentMaterial.GetFloat(SpecularSizeId);
            
            if (rampSmoothnessSlider != null)
                rampSmoothnessSlider.value = currentMaterial.GetFloat(RampSmoothnessId);
            
            // Update toggles
            if (enableOutlineToggle != null)
                enableOutlineToggle.isOn = currentMaterial.GetFloat(EnableOutlineId) > 0.5f;
            
            if (enableRimToggle != null)
                enableRimToggle.isOn = currentMaterial.GetFloat(EnableRimId) > 0.5f;
            
            if (enableSpecularToggle != null)
                enableSpecularToggle.isOn = currentMaterial.GetFloat(EnableSpecularId) > 0.5f;
            
            if (enableHatchingToggle != null)
                enableHatchingToggle.isOn = currentMaterial.GetFloat(EnableHatchingId) > 0.5f;
            
            // Update color pickers
            if (outlineColorPicker != null)
                outlineColorPicker.CurrentColor = currentMaterial.GetColor(OutlineColorId);
            
            if (rimColorPicker != null)
                rimColorPicker.CurrentColor = currentMaterial.GetColor(RimColorId);
        }
        
        private void UpdateMaterialInfo()
        {
            if (materialInfoText != null && currentMaterial != null)
            {
                string info = $"Material: {currentMaterial.name}\n";
                info += $"Shader: {currentMaterial.shader.name}\n";
                
                // Count enabled features
                int enabledFeatures = 0;
                if (currentMaterial.GetFloat(EnableOutlineId) > 0.5f) enabledFeatures++;
                if (currentMaterial.GetFloat(EnableRimId) > 0.5f) enabledFeatures++;
                if (currentMaterial.GetFloat(EnableSpecularId) > 0.5f) enabledFeatures++;
                if (currentMaterial.GetFloat(EnableHatchingId) > 0.5f) enabledFeatures++;
                
                info += $"Enabled Features: {enabledFeatures}\n";
                info += $"Keywords: {currentMaterial.enabledKeywords.Length}";
                
                materialInfoText.text = info;
            }
        }
        
        private void SetKeyword(string keyword, bool enabled)
        {
            if (currentMaterial == null)
                return;
            
            if (enabled)
                currentMaterial.EnableKeyword(keyword);
            else
                currentMaterial.DisableKeyword(keyword);
        }
        
        private void OnGUI()
        {
            if (!showPerformanceInfo)
                return;
            
            // Draw keyboard shortcuts help
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("Keyboard Shortcuts:", EditorGUIUtility.isProSkin ? GUI.skin.box : GUI.skin.label);
            GUILayout.Label("1-9: Switch Materials");
            GUILayout.Label("F1: Toggle Outline");
            GUILayout.Label("F2: Toggle Rim Lighting");
            GUILayout.Label("F3: Toggle Specular");
            GUILayout.Label("F4: Toggle Hatching");
            GUILayout.Label("R: Toggle Auto Rotate");
            GUILayout.Label("F12: Toggle Performance Info");
            GUILayout.EndArea();
        }
    }
    
    // Simple color picker interface
    public interface ColorPicker
    {
        Color CurrentColor { get; set; }
        System.Action<Color> onColorChanged { get; set; }
    }
}