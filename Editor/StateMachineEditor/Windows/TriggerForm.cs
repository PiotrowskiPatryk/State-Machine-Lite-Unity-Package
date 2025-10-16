using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Windows
{
    public class TriggerForm : EditorWindow
    {
        public sealed class Result
        {
            public string Id { get; }
            public string Name { get; }
            public string Description { get; }
            public string TypeName { get; }
            public IPayload Payload { get; }

            public Result(string id, string name, string description, string typeName, IPayload payload)
            {
                Id = id;
                Name = name;
                Description = description;
                TypeName = typeName;
                Payload = payload;
            }
        }
        
        private enum Mode { Create, Edit }
        
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;
        
        private Mode _mode = Mode.Create;
        private Action<Result> _onSubmit;
        private string _initialId;
        private string _initialName;
        private string _initialDescription;
        private string _initialTypeName;
        private IPayload _initialPayload;
        
        private TriggerDefinitionWrapper _wrapper;
        private SerializedObject _serializedWrapper;
        private SerializedProperty _dataProperty;
        private PropertyField _triggerPropertyField;
        private Button _createButton;
        private Button _cancelButton;

        public static void Show(Action<Result> onCreate)
        {
            var window = CreateInstance<TriggerForm>();
            window._mode = Mode.Create;
            window._onSubmit = onCreate;
            window.titleContent = new GUIContent("New Trigger");
            window.minSize = new Vector2(640, 320);
            window.position = GetCenteredPosition(new Vector2(640, 320));
            window.ShowUtility();
        }
        
        public static void ShowEdit(string id, string name, string description, string typeName, IPayload payload, Action<Result> onSave)
        {
            var window = CreateInstance<TriggerForm>();
            window._mode = Mode.Edit;
            window._onSubmit = onSave;
            window._initialId = id;
            window._initialName = name;
            window._initialDescription = description;
            window._initialTypeName = typeName;
            window._initialPayload = payload;
            window.titleContent = new GUIContent("Edit Trigger");
            window.minSize = new Vector2(640, 320);
            window.position = GetCenteredPosition(new Vector2(640, 320));
            window.ShowUtility();
        }

        public void CreateGUI()
        {
            // Prepare serialized wrapper
            _wrapper = ScriptableObject.CreateInstance<TriggerDefinitionWrapper>();
            _serializedWrapper = new SerializedObject(_wrapper);
            _dataProperty = _serializedWrapper.FindProperty("Data");

            // Prepare initial data before binding depending on mode
            _serializedWrapper.Update();
            if (_mode == Mode.Create)
            {
                // Generate a new ID for a new trigger
                _dataProperty.FindPropertyRelative("_id").stringValue = Guid.NewGuid().ToString();
                _dataProperty.FindPropertyRelative("_name").stringValue = string.Empty;
                _dataProperty.FindPropertyRelative("_description").stringValue = string.Empty;
                _dataProperty.FindPropertyRelative("_typeName").stringValue = string.Empty;
            }
            else
            {
                // Prefill with provided values and do NOT change the ID
                _dataProperty.FindPropertyRelative("_id").stringValue = _initialId ?? string.Empty;
                _dataProperty.FindPropertyRelative("_name").stringValue = _initialName ?? string.Empty;
                _dataProperty.FindPropertyRelative("_description").stringValue = _initialDescription ?? string.Empty;
                _dataProperty.FindPropertyRelative("_typeName").stringValue = _initialTypeName ?? string.Empty;
                var payloadPropInit = _dataProperty.FindPropertyRelative("_payload");
                if (payloadPropInit != null)
                {
                    try { payloadPropInit.managedReferenceValue = _initialPayload; } catch { }
                }
            }
            _serializedWrapper.ApplyModifiedPropertiesWithoutUndo();

            var root = rootVisualElement;
            root.Clear();
            // Ensure the window content fits the Unity editor layout styling
            root.style.flexGrow = 1f;
            root.StretchToParentSize();
            root.style.paddingLeft = 0;
            root.style.paddingRight = 0;
            root.style.paddingTop = 0;
            root.style.paddingBottom = 0;
            
            if (_visualTreeAsset != null)
            {
                _visualTreeAsset.CloneTree(root);

                // Ensure cloned root fits and uses editor theme variables
                var clonedRoot = root.Q<VisualElement>("Root");
                if (clonedRoot != null)
                {
                    clonedRoot.style.flexGrow = 1f;
                    clonedRoot.StretchToParentSize();
                    clonedRoot.style.paddingLeft = 0;
                    clonedRoot.style.paddingRight = 0;
                    clonedRoot.style.paddingTop = 0;
                    clonedRoot.style.paddingBottom = 0;
                    clonedRoot.AddToClassList("unity-theme-variables");
                }

                // PropertyField to utilize the TriggerDefinition PropertyDrawer
                _triggerPropertyField = new PropertyField(_dataProperty)
                {
                    name = "TriggerDefinitionField"
                };
                _triggerPropertyField.Bind(_serializedWrapper);
                _triggerPropertyField.style.flexGrow = 1f;

                var triggerContainer = root.Q<VisualElement>("TriggerContainer");
                if (triggerContainer != null)
                {
                    triggerContainer.style.flexGrow = 1f;
                    triggerContainer.Add(_triggerPropertyField);
                }
                else
                {
                    root.Add(_triggerPropertyField);
                }

                _cancelButton = root.Q<Button>("CancelButton");
                _createButton = root.Q<Button>("CreateButton");

                if (_cancelButton != null)
                    _cancelButton.clicked += Close;
                if (_createButton != null)
                    _createButton.clicked += OnSubmitClicked;
                if (_createButton != null)
                    _createButton.text = _mode == Mode.Create ? "Create" : "Save";
            }
            else
            {
                // Fallback to programmatic UI if UXML is missing
                // PropertyField to utilize the TriggerDefinition PropertyDrawer
                _triggerPropertyField = new PropertyField(_dataProperty)
                {
                    name = "TriggerDefinitionField"
                };
                
                _triggerPropertyField.Bind(_serializedWrapper);
                root.Add(_triggerPropertyField);
                _triggerPropertyField.style.flexGrow = 1f;

                // Buttons row
                var buttonsRow = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        justifyContent = Justify.FlexEnd,
                        marginTop = 6,
                        paddingLeft = 4,
                        paddingRight = 4,
                        paddingTop = 2,
                        paddingBottom = 2
                    }
                };
                root.Add(buttonsRow);

                _cancelButton = new Button() { text = "Cancel" };
                _createButton = new Button() { text = _mode == Mode.Create ? "Create" : "Save" };
                _cancelButton.clicked += Close;
                _createButton.clicked += OnSubmitClicked;

                buttonsRow.Add(_cancelButton);
                buttonsRow.Add(_createButton);
            }
        }

        private void OnSubmitClicked()
        {
            _serializedWrapper.ApplyModifiedPropertiesWithoutUndo();

            var id = _dataProperty.FindPropertyRelative("_id").stringValue?.Trim() ?? string.Empty;
            var name = _dataProperty.FindPropertyRelative("_name").stringValue?.Trim() ?? string.Empty;
            var description = _dataProperty.FindPropertyRelative("_description").stringValue?.Trim() ?? string.Empty;
            var typeName = _dataProperty.FindPropertyRelative("_typeName").stringValue?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(id))
            {
                EditorUtility.DisplayDialog("Validation", "ID is required.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                EditorUtility.DisplayDialog("Validation", "Name is required.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(typeName))
            {
                EditorUtility.DisplayDialog("Validation", "Type is required.", "OK");
                return;           
            }

            // Extract payload managed reference if any
            IPayload payload = null;
            var payloadProp = _dataProperty.FindPropertyRelative("_payload");
            if (payloadProp != null)
            {
                try
                {
                    payload = payloadProp.managedReferenceValue as IPayload;

                    if (payload?.IsValid() == false)
                    {
                        EditorUtility.DisplayDialog("Validation", "Payload is invalid.", "OK");
                        return;   
                    }
                }
                catch
                {
                    payload = null;
                }
            }

            _onSubmit?.Invoke(new Result(id, name, description, typeName, payload));
            Close();
        }

        private static Rect GetCenteredPosition(Vector2 size)
        {
            var main = EditorGUIUtility.GetMainWindowPosition();
            var x = main.x + (main.width - size.x) * 0.5f;
            var y = main.y + (main.height - size.y) * 0.5f;
            return new Rect(x, y, size.x, size.y);
        }
    }
}