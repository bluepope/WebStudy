using System;
using System.Collections.Generic;
using System.Text;

namespace BluePope.BlazorChat.Data
{
    public class ChatModel
    {
        public enum ChatTypeEnum
        {
            None,
            System,
            In,
            Out,
            Me
        }

        public int ChatRoomId { get; set; }
        public int ChatId { get; set; }
        public string Chat { get; set; }
        
        public int UserSeq { get; set; }
        public string UserName { get; set; }
        public DateTime ChatTime { get; set; }

        public ChatTypeEnum ChatType { get; set; }
        public ChatModel()
        {
            ChatTime = DateTime.UtcNow;
        }
    }
}
