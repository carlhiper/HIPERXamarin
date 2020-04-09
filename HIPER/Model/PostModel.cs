using System;
using SQLite;


namespace HIPER.Model
{
    public class PostModel
    {

        [PrimaryKey]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Post { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
