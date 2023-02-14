using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using System;
using HughUtility;
public class HughServer : LazySingleton<HughServer>
{
    private string Scheme = "http";
    private string Host = "localhost";
    private int Port = 7350;
    private string ServerKey = "defaultkey";

    private const string SessionPrefName = "nakama.session";
    private const string DeviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";

    public IClient Client;
    public ISession Session;
    public ISocket Socket;

    public async Task ConnecToServer()
    {
        //device id login
        Client = new Nakama.Client(Scheme, Host, Port, ServerKey, UnityWebRequestAdapter.Instance);

        var authToken = PlayerPrefs.GetString(SessionPrefName);
        if (!string.IsNullOrEmpty(authToken))
        {
            var session = Nakama.Session.Restore(authToken);
            if (!session.IsExpired)
            {
                Session = session;
            }
        }

        if (Session == null)
        {
            string deviceId;
            if (PlayerPrefs.HasKey(DeviceIdentifierPrefName))
            {
                deviceId = PlayerPrefs.GetString(DeviceIdentifierPrefName);
            }
            else
            {
                deviceId = SystemInfo.deviceUniqueIdentifier;
                if (deviceId == SystemInfo.unsupportedIdentifier)
                {
                    deviceId = System.Guid.NewGuid().ToString();
                }
                PlayerPrefs.SetString(DeviceIdentifierPrefName, deviceId);
            }
            Session = await Client.AuthenticateDeviceAsync(deviceId);
            PlayerPrefs.SetString(SessionPrefName, Session.AuthToken);
        }

        Socket = Client.NewSocket();
        await Socket.ConnectAsync(Session, true);
#if UNITY_EDITOR
        Debug.Log("<color=orange><b>[HughServer]</b> Socekt Connect : {0} </color>");
#endif
    }

    public async Task Disconnect()
    {
        if (Socket != null)
        {
            await Socket.CloseAsync();
            Socket = null;
        }

        if (Session != null)
        {
            //await Client.SessionLogoutAsync(Session);
            Session = null;
        }

#if UNITY_EDITOR
        Debug.Log("<color=red><b>[HughServer]</b> Socekt DisConnect : {0} </color>");
#endif
    }
}
