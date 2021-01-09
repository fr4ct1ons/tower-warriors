// GENERATED AUTOMATICALLY FROM 'Assets/Input Maps/DialogueInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @DialogueInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @DialogueInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""DialogueInputs"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""ee857420-eb07-48cc-bb38-83da0adbbc31"",
            ""actions"": [
                {
                    ""name"": ""Advance"",
                    ""type"": ""Button"",
                    ""id"": ""523c0fbd-c157-451e-8c5b-4db54acf390b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a2c5ac45-8795-4f47-bff1-25687bb90ea5"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Advance"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Advance = m_Gameplay.FindAction("Advance", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Advance;
    public struct GameplayActions
    {
        private @DialogueInputs m_Wrapper;
        public GameplayActions(@DialogueInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Advance => m_Wrapper.m_Gameplay_Advance;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Advance.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAdvance;
                @Advance.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAdvance;
                @Advance.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAdvance;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Advance.started += instance.OnAdvance;
                @Advance.performed += instance.OnAdvance;
                @Advance.canceled += instance.OnAdvance;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnAdvance(InputAction.CallbackContext context);
    }
}
