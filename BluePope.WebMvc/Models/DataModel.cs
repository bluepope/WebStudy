using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BluePope.WebMvc.Models
{
    public class DataModel : ModelBase
    {
        public string Col1 { get; set; }
        public long Col2 { get; set; }

        public static List<DataModel> GetList(string search)
        {
            //sql 및 dapper는 결국 이 작업을 db에서 가져와 해주는 것이라고 볼수 있음
            var list = new List<DataModel>();
            list.Add(new DataModel() { Col1 = "row1 col1", Col2 = 1 });
            list.Add(new DataModel() { Col1 = "row2 col2", Col2 = 2 });

            return list;
        }

        public int Insert(SqlConnection conn)
        {
            //throw new NotImplementedException();
            return 0;
        }

        public int Update(SqlConnection conn)
        {
            //throw new NotImplementedException();
            return 0;
        }
        public int Delete(SqlConnection conn)
        {
            //throw new NotImplementedException();
            return 0;
        }
    }
}
