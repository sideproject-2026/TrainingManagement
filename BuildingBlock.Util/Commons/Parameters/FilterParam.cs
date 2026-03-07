using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlock.Util.Commons.Parameters;

public class FilterParam
{
    public static FilterParam Default => new FilterParam() { 
        PageNumber = 1,
        PageSize = 10,
        IsPaginate = true,
    };

    public static FilterParam NoPaginateDefault => new FilterParam() { 
        PageNumber = 1,
        PageSize = 10,
        IsPaginate = false,
    };
    public FilterParam() { }

    public Dictionary<string,object> Filters { get; set; } = new Dictionary<string,object>();
    public int PageSize { get; set; }
    public int PageNumber { get; set; }

    public bool IsPaginate { get; set; }
}
