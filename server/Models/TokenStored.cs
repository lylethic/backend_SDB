using System;
using System.Collections.Generic;

namespace server.Models;

public partial class TokenStored
{
    public int TokenStoredId { get; set; }

    public string TokenString { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
