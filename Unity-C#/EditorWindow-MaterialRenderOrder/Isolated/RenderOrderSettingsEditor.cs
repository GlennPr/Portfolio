using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

// Compatibly with Unity Version 2021 and higher

namespace EditorTools
{
	public class RenderOrderSettingsEditor : EditorWindow
	{
		private const string _projectAssetsFolder = "Assets";
		private const string _projectResourcesFolder = "Resources";
		private const string _menuItem = "Tools/";
		private const string _menuName = "Render Order Editor";
		private const string _folderName = "Editor";
		private const string _dataFileName = "EditorTools_RenderOrderSettings_DATA";

		private static RenderOrderSettings _settings;

		private bool _configGroupsMode = false;
		private bool _listMode = true;
		private Vector2 _scrollPos;

		private Dictionary<RenderOrderSettings.Slot, int> _slotQueueMapping = new Dictionary<RenderOrderSettings.Slot, int>();
		private List<KeyValuePair<RenderOrderSettings.Slot, Rect>> _slotRects = new List<KeyValuePair<RenderOrderSettings.Slot, Rect>>();
		private List<KeyValuePair<int, Rect>> _groupRects = new List<KeyValuePair<int, Rect>>();

		/// <summary>
		/// List/Bundle shader & material files to keep an better overview of the order in which they render 
		/// </summary>
		[MenuItem(_menuItem + _menuName)]
		private static void OpenWindow()
		{
			RenderOrderSettingsEditor window = GetWindow<RenderOrderSettingsEditor>();
			window.titleContent = new GUIContent(_menuName);
		}
		public override void SaveChanges()
		{
			Save(true);

			Debug.Log($"{this} saved successfully!!!");
			base.SaveChanges();
		}

		private void OnEnable()
		{
			_configGroupsMode = false;
			_listMode = true;
			UpdateSlotQueueMapping();
		}

		private void OnGUI()
		{
			_slotRects.Clear();
			_groupRects.Clear();

			if (_settings == null)
			{
				FindSettings(ref _settings);
			}

			if (_settings == null)
			{
				GUILayout.Label("NO Settings Found", EditorStyles.boldLabel);

				if (GUILayout.Button("Generate Settings object"))
				{
					var pathToResourcesFolder = _projectAssetsFolder + "/" + _projectResourcesFolder;

					if (AssetDatabase.IsValidFolder(pathToResourcesFolder) == false)
					{
						AssetDatabase.CreateFolder(_projectAssetsFolder, _projectResourcesFolder);
					}

					if (AssetDatabase.IsValidFolder(pathToResourcesFolder + "/" + _folderName) == false)
					{
						AssetDatabase.CreateFolder(pathToResourcesFolder, _folderName);
					}

					var instance = (RenderOrderSettings) ScriptableObject.CreateInstance(typeof(RenderOrderSettings));
					AssetDatabase.CreateAsset(instance, "Assets/Resources/" + _folderName + "/" + _dataFileName + ".asset");

					UpdateSlotQueueMapping();
				}
			}
			else
			{
				_settings.CheckValidity();

				if (_configGroupsMode)
				{
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("RenderQueue List Mode"))
					{
						_configGroupsMode = false;
						_scrollPos = Vector2.zero;
						Save();
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.HelpBox("<b><i>INSTRUCTIONS</i></b>\n\n" +
					                        "Groups are used to categorize effect files, improving their readability\n" +
					                        "\n" +
					                        "<b>ADD</b>: press the ADD button\n" +
					                        "\n" +
					                        "<b>REMOVE</b>: by right clicking on their horizontal slot", MessageType.Info, true);

					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Add Group"))
					{
						AddGroup();
					}
					EditorGUILayout.EndHorizontal();

					_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, true);
					DrawGroupMode();
					ProcessEvents(Event.current);
					EditorGUILayout.EndScrollView();
				}
				else
				{
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Config groups Mode"))
					{
						_configGroupsMode = true;
						_scrollPos = Vector2.zero;
						Save();
					}

					EditorGUILayout.EndHorizontal();


					var originalContentColor = GUI.contentColor;
					EditorGUILayout.BeginHorizontal();

					GUI.contentColor = _listMode ? Color.green : originalContentColor;
					if (GUILayout.Button("List Mode"))
					{
						_listMode = true;
					}

					GUI.contentColor = _listMode == false ? Color.green : originalContentColor;
					if (GUILayout.Button("Grouped Mode"))
					{
						_listMode = false;
					}

					EditorGUILayout.EndHorizontal();
					GUI.contentColor = originalContentColor;

					// somehow setting rich text true here, fixes the text below, even though this GUIStyle is not referenced
					GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
					myStyle.richText = true;

					EditorGUILayout.HelpBox("<b><i>INSTRUCTIONS</i></b>\n\n" +
					                        "Shader & Materials can be organized here in different ways to get a project level overview of their render ordering\n" +
											 "\n" +
											"<b><i>NOTE</i></b>: Render order can still be overwritten externally\n" +
											"\n" +
					                        "<b>ADD</b>: Drag and drop files into this view to add them\n" +
					                        "\n" +
					                        "<b>REMOVE</b>: by right clicking on their horizontal slot. THIS WILL NOT DELETE THEM FROM THE PROJECT", MessageType.Info, true);
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.Space();

					_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, true);

					DrawSlots();
					ProcessEvents(Event.current);

					EditorGUILayout.EndScrollView();

					ProcessDragIn();
				}
			}

			if (GUI.changed)
			{
				Repaint();
			}
		}

		private void Save(bool closingWindow = false)
		{
			if (_settings != null)
			{
				// force update when closing the window
				if (closingWindow)
				{
					for (int i = 0; i < _settings.Slots.Length; i++)
					{
						var slot = _settings.Slots[i];

						if (slot.IsMaterialSlot())
						{
							var material = _settings.Materials[slot.MaterialIndex];
							material.renderQueue = _slotQueueMapping[slot];
						}
						else
						{
							_settings.ShadersRenderQueue[slot.ShaderIndex] = _slotQueueMapping[slot];
						}
					}
				}

				EditorUtility.SetDirty(_settings);
				AssetDatabase.SaveAssets();
			}

			hasUnsavedChanges = false;
		}


		#region Draw Elements
		private void DrawSlots()
		{
			var slots = _settings.Slots;

			if (slots == null || slots.Length == 0)
			{
				return;
			}

			hasUnsavedChanges = false;

			if (_listMode)
			{
				slots = _settings.Slots.OrderBy(x => _settings.GetQueueOfSlot(x)).ToArray();
			}
			else
			{
				slots = _settings.Slots.OrderBy(x => x.GroupIndex).ThenBy(x => _settings.GetQueueOfSlot(x)).ToArray();
			}

			int prevQueue = -1;
			int currentQueue = -1;
			int currentGroup = -1;

			GUIStyle queueFieldStyle = new GUIStyle(EditorStyles.textField);
			var objectFieldOptions = new GUILayoutOption[] {GUILayout.MaxWidth(350)};
			var queueFieldOptions = new GUILayoutOption[] {GUILayout.MaxWidth(100)};
			var groupFieldOptions = new GUILayoutOption[] {GUILayout.MaxWidth(150)};

			// group options
			var groupsDefined = _settings.GroupNames != null && _settings.GroupNames.Length > 0;
			var groupNameOptions = GenerateGroupNameOptions(groupsDefined);
			var groupColorOptions = GenerateGroupColorOptions(groupsDefined);


			// fill in slots
			for (int i = 0; i < slots.Length; i++)
			{
				var slot = slots[i];
				var queueChanged = false;
				var groupChanged = false;
				var slotQueue = _settings.GetQueueOfSlot(slot);
				var slotGroupIndex = slot.GroupIndex;

				if (i == 0 || slotQueue != currentQueue)
				{
					prevQueue = currentQueue;
					currentQueue = slotQueue;
					queueChanged = true;
				}

				if (i == 0 || slotGroupIndex != currentGroup)
				{
					currentGroup = slotGroupIndex;
					groupChanged = true;
				}


				if (_listMode)
				{
					if (queueChanged)
					{
						var renderQueues = GetEnum(typeof(RenderQueue)).ToList();
						for (int j = 0; j < renderQueues.Count; j++)
						{
							var current = renderQueues[j];
							var title = current.key.ToUpperInvariant() + " : " + current.value.ToString();

							if (currentQueue >= current.value && prevQueue < current.value)
							{
								DrawTitle(title, Color.yellow, j == 0);
							}
						}

						if (i != 0)
						{
							EditorGUILayout.EndVertical();
							EditorGUI.indentLevel--;
						}

						EditorGUILayout.Space();
						EditorGUILayout.Space();
						EditorGUILayout.BeginVertical();
						EditorGUI.indentLevel++;
					}
				}
				else
				{
					if (groupChanged)
					{
						if (groupsDefined)
						{
							DrawTitle(groupNameOptions[currentGroup + 1], groupColorOptions[currentGroup + 1], i == 0);
						}

						if (i != 0)
						{
							EditorGUILayout.EndVertical();
							EditorGUI.indentLevel--;
						}

						EditorGUILayout.Space();
						EditorGUILayout.Space();
						EditorGUILayout.BeginVertical();
						EditorGUI.indentLevel++;
					}

				}

				//Horizontal group
				if (_listMode && queueChanged)
				{
					DrawUILine(Color.Lerp(Color.white, Color.clear, 0.5f));
				}

				if (!_listMode && groupChanged)
				{
					DrawUILine(Color.Lerp(Color.white, Color.clear, 0.5f));
				}

				var slotRect = EditorGUILayout.BeginHorizontal();
				_slotRects.Add(new KeyValuePair<RenderOrderSettings.Slot, Rect>(slot, slotRect));

				//Group Selection
				if (groupsDefined)
				{
					EditorGUI.BeginChangeCheck();
					slot.GroupIndex = EditorGUILayout.Popup(slot.GroupIndex + 1, groupNameOptions, groupFieldOptions) - 1;
					if (EditorGUI.EndChangeCheck())
					{
						Save();
					}
				}

				// Object + Queue
				var isQueueChangeSaved = _slotQueueMapping[slot] == currentQueue;
				if (isQueueChangeSaved)
				{
					queueFieldStyle.normal.textColor = slot.IsMaterialSlot() ? Color.white : Color.grey;
				}
				else
				{
					queueFieldStyle.normal.textColor = Color.red;
					hasUnsavedChanges = true;
				}

				if (slot.IsMaterialSlot())
				{
					var material = _settings.Materials[slot.MaterialIndex];

					EditorGUILayout.ObjectField(material, typeof(Material), false, objectFieldOptions);
					_slotQueueMapping[slot] = EditorGUILayout.IntField(_slotQueueMapping[slot], queueFieldStyle, queueFieldOptions);

					if (GUILayout.Button("Set Queue"))
					{
						material.renderQueue = _slotQueueMapping[slot];
						UpdateSlotQueueMapping();
						Save();
					}
				}
				else
				{
					var shader = _settings.Shaders[slot.ShaderIndex];

					EditorGUILayout.ObjectField(shader, typeof(Shader), false, objectFieldOptions);
					EditorGUILayout.LabelField(shader.renderQueue.ToString(), queueFieldStyle, queueFieldOptions);
				}

				EditorGUILayout.EndHorizontal();
				DrawUILine(Color.Lerp(Color.white, Color.clear, 0.5f));
			}

			EditorGUILayout.EndVertical();
			EditorGUI.indentLevel--;
		}

		private void DrawGroupMode()
		{
			EditorGUI.BeginChangeCheck();

			if (_settings.GroupColors != null)
			{
				for (int i = 0; i < _settings.GroupColors.Length; i++)
				{
					var groupRect = EditorGUILayout.BeginHorizontal();
					_groupRects.Add(new KeyValuePair<int, Rect>(i, groupRect));

					_settings.GroupColors[i] = EditorGUILayout.ColorField(_settings.GroupColors[i], new GUILayoutOption[] {GUILayout.MaxWidth(150)});
					_settings.GroupNames[i] = EditorGUILayout.TextField(_settings.GroupNames[i]);

					EditorGUILayout.EndHorizontal();
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				hasUnsavedChanges = true;
			}

		}

		public void DrawUILine(Color color, int thickness = 2, int padding = 4)
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
			r.height = thickness;
			r.y += padding / 2f;
			r.x -= 2;
			r.width += 6;
			EditorGUI.DrawRect(r, color);
		}

		private void DrawTitle(string text, Color color, bool first)
		{
			if (first == false)
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}

			GUIStyle titleStyle = new GUIStyle(EditorStyles.textField);
			titleStyle.normal.textColor = color;
			titleStyle.fontStyle = FontStyle.Bold;
			titleStyle.fontSize = 12;
			titleStyle.alignment = TextAnchor.MiddleCenter;
			var layoutOptions = new GUILayoutOption[] {GUILayout.MinHeight(25)};

			EditorGUILayout.LabelField(text.ToUpperInvariant(), titleStyle, layoutOptions);
		}
		#endregion

	

		#region Actions
		private void OnClickRemoveSlot(RenderOrderSettings.Slot slot)
		{
			_settings.RemoveSlot(slot);
			UpdateSlotQueueMapping();
			Save();
		}

		private void OnClickRemoveGroup(int index)
		{
			_settings.RemoveGroup(index);
			UpdateSlotQueueMapping();
			Save();
		}

		private void ProcessDragIn()
		{
			if (Event.current.type == EventType.DragUpdated)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				Event.current.Use();
			}
			else if (Event.current.type == EventType.DragPerform)
			{
				// To consume drag data.
				DragAndDrop.AcceptDrag();

				// Unity Assets including folder.
				if (DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length)
				{
					for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
					{
						Object obj = DragAndDrop.objectReferences[i];

						if (obj is Material)
						{
							AddMaterial(obj as Material);
						}
						else if (obj is Shader)
						{
							AddShader(obj as Shader);
						}
					}
				}
			}
		}

		private void ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 1)
					{
						if (_configGroupsMode)
						{
							for (int i = 0; i < _groupRects.Count; i++)
							{
								if (_groupRects[i].Value.Contains(e.mousePosition))
								{
									DisplayGroupRemoveMenu(_groupRects[i].Key);
									e.Use();
									break;
								}
							}
						}
						else
						{
							for (int i = 0; i < _slotRects.Count; i++)
							{
								if (_slotRects[i].Value.Contains(e.mousePosition))
								{
									SlotRemoveMenu(_slotRects[i].Key);
									e.Use();
									break;
								}
							}
						}
					}

					break;
			}
		}
		#endregion


		#region Addition to list
		private void AddGroup()
		{
			AddToArray(ref _settings.GroupColors, Color.white);
			AddToArray(ref _settings.GroupNames, "New Group");
			Save();
		}

		private void AddMaterial(Material value)
		{
			if (_settings != null && (_settings.Materials == null || _settings.Materials.Contains(value) == false))
			{
				AddToArray(ref _settings.Materials, value);

				var slot = new RenderOrderSettings.Slot();
				slot.MaterialIndex = _settings.Materials.Length - 1;

				AddToArray(ref _settings.Slots, slot);

				UpdateSlotQueueMapping();
				Save();
			}
		}

		private void AddShader(Shader value)
		{
			if (_settings != null && (_settings.Shaders == null || _settings.Shaders.Contains(value) == false))
			{
				AddToArray(ref _settings.Shaders, value);
				AddToArray(ref _settings.ShadersRenderQueue, value.renderQueue);

				var slot = new RenderOrderSettings.Slot();
				slot.ShaderIndex = _settings.Shaders.Length - 1;

				AddToArray(ref _settings.Slots, slot);

				UpdateSlotQueueMapping();
				Save();
			}
		}
		#endregion


		#region Menus
		private void DisplayGroupRemoveMenu(int index)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Remove Group"), false, () => OnClickRemoveGroup(index));
			genericMenu.ShowAsContext();
		}

		private void SlotRemoveMenu(RenderOrderSettings.Slot slot)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Remove Slot"), false, () => OnClickRemoveSlot(slot));
			genericMenu.ShowAsContext();
		}
		#endregion


		#region HelperFunctions
		public static void FindSettings(ref RenderOrderSettings settings)
		{
			// returns 'null' if not loaded / existing
			settings = Resources.Load<RenderOrderSettings>(_folderName + "/" + _dataFileName);
		}

		private void UpdateSlotQueueMapping()
		{
			FindSettings(ref _settings);
			_slotQueueMapping.Clear();

			if (_settings != null && _settings.Slots != null)
			{
				for (int i = 0; i < _settings.Slots.Length; i++)
				{
					var slot = _settings.Slots[i];
					_slotQueueMapping.Add(slot, _settings.GetQueueOfSlot(slot));
				}
			}
		}

		private string[] GenerateGroupNameOptions(bool groupsDefined)
		{
			if (groupsDefined)
			{
				var options = new string[_settings.GroupNames.Length + 1];
				options[0] = "NONE";
				for (int i = 0; i < _settings.GroupNames.Length; i++)
				{
					options[i + 1] = _settings.GroupNames[i];
				}

				return options;
			}

			return new string[0];
		}

		private Color[] GenerateGroupColorOptions(bool groupsDefined)
		{
			if (groupsDefined)
			{
				var options = new Color[_settings.GroupColors.Length + 1];
				options[0] = Color.yellow;
				for (int i = 0; i < _settings.GroupColors.Length; i++)
				{
					options[i + 1] = _settings.GroupColors[i];
				}

				return options;
			}
			else
			{
				return new Color[0];
			}
		}

		private IEnumerable<(string key, int value)> GetEnum(Type enumType)
		{
			return Enum.GetValues(enumType)
				.Cast<int>()
				.Select(e => (Enum.GetName(enumType, e), e));
		}

		private void AddToArray<T>(ref T[] Org, T New_Value)
		{
			if (Org == null)
			{
				Org = new T[0];
			}

			T[] New = new T[Org.Length + 1];
			Org.CopyTo(New, 0);
			New[Org.Length] = New_Value;

			Org = New;
		}
		#endregion
	}
}
