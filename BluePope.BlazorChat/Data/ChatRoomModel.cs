using System;
using System.Collections.Generic;
using System.Text;

namespace BluePope.BlazorChat.Data
{
    public class ChatRoomModel
    {
        public List<ChatModel> ChatData { get; set; }

        public List<string> UserList { get; set; }

    }
}
