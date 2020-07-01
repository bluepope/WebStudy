using System;
using System.Linq;

namespace BluePope.WebMvc.Models
{
    //web에서만 쓸거라 INotifyPropertyChanged를 상속받지는 않겠습니다
    public abstract class ModelBase
    {
        public enum ItemStateEnum
        {
            None = 0,
            Modified = 1,
            Added = 2,
            Deleted = 3
        }

        public ItemStateEnum ItemState { get; set; } = ItemStateEnum.None;
    }
}
