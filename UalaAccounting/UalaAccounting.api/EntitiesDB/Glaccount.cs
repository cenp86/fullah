using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Glaccount
{
    public string Encodedkey { get; set; } = null!;

    public ulong? Activated { get; set; }

    public DateTime? Creationdate { get; set; }

    public string? Description { get; set; }

    public string? Glcode { get; set; }

    public DateTime? Lastmodifieddate { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Usage { get; set; } = null!;

    public string? Migrationeventkey { get; set; }

    public ulong? Allowmanualjournalentries { get; set; }

    public ulong? Striptrailingzeros { get; set; }

    public string? Currencycode { get; set; }

    public virtual ICollection<Gljournalentry> Gljournalentries { get; set; } = new List<Gljournalentry>();
}
