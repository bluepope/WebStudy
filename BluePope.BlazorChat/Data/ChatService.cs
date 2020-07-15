using System;
using System.Collections.Generic;

namespace BluePope.BlazorChat.Data
{
    public class ChatService
    {
        public List<ChatModel> ChatRoom { get; private set; } = new List<ChatModel>();

        public event EventHandler OnChatUpdate;

        public bool AppendChat(ChatModel chat)
        {
            //if (ChatRoomList.ContainsKey(chatRoomSeq) == false)
            //return false;

            //ChatRoomList[chatRoomSeq].Add(chat);

            ChatRoom.Add(chat);

            OnChatUpdate?.Invoke(null, EventArgs.Empty);

            Console.WriteLine(OnChatUpdate?.GetInvocationList().Length);

            return true;
        }
    }
}
