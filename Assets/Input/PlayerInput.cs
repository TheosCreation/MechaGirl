//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input/PlayerInput.inputactions
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

public partial class @PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""InGame"",
            ""id"": ""d3764e3c-a15e-4574-866e-d8992af133fe"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f9b38d1d-5ae0-4231-a4ea-880eb81f8268"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a6ff5d2a-65ef-4e0e-9c2c-e692153eb7fd"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""c778b08f-0798-412f-8ee3-a8586a6eac06"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""1799dd3e-3ffe-4c55-8a4d-b36f1477e650"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponSwitch"",
                    ""type"": ""Value"",
                    ""id"": ""69428700-def8-41dc-89d5-49ba5fccb4d5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""WeaponThrow"",
                    ""type"": ""Button"",
                    ""id"": ""2b71d7bc-2a88-4e4c-8485-f5863e3e1f0f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponPickUp"",
                    ""type"": ""Button"",
                    ""id"": ""74ad1caf-5e20-4bdd-9917-b60a1094bbdf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""1352952f-75f5-4d39-b2fc-d17b406f268e"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f4d0aa40-bcee-4fff-b677-718c047b353f"",
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
                    ""id"": ""6ae996b2-25e7-49f3-826e-c7f15e75f7d3"",
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
                    ""id"": ""6446a4ca-ddf8-49f1-8b94-c7d40e9ea083"",
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
                    ""id"": ""abb49834-ff63-496c-9089-1fb3849d0266"",
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
                    ""id"": ""fca82a1e-8d35-4b39-b273-165b05e0e5ea"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6aa64499-4f40-4fd4-b884-39f4a49d778a"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b96dea5c-87be-4169-bce2-b80ab2d63f46"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee489d61-8537-4b12-a155-6425f6a3d20f"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85080e82-3d9e-48f8-86cc-12d852982640"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WeaponThrow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62dd9236-5061-462d-a28b-837ac536f4b6"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WeaponSwitch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2b993a44-efc3-48c8-bc27-aef050df79bb"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WeaponPickUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Ui"",
            ""id"": ""6f8fc243-7e27-4192-8769-3d1a1790692a"",
            ""actions"": [
                {
                    ""name"": ""Exit"",
                    ""type"": ""Button"",
                    ""id"": ""f64c6a0d-e736-4d25-91b9-126e82656bf7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""af3c9850-c894-492a-b341-c90e53559799"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Restart"",
                    ""type"": ""Button"",
                    ""id"": ""d86ddb09-a3b4-46b9-a607-503649abddc6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""488fc977-639a-4cbc-8055-6e70224f96bc"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""93ebd959-efc0-4b23-881e-08d3c46f8e87"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6d32b069-7929-4463-b3c8-2c49c885c0aa"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Restart"",
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
        m_InGame_Movement = m_InGame.FindAction("Movement", throwIfNotFound: true);
        m_InGame_Look = m_InGame.FindAction("Look", throwIfNotFound: true);
        m_InGame_Jump = m_InGame.FindAction("Jump", throwIfNotFound: true);
        m_InGame_Shoot = m_InGame.FindAction("Shoot", throwIfNotFound: true);
        m_InGame_WeaponSwitch = m_InGame.FindAction("WeaponSwitch", throwIfNotFound: true);
        m_InGame_WeaponThrow = m_InGame.FindAction("WeaponThrow", throwIfNotFound: true);
        m_InGame_WeaponPickUp = m_InGame.FindAction("WeaponPickUp", throwIfNotFound: true);
        // Ui
        m_Ui = asset.FindActionMap("Ui", throwIfNotFound: true);
        m_Ui_Exit = m_Ui.FindAction("Exit", throwIfNotFound: true);
        m_Ui_Pause = m_Ui.FindAction("Pause", throwIfNotFound: true);
        m_Ui_Restart = m_Ui.FindAction("Restart", throwIfNotFound: true);
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

    // InGame
    private readonly InputActionMap m_InGame;
    private List<IInGameActions> m_InGameActionsCallbackInterfaces = new List<IInGameActions>();
    private readonly InputAction m_InGame_Movement;
    private readonly InputAction m_InGame_Look;
    private readonly InputAction m_InGame_Jump;
    private readonly InputAction m_InGame_Shoot;
    private readonly InputAction m_InGame_WeaponSwitch;
    private readonly InputAction m_InGame_WeaponThrow;
    private readonly InputAction m_InGame_WeaponPickUp;
    public struct InGameActions
    {
        private @PlayerInput m_Wrapper;
        public InGameActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_InGame_Movement;
        public InputAction @Look => m_Wrapper.m_InGame_Look;
        public InputAction @Jump => m_Wrapper.m_InGame_Jump;
        public InputAction @Shoot => m_Wrapper.m_InGame_Shoot;
        public InputAction @WeaponSwitch => m_Wrapper.m_InGame_WeaponSwitch;
        public InputAction @WeaponThrow => m_Wrapper.m_InGame_WeaponThrow;
        public InputAction @WeaponPickUp => m_Wrapper.m_InGame_WeaponPickUp;
        public InputActionMap Get() { return m_Wrapper.m_InGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InGameActions set) { return set.Get(); }
        public void AddCallbacks(IInGameActions instance)
        {
            if (instance == null || m_Wrapper.m_InGameActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_InGameActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Shoot.started += instance.OnShoot;
            @Shoot.performed += instance.OnShoot;
            @Shoot.canceled += instance.OnShoot;
            @WeaponSwitch.started += instance.OnWeaponSwitch;
            @WeaponSwitch.performed += instance.OnWeaponSwitch;
            @WeaponSwitch.canceled += instance.OnWeaponSwitch;
            @WeaponThrow.started += instance.OnWeaponThrow;
            @WeaponThrow.performed += instance.OnWeaponThrow;
            @WeaponThrow.canceled += instance.OnWeaponThrow;
            @WeaponPickUp.started += instance.OnWeaponPickUp;
            @WeaponPickUp.performed += instance.OnWeaponPickUp;
            @WeaponPickUp.canceled += instance.OnWeaponPickUp;
        }

        private void UnregisterCallbacks(IInGameActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Shoot.started -= instance.OnShoot;
            @Shoot.performed -= instance.OnShoot;
            @Shoot.canceled -= instance.OnShoot;
            @WeaponSwitch.started -= instance.OnWeaponSwitch;
            @WeaponSwitch.performed -= instance.OnWeaponSwitch;
            @WeaponSwitch.canceled -= instance.OnWeaponSwitch;
            @WeaponThrow.started -= instance.OnWeaponThrow;
            @WeaponThrow.performed -= instance.OnWeaponThrow;
            @WeaponThrow.canceled -= instance.OnWeaponThrow;
            @WeaponPickUp.started -= instance.OnWeaponPickUp;
            @WeaponPickUp.performed -= instance.OnWeaponPickUp;
            @WeaponPickUp.canceled -= instance.OnWeaponPickUp;
        }

        public void RemoveCallbacks(IInGameActions instance)
        {
            if (m_Wrapper.m_InGameActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IInGameActions instance)
        {
            foreach (var item in m_Wrapper.m_InGameActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_InGameActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public InGameActions @InGame => new InGameActions(this);

    // Ui
    private readonly InputActionMap m_Ui;
    private List<IUiActions> m_UiActionsCallbackInterfaces = new List<IUiActions>();
    private readonly InputAction m_Ui_Exit;
    private readonly InputAction m_Ui_Pause;
    private readonly InputAction m_Ui_Restart;
    public struct UiActions
    {
        private @PlayerInput m_Wrapper;
        public UiActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Exit => m_Wrapper.m_Ui_Exit;
        public InputAction @Pause => m_Wrapper.m_Ui_Pause;
        public InputAction @Restart => m_Wrapper.m_Ui_Restart;
        public InputActionMap Get() { return m_Wrapper.m_Ui; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UiActions set) { return set.Get(); }
        public void AddCallbacks(IUiActions instance)
        {
            if (instance == null || m_Wrapper.m_UiActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UiActionsCallbackInterfaces.Add(instance);
            @Exit.started += instance.OnExit;
            @Exit.performed += instance.OnExit;
            @Exit.canceled += instance.OnExit;
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @Restart.started += instance.OnRestart;
            @Restart.performed += instance.OnRestart;
            @Restart.canceled += instance.OnRestart;
        }

        private void UnregisterCallbacks(IUiActions instance)
        {
            @Exit.started -= instance.OnExit;
            @Exit.performed -= instance.OnExit;
            @Exit.canceled -= instance.OnExit;
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @Restart.started -= instance.OnRestart;
            @Restart.performed -= instance.OnRestart;
            @Restart.canceled -= instance.OnRestart;
        }

        public void RemoveCallbacks(IUiActions instance)
        {
            if (m_Wrapper.m_UiActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUiActions instance)
        {
            foreach (var item in m_Wrapper.m_UiActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UiActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UiActions @Ui => new UiActions(this);
    public interface IInGameActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnWeaponSwitch(InputAction.CallbackContext context);
        void OnWeaponThrow(InputAction.CallbackContext context);
        void OnWeaponPickUp(InputAction.CallbackContext context);
    }
    public interface IUiActions
    {
        void OnExit(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnRestart(InputAction.CallbackContext context);
    }
}
