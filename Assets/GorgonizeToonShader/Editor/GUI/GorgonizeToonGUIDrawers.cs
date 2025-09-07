using UnityEngine;
using UnityEditor;
using Gorgonize.ToonShader.Settings;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Custom property drawer for ToonLightingSettings
    /// </summary>
    [CustomPropertyDrawer(typeof(ToonLightingSettings))]
    public class ToonLightingSettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var foldout = property.isExpanded;
            var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            
            property.isExpanded = EditorGUI.Foldout(foldoutRect, foldout, "💡 Lighting Settings", true);
            
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                var yOffset = EditorGUIUtility.singleLineHeight + 2;
                
                // Shadow settings
                var shadowThreshold = property.FindPropertyRelative("shadowThreshold");
                var shadowSmoothness = property.FindPropertyRelative("shadowSmoothness");
                var shadowColor = property.FindPropertyRelative("shadowColor");
                var shadowIntensity = property.FindPropertyRelative("shadowIntensity");
                
                EditorGUI.LabelField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                   "Shadow Configuration", EditorStyles.boldLabel);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      shadowThreshold);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      shadowSmoothness);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      shadowColor);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      shadowIntensity);
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return (EditorGUIUtility.singleLineHeight + 2) * 6; // Header + 5 properties
            }
            return EditorGUIUtility.singleLineHeight;
        }
    }

    /// <summary>
    /// Custom property drawer for ToonRimLighting
    /// </summary>
    [CustomPropertyDrawer(typeof(ToonRimLighting))]
    public class ToonRimLightingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var enableRimLighting = property.FindPropertyRelative("enableRimLighting");
            var toggleRect = new Rect(position.x, position.y, 20, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(position.x + 25, position.y, position.width - 25, EditorGUIUtility.singleLineHeight);
            
            enableRimLighting.boolValue = EditorGUI.Toggle(toggleRect, enableRimLighting.boolValue);
            EditorGUI.LabelField(labelRect, "🌟 Rim Lighting", ToonGUIStyles.BoldLabelStyle);
            
            if (enableRimLighting.boolValue)
            {
                EditorGUI.indentLevel++;
                var yOffset = EditorGUIUtility.singleLineHeight + 2;
                
                var rimColor = property.FindPropertyRelative("rimColor");
                var rimPower = property.FindPropertyRelative("rimPower");
                var rimIntensity = property.FindPropertyRelative("rimIntensity");
                var rimThreshold = property.FindPropertyRelative("rimThreshold");
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      rimColor);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      rimPower);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      rimIntensity);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      rimThreshold);
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enableRimLighting = property.FindPropertyRelative("enableRimLighting");
            if (enableRimLighting.boolValue)
            {
                return (EditorGUIUtility.singleLineHeight + 2) * 5; // Toggle + 4 properties
            }
            return EditorGUIUtility.singleLineHeight;
        }
    }

    /// <summary>
    /// Custom property drawer for ToonHatching
    /// </summary>
    [CustomPropertyDrawer(typeof(ToonHatching))]
    public class ToonHatchingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var enableHatching = property.FindPropertyRelative("enableHatching");
            var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            
            // Custom foldout with toggle
            var toggleRect = new Rect(position.x, position.y, 20, EditorGUIUtility.singleLineHeight);
            var foldoutToggleRect = new Rect(position.x + 20, position.y, 20, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(position.x + 40, position.y, position.width - 40, EditorGUIUtility.singleLineHeight);
            
            enableHatching.boolValue = EditorGUI.Toggle(toggleRect, enableHatching.boolValue);
            property.isExpanded = EditorGUI.Foldout(foldoutToggleRect, property.isExpanded, "");
            EditorGUI.LabelField(labelRect, "🖊️ Hatching Effects", ToonGUIStyles.BoldLabelStyle);
            
            if (property.isExpanded && enableHatching.boolValue)
            {
                EditorGUI.indentLevel++;
                var yOffset = EditorGUIUtility.singleLineHeight + 2;
                
                // Basic hatching properties
                var hatchingTexture = property.FindPropertyRelative("hatchingTexture");
                var crossHatchingTexture = property.FindPropertyRelative("crossHatchingTexture");
                var hatchingDensity = property.FindPropertyRelative("hatchingDensity");
                var hatchingIntensity = property.FindPropertyRelative("hatchingIntensity");
                var hatchingThreshold = property.FindPropertyRelative("hatchingThreshold");
                var crossHatchingThreshold = property.FindPropertyRelative("crossHatchingThreshold");
                var hatchingRotation = property.FindPropertyRelative("hatchingRotation");
                
                // Screen space hatching
                var enableScreenSpaceHatching = property.FindPropertyRelative("enableScreenSpaceHatching");
                var screenHatchScale = property.FindPropertyRelative("screenHatchScale");
                var screenHatchBias = property.FindPropertyRelative("screenHatchBias");
                
                // Textures
                EditorGUI.LabelField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                   "Textures", EditorStyles.boldLabel);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      hatchingTexture);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      crossHatchingTexture);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                // Parameters
                EditorGUI.LabelField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                   "Parameters", EditorStyles.boldLabel);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      hatchingDensity);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      hatchingIntensity);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      hatchingThreshold);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      crossHatchingThreshold);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      hatchingRotation);
                yOffset += EditorGUIUtility.singleLineHeight + 4;
                
                // Screen space hatching
                EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                      enableScreenSpaceHatching);
                
                if (enableScreenSpaceHatching.boolValue)
                {
                    yOffset += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.indentLevel++;
                    
                    EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                          screenHatchScale);
                    yOffset += EditorGUIUtility.singleLineHeight + 2;
                    
                    EditorGUI.PropertyField(new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight), 
                                          screenHatchBias);
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enableHatching = property.FindPropertyRelative("enableHatching");
            if (!property.isExpanded || !enableHatching.boolValue)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            
            var enableScreenSpaceHatching = property.FindPropertyRelative("enableScreenSpaceHatching");
            int lineCount = 12; // Base properties
            
            if (enableScreenSpaceHatching.boolValue)
            {
                lineCount += 2; // Screen space properties
            }
            
            return (EditorGUIUtility.singleLineHeight + 2) * lineCount;
        }
    }

    /// <summary>
    /// Range slider with value display
    /// </summary>
    [CustomPropertyDrawer(typeof(RangeAttribute))]
    public class ToonRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Float)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            var rangeAttribute = (RangeAttribute)attribute;
            
            EditorGUI.BeginProperty(position, label, property);
            
            // Split the position for label, slider, and value field
            var labelWidth = EditorGUIUtility.labelWidth;
            var valueFieldWidth = 50f;
            var sliderWidth = position.width - labelWidth - valueFieldWidth - 10f;
            
            var labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            var sliderRect = new Rect(position.x + labelWidth, position.y, sliderWidth, position.height);
            var valueRect = new Rect(position.x + labelWidth + sliderWidth + 5f, position.y, valueFieldWidth, position.height);
            
            // Draw label
            EditorGUI.LabelField(labelRect, label);
            
            // Draw slider
            EditorGUI.BeginChangeCheck();
            var newValue = GUI.HorizontalSlider(sliderRect, property.floatValue, rangeAttribute.min, rangeAttribute.max);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = newValue;
            }
            
            // Draw value field
            EditorGUI.BeginChangeCheck();
            newValue = EditorGUI.FloatField(valueRect, property.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = Mathf.Clamp(newValue, rangeAttribute.min, rangeAttribute.max);
            }
            
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// Color field with HDR support indicator
    /// </summary>
    public static class ToonColorField
    {
        public static Color Draw(Rect position, GUIContent label, Color value, bool hdr = false)
        {
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth - 15, position.height);
            var hdrRect = new Rect(position.x + EditorGUIUtility.labelWidth - 15, position.y, 15, position.height);
            var colorRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 
                                   position.width - EditorGUIUtility.labelWidth, position.height);
            
            EditorGUI.LabelField(labelRect, label);
            
            if (hdr)
            {
                var hdrStyle = new GUIStyle(GUI.skin.label) { fontSize = 8, normal = { textColor = ToonGUIStyles.AccentColor } };
                EditorGUI.LabelField(hdrRect, "HDR", hdrStyle);
            }
            
            return EditorGUI.ColorField(colorRect, value, true, true, hdr);
        }
    }

    /// <summary>
    /// Texture field with preview
    /// </summary>
    public static class ToonTextureField
    {
        public static Texture2D Draw(Rect position, GUIContent label, Texture2D texture, bool showPreview = true)
        {
            var previewSize = showPreview ? 32f : 0f;
            var fieldRect = new Rect(position.x, position.y, position.width - previewSize - 5f, position.height);
            var previewRect = new Rect(position.x + position.width - previewSize, position.y, previewSize, previewSize);
            
            var result = (Texture2D)EditorGUI.ObjectField(fieldRect, label, texture, typeof(Texture2D), false);
            
            if (showPreview && result != null)
            {
                EditorGUI.DrawPreviewTexture(previewRect, result);
            }
            
            return result;
        }
    }

    /// <summary>
    /// Performance indicator drawer
    /// </summary>
    public static class ToonPerformanceDrawer
    {
        public static void Draw(Rect position, float performanceCost, string label = "Performance Cost")
        {
            var labelRect = new Rect(position.x, position.y, 100, position.height);
            var barRect = new Rect(position.x + 105, position.y, position.width - 155, position.height);
            var valueRect = new Rect(position.x + position.width - 45, position.y, 45, position.height);
            
            EditorGUI.LabelField(labelRect, label, ToonGUIStyles.SmallLabelStyle);
            
            // Performance bar
            EditorGUI.DrawRect(barRect, Color.black * 0.3f);
            
            var fillRect = new Rect(barRect.x, barRect.y, barRect.width * performanceCost, barRect.height);
            Color fillColor = Color.green;
            if (performanceCost > 0.6f) fillColor = Color.yellow;
            if (performanceCost > 0.8f) fillColor = Color.red;
            
            EditorGUI.DrawRect(fillRect, fillColor);
            
            // Value
            EditorGUI.LabelField(valueRect, $"{performanceCost:P0}", ToonGUIStyles.SmallLabelStyle);
        }
    }

    /// <summary>
    /// Quality level selector with icons
    /// </summary>
    [CustomPropertyDrawer(typeof(System.Enum))]
    public class ToonQualityDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.name != "qualityLevel")
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            EditorGUI.BeginProperty(position, label, property);
            
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var buttonAreaRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 
                                        position.width - EditorGUIUtility.labelWidth, position.height);
            
            EditorGUI.LabelField(labelRect, label);
            
            string[] qualityNames = { "Low", "Medium", "High", "Ultra" };
            string[] qualityIcons = { "🔋", "⚡", "🔥", "💫" };
            
            var buttonWidth = buttonAreaRect.width / 4f;
            
            for (int i = 0; i < 4; i++)
            {
                var buttonRect = new Rect(buttonAreaRect.x + i * buttonWidth, buttonAreaRect.y, 
                                        buttonWidth - 2, buttonAreaRect.height);
                
                var isSelected = property.intValue == i;
                var originalColor = GUI.backgroundColor;
                
                if (isSelected)
                    GUI.backgroundColor = ToonGUIStyles.AccentColor;
                
                if (GUI.Button(buttonRect, $"{qualityIcons[i]} {qualityNames[i]}", ToonGUIStyles.ToggleButtonStyle))
                {
                    property.intValue = i;
                }
                
                GUI.backgroundColor = originalColor;
            }
            
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// Animation curve with preset buttons
    /// </summary>
    public static class ToonAnimationCurveField
    {
        public static AnimationCurve Draw(Rect position, GUIContent label, AnimationCurve curve)
        {
            var presetButtonWidth = 60f;
            var curveRect = new Rect(position.x, position.y, position.width - presetButtonWidth - 5f, position.height);
            var presetRect = new Rect(position.x + position.width - presetButtonWidth, position.y, 
                                    presetButtonWidth, position.height);
            
            var result = EditorGUI.CurveField(curveRect, label, curve);
            
            if (GUI.Button(presetRect, "Presets", EditorStyles.miniButton))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Linear"), false, () => 
                {
                    result = AnimationCurve.Linear(0, 0, 1, 1);
                });
                menu.AddItem(new GUIContent("Ease In Out"), false, () => 
                {
                    result = AnimationCurve.EaseInOut(0, 0, 1, 1);
                });
                menu.AddItem(new GUIContent("Pulse"), false, () => 
                {
                    result = new AnimationCurve(
                        new Keyframe(0, 0.5f),
                        new Keyframe(0.5f, 1f),
                        new Keyframe(1f, 0.5f)
                    );
                });
                menu.AddItem(new GUIContent("Bounce"), false, () => 
                {
                    result = new AnimationCurve(
                        new Keyframe(0, 0f),
                        new Keyframe(0.3f, 1.2f),
                        new Keyframe(0.6f, 0.8f),
                        new Keyframe(1f, 1f)
                    );
                });
                menu.ShowAsContext();
            }
            
            return result;
        }
    }

    /// <summary>
    /// Shader keyword toggle with dependency checking
    /// </summary>
    public static class ToonKeywordToggle
    {
        public static bool Draw(Rect position, GUIContent label, bool value, Material material, string keyword)
        {
            var result = EditorGUI.Toggle(position, label, value);
            
            if (material != null && Event.current.type == EventType.Repaint)
            {
                var hasKeyword = material.IsKeywordEnabled(keyword);
                if (hasKeyword != result)
                {
                    // Visual indicator that material is out of sync
                    var warningRect = new Rect(position.x + position.width - 20, position.y, 20, position.height);
                    var warningStyle = new GUIStyle(GUI.skin.label) 
                    { 
                        normal = { textColor = ToonGUIStyles.WarningColor },
                        fontSize = 12,
                        alignment = TextAnchor.MiddleCenter
                    };
                    EditorGUI.LabelField(warningRect, "⚠", warningStyle);
                }
            }
            
            return result;
        }
    }
}