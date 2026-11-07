using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay.Contracts.CategoryRequest
{
    public class RequestCatDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
