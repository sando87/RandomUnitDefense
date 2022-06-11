// GENERATED AUTOMATICALLY FROM 'Assets/InputActions/InputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""InGame"",
            ""id"": ""c1152e60-2c8b-4972-959f-1a303342df02"",
            ""actions"": [
                {
                    ""name"": ""OnClick"",
                    ""type"": ""Button"",
                    ""id"": ""5d474bc5-bcfd-411c-b1d5-215325f04c2b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PointerMoving"",
                    ""type"": ""Value"",
                    ""id"": ""f66b7dbb-47e4-43ba-9cc7-0121642478a5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""KeyA"",
                    ""type"": ""Button"",
                    ""id"": ""06c2f171-3e6a-461d-bc34-21828bb5ff31"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""KeyB"",
                    ""type"": ""Button"",
                    ""id"": ""9425a758-6f39-491a-a682-7684eee67971"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f3178489-8c49-4c48-9ee3-f7e74b4e1f50"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OnClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""27a279c9-9870-4b7e-8a00-034ca8181344"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerMoving"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d74f57d3-5aba-46e3-a824-8d4f533fdb58"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyA"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3e6d9b93-5f00-421a-8472-aa273b87d328"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // InGame
        m_InGame = asset.FindActionMap("InGame", throwIfNotFound: true);
        m_InGame_OnClick = m_InGame.FindAction("OnClick", throwIfNotFound: true);
        m_InGame_PointerMoving = m_InGame.FindAction("PointerMoving", throwIfNotFound: true);
        m_InGame_KeyA = m_InGame.FindAction("KeyA", throwIfNotFound: true);
        m_InGame_KeyB = m_InGame.FindAction("KeyB", throwIfNotFound: true);
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

    // InGame
    private readonly InputActionMap m_InGame;
    private IInGameActions m_InGameActionsCallbackInterface;
    private readonly InputAction m_InGame_OnClick;
    private readonly InputAction m_InGame_PointerMoving;
    private readonly InputAction m_InGame_KeyA;
    private readonly InputAction m_InGame_KeyB;
    public struct InGameActions
    {
        private @InputActions m_Wrapper;
        public InGameActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @OnClick => m_Wrapper.m_InGame_OnClick;
        public InputAction @PointerMoving => m_Wrapper.m_InGame_PointerMoving;
        public InputAction @KeyA => m_Wrapper.m_InGame_KeyA;
        public InputAction @KeyB => m_Wrapper.m_InGame_KeyB;
        public InputActionMap Get() { return m_Wrapper.m_InGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InGameActions set) { return set.Get(); }
        public void SetCallbacks(IInGameActions instance)
        {
            if (m_Wrapper.m_InGameActionsCallbackInterface != null)
            {
                @OnClick.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnOnClick;
                @OnClick.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnOnClick;
                @OnClick.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnOnClick;
                @PointerMoving.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnPointerMoving;
                @PointerMoving.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnPointerMoving;
                @PointerMoving.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnPointerMoving;
                @KeyA.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnKeyA;
                @KeyA.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnKeyA;
                @KeyA.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnKeyA;
                @KeyB.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnKeyB;
                @KeyB.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnKeyB;
                @KeyB.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnKeyB;
            }
            m_Wrapper.m_InGameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @OnClick.started += instance.OnOnClick;
                @OnClick.performed += instance.OnOnClick;
                @OnClick.canceled += instance.OnOnClick;
                @PointerMoving.started += instance.OnPointerMoving;
                @PointerMoving.performed += instance.OnPointerMoving;
                @PointerMoving.canceled += instance.OnPointerMoving;
                @KeyA.started += instance.OnKeyA;
                @KeyA.performed += instance.OnKeyA;
                @KeyA.canceled += instance.OnKeyA;
                @KeyB.started += instance.OnKeyB;
                @KeyB.performed += instance.OnKeyB;
                @KeyB.canceled += instance.OnKeyB;
            }
        }
    }
    public InGameActions @InGame => new InGameActions(this);
    public interface IInGameActions
    {
        void OnOnClick(InputAction.CallbackContext context);
        void OnPointerMoving(InputAction.CallbackContext context);
        void OnKeyA(InputAction.CallbackContext context);
        void OnKeyB(InputAction.CallbackContext context);
    }
}
