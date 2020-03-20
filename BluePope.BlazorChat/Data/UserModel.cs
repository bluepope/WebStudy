using System;
using System.Collections.Generic;
using System.Text;

namespace BluePope.BlazorChat.Data
{
    public class UserModel
    {
        public int UserSeq { get; set; }
        public string UserName { get; set; }
        public DateTime LoginTime { get; set; }

    }
}
