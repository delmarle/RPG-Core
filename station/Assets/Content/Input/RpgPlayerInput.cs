//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Content/Input/RpgPlayerInput.inputactions
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

public partial class @RpgPlayerInput : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @RpgPlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""RpgPlayerInput"",
    ""maps"": [
        {
            ""name"": ""Character"",
            ""id"": ""42ca3914-983d-4017-9ce0-3106e9eae071"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""ad9a0e34-7d90-49ef-aceb-9869972eecdd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""ba69dc20-b67f-4895-a1f0-7164642ab019"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""b8eeb013-2d91-462a-9475-acffd7a8b834"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Sneak"",
                    ""type"": ""Button"",
                    ""id"": ""f29d5807-9307-4ea7-8d47-c340446aea49"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""InteractionPrimary"",
                    ""type"": ""Button"",
                    ""id"": ""a64c5095-ba8b-45d4-9407-702344b3b96a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""InteractionSecondary"",
                    ""type"": ""Button"",
                    ""id"": ""7df24e9b-91e8-4eb5-a455-01beb9d883a2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5b90b141-5f78-4470-9d80-6268bbe58f38"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""7e162ad7-2dd7-4bf4-a6c0-038a13ad5481"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""803cf479-b177-4a97-855c-7368613fb352"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""53350e61-19fc-4d26-9140-cac88eed6879"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7d7197b0-0cb8-495d-8884-4673419a51e3"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""dfd8656c-7e91-408e-a4c6-285b72265ac2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0c3bce5d-c8aa-45f8-91bb-dcbf52a2ca8b"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd4f2f4b-a62c-4050-b9b5-dac21879d9b2"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sneak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff070350-66f4-40e8-86ce-9476a5360237"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InteractionPrimary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6fe8f389-c83b-4449-9ada-344745281f07"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InteractionPrimary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c22a8e3-96e5-4c5f-af67-f10b45ffe36f"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InteractionSecondary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Character
        m_Character = asset.FindActionMap("Character", throwIfNotFound: true);
        m_Character_Jump = m_Character.FindAction("Jump", throwIfNotFound: true);
        m_Character_Movement = m_Character.FindAction("Movement", throwIfNotFound: true);
        m_Character_Sprint = m_Character.FindAction("Sprint", throwIfNotFound: true);
        m_Character_Sneak = m_Character.FindAction("Sneak", throwIfNotFound: true);
        m_Character_InteractionPrimary = m_Character.FindAction("InteractionPrimary", throwIfNotFound: true);
        m_Character_InteractionSecondary = m_Character.FindAction("InteractionSecondary", throwIfNotFound: true);
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

    // Character
    private readonly InputActionMap m_Character;
    private ICharacterActions m_CharacterActionsCallbackInterface;
    private readonly InputAction m_Character_Jump;
    private readonly InputAction m_Character_Movement;
    private readonly InputAction m_Character_Sprint;
    private readonly InputAction m_Character_Sneak;
    private readonly InputAction m_Character_InteractionPrimary;
    private readonly InputAction m_Character_InteractionSecondary;
    public struct CharacterActions
    {
        private @RpgPlayerInput m_Wrapper;
        public CharacterActions(@RpgPlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_Character_Jump;
        public InputAction @Movement => m_Wrapper.m_Character_Movement;
        public InputAction @Sprint => m_Wrapper.m_Character_Sprint;
        public InputAction @Sneak => m_Wrapper.m_Character_Sneak;
        public InputAction @InteractionPrimary => m_Wrapper.m_Character_InteractionPrimary;
        public InputAction @InteractionSecondary => m_Wrapper.m_Character_InteractionSecondary;
        public InputActionMap Get() { return m_Wrapper.m_Character; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterActions set) { return set.Get(); }
        public void SetCallbacks(ICharacterActions instance)
        {
            if (m_Wrapper.m_CharacterActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnJump;
                @Movement.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMovement;
                @Sprint.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnSprint;
                @Sneak.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnSneak;
                @Sneak.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnSneak;
                @Sneak.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnSneak;
                @InteractionPrimary.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnInteractionPrimary;
                @InteractionPrimary.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnInteractionPrimary;
                @InteractionPrimary.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnInteractionPrimary;
                @InteractionSecondary.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnInteractionSecondary;
                @InteractionSecondary.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnInteractionSecondary;
                @InteractionSecondary.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnInteractionSecondary;
            }
            m_Wrapper.m_CharacterActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Sneak.started += instance.OnSneak;
                @Sneak.performed += instance.OnSneak;
                @Sneak.canceled += instance.OnSneak;
                @InteractionPrimary.started += instance.OnInteractionPrimary;
                @InteractionPrimary.performed += instance.OnInteractionPrimary;
                @InteractionPrimary.canceled += instance.OnInteractionPrimary;
                @InteractionSecondary.started += instance.OnInteractionSecondary;
                @InteractionSecondary.performed += instance.OnInteractionSecondary;
                @InteractionSecondary.canceled += instance.OnInteractionSecondary;
            }
        }
    }
    public CharacterActions @Character => new CharacterActions(this);
    public interface ICharacterActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnSneak(InputAction.CallbackContext context);
        void OnInteractionPrimary(InputAction.CallbackContext context);
        void OnInteractionSecondary(InputAction.CallbackContext context);
    }
}
