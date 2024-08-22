//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Player/Input/PlayerControls.inputactions
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

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Locomotion"",
            ""id"": ""259cb00a-843b-45d5-9c2c-ef38c02d04c6"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""b059cc7a-9588-4a16-befc-98872a5506b5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""bb8fba07-f0aa-47ba-bbdb-aab97dc62dd8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""aac372a0-51be-407d-bc71-d721283feaa9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""cbf4c300-c42d-43ab-a8fa-71b980b57e4e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""ac027819-53fe-45a0-8be3-00c88a3e771d"",
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
                    ""id"": ""11db1e0c-9c3d-4713-aca4-3cb0b145158a"",
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
                    ""id"": ""9edf5ce2-5190-48aa-8322-05bd1427f93c"",
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
                    ""id"": ""0ee97593-ce80-486f-8731-2e0a8f4658d9"",
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
                    ""id"": ""d005cb46-c507-4df9-b165-9dd970ff63ef"",
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
                    ""id"": ""cec5d609-96cf-4a67-bd05-ada232ca8885"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2d5af0bb-2b2d-4e54-8292-5a9c477930a9"",
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
                    ""id"": ""77018589-cbc8-451d-84ad-9849b5284492"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Aim"",
            ""id"": ""7ee34420-8684-49a5-bef9-31685efac99a"",
            ""actions"": [
                {
                    ""name"": ""Mouse"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d148ca1e-6c23-408d-92fd-2d787de8e3c8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""cdf98eb4-f66b-4012-ae17-1be4be595a4b"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Actions"",
            ""id"": ""677f90f3-922f-4883-8293-4b1cee4b4bc4"",
            ""actions"": [
                {
                    ""name"": ""Use"",
                    ""type"": ""Button"",
                    ""id"": ""a5af5297-a5b3-4abc-a61a-f284c017386e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""d4d63b3b-666b-4245-947e-90315b1bfad6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""7064b249-4105-4106-97f1-65444e09b018"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""3948a973-249a-4aff-9ce8-14943164ce4d"",
                    ""expectedControlType"": ""Delta"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""HotBarSlot1"",
                    ""type"": ""Button"",
                    ""id"": ""06c384af-9441-4255-9d9e-5c6214e636b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HotBarSlot2"",
                    ""type"": ""Button"",
                    ""id"": ""785e76e0-d24e-401f-bb20-5e97e79bbbbd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HotBarSlot3"",
                    ""type"": ""Button"",
                    ""id"": ""edf8b0c8-ab00-4664-81b2-bcaf3fd8796c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HotBarSlot4"",
                    ""type"": ""Button"",
                    ""id"": ""43079044-3886-4fc5-b23a-5c47c551996a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HotBarSlot5"",
                    ""type"": ""Button"",
                    ""id"": ""2c209ac5-20b2-4b7e-ab4e-af37fd5dfdf7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HotBarSlot6"",
                    ""type"": ""Button"",
                    ""id"": ""3516134a-c4a9-4835-b560-04a52d78bb18"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HotBarSlot7"",
                    ""type"": ""Button"",
                    ""id"": ""9f34d409-28c0-4f4b-8979-9871cecf5fc8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HotBarSlot8"",
                    ""type"": ""Button"",
                    ""id"": ""79268748-3a37-4c9a-8c8c-55873d50f760"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ea54baba-d81e-40e7-b867-ee665ce25c15"",
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
                    ""id"": ""5ba54cd3-b2e4-4e95-9014-4a6e7eee05a4"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e19a831-9ea9-4ad0-aede-03476de221de"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HotBarSlot1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9534ddc9-b4dc-4f7c-82ff-d5306d10c9e9"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HotBarSlot2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""849ff5e6-1e4e-4ebb-9632-f0431cb0b5b8"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HotBarSlot3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6525624-ffa2-46f0-bac6-f9f3c405348d"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HotBarSlot4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dabc532f-3d8d-4b63-aac1-1c09ab9bb23a"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HotBarSlot5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86a91cff-d496-46cc-9692-d745493b5478"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HotBarSlot6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7961c46c-7d9d-43ad-8744-2a08da9d650b"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HotBarSlot7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""abd3aef7-8fc3-443f-9fff-facad2df7469"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HotBarSlot8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c9eafdf-d087-4c28-877e-105d5f323302"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6455754-bd57-4ff0-a859-baea03c6b7b3"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Locomotion
        m_Locomotion = asset.FindActionMap("Locomotion", throwIfNotFound: true);
        m_Locomotion_Movement = m_Locomotion.FindAction("Movement", throwIfNotFound: true);
        m_Locomotion_Run = m_Locomotion.FindAction("Run", throwIfNotFound: true);
        m_Locomotion_Jump = m_Locomotion.FindAction("Jump", throwIfNotFound: true);
        m_Locomotion_Crouch = m_Locomotion.FindAction("Crouch", throwIfNotFound: true);
        // Aim
        m_Aim = asset.FindActionMap("Aim", throwIfNotFound: true);
        m_Aim_Mouse = m_Aim.FindAction("Mouse", throwIfNotFound: true);
        // Actions
        m_Actions = asset.FindActionMap("Actions", throwIfNotFound: true);
        m_Actions_Use = m_Actions.FindAction("Use", throwIfNotFound: true);
        m_Actions_Shoot = m_Actions.FindAction("Shoot", throwIfNotFound: true);
        m_Actions_Aim = m_Actions.FindAction("Aim", throwIfNotFound: true);
        m_Actions_Scroll = m_Actions.FindAction("Scroll", throwIfNotFound: true);
        m_Actions_HotBarSlot1 = m_Actions.FindAction("HotBarSlot1", throwIfNotFound: true);
        m_Actions_HotBarSlot2 = m_Actions.FindAction("HotBarSlot2", throwIfNotFound: true);
        m_Actions_HotBarSlot3 = m_Actions.FindAction("HotBarSlot3", throwIfNotFound: true);
        m_Actions_HotBarSlot4 = m_Actions.FindAction("HotBarSlot4", throwIfNotFound: true);
        m_Actions_HotBarSlot5 = m_Actions.FindAction("HotBarSlot5", throwIfNotFound: true);
        m_Actions_HotBarSlot6 = m_Actions.FindAction("HotBarSlot6", throwIfNotFound: true);
        m_Actions_HotBarSlot7 = m_Actions.FindAction("HotBarSlot7", throwIfNotFound: true);
        m_Actions_HotBarSlot8 = m_Actions.FindAction("HotBarSlot8", throwIfNotFound: true);
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

    // Locomotion
    private readonly InputActionMap m_Locomotion;
    private List<ILocomotionActions> m_LocomotionActionsCallbackInterfaces = new List<ILocomotionActions>();
    private readonly InputAction m_Locomotion_Movement;
    private readonly InputAction m_Locomotion_Run;
    private readonly InputAction m_Locomotion_Jump;
    private readonly InputAction m_Locomotion_Crouch;
    public struct LocomotionActions
    {
        private @PlayerControls m_Wrapper;
        public LocomotionActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Locomotion_Movement;
        public InputAction @Run => m_Wrapper.m_Locomotion_Run;
        public InputAction @Jump => m_Wrapper.m_Locomotion_Jump;
        public InputAction @Crouch => m_Wrapper.m_Locomotion_Crouch;
        public InputActionMap Get() { return m_Wrapper.m_Locomotion; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(LocomotionActions set) { return set.Get(); }
        public void AddCallbacks(ILocomotionActions instance)
        {
            if (instance == null || m_Wrapper.m_LocomotionActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_LocomotionActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Run.started += instance.OnRun;
            @Run.performed += instance.OnRun;
            @Run.canceled += instance.OnRun;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Crouch.started += instance.OnCrouch;
            @Crouch.performed += instance.OnCrouch;
            @Crouch.canceled += instance.OnCrouch;
        }

        private void UnregisterCallbacks(ILocomotionActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Run.started -= instance.OnRun;
            @Run.performed -= instance.OnRun;
            @Run.canceled -= instance.OnRun;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Crouch.started -= instance.OnCrouch;
            @Crouch.performed -= instance.OnCrouch;
            @Crouch.canceled -= instance.OnCrouch;
        }

        public void RemoveCallbacks(ILocomotionActions instance)
        {
            if (m_Wrapper.m_LocomotionActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ILocomotionActions instance)
        {
            foreach (var item in m_Wrapper.m_LocomotionActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_LocomotionActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public LocomotionActions @Locomotion => new LocomotionActions(this);

    // Aim
    private readonly InputActionMap m_Aim;
    private List<IAimActions> m_AimActionsCallbackInterfaces = new List<IAimActions>();
    private readonly InputAction m_Aim_Mouse;
    public struct AimActions
    {
        private @PlayerControls m_Wrapper;
        public AimActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Mouse => m_Wrapper.m_Aim_Mouse;
        public InputActionMap Get() { return m_Wrapper.m_Aim; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(AimActions set) { return set.Get(); }
        public void AddCallbacks(IAimActions instance)
        {
            if (instance == null || m_Wrapper.m_AimActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_AimActionsCallbackInterfaces.Add(instance);
            @Mouse.started += instance.OnMouse;
            @Mouse.performed += instance.OnMouse;
            @Mouse.canceled += instance.OnMouse;
        }

        private void UnregisterCallbacks(IAimActions instance)
        {
            @Mouse.started -= instance.OnMouse;
            @Mouse.performed -= instance.OnMouse;
            @Mouse.canceled -= instance.OnMouse;
        }

        public void RemoveCallbacks(IAimActions instance)
        {
            if (m_Wrapper.m_AimActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IAimActions instance)
        {
            foreach (var item in m_Wrapper.m_AimActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_AimActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public AimActions @Aim => new AimActions(this);

    // Actions
    private readonly InputActionMap m_Actions;
    private List<IActionsActions> m_ActionsActionsCallbackInterfaces = new List<IActionsActions>();
    private readonly InputAction m_Actions_Use;
    private readonly InputAction m_Actions_Shoot;
    private readonly InputAction m_Actions_Aim;
    private readonly InputAction m_Actions_Scroll;
    private readonly InputAction m_Actions_HotBarSlot1;
    private readonly InputAction m_Actions_HotBarSlot2;
    private readonly InputAction m_Actions_HotBarSlot3;
    private readonly InputAction m_Actions_HotBarSlot4;
    private readonly InputAction m_Actions_HotBarSlot5;
    private readonly InputAction m_Actions_HotBarSlot6;
    private readonly InputAction m_Actions_HotBarSlot7;
    private readonly InputAction m_Actions_HotBarSlot8;
    public struct ActionsActions
    {
        private @PlayerControls m_Wrapper;
        public ActionsActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Use => m_Wrapper.m_Actions_Use;
        public InputAction @Shoot => m_Wrapper.m_Actions_Shoot;
        public InputAction @Aim => m_Wrapper.m_Actions_Aim;
        public InputAction @Scroll => m_Wrapper.m_Actions_Scroll;
        public InputAction @HotBarSlot1 => m_Wrapper.m_Actions_HotBarSlot1;
        public InputAction @HotBarSlot2 => m_Wrapper.m_Actions_HotBarSlot2;
        public InputAction @HotBarSlot3 => m_Wrapper.m_Actions_HotBarSlot3;
        public InputAction @HotBarSlot4 => m_Wrapper.m_Actions_HotBarSlot4;
        public InputAction @HotBarSlot5 => m_Wrapper.m_Actions_HotBarSlot5;
        public InputAction @HotBarSlot6 => m_Wrapper.m_Actions_HotBarSlot6;
        public InputAction @HotBarSlot7 => m_Wrapper.m_Actions_HotBarSlot7;
        public InputAction @HotBarSlot8 => m_Wrapper.m_Actions_HotBarSlot8;
        public InputActionMap Get() { return m_Wrapper.m_Actions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ActionsActions set) { return set.Get(); }
        public void AddCallbacks(IActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_ActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_ActionsActionsCallbackInterfaces.Add(instance);
            @Use.started += instance.OnUse;
            @Use.performed += instance.OnUse;
            @Use.canceled += instance.OnUse;
            @Shoot.started += instance.OnShoot;
            @Shoot.performed += instance.OnShoot;
            @Shoot.canceled += instance.OnShoot;
            @Aim.started += instance.OnAim;
            @Aim.performed += instance.OnAim;
            @Aim.canceled += instance.OnAim;
            @Scroll.started += instance.OnScroll;
            @Scroll.performed += instance.OnScroll;
            @Scroll.canceled += instance.OnScroll;
            @HotBarSlot1.started += instance.OnHotBarSlot1;
            @HotBarSlot1.performed += instance.OnHotBarSlot1;
            @HotBarSlot1.canceled += instance.OnHotBarSlot1;
            @HotBarSlot2.started += instance.OnHotBarSlot2;
            @HotBarSlot2.performed += instance.OnHotBarSlot2;
            @HotBarSlot2.canceled += instance.OnHotBarSlot2;
            @HotBarSlot3.started += instance.OnHotBarSlot3;
            @HotBarSlot3.performed += instance.OnHotBarSlot3;
            @HotBarSlot3.canceled += instance.OnHotBarSlot3;
            @HotBarSlot4.started += instance.OnHotBarSlot4;
            @HotBarSlot4.performed += instance.OnHotBarSlot4;
            @HotBarSlot4.canceled += instance.OnHotBarSlot4;
            @HotBarSlot5.started += instance.OnHotBarSlot5;
            @HotBarSlot5.performed += instance.OnHotBarSlot5;
            @HotBarSlot5.canceled += instance.OnHotBarSlot5;
            @HotBarSlot6.started += instance.OnHotBarSlot6;
            @HotBarSlot6.performed += instance.OnHotBarSlot6;
            @HotBarSlot6.canceled += instance.OnHotBarSlot6;
            @HotBarSlot7.started += instance.OnHotBarSlot7;
            @HotBarSlot7.performed += instance.OnHotBarSlot7;
            @HotBarSlot7.canceled += instance.OnHotBarSlot7;
            @HotBarSlot8.started += instance.OnHotBarSlot8;
            @HotBarSlot8.performed += instance.OnHotBarSlot8;
            @HotBarSlot8.canceled += instance.OnHotBarSlot8;
        }

        private void UnregisterCallbacks(IActionsActions instance)
        {
            @Use.started -= instance.OnUse;
            @Use.performed -= instance.OnUse;
            @Use.canceled -= instance.OnUse;
            @Shoot.started -= instance.OnShoot;
            @Shoot.performed -= instance.OnShoot;
            @Shoot.canceled -= instance.OnShoot;
            @Aim.started -= instance.OnAim;
            @Aim.performed -= instance.OnAim;
            @Aim.canceled -= instance.OnAim;
            @Scroll.started -= instance.OnScroll;
            @Scroll.performed -= instance.OnScroll;
            @Scroll.canceled -= instance.OnScroll;
            @HotBarSlot1.started -= instance.OnHotBarSlot1;
            @HotBarSlot1.performed -= instance.OnHotBarSlot1;
            @HotBarSlot1.canceled -= instance.OnHotBarSlot1;
            @HotBarSlot2.started -= instance.OnHotBarSlot2;
            @HotBarSlot2.performed -= instance.OnHotBarSlot2;
            @HotBarSlot2.canceled -= instance.OnHotBarSlot2;
            @HotBarSlot3.started -= instance.OnHotBarSlot3;
            @HotBarSlot3.performed -= instance.OnHotBarSlot3;
            @HotBarSlot3.canceled -= instance.OnHotBarSlot3;
            @HotBarSlot4.started -= instance.OnHotBarSlot4;
            @HotBarSlot4.performed -= instance.OnHotBarSlot4;
            @HotBarSlot4.canceled -= instance.OnHotBarSlot4;
            @HotBarSlot5.started -= instance.OnHotBarSlot5;
            @HotBarSlot5.performed -= instance.OnHotBarSlot5;
            @HotBarSlot5.canceled -= instance.OnHotBarSlot5;
            @HotBarSlot6.started -= instance.OnHotBarSlot6;
            @HotBarSlot6.performed -= instance.OnHotBarSlot6;
            @HotBarSlot6.canceled -= instance.OnHotBarSlot6;
            @HotBarSlot7.started -= instance.OnHotBarSlot7;
            @HotBarSlot7.performed -= instance.OnHotBarSlot7;
            @HotBarSlot7.canceled -= instance.OnHotBarSlot7;
            @HotBarSlot8.started -= instance.OnHotBarSlot8;
            @HotBarSlot8.performed -= instance.OnHotBarSlot8;
            @HotBarSlot8.canceled -= instance.OnHotBarSlot8;
        }

        public void RemoveCallbacks(IActionsActions instance)
        {
            if (m_Wrapper.m_ActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_ActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_ActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public ActionsActions @Actions => new ActionsActions(this);
    public interface ILocomotionActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
    }
    public interface IAimActions
    {
        void OnMouse(InputAction.CallbackContext context);
    }
    public interface IActionsActions
    {
        void OnUse(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
        void OnHotBarSlot1(InputAction.CallbackContext context);
        void OnHotBarSlot2(InputAction.CallbackContext context);
        void OnHotBarSlot3(InputAction.CallbackContext context);
        void OnHotBarSlot4(InputAction.CallbackContext context);
        void OnHotBarSlot5(InputAction.CallbackContext context);
        void OnHotBarSlot6(InputAction.CallbackContext context);
        void OnHotBarSlot7(InputAction.CallbackContext context);
        void OnHotBarSlot8(InputAction.CallbackContext context);
    }
}
