using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Gorgonize.ToonShader.Presets;
using Gorgonize.ToonShader.Settings;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Editor window for creating, editing, and managing toon shader presets
    /// </summary>
    public class ToonPresetEditor : EditorWindow
    {
        private Vector2 scrollPosition;
        private ToonShaderPreset currentPreset;
        private ToonShaderPreset[] builtInPresets;
        private List<ToonShaderPreset> customPresets = new List<ToonShaderPreset>();
        
        private int selectedPresetIndex = -1;
        private bool showBuiltInPresets = true;
        private bool showCustomPresets = true;
        private bool showPresetDetails = true;
        
        private string newPresetName = "New Preset";
        private string presetDescription = "";
        private Texture2D presetPreviewImage;
        
        private Material previewMaterial;
        private ToonMaterialController selectedController;
        
        // Search and filter
        private string searchFilter = "";
        private int performanceFilter = 0; // 0=All, 1=Light, 2=Medium, 3=Heavy, 4=Ultra
        
        private static readonly string CUSTOM_PRESETS_PATH = "Assets/UltimateToonShaderPro/Presets/Custom/";
        
        [MenuItem("Window/Ultimate Toon Shader/Preset Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<ToonPresetEditor>("Toon Preset Editor");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        private void OnEnable()
        {
            LoadPresets();
            FindSelectedController();
        }
        
        private void OnGUI()
        {
            ToonGUIStyles.RefreshStyles();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            DrawHeader();
            ToonGUIStyles.DrawSeparator();
            
            DrawToolbar();
            ToonGUIStyles.DrawSeparator();
            
            DrawPresetList();
            ToonGUIStyles.DrawSeparator();
            
            DrawPresetDetails();
            ToonGUIStyles.DrawSeparator();
            
            DrawActions();
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Ultimate Toon Shader - Preset Editor", ToonGUIStyles.HeaderStyle);
            EditorGUILayout.LabelField("Create, edit, and manage shader presets", ToonGUIStyles.VersionLabelStyle);
            
            ToonGUIStyles.AddVerticalSpace(10f);
            
            // Quick stats
            ToonGUIStyles.BeginStyledBox();
            ToonGUIStyles.BeginHorizontalLayout();
            
            int builtInCount = builtInPresets?.Length ?? 0;
            int customCount = customPresets?.Count ?? 0;
            
            EditorGUILayout.LabelField($"Built-in Presets: {builtInCount}", ToonGUIStyles.SmallLabelStyle);
            EditorGUILayout.LabelField($"Custom Presets: {customCount}", ToonGUIStyles.SmallLabelStyle);
            EditorGUILayout.LabelField($"Total: {builtInCount + customCount}", ToonGUIStyles.SmallLabelStyle);
            
            ToonGUIStyles.EndHorizontalLayout();
            ToonGUIStyles.EndStyledBox();
        }
        
        private void DrawToolbar()
        {
            ToonGUIStyles.BeginStyledBox();
            
            // Search and filter section
            ToonGUIStyles.BeginHorizontalLayout();
            
            EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
            searchFilter = EditorGUILayout.TextField(searchFilter, GUILayout.ExpandWidth(true));
            
            EditorGUILayout.LabelField("Performance:", GUILayout.Width(80));
            string[] performanceOptions = { "All", "Light", "Medium", "Heavy", "Ultra" };
            performanceFilter = EditorGUILayout.Popup(performanceFilter, performanceOptions, GUILayout.Width(80));
            
            ToonGUIStyles.EndHorizontalLayout();
            
            ToonGUIStyles.AddVerticalSpace(5f);
            
            // View options
            ToonGUIStyles.BeginHorizontalLayout();
            
            showBuiltInPresets = ToonGUIStyles.DrawToggleButton(showBuiltInPresets, "Built-in", 80f);
            showCustomPresets = ToonGUIStyles.DrawToggleButton(showCustomPresets, "Custom", 80f);
            showPresetDetails = ToonGUIStyles.DrawToggleButton(showPresetDetails, "Details", 80f);
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                LoadPresets();
            }
            
            if (GUILayout.Button("Create New", GUILayout.Width(80)))
            {
                CreateNewPreset();
            }
            
            ToonGUIStyles.EndHorizontalLayout();
            ToonGUIStyles.EndStyledBox();
        }
        
        private void DrawPresetList()
        {
            ToonGUIStyles.BeginStyledBox();
            EditorGUILayout.LabelField("Available Presets", ToonGUIStyles.BoldLabelStyle);
            
            // Built-in presets
            if (showBuiltInPresets && builtInPresets != null)
            {
                EditorGUILayout.LabelField("Built-in Presets", ToonGUIStyles.SubHeaderStyle);
                
                for (int i = 0; i < builtInPresets.Length; i++)
                {
                    var preset = builtInPresets[i];
                    if (ShouldShowPreset(preset))
                    {
                        DrawPresetItem(preset, i, true);
                    }
                }
            }
            
            // Custom presets
            if (showCustomPresets && customPresets != null)
            {
                EditorGUILayout.LabelField("Custom Presets", ToonGUIStyles.SubHeaderStyle);
                
                for (int i = 0; i < customPresets.Count; i++)
                {
                    var preset = customPresets[i];
                    if (ShouldShowPreset(preset))
                    {
                        DrawPresetItem(preset, i + (builtInPresets?.Length ?? 0), false);
                    }
                }
            }
            
            ToonGUIStyles.EndStyledBox();
        }
        
        private void DrawPresetItem(ToonShaderPreset preset, int index, bool isBuiltIn)
        {
            var isSelected = selectedPresetIndex == index;
            var originalColor = GUI.backgroundColor;
            
            if (isSelected)
                GUI.backgroundColor = ToonGUIStyles.AccentColor;
            
            ToonGUIStyles.BeginHorizontalLayout();
            
            // Preview thumbnail
            if (preset.previewImage != null)
            {
                ToonGUIStyles.DrawTexturePreview(preset.previewImage, 32f);
            }
            else
            {
                GUILayout.Space(32f);
            }
            
            // Preset info
            EditorGUILayout.BeginVertical();
            
            if (GUILayout.Button(preset.presetName, ToonGUIStyles.BoldLabelStyle))
            {
                selectedPresetIndex = index;
                currentPreset = preset;
            }
            
            EditorGUILayout.LabelField(preset.description, ToonGUIStyles.SmallLabelStyle);
            
            // Performance indicator
            float performanceCost = ToonPresetManager.EstimatePresetPerformance(preset);
            string performanceCategory = ToonPresetManager.GetPerformanceCategory(preset);
            EditorGUILayout.LabelField($"Performance: {performanceCategory} ({performanceCost:P0})", ToonGUIStyles.SmallLabelStyle);
            
            EditorGUILayout.EndVertical();
            
            // Action buttons
            EditorGUILayout.BeginVertical(GUILayout.Width(80));
            
            if (GUILayout.Button("Apply", EditorStyles.miniButton))
            {
                ApplyPresetToSelectedController(preset);
            }
            
            if (!isBuiltIn)
            {
                if (GUILayout.Button("Edit", EditorStyles.miniButton))
                {
                    EditCustomPreset(preset);
                }
                
                if (GUILayout.Button("Delete", EditorStyles.miniButton))
                {
                    DeleteCustomPreset(preset);
                }
            }
            else
            {
                if (GUILayout.Button("Duplicate", EditorStyles.miniButton))
                {
                    DuplicatePreset(preset);
                }
            }
            
            EditorGUILayout.EndVertical();
            
            ToonGUIStyles.EndHorizontalLayout();
            GUI.backgroundColor = originalColor;
            
            ToonGUIStyles.AddVerticalSpace(2f);
        }
        
        private void DrawPresetDetails()
        {
            if (!showPresetDetails || currentPreset == null)
                return;
            
            ToonGUIStyles.BeginStyledBox();
            EditorGUILayout.LabelField("Preset Details", ToonGUIStyles.BoldLabelStyle);
            
            // Basic info
            EditorGUILayout.LabelField("Name:", currentPreset.presetName, ToonGUIStyles.BoldLabelStyle);
            EditorGUILayout.LabelField("Description:", currentPreset.description);
            
            ToonGUIStyles.AddVerticalSpace(5f);
            
            // Performance info
            float performanceCost = ToonPresetManager.EstimatePresetPerformance(currentPreset);
            ToonGUIStyles.DrawPerformanceBar(performanceCost, "Performance Cost");
            
            ToonGUIStyles.AddVerticalSpace(5f);
            
            // Feature breakdown
            EditorGUILayout.LabelField("Active Features:", ToonGUIStyles.BoldLabelStyle);
            
            if (currentPreset.visualEffects != null)
            {
                int activeEffects = currentPreset.visualEffects.GetActiveEffectCount();
                EditorGUILayout.LabelField($"Visual Effects: {activeEffects}", ToonGUIStyles.SmallLabelStyle);
            }
            
            if (currentPreset.stylization != null)
            {
                int activeStylization = currentPreset.stylization.GetActiveEffectCount();
                EditorGUILayout.LabelField($"Stylization Effects: {activeStylization}", ToonGUIStyles.SmallLabelStyle);
            }
            
            if (currentPreset.animationSettings != null)
            {
                int activeAnimations = currentPreset.animationSettings.GetActiveAnimationCount();
                EditorGUILayout.LabelField($"Active Animations: {activeAnimations}", ToonGUIStyles.SmallLabelStyle);
            }
            
            ToonGUIStyles.EndStyledBox();
        }
        
        private void DrawActions()
        {
            ToonGUIStyles.BeginStyledBox();
            EditorGUILayout.LabelField("Actions", ToonGUIStyles.BoldLabelStyle);
            
            // Selected controller info
            if (selectedController != null)
            {
                EditorGUILayout.LabelField($"Selected Controller: {selectedController.name}", ToonGUIStyles.SmallLabelStyle);
            }
            else
            {
                ToonGUIStyles.DrawInfoBox("No ToonMaterialController selected. Select one in the scene to apply presets.", MessageType.Warning);
            }
            
            ToonGUIStyles.AddVerticalSpace(5f);
            
            // Create preset from current controller
            ToonGUIStyles.BeginHorizontalLayout();
            
            EditorGUILayout.LabelField("New Preset Name:", GUILayout.Width(120));
            newPresetName = EditorGUILayout.TextField(newPresetName);
            
            if (GUILayout.Button("Create from Current", GUILayout.Width(120)))
            {
                CreatePresetFromController();
            }
            
            ToonGUIStyles.EndHorizontalLayout();
            
            // Description for new preset
            EditorGUILayout.LabelField("Description:");
            presetDescription = EditorGUILayout.TextArea(presetDescription, GUILayout.Height(40));
            
            ToonGUIStyles.AddVerticalSpace(5f);
            
            // Import/Export section
            EditorGUILayout.LabelField("Import/Export", ToonGUIStyles.BoldLabelStyle);
            
            ToonGUIStyles.BeginHorizontalLayout();
            
            if (GUILayout.Button("Export Selected"))
            {
                ExportPreset(currentPreset);
            }
            
            if (GUILayout.Button("Import Preset"))
            {
                ImportPreset();
            }
            
            if (GUILayout.Button("Export All Custom"))
            {
                ExportAllCustomPresets();
            }
            
            ToonGUIStyles.EndHorizontalLayout();
            
            ToonGUIStyles.EndStyledBox();
        }
        
        #region Preset Operations
        
        private void LoadPresets()
        {
            // Load built-in presets
            builtInPresets = ToonPresetManager.GetBuiltInPresets();
            
            // Load custom presets
            LoadCustomPresets();
        }
        
        private void LoadCustomPresets()
        {
            customPresets.Clear();
            
            if (!Directory.Exists(CUSTOM_PRESETS_PATH))
            {
                Directory.CreateDirectory(CUSTOM_PRESETS_PATH);
                return;
            }
            
            string[] presetFiles = Directory.GetFiles(CUSTOM_PRESETS_PATH, "*.asset");
            
            foreach (string file in presetFiles)
            {
                var preset = AssetDatabase.LoadAssetAtPath<ToonShaderPreset>(file);
                if (preset != null)
                {
                    customPresets.Add(preset);
                }
            }
        }
        
        private void CreateNewPreset()
        {
            currentPreset = ScriptableObject.CreateInstance<ToonShaderPreset>();
            currentPreset.presetName = "New Custom Preset";
            currentPreset.description = "Custom shader configuration";
            selectedPresetIndex = -1;
        }
        
        private void CreatePresetFromController()
        {
            if (selectedController == null)
            {
                EditorUtility.DisplayDialog("Error", "No ToonMaterialController selected!", "OK");
                return;
            }
            
            if (string.IsNullOrEmpty(newPresetName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a name for the preset!", "OK");
                return;
            }
            
            var preset = ToonPresetManager.CreatePresetFromController(selectedController, newPresetName);
            preset.description = presetDescription;
            
            SaveCustomPreset(preset);
            LoadPresets();
            
            newPresetName = "New Preset";
            presetDescription = "";
        }
        
        private void SaveCustomPreset(ToonShaderPreset preset)
        {
            if (!Directory.Exists(CUSTOM_PRESETS_PATH))
            {
                Directory.CreateDirectory(CUSTOM_PRESETS_PATH);
            }
            
            string fileName = preset.presetName.Replace(" ", "_").Replace("/", "_") + ".asset";
            string path = Path.Combine(CUSTOM_PRESETS_PATH, fileName);
            
            AssetDatabase.CreateAsset(preset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private void EditCustomPreset(ToonShaderPreset preset)
        {
            // Open preset for editing
            currentPreset = preset;
            selectedPresetIndex = -1;
            
            // You could open a detailed editor window here
            ToonPresetDetailEditor.ShowWindow(preset);
        }
        
        private void DeleteCustomPreset(ToonShaderPreset preset)
        {
            if (EditorUtility.DisplayDialog("Delete Preset", 
                $"Are you sure you want to delete '{preset.presetName}'?", "Delete", "Cancel"))
            {
                string path = AssetDatabase.GetAssetPath(preset);
                AssetDatabase.DeleteAsset(path);
                LoadPresets();
            }
        }
        
        private void DuplicatePreset(ToonShaderPreset preset)
        {
            var duplicate = ScriptableObject.CreateInstance<ToonShaderPreset>();
            duplicate.presetName = preset.presetName + " (Copy)";
            duplicate.description = preset.description;
            duplicate.baseColor = preset.baseColor;
            
            // Copy settings
            if (preset.lightingSettings != null)
            {
                duplicate.lightingSettings = new ToonLightingSettings();
                duplicate.lightingSettings.CopyFrom(preset.lightingSettings);
            }
            
            if (preset.visualEffects != null)
            {
                duplicate.visualEffects = new ToonVisualEffects();
                duplicate.visualEffects.CopyFrom(preset.visualEffects);
            }
            
            if (preset.stylization != null)
            {
                duplicate.stylization = new ToonStylization();
                // Add copy method to stylization if needed
            }
            
            if (preset.animationSettings != null)
            {
                duplicate.animationSettings = new ToonAnimationSettings();
                duplicate.animationSettings.CopyFrom(preset.animationSettings);
            }
            
            SaveCustomPreset(duplicate);
            LoadPresets();
        }
        
        private void ApplyPresetToSelectedController(ToonShaderPreset preset)
        {
            if (selectedController == null)
            {
                EditorUtility.DisplayDialog("Error", "No ToonMaterialController selected!", "OK");
                return;
            }
            
            ToonPresetManager.ApplyPreset(selectedController, preset);
            EditorUtility.SetDirty(selectedController);
        }
        
        private void FindSelectedController()
        {
            var selection = Selection.activeGameObject;
            if (selection != null)
            {
                selectedController = selection.GetComponent<ToonMaterialController>();
            }
        }
        
        private bool ShouldShowPreset(ToonShaderPreset preset)
        {
            if (preset == null) return false;
            
            // Search filter
            if (!string.IsNullOrEmpty(searchFilter))
            {
                bool nameMatch = preset.presetName.ToLower().Contains(searchFilter.ToLower());
                bool descMatch = preset.description.ToLower().Contains(searchFilter.ToLower());
                if (!nameMatch && !descMatch) return false;
            }
            
            // Performance filter
            if (performanceFilter > 0)
            {
                string category = ToonPresetManager.GetPerformanceCategory(preset);
                string[] categories = { "", "Light", "Medium", "Heavy", "Ultra" };
                if (category != categories[performanceFilter]) return false;
            }
            
            return true;
        }
        
        #endregion
        
        #region Import/Export
        
        private void ExportPreset(ToonShaderPreset preset)
        {
            if (preset == null) return;
            
            string path = EditorUtility.SaveFilePanel("Export Preset", "", preset.presetName + ".json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                string json = JsonUtility.ToJson(preset, true);
                File.WriteAllText(path, json);
                EditorUtility.DisplayDialog("Export Complete", $"Preset exported to {path}", "OK");
            }
        }
        
        private void ImportPreset()
        {
            string path = EditorUtility.OpenFilePanel("Import Preset", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    var preset = ScriptableObject.CreateInstance<ToonShaderPreset>();
                    JsonUtility.FromJsonOverwrite(json, preset);
                    
                    SaveCustomPreset(preset);
                    LoadPresets();
                    
                    EditorUtility.DisplayDialog("Import Complete", "Preset imported successfully!", "OK");
                }
                catch (System.Exception e)
                {
                    EditorUtility.DisplayDialog("Import Error", $"Failed to import preset: {e.Message}", "OK");
                }
            }
        }
        
        private void ExportAllCustomPresets()
        {
            string folder = EditorUtility.SaveFolderPanel("Export All Custom Presets", "", "");
            if (!string.IsNullOrEmpty(folder))
            {
                int exported = 0;
                foreach (var preset in customPresets)
                {
                    string json = JsonUtility.ToJson(preset, true);
                    string filePath = Path.Combine(folder, preset.presetName + ".json");
                    File.WriteAllText(filePath, json);
                    exported++;
                }
                
                EditorUtility.DisplayDialog("Export Complete", $"Exported {exported} custom presets to {folder}", "OK");
            }
        }
        
        #endregion
        
        private void OnSelectionChange()
        {
            FindSelectedController();
            Repaint();
        }
    }
    
    /// <summary>
    /// Detailed preset editor window for advanced editing
    /// </summary>
    public class ToonPresetDetailEditor : EditorWindow
    {
        private ToonShaderPreset preset;
        private Vector2 scrollPosition;
        
        public static void ShowWindow(ToonShaderPreset preset)
        {
            var window = GetWindow<ToonPresetDetailEditor>("Preset Details");
            window.preset = preset;
            window.minSize = new Vector2(300, 400);
            window.Show();
        }
        
        private void OnGUI()
        {
            if (preset == null)
            {
                EditorGUILayout.HelpBox("No preset selected.", MessageType.Warning);
                return;
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            EditorGUILayout.LabelField("Preset Details", ToonGUIStyles.HeaderStyle);
            ToonGUIStyles.DrawSeparator();
            
            // Basic properties
            preset.presetName = EditorGUILayout.TextField("Preset Name", preset.presetName);
            preset.description = EditorGUILayout.TextArea(preset.description, GUILayout.Height(60));
            preset.previewImage = (Texture2D)EditorGUILayout.ObjectField("Preview Image", preset.previewImage, typeof(Texture2D), false);
            
            ToonGUIStyles.DrawSeparator();
            
            // Color
            preset.baseColor = EditorGUILayout.ColorField("Base Color", preset.baseColor);
            
            // Here you would add detailed editors for each settings category
            // This is a simplified version - in practice you'd want full property editors
            
            ToonGUIStyles.DrawSeparator();
            
            if (GUILayout.Button("Save Changes"))
            {
                EditorUtility.SetDirty(preset);
                AssetDatabase.SaveAssets();
                Close();
            }
            
            EditorGUILayout.EndScrollView();
        }
    }
}