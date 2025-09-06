using UnityEngine;
using UnityEditor;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Custom GUI styles for the Ultimate Toon Shader editor
    /// Provides consistent visual styling across all editor windows
    /// </summary>
    public static class ToonGUIStyles
    {
        private static bool _initialized = false;
        
        // Header styles
        private static GUIStyle _headerStyle;
        private static GUIStyle _subHeaderStyle;
        private static GUIStyle _sectionHeaderStyle;
        
        // Content styles
        private static GUIStyle _boxStyle;
        private static GUIStyle _presetButtonStyle;
        private static GUIStyle _toggleButtonStyle;
        private static GUIStyle _helpBoxStyle;
        
        // Label styles
        private static GUIStyle _centeredLabelStyle;
        private static GUIStyle _boldLabelStyle;
        private static GUIStyle _smallLabelStyle;
        private static GUIStyle _versionLabelStyle;
        
        // Colors
        public static readonly Color HeaderColor = new Color(0.3f, 0.7f, 1f, 1f);
        public static readonly Color AccentColor = new Color(0.2f, 0.8f, 0.4f, 1f);
        public static readonly Color WarningColor = new Color(1f, 0.7f, 0.2f, 1f);
        public static readonly Color ErrorColor = new Color(1f, 0.3f, 0.3f, 1f);
        public static readonly Color BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        #region Style Properties
        
        public static GUIStyle HeaderStyle
        {
            get
            {
                InitializeStyles();
                return _headerStyle;
            }
        }
        
        public static GUIStyle SubHeaderStyle
        {
            get
            {
                InitializeStyles();
                return _subHeaderStyle;
            }
        }
        
        public static GUIStyle SectionHeaderStyle
        {
            get
            {
                InitializeStyles();
                return _sectionHeaderStyle;
            }
        }
        
        public static GUIStyle BoxStyle
        {
            get
            {
                InitializeStyles();
                return _boxStyle;
            }
        }
        
        public static GUIStyle PresetButtonStyle
        {
            get
            {
                InitializeStyles();
                return _presetButtonStyle;
            }
        }
        
        public static GUIStyle ToggleButtonStyle
        {
            get
            {
                InitializeStyles();
                return _toggleButtonStyle;
            }
        }
        
        public static GUIStyle HelpBoxStyle
        {
            get
            {
                InitializeStyles();
                return _helpBoxStyle;
            }
        }
        
        public static GUIStyle CenteredLabelStyle
        {
            get
            {
                InitializeStyles();
                return _centeredLabelStyle;
            }
        }
        
        public static GUIStyle BoldLabelStyle
        {
            get
            {
                InitializeStyles();
                return _boldLabelStyle;
            }
        }
        
        public static GUIStyle SmallLabelStyle
        {
            get
            {
                InitializeStyles();
                return _smallLabelStyle;
            }
        }
        
        public static GUIStyle VersionLabelStyle
        {
            get
            {
                InitializeStyles();
                return _versionLabelStyle;
            }
        }
        
        #endregion

        /// <summary>
        /// Initializes all custom GUI styles
        /// </summary>
        private static void InitializeStyles()
        {
            if (_initialized) return;
            
            // Header styles
            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 18,
                normal = { textColor = HeaderColor },
                wordWrap = true
            };
            
            _subHeaderStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                normal = { textColor = HeaderColor },
                wordWrap = true
            };
            
            _sectionHeaderStyle = new GUIStyle(EditorStyles.foldoutHeader)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                normal = { textColor = HeaderColor }
            };
            
            // Content styles
            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5)
            };
            
            _presetButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                fixedHeight = 25,
                normal = { textColor = Color.white }
            };
            
            _toggleButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 10,
                fixedHeight = 20
            };
            
            _helpBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 11,
                wordWrap = true,
                padding = new RectOffset(10, 10, 5, 5)
            };
            
            // Label styles
            _centeredLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            
            _boldLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };
            
            _smallLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                normal = { textColor = Color.gray }
            };
            
            _versionLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
                fontSize = 10,
                normal = { textColor = Color.gray }
            };
            
            _initialized = true;
        }

        /// <summary>
        /// Forces style re-initialization (useful when switching between light/dark themes)
        /// </summary>
        public static void RefreshStyles()
        {
            _initialized = false;
            InitializeStyles();
        }

        #region Utility Methods

        /// <summary>
        /// Creates a colored horizontal line
        /// </summary>
        public static void DrawColoredLine(Color color, float height = 1f)
        {
            var rect = GUILayoutUtility.GetRect(0, height, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, color);
        }

        /// <summary>
        /// Creates a separator with some spacing
        /// </summary>
        public static void DrawSeparator(float spacing = 5f)
        {
            GUILayout.Space(spacing);
            DrawColoredLine(Color.gray * 0.6f);
            GUILayout.Space(spacing);
        }

        /// <summary>
        /// Draws a performance indicator bar
        /// </summary>
        public static void DrawPerformanceBar(float value, string label = "Performance")
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(80));
            
            var rect = GUILayoutUtility.GetRect(0, 16, GUILayout.ExpandWidth(true));
            
            // Background
            EditorGUI.DrawRect(rect, Color.black * 0.3f);
            
            // Fill
            var fillRect = new Rect(rect.x, rect.y, rect.width * value, rect.height);
            Color fillColor = Color.green;
            if (value > 0.6f) fillColor = Color.yellow;
            if (value > 0.8f) fillColor = Color.red;
            
            EditorGUI.DrawRect(fillRect, fillColor);
            
            // Text
            var textStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10
            };
            EditorGUI.LabelField(rect, $"{value:P0}", textStyle);
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws an info box with icon
        /// </summary>
        public static void DrawInfoBox(string message, MessageType messageType = MessageType.Info)
        {
            EditorGUILayout.HelpBox(message, messageType);
        }

        /// <summary>
        /// Draws a foldout with custom styling
        /// </summary>
        public static bool DrawStyledFoldout(bool foldout, string content, string emoji = "")
        {
            var style = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };
            
            return EditorGUILayout.Foldout(foldout, $"{emoji} {content}", style);
        }

        /// <summary>
        /// Draws a toggle button with custom styling
        /// </summary>
        public static bool DrawToggleButton(bool value, string text, float width = 100f)
        {
            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = value ? AccentColor : originalColor;
            
            var result = GUILayout.Toggle(value, text, ToggleButtonStyle, GUILayout.Width(width));
            
            GUI.backgroundColor = originalColor;
            return result;
        }

        /// <summary>
        /// Draws a preset button with hover effects
        /// </summary>
        public static bool DrawPresetButton(string text, string description = "")
        {
            var rect = GUILayoutUtility.GetRect(0, 25, GUILayout.ExpandWidth(true));
            
            // Add hover effect
            if (Event.current.type == EventType.Repaint && rect.Contains(Event.current.mousePosition))
            {
                GUI.backgroundColor = HeaderColor * 1.2f;
            }
            
            var result = GUI.Button(rect, new GUIContent(text, description), PresetButtonStyle);
            GUI.backgroundColor = Color.white;
            
            return result;
        }

        /// <summary>
        /// Draws a property with custom label width
        /// </summary>
        public static void DrawPropertyWithLabelWidth(SerializedProperty property, float labelWidth = 150f)
        {
            var originalWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUILayout.PropertyField(property);
            EditorGUIUtility.labelWidth = originalWidth;
        }

        /// <summary>
        /// Draws a texture preview with frame
        /// </summary>
        public static void DrawTexturePreview(Texture2D texture, float size = 64f)
        {
            if (texture == null) return;
            
            var rect = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size));
            
            // Frame
            EditorGUI.DrawRect(new Rect(rect.x - 1, rect.y - 1, size + 2, size + 2), Color.black);
            EditorGUI.DrawRect(rect, Color.white);
            
            // Texture
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
        }

        /// <summary>
        /// Draws a quality level indicator
        /// </summary>
        public static void DrawQualityIndicator(int level)
        {
            string[] qualityNames = { "Low", "Medium", "High", "Ultra" };
            Color[] qualityColors = { 
                Color.red, 
                Color.yellow, 
                Color.green, 
                Color.cyan 
            };
            
            if (level >= 0 && level < qualityNames.Length)
            {
                var originalColor = GUI.contentColor;
                GUI.contentColor = qualityColors[level];
                EditorGUILayout.LabelField($"Quality: {qualityNames[level]}", BoldLabelStyle);
                GUI.contentColor = originalColor;
            }
        }

        /// <summary>
        /// Draws a feature count indicator
        /// </summary>
        public static void DrawFeatureCount(int activeCount, int totalCount)
        {
            var color = activeCount > totalCount * 0.7f ? WarningColor : AccentColor;
            var originalColor = GUI.contentColor;
            GUI.contentColor = color;
            EditorGUILayout.LabelField($"Active Features: {activeCount}/{totalCount}", SmallLabelStyle);
            GUI.contentColor = originalColor;
        }

        #endregion

        #region Layout Helpers

        /// <summary>
        /// Begins a styled vertical box
        /// </summary>
        public static void BeginStyledBox()
        {
            EditorGUILayout.BeginVertical(BoxStyle);
        }

        /// <summary>
        /// Ends a styled vertical box
        /// </summary>
        public static void EndStyledBox()
        {
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Creates a horizontal layout with proper spacing
        /// </summary>
        public static void BeginHorizontalLayout()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
        }

        /// <summary>
        /// Ends horizontal layout with proper spacing
        /// </summary>
        public static void EndHorizontalLayout()
        {
            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Adds consistent vertical spacing
        /// </summary>
        public static void AddVerticalSpace(float space = 5f)
        {
            GUILayout.Space(space);
        }

        #endregion
    }
}