using System;

namespace Poll.Data.DataModel
{
    public class Poll
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Poll Poll { get; set; }
    }
}
