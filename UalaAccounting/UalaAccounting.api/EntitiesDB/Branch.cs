using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Branch
{
    public string Encodedkey { get; set; } = null!;

    public DateTime? Creationdate { get; set; }

    public string Id { get; set; } = null!;

    public DateTime? Lastmodifieddate { get; set; }

    public string? Name { get; set; }

    public string State { get; set; } = null!;

    public string? Notes { get; set; }

    public string? Phonenumber { get; set; }

    public string? Emailaddress { get; set; }

    public virtual ICollection<Loanaccount> Loanaccounts { get; set; } = new List<Loanaccount>();

    public virtual ICollection<Loantransaction> Loantransactions { get; set; } = new List<Loantransaction>();
}
