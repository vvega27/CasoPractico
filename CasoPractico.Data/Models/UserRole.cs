﻿using System;
using System.Collections.Generic;

namespace CasoPractico.Data.Models;

public partial class UserRole
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string? Description { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
