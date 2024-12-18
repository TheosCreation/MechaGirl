﻿#if !DISABLESTEAMWORKS && HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH)
using UnityEngine;
using UnityEngine.Events;
using Steamworks;
using UserAPI = HeathenEngineering.SteamworksIntegration.API.User.Client;
using FriendsAPI = HeathenEngineering.SteamworksIntegration.API.Friends.Client;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    /// <summary>
    /// Applies the avatar of the indicated user to the attached RawImage
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class SetUserAvatar : MonoBehaviour
    {
        private UnityEngine.UI.Image image;
        [SerializeField]
        [Tooltip("Should the component load the local user's avatar on Start.\nIf false you must call LoadAvatar and provide the ID of the user to load")]
        private bool useLocalUser;
        public UnityEvent evtLoaded;

        public UserData UserData
        {
            get
            {
                return currentUser;
            }
            set
            {
                LoadAvatar(value);
            }
        }

        private UserData currentUser;

        private void OnEnable()
        {
            FriendsAPI.EventPersonaStateChange.AddListener(HandlePersonaStateChange);
        }

        private void OnDisable()
        {
            FriendsAPI.EventPersonaStateChange.RemoveListener(HandlePersonaStateChange);
        }

        private void Start()
        {
            image = GetComponent<UnityEngine.UI.Image>();

            if (useLocalUser)
            {
                var user = UserAPI.Id;
                LoadAvatar(user);
            }
        }

        private void HandlePersonaStateChange(PersonaStateChange_t arg)
        {
            if (FriendsAPI.PersonaChangeHasFlag(arg.m_nChangeFlags, EPersonaChange.k_EPersonaChangeAvatar)
                && arg.m_ulSteamID == currentUser.SteamId)
            {
                UserData user = arg.m_ulSteamID;
                if(user == currentUser)
                {
                    user.LoadAvatar((t) =>
                    {
                        image.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
                        evtLoaded?.Invoke();
                    });
                }
            }
        }

        public void LoadAvatar(UserData user) => user.LoadAvatar((t) =>
        {
            if(image == null)
                image = GetComponent<UnityEngine.UI.Image>();

            if (image == null)
                return;

            currentUser = user;
            image.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
            evtLoaded?.Invoke();
        });

        public void LoadAvatar(CSteamID user) => UserData.Get(user).LoadAvatar((t) =>
        {
            if (image == null)
                image = GetComponent<UnityEngine.UI.Image>();

            if (image == null)
                return;

            currentUser = user;
            image.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
            evtLoaded?.Invoke();
        });

        public void LoadAvatar(ulong user) => UserData.Get(user).LoadAvatar((t) =>
        {
            if (image == null)
                image = GetComponent<UnityEngine.UI.Image>();

            if (image == null)
                return;

            currentUser = user;
            image.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
            evtLoaded?.Invoke();
        });
    }
}
#endif