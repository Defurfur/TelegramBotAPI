using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentCommandHandler;

internal struct ConditionItem<TSource>
{
    public Func<TSource, Task<bool>>? AsyncCondition { get; set; }
    public Func<TSource, bool>? SyncCondition { get; set; }
    private bool _isValid => (AsyncCondition is not null & SyncCondition is null) 
        | (SyncCondition is not null & AsyncCondition is null);
    public bool IsValid { get => _isValid; }

}
