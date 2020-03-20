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
        public DateTime ChatTimeUTC { get; set; }
        public DateTime ChatTimeLocal
        {
            get
            {
                //Korea Standard Time 이건 윈도우
                //Asia/Seoul" Linux
                return TimeZoneInfo.ConvertTimeFromUtc(ChatTimeUTC, TimeZoneConverter.TZConvert.GetTimeZoneInfo("Asia/Seoul"));
            }
        }
        public ChatTypeEnum ChatType { get; set; }
        public ChatModel()
        {
            ChatTimeUTC = DateTime.UtcNow;
        }
    }
}
