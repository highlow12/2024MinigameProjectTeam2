using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MessageHandler : NetworkBehaviour
{
    public static MessageHandler Instance;
    // public struct SharedMessage : INetworkStruct
    // {
    //     // message content
    //     public NetworkString<_512> Message;
    //     // nickname of the sender
    //     public NetworkString<_16> Sender;
    //     public int Tick;
    // }
    // [Networked]
    // [Capacity(1000)]
    // public NetworkArray<SharedMessage> Messages { get; } = MakeInitializer(
    //     new SharedMessage[] { }
    // );

    void Start()
    {
        Instance = this;
    }

    void Update()
    {

    }

    // private int GetArrayIndex()
    // {
    //     int index;
    //     for (index = 0; index < Messages.Length; index++)
    //     {
    //         if (Equals(Messages[index], default(SharedMessage)))
    //         {
    //             return index;
    //         }
    //     }
    //     // remove the oldest message
    //     Messages.Set(0, default);
    //     return 0;
    // }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ReceieveMessage(string message, string sender, int tick)
    {
        DebugConsole.Line consoleLine = new()
        {
            text = $"<b>{sender}</b>: {message}",
            messageType = DebugConsole.MessageType.Shared,
            tick = tick
        };
        DebugConsole.Instance.MergeLine(consoleLine);
    }
}
