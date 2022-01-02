using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

namespace YsoCorp {

    public class ScreenShot : EditorWindow {

        private enum ControlType {
            Target,
            Camera,
        }

        private const int COLUMNS = 4;
        private static Vector2 SPACE = new Vector2(0, 0);

        private Vector2 scrollPosition;
        private GameObject originalTarget;
        private GameObject target;
        private Vector2Int screenShotSize = new Vector2Int(256, 256);
        private PreviewRenderUtility previewRenderer;
        private bool controlFoldout;
        private ControlType controlType;
        private float dragSensibility = 50;

        private readonly List<Texture2D> screenshots = new List<Texture2D>();

        private bool isMoving;
        private bool isRotating;

        [MenuItem("Window/Screenshot")]
        private static void Init() {
            ScreenShot window = (ScreenShot)GetWindow(typeof(ScreenShot));
            window.titleContent = new GUIContent("ScreenShot");
            window.Show();
        }

        private void OnEnable() {
            if (originalTarget != null) {
                PreparePreviewRenderer();
            }
        }

        private void OnDisable() {
            if (target != null) {
                DestroyImmediate(target);
            }
            if (previewRenderer != null) {
                previewRenderer.Cleanup();
            }
        }

        private void OnGUI() {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUI.BeginChangeCheck();
            originalTarget = (GameObject)EditorGUILayout.ObjectField("Target", originalTarget, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck()) {
                PreparePreviewRenderer();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Size");
            screenShotSize = EditorGUILayout.Vector2IntField("", screenShotSize);
            EditorGUILayout.EndHorizontal();

            if (position.width < screenShotSize.x || position.height < screenShotSize.y) {
                EditorGUILayout.HelpBox("Expand the window to see the full preview", MessageType.Warning);
                EditorGUILayout.EndScrollView();
                return;
            }

            if (target != null) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                Rect previewRenderRect = EditorGUILayout.GetControlRect(GUILayout.Width(screenShotSize.x), GUILayout.Height(screenShotSize.y));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                DrawPreviewRenderer(previewRenderRect);
                DrawControls();
                CheckPreviewMouseInteractions(previewRenderRect);

                if (GUILayout.Button("Take screenshot")) {
                    TakeScreenshot(previewRenderRect);
                }
            }

            if (screenshots.Count > 0) {
                //*

                float textureWidth = (EditorGUIUtility.currentViewWidth - SPACE.x * (COLUMNS - 1)) / COLUMNS;
                int totalRows = (screenshots.Count - 1) / COLUMNS + 1;

                Rect remainingPosition = EditorGUILayout.GetControlRect(false, textureWidth * totalRows + SPACE.y * (totalRows - 1), GUI.skin.box);
                Rect previewTexturePosition = new Rect(remainingPosition.position, new Vector2(textureWidth, textureWidth));

                for (int i = 0; i < screenshots.Count; i += 1) {
                    int row = i / COLUMNS;
                    int col = i % COLUMNS;

                    previewTexturePosition.position = remainingPosition.position + new Vector2((textureWidth + SPACE.x) * col, (textureWidth + SPACE.y) * row);
                    EditorGUI.DrawPreviewTexture(previewTexturePosition, screenshots[i]);
                }

                /*/

                int row = (screenshots.Count - 1) / COLUMNS + 1;

                Rect totalPosition = EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandHeight(true));
                float buttonWidth = (totalPosition.width - SPACE.x * (COLUMNS - 1)) / COLUMNS;

                for (int i = 0; i < row; i += 1) {
                    if (i > 0) {
                        EditorGUILayout.Space(SPACE.y);
                    }
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; i * COLUMNS + j < screenshots.Count && j < COLUMNS; j += 1) {
                        Texture2D screenshot = screenshots[i * COLUMNS + j];

                        if (j > 0) {
                            EditorGUILayout.Space(SPACE.x);
                        }
                        if (GUILayout.Button(screenshot)) {
                            screenshots.Remove(screenshot);
                        }
                        Debug.Log(GUILayoutUtility.GetLastRect());
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                //*/

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Clear")) {
                    screenshots.Clear();
                }
                if (GUILayout.Button("Save")) {
                    Save();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void PreparePreviewRenderer() {
            if (previewRenderer == null) {
                previewRenderer = new PreviewRenderUtility();
                previewRenderer.cameraFieldOfView = 60;
                previewRenderer.camera.nearClipPlane = 0.5f;
                previewRenderer.camera.farClipPlane = 50;
            }

            screenshots.Clear();

            if (target != null) {
                DestroyImmediate(target);
            }
            target = previewRenderer.InstantiatePrefabInScene(originalTarget);

            previewRenderer.camera.transform.position = target.transform.position + new Vector3(0, 0, -5);
        }

        private void DrawPreviewRenderer(Rect rect) {
            previewRenderer.BeginPreview(rect, GUIStyle.none);

            /*
            float scaleFac = previewRenderer.GetScaleFactor(rect.width, rect.height);
            int rtWidth = (int)(rect.width * scaleFac);
            int rtHeight = (int)(rect.height * scaleFac);

            Texture2D transparentBackground = new Texture2D(1, 1, TextureFormat.RGBA32, true);
            transparentBackground.SetPixel(0, 0, Color.white);
            transparentBackground.Apply();
            Graphics.DrawTexture(new Rect(0, 0, rtWidth, rtHeight), transparentBackground);
            DestroyImmediate(transparentBackground);
            */

            previewRenderer.Render(false, false);
            previewRenderer.EndAndDrawPreview(rect);
        }

        private void DrawControls() {
            controlFoldout = EditorGUILayout.Foldout(controlFoldout, "Controls", true);
            if (controlFoldout) {
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);

                controlType = (ControlType)EditorGUILayout.EnumPopup("Control type", controlType);
                switch (controlType) {
                    case ControlType.Target:
                        previewRenderer.camera.transform.position = new Vector3(previewRenderer.camera.transform.position.x, previewRenderer.camera.transform.position.y, EditorGUILayout.FloatField("Offset", previewRenderer.camera.transform.position.z));
                        target.transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", target.transform.rotation.eulerAngles));
                        break;
                    case ControlType.Camera:
                        dragSensibility = Mathf.Max(1, EditorGUILayout.FloatField("Sensibility", dragSensibility));
                        previewRenderer.camera.transform.position = EditorGUILayout.Vector3Field("Position", previewRenderer.camera.transform.position);
                        previewRenderer.camera.transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", previewRenderer.camera.transform.rotation.eulerAngles));
                        break;
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void CheckPreviewMouseInteractions(Rect rect) {
            Event currentEvent = Event.current;
            if (currentEvent == null || !rect.Contains(currentEvent.mousePosition)) return;

            if (controlType == ControlType.Target) {
                switch (currentEvent.type) {
                    case EventType.MouseDrag:
                        currentEvent.Use();
                        float angle = Vector3.Dot(currentEvent.delta, previewRenderer.camera.transform.right);

                        if (Vector3.Dot(target.transform.up, Vector3.up) >= 0) {
                            angle = -angle;
                        }
                        target.transform.Rotate(target.transform.up, angle, Space.World);
                        target.transform.Rotate(previewRenderer.camera.transform.right, Vector3.Dot(currentEvent.delta, previewRenderer.camera.transform.up), Space.World);
                        break;
                    case EventType.ScrollWheel:
                        currentEvent.Use();
                        previewRenderer.camera.transform.position += new Vector3(0, 0, currentEvent.delta.y);
                        break;
                }
            } else if (controlType == ControlType.Camera) {
                switch (currentEvent.type) {
                    case EventType.MouseDown:
                        currentEvent.Use();
                        if ((currentEvent.alt && !currentEvent.command) || currentEvent.button == 1) {
                            isRotating = true;
                        } else {
                            isMoving = true;
                        }
                        break;
                    case EventType.MouseDrag:
                        currentEvent.Use();
                        if (isMoving) {
                            previewRenderer.camera.transform.position += new Vector3(-currentEvent.delta.x, currentEvent.delta.y, 0) / dragSensibility;
                        } else if (isRotating) {
                            // TODO
                        }
                        break;
                    case EventType.MouseUp:
                        currentEvent.Use();
                        isMoving = false;
                        isRotating = false;
                        break;
                    case EventType.ScrollWheel:
                        currentEvent.Use();
                        previewRenderer.camera.transform.position += new Vector3(0, 0, currentEvent.delta.y);
                        break;
                }
            }
        }

        private void TakeScreenshot(Rect rect) {
            CameraClearFlags oldClearFlags = previewRenderer.camera.clearFlags;
            Color oldBackgroundColor = previewRenderer.camera.backgroundColor;
            RenderTexture oldActive = RenderTexture.active;
            RenderTexture tmp = RenderTexture.GetTemporary((int)rect.width, (int)rect.height, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);

            previewRenderer.camera.clearFlags = CameraClearFlags.SolidColor;
            previewRenderer.camera.backgroundColor = Color.clear;

            previewRenderer.BeginPreview(rect, GUIStyle.none);
            previewRenderer.Render(false, false);

            RenderTexture texture = previewRenderer.EndPreview() as RenderTexture;
            RenderTexture.active = texture;
            Graphics.Blit(texture, tmp);

            Texture2D texture2D = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false, false);
            texture2D.ReadPixels(new Rect(0, 0, rect.width, rect.height), 0, 0);
            texture2D.Apply();

            screenshots.Add(texture2D);

            previewRenderer.camera.clearFlags = oldClearFlags;
            previewRenderer.camera.backgroundColor = oldBackgroundColor;
            RenderTexture.active = oldActive;
            RenderTexture.ReleaseTemporary(tmp);
        }

        private void Save() {
            string path = EditorUtility.SaveFilePanelInProject("Save screenshots", originalTarget.name + ".png", "png", "");

            if (!string.IsNullOrEmpty(path)) {
                File.WriteAllBytes(path, MergeScreenShots().EncodeToPNG());
                AssetDatabase.Refresh();
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = screenshots.Count > 1 ? SpriteImportMode.Multiple : SpriteImportMode.Single;
                importer.spritesheet = GenerateMetaData();
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }

        private Texture2D MergeScreenShots() {
            int rows = (screenshots.Count - 1) / COLUMNS + 1;
            int width = screenShotSize.x * COLUMNS;
            int height = screenShotSize.y * rows;

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int i = 0; i < screenshots.Count; i += 1) {
                int row = i / COLUMNS;
                int textureY = (rows - 1 - row) * screenShotSize.y;

                int col = i % COLUMNS;
                int textureX = col * screenShotSize.x;

                for (int x = 0; x < screenShotSize.x; x += 1) {
                    for (int y = 0; y < screenShotSize.y; y += 1) {
                        if (x == 0 && y == 0) {
                            Debug.Log(screenshots[i].GetPixel(x, y));
                        }
                        texture.SetPixel(textureX + x, textureY + y, screenshots[i].GetPixel(x, y));
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        private SpriteMetaData[] GenerateMetaData() {
            SpriteMetaData[] metaDatas = new SpriteMetaData[screenshots.Count];
            int rows = (screenshots.Count - 1) / COLUMNS + 1;

            for (int i = 0; i < screenshots.Count; i += 1) {
                int row = i / COLUMNS;
                int textureY = (rows - 1 - row) * screenShotSize.y;

                int col = i % COLUMNS;
                int textureX = col * screenShotSize.x;

                metaDatas[i].name = originalTarget.name + "_" + i;
                metaDatas[i].rect = new Rect(textureX, textureY, screenShotSize.x, screenShotSize.y);
            }
            return metaDatas;
        }

    }

}
