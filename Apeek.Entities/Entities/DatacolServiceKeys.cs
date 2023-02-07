using System;
using System.Collections.Generic;
using System.Linq;
namespace Apeek.Entities.Entities
{
    public class DatacolServiceKeys
    {
        public virtual int Id { get; set; }
        public virtual int ServiceId { get; set; }
        public virtual string Keys { get; set; }
        private List<string> _keyList { get; set; }
        public virtual List<string> GetKeyList()
        {
            if (_keyList == null)
            {
                if(!string.IsNullOrWhiteSpace(Keys))
                    _keyList = Keys.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList();
                else
                {
                    _keyList = new List<string>();
                }
            }
            return _keyList;
        }
    }
}