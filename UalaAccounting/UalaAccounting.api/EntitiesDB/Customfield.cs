using System;
using System.Collections.Generic;

namespace UalaAccounting.api.EntitiesDB;

public partial class Customfield
{
    public string Encodedkey { get; set; } = null!;

    public string? Id { get; set; }

    public DateTime? Creationdate { get; set; }

    public DateTime? Lastmodifieddate { get; set; }

    public string? Datatype { get; set; }

    public ulong? Isdefault { get; set; }

    public ulong? Isrequired { get; set; }

    public string? Name { get; set; }

    public byte[]? Values { get; set; }

    public byte[]? Amounts { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public string Valuelength { get; set; } = null!;

    public int? Indexinlist { get; set; }

    public string CustomfieldsetEncodedkeyOid { get; set; } = null!;

    public string State { get; set; } = null!;

    public string? Validationpattern { get; set; }

    public string? Viewusagerightskey { get; set; }

    public string? Editusagerightskey { get; set; }

    public string? Builtincustomfieldid { get; set; }

    public ulong Unique { get; set; }

    public string Temporaryid { get; set; } = null!;

    public bool Availableforall { get; set; }

    public uint IdGenerated { get; set; }
}
