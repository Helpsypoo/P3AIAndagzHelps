//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input/BaseInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @BaseInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @BaseInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BaseInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""17370877-260f-4373-9c1e-07e73830ab6a"",
            ""actions"": [
                {
                    ""name"": ""SelectClick"",
                    ""type"": ""Button"",
                    ""id"": ""41ab0fc4-0d24-48b7-969a-f9f72a70afb3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Mouse"",
                    ""type"": ""Value"",
                    ""id"": ""f0705ad9-14c3-4d1a-9a47-87dec7b97cf7"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ActionClick"",
                    ""type"": ""Button"",
                    ""id"": ""35e11699-55ad-4705-9143-b07b24c34eee"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraClick"",
                    ""type"": ""Button"",
                    ""id"": ""e21b0dcf-9cfe-4693-9dcb-18b110c4750c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Unit1"",
                    ""type"": ""Button"",
                    ""id"": ""fa9c05ca-d7d6-4205-859a-521a52c9cc6f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Unit2"",
                    ""type"": ""Button"",
                    ""id"": ""a8a811e2-b63e-4f6f-8bcf-6f3087b50cb2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Unit3"",
                    ""type"": ""Button"",
                    ""id"": ""3a16d722-bb8d-48fc-ac51-d82fc2851194"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Unit4"",
                    ""type"": ""Button"",
                    ""id"": ""aeb5b475-1cec-4883-98e7-bb6b35f8ae54"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ActionModifier"",
                    ""type"": ""Button"",
                    ""id"": ""c663756b-e2d5-465e-abd3-f6bf0ff68e47"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SelectionModifier"",
                    ""type"": ""Button"",
                    ""id"": ""8d3dd3a0-6e18-4165-8d9b-33b401d0f043"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseScroll"",
                    ""type"": ""Value"",
                    ""id"": ""0ce90524-022c-4de9-b349-db6181cc3e81"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dd9bfab1-a91d-4b22-93ee-18b0938a2011"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b7f3b171-0605-4f3b-b9d0-12ba0b6b8842"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c612e872-0aac-440d-b37c-c4058b1cbb5d"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ActionClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2585acd8-2d2f-4802-b224-d14583eec289"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a122413-1bda-4adb-b307-37e5c9fb8a38"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unit1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b4573497-ffd9-404b-9ab9-e440aaf69637"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unit2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e3983e3a-5854-480e-a71c-bb7e75fcc517"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unit3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6c11b23-b82f-400b-bc34-e2297c5fda54"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unit4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b98119c-5c22-4712-bec7-4127bf0abb1d"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ActionModifier"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48948809-03db-43d2-a86d-75a3fa315c83"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ActionModifier"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9977c085-5194-49b2-bbb5-38d9a45b6c6f"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectionModifier"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c60a2c7-cac0-4c46-8538-6606196f0ea4"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_SelectClick = m_Player.FindAction("SelectClick", throwIfNotFound: true);
        m_Player_Mouse = m_Player.FindAction("Mouse", throwIfNotFound: true);
        m_Player_ActionClick = m_Player.FindAction("ActionClick", throwIfNotFound: true);
        m_Player_CameraClick = m_Player.FindAction("CameraClick", throwIfNotFound: true);
        m_Player_Unit1 = m_Player.FindAction("Unit1", throwIfNotFound: true);
        m_Player_Unit2 = m_Player.FindAction("Unit2", throwIfNotFound: true);
        m_Player_Unit3 = m_Player.FindAction("Unit3", throwIfNotFound: true);
        m_Player_Unit4 = m_Player.FindAction("Unit4", throwIfNotFound: true);
        m_Player_ActionModifier = m_Player.FindAction("ActionModifier", throwIfNotFound: true);
        m_Player_SelectionModifier = m_Player.FindAction("SelectionModifier", throwIfNotFound: true);
        m_Player_MouseScroll = m_Player.FindAction("MouseScroll", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_SelectClick;
    private readonly InputAction m_Player_Mouse;
    private readonly InputAction m_Player_ActionClick;
    private readonly InputAction m_Player_CameraClick;
    private readonly InputAction m_Player_Unit1;
    private readonly InputAction m_Player_Unit2;
    private readonly InputAction m_Player_Unit3;
    private readonly InputAction m_Player_Unit4;
    private readonly InputAction m_Player_ActionModifier;
    private readonly InputAction m_Player_SelectionModifier;
    private readonly InputAction m_Player_MouseScroll;
    public struct PlayerActions
    {
        private @BaseInput m_Wrapper;
        public PlayerActions(@BaseInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @SelectClick => m_Wrapper.m_Player_SelectClick;
        public InputAction @Mouse => m_Wrapper.m_Player_Mouse;
        public InputAction @ActionClick => m_Wrapper.m_Player_ActionClick;
        public InputAction @CameraClick => m_Wrapper.m_Player_CameraClick;
        public InputAction @Unit1 => m_Wrapper.m_Player_Unit1;
        public InputAction @Unit2 => m_Wrapper.m_Player_Unit2;
        public InputAction @Unit3 => m_Wrapper.m_Player_Unit3;
        public InputAction @Unit4 => m_Wrapper.m_Player_Unit4;
        public InputAction @ActionModifier => m_Wrapper.m_Player_ActionModifier;
        public InputAction @SelectionModifier => m_Wrapper.m_Player_SelectionModifier;
        public InputAction @MouseScroll => m_Wrapper.m_Player_MouseScroll;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @SelectClick.started += instance.OnSelectClick;
            @SelectClick.performed += instance.OnSelectClick;
            @SelectClick.canceled += instance.OnSelectClick;
            @Mouse.started += instance.OnMouse;
            @Mouse.performed += instance.OnMouse;
            @Mouse.canceled += instance.OnMouse;
            @ActionClick.started += instance.OnActionClick;
            @ActionClick.performed += instance.OnActionClick;
            @ActionClick.canceled += instance.OnActionClick;
            @CameraClick.started += instance.OnCameraClick;
            @CameraClick.performed += instance.OnCameraClick;
            @CameraClick.canceled += instance.OnCameraClick;
            @Unit1.started += instance.OnUnit1;
            @Unit1.performed += instance.OnUnit1;
            @Unit1.canceled += instance.OnUnit1;
            @Unit2.started += instance.OnUnit2;
            @Unit2.performed += instance.OnUnit2;
            @Unit2.canceled += instance.OnUnit2;
            @Unit3.started += instance.OnUnit3;
            @Unit3.performed += instance.OnUnit3;
            @Unit3.canceled += instance.OnUnit3;
            @Unit4.started += instance.OnUnit4;
            @Unit4.performed += instance.OnUnit4;
            @Unit4.canceled += instance.OnUnit4;
            @ActionModifier.started += instance.OnActionModifier;
            @ActionModifier.performed += instance.OnActionModifier;
            @ActionModifier.canceled += instance.OnActionModifier;
            @SelectionModifier.started += instance.OnSelectionModifier;
            @SelectionModifier.performed += instance.OnSelectionModifier;
            @SelectionModifier.canceled += instance.OnSelectionModifier;
            @MouseScroll.started += instance.OnMouseScroll;
            @MouseScroll.performed += instance.OnMouseScroll;
            @MouseScroll.canceled += instance.OnMouseScroll;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @SelectClick.started -= instance.OnSelectClick;
            @SelectClick.performed -= instance.OnSelectClick;
            @SelectClick.canceled -= instance.OnSelectClick;
            @Mouse.started -= instance.OnMouse;
            @Mouse.performed -= instance.OnMouse;
            @Mouse.canceled -= instance.OnMouse;
            @ActionClick.started -= instance.OnActionClick;
            @ActionClick.performed -= instance.OnActionClick;
            @ActionClick.canceled -= instance.OnActionClick;
            @CameraClick.started -= instance.OnCameraClick;
            @CameraClick.performed -= instance.OnCameraClick;
            @CameraClick.canceled -= instance.OnCameraClick;
            @Unit1.started -= instance.OnUnit1;
            @Unit1.performed -= instance.OnUnit1;
            @Unit1.canceled -= instance.OnUnit1;
            @Unit2.started -= instance.OnUnit2;
            @Unit2.performed -= instance.OnUnit2;
            @Unit2.canceled -= instance.OnUnit2;
            @Unit3.started -= instance.OnUnit3;
            @Unit3.performed -= instance.OnUnit3;
            @Unit3.canceled -= instance.OnUnit3;
            @Unit4.started -= instance.OnUnit4;
            @Unit4.performed -= instance.OnUnit4;
            @Unit4.canceled -= instance.OnUnit4;
            @ActionModifier.started -= instance.OnActionModifier;
            @ActionModifier.performed -= instance.OnActionModifier;
            @ActionModifier.canceled -= instance.OnActionModifier;
            @SelectionModifier.started -= instance.OnSelectionModifier;
            @SelectionModifier.performed -= instance.OnSelectionModifier;
            @SelectionModifier.canceled -= instance.OnSelectionModifier;
            @MouseScroll.started -= instance.OnMouseScroll;
            @MouseScroll.performed -= instance.OnMouseScroll;
            @MouseScroll.canceled -= instance.OnMouseScroll;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnSelectClick(InputAction.CallbackContext context);
        void OnMouse(InputAction.CallbackContext context);
        void OnActionClick(InputAction.CallbackContext context);
        void OnCameraClick(InputAction.CallbackContext context);
        void OnUnit1(InputAction.CallbackContext context);
        void OnUnit2(InputAction.CallbackContext context);
        void OnUnit3(InputAction.CallbackContext context);
        void OnUnit4(InputAction.CallbackContext context);
        void OnActionModifier(InputAction.CallbackContext context);
        void OnSelectionModifier(InputAction.CallbackContext context);
        void OnMouseScroll(InputAction.CallbackContext context);
    }
}
