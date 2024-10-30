using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace UalaAccounting.api.EntitiesDB;

public partial class ContaContext : DbContext
{
    public ContaContext()
    {
    }

    public ContaContext(DbContextOptions<ContaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accountchart> Accountcharts { get; set; }

    public virtual DbSet<Accountinghubentry> Accountinghubentries { get; set; }

    public virtual DbSet<Accountinghubexit> Accountinghubexits { get; set; }

    public virtual DbSet<Accountinghublog> Accountinghublogs { get; set; }

    public virtual DbSet<Accountinghubprocesscontrol> Accountinghubprocesscontrols { get; set; }

    public virtual DbSet<Accountinginterestaccrualbreakdown> Accountinginterestaccrualbreakdowns { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Configurationaccountinghub> Configurationaccountinghubs { get; set; }

    public virtual DbSet<Configurationsheet> Configurationsheets { get; set; }

    public virtual DbSet<Configurationsheetcolumn> Configurationsheetcolumns { get; set; }

    public virtual DbSet<Customfield> Customfields { get; set; }

    public virtual DbSet<Customfieldvalue> Customfieldvalues { get; set; }

    public virtual DbSet<Glaccount> Glaccounts { get; set; }

    public virtual DbSet<Gljournalentry> Gljournalentries { get; set; }

    public virtual DbSet<Loanaccount> Loanaccounts { get; set; }

    public virtual DbSet<Loanaccounthistory> Loanaccounthistories { get; set; }

    public virtual DbSet<Loanproduct> Loanproducts { get; set; }

    public virtual DbSet<Loantransaction> Loantransactions { get; set; }

    public virtual DbSet<Productaccountinghub> Productaccountinghubs { get; set; }

    public virtual DbSet<Sheetparametrization> Sheetparametrizations { get; set; }

    public virtual DbSet<Transactionchannel> Transactionchannels { get; set; }

    public virtual DbSet<Transactiondetail> Transactiondetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Accountchart>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity.ToTable("accountchart");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(45)
                .HasColumnName("ENCODEDKEY");
            entity.Property(e => e.Accountchartid)
                .HasMaxLength(45)
                .HasColumnName("ACCOUNTCHARTID");
            entity.Property(e => e.Enable).HasColumnName("ENABLE");
            entity.Property(e => e.Glcode)
                .HasMaxLength(45)
                .HasColumnName("GLCODE");
            entity.Property(e => e.Glname)
                .HasMaxLength(45)
                .HasColumnName("GLNAME");
            entity.Property(e => e.Owner)
                .HasMaxLength(45)
                .HasColumnName("OWNER");
            entity.Property(e => e.Type)
                .HasMaxLength(45)
                .HasColumnName("TYPE");
        });

        modelBuilder.Entity<Accountinghubentry>(entity =>
        {
            entity.HasKey(e => new { e.Loanid, e.Entryid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("accountinghubentry");

            entity.HasIndex(e => e.Creationdate, "idx_rpaoriginal_CREATIONDATE");

            entity.HasIndex(e => e.Reversaltransactionid, "idx_rpaoriginal_REVERSALTRANSACTIONID");

            entity.Property(e => e.Loanid)
                .HasMaxLength(32)
                .HasColumnName("LOANID")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Entryid).HasColumnName("ENTRYID");
            entity.Property(e => e.Actualstage)
                .HasColumnType("text")
                .HasColumnName("ACTUALSTAGE")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Amount)
                .HasPrecision(43, 2)
                .HasColumnName("AMOUNT");
            entity.Property(e => e.BookingDate)
                .HasColumnType("datetime")
                .HasColumnName("BookingDATE");
            entity.Property(e => e.Branchname)
                .HasMaxLength(256)
                .HasColumnName("BRANCHNAME")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Glcode)
                .HasMaxLength(32)
                .HasColumnName("GLCODE")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Glname)
                .HasMaxLength(256)
                .HasDefaultValueSql("''")
                .HasColumnName("GLNAME")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.IsOverdue).HasColumnName("IS_OVERDUE");
            entity.Property(e => e.IsPayoff).HasColumnName("IS_PAYOFF");
            entity.Property(e => e.IsPrepayment).HasColumnName("IS_PREPAYMENT");
            entity.Property(e => e.Laststagechange)
                .HasColumnType("text")
                .HasColumnName("LASTSTAGECHANGE")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Loantransactiontype)
                .HasMaxLength(256)
                .HasColumnName("LOANTRANSACTIONTYPE")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Pricinpalbalance)
                .HasPrecision(43, 2)
                .HasColumnName("PRICINPALBALANCE");
            entity.Property(e => e.Principaldue)
                .HasPrecision(43, 2)
                .HasColumnName("PRINCIPALDUE");
            entity.Property(e => e.Productencodedkey)
                .HasMaxLength(32)
                .HasDefaultValueSql("''")
                .HasColumnName("PRODUCTENCODEDKEY")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Reversaltransactionid)
                .HasMaxLength(32)
                .HasColumnName("REVERSALTRANSACTIONID");
            entity.Property(e => e.Transactionchannel)
                .HasMaxLength(45)
                .HasColumnName("TRANSACTIONCHANNEL");
            entity.Property(e => e.Transactionid)
                .HasMaxLength(32)
                .HasDefaultValueSql("''")
                .HasColumnName("TRANSACTIONID")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Type)
                .HasMaxLength(256)
                .HasDefaultValueSql("''")
                .HasColumnName("TYPE")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Accountinghubexit>(entity =>
        {
            entity.HasKey(e => new { e.Loanid, e.Glcode, e.Transactionid, e.Entryid, e.Type })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0, 0 });

            entity.ToTable("accountinghubexit");

            entity.HasIndex(e => new { e.Loanid, e.Glcode, e.Entryid, e.Creationdate, e.Type }, "idx_rpatransformado_LOANID_GLCODE_ENTRYID_CREATIONDATE_TYPE");

            entity.HasIndex(e => new { e.Mambuglcode, e.Transactionid }, "idx_rpatransformado_TRANSACTIONID_MAMBUGLCODE");

            entity.Property(e => e.Loanid)
                .HasMaxLength(32)
                .HasColumnName("LOANID")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Glcode)
                .HasMaxLength(32)
                .HasColumnName("GLCODE")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Transactionid)
                .HasMaxLength(32)
                .HasDefaultValueSql("''")
                .HasColumnName("TRANSACTIONID")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Entryid).HasColumnName("ENTRYID");
            entity.Property(e => e.Type)
                .HasMaxLength(256)
                .HasDefaultValueSql("''")
                .HasColumnName("TYPE")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Actualstage)
                .HasColumnType("text")
                .HasColumnName("ACTUALSTAGE")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Amount)
                .HasPrecision(50, 2)
                .HasColumnName("AMOUNT");
            entity.Property(e => e.BookingDate)
                .HasColumnType("datetime")
                .HasColumnName("BookingDATE");
            entity.Property(e => e.Branchname)
                .HasMaxLength(256)
                .HasColumnName("BRANCHNAME")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Entryidrev)
                .HasDefaultValueSql("'0'")
                .HasColumnName("ENTRYIDREV");
            entity.Property(e => e.Glname)
                .HasMaxLength(256)
                .HasDefaultValueSql("''")
                .HasColumnName("GLNAME")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Laststagechange)
                .HasColumnType("text")
                .HasColumnName("LASTSTAGECHANGE")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Loantransactiontype)
                .HasMaxLength(256)
                .HasColumnName("LOANTRANSACTIONTYPE")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Mambuglcode)
                .HasMaxLength(32)
                .HasColumnName("MAMBUGLCODE");
            entity.Property(e => e.Productencodedkey)
                .HasMaxLength(32)
                .HasDefaultValueSql("''")
                .HasColumnName("PRODUCTENCODEDKEY")
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Transactionidrev)
                .HasMaxLength(32)
                .HasDefaultValueSql("''")
                .HasColumnName("TRANSACTIONIDREV")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Accountinghublog>(entity =>
        {
            entity.HasKey(e => e.Logid).HasName("PRIMARY");

            entity.ToTable("accountinghublogs");

            entity.Property(e => e.Logid).HasColumnName("LOGID");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Logline)
                .HasMaxLength(120)
                .HasColumnName("LOGLINE")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Accountinghubprocesscontrol>(entity =>
        {
            entity.HasKey(e => e.Processuuid).HasName("PRIMARY");

            entity.ToTable("accountinghubprocesscontrol");

            entity.HasIndex(e => e.Startdate, "STARTDATE_IDX");

            entity.HasIndex(e => e.Status, "STATUS_IDX");

            entity.Property(e => e.Processuuid)
                .HasMaxLength(45)
                .HasColumnName("PROCESSUUID");
            entity.Property(e => e.Currentstep)
                .HasMaxLength(45)
                .HasColumnName("CURRENTSTEP");
            entity.Property(e => e.Enddate)
                .HasColumnType("datetime")
                .HasColumnName("ENDDATE");
            entity.Property(e => e.Startdate)
                .HasColumnType("datetime")
                .HasColumnName("STARTDATE");
            entity.Property(e => e.Status)
                .HasMaxLength(45)
                .HasColumnName("STATUS");
        });

        modelBuilder.Entity<Accountinginterestaccrualbreakdown>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("accountinginterestaccrualbreakdown")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Accountencodedkey, "ACCOUNTINGINTERESTACCRUALBREAKDOWN_ACCOUNTENCODEDKEY_IDX");

            entity.HasIndex(e => e.Accountid, "ACCOUNTINGINTERESTACCRUALBREAKDOWN_ACCOUNTID_IDX");

            entity.HasIndex(e => e.Bookingdate, "ACCOUNTINGINTERESTACCRUALBREAKDOWN_BOOKINGDATE_IDX");

            entity.HasIndex(e => e.Branchencodedkey, "ACCOUNTINGINTERESTACCRUALBREAKDOWN_BRANCHENCODEDKEY_IDX");

            entity.HasIndex(e => e.Creationdate, "ACCOUNTINGINTERESTACCRUALBREAKDOWN_CREATIONDATE_IDX");

            entity.HasIndex(e => e.Entryid, "ACCOUNTINGINTERESTACCRUALBREAKDOWN_ENTRYID_IDX");

            entity.HasIndex(e => e.Glaccountencodedkey, "ACCOUNTINGINTERESTACCRUALBREAKDOWN_GLACCOUNTENCODEDKEY_IDX");

            entity.HasIndex(e => e.Productencodedkey, "ACCOUNTINGINTERESTACCRUALBREAKDOWN_PRODUCTENCODEDKEY_IDX");

            entity.HasIndex(e => e.Transactionid, "ACCOUNTINGINTERESTACCRUALBREAKDOWN_TRANSACTIONID_IDX");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Accountencodedkey)
                .HasMaxLength(32)
                .HasColumnName("ACCOUNTENCODEDKEY");
            entity.Property(e => e.Accountid)
                .HasMaxLength(32)
                .HasColumnName("ACCOUNTID");
            entity.Property(e => e.Accountingratekey)
                .HasMaxLength(32)
                .HasColumnName("ACCOUNTINGRATEKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accrualtype)
                .HasMaxLength(32)
                .HasDefaultValueSql("'INTEREST_ACCRUED'")
                .HasColumnName("ACCRUALTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Amount)
                .HasPrecision(50, 10)
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Bookingdate)
                .HasColumnType("datetime")
                .HasColumnName("BOOKINGDATE");
            entity.Property(e => e.Branchencodedkey)
                .HasMaxLength(32)
                .HasColumnName("BRANCHENCODEDKEY");
            entity.Property(e => e.Creationdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Currencycode)
                .HasMaxLength(3)
                .HasColumnName("CURRENCYCODE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Entryid).HasColumnName("ENTRYID");
            entity.Property(e => e.Entrytype)
                .HasMaxLength(32)
                .HasColumnName("ENTRYTYPE");
            entity.Property(e => e.Foreignamount)
                .HasPrecision(50, 10)
                .HasColumnName("FOREIGNAMOUNT");
            entity.Property(e => e.Glaccountencodedkey)
                .HasMaxLength(32)
                .HasColumnName("GLACCOUNTENCODEDKEY");
            entity.Property(e => e.Glaccounttype)
                .HasMaxLength(32)
                .HasColumnName("GLACCOUNTTYPE");
            entity.Property(e => e.Processed).HasColumnName("PROCESSED");
            entity.Property(e => e.Productencodedkey)
                .HasMaxLength(32)
                .HasColumnName("PRODUCTENCODEDKEY");
            entity.Property(e => e.Producttype)
                .HasMaxLength(32)
                .HasColumnName("PRODUCTTYPE");
            entity.Property(e => e.Sent).HasColumnName("SENT");
            entity.Property(e => e.Transactionid)
                .HasMaxLength(32)
                .HasColumnName("TRANSACTIONID");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("branch")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Id, "BRANCH_ID").IsUnique();

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Emailaddress)
                .HasMaxLength(256)
                .HasColumnName("EMAILADDRESS")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Id)
                .HasMaxLength(32)
                .HasDefaultValueSql("''")
                .HasColumnName("ID");
            entity.Property(e => e.Lastmodifieddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTMODIFIEDDATE");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("NAME")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Notes)
                .HasColumnType("mediumtext")
                .HasColumnName("NOTES");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(256)
                .HasColumnName("PHONENUMBER")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasDefaultValueSql("'ACTIVE'")
                .HasColumnName("STATE")
                .UseCollation("utf8mb3_bin");
        });

        modelBuilder.Entity<Configurationaccountinghub>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity.ToTable("configurationaccountinghub");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(45)
                .HasColumnName("ENCODEDKEY");
            entity.Property(e => e.Enable).HasColumnName("ENABLE");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Valueconfiguration)
                .HasMaxLength(500)
                .HasColumnName("VALUECONFIGURATION");
        });

        modelBuilder.Entity<Configurationsheet>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity.ToTable("configurationsheets");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(45)
                .HasColumnName("ENCODEDKEY");
            entity.Property(e => e.Enable).HasColumnName("ENABLE");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("NAME");
            entity.Property(e => e.Numbercolumns).HasColumnName("NUMBERCOLUMNS");
            entity.Property(e => e.Reclassification).HasColumnName("RECLASSIFICATION");
            entity.Property(e => e.Requiredid).HasColumnName("REQUIREDID");
            entity.Property(e => e.Startposition).HasColumnName("STARTPOSITION");
            entity.Property(e => e.Startpositioncolumnname).HasColumnName("STARTPOSITIONCOLUMNNAME");
            entity.Property(e => e.Startpositionid).HasColumnName("STARTPOSITIONID");
        });

        modelBuilder.Entity<Configurationsheetcolumn>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity.ToTable("configurationsheetcolumns");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(45)
                .HasColumnName("ENCODEDKEY");
            entity.Property(e => e.Columnindex).HasColumnName("COLUMNINDEX");
            entity.Property(e => e.Configurationsheetencodedkey)
                .HasMaxLength(45)
                .HasColumnName("CONFIGURATIONSHEETENCODEDKEY");
            entity.Property(e => e.Enable).HasColumnName("ENABLE");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("NAME");
            entity.Property(e => e.Required).HasColumnName("REQUIRED");
        });

        modelBuilder.Entity<Customfield>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("customfield")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.CustomfieldsetEncodedkeyOid, "CUSTOMFIELD_CUSTOMFIELDSET");

            entity.HasIndex(e => e.Editusagerightskey, "CUSTOMFIELD_EDITUSAGERIGHTSKEY_KEY");

            entity.HasIndex(e => e.Id, "CUSTOMFIELD_ID").IsUnique();

            entity.HasIndex(e => e.Temporaryid, "CUSTOMFIELD_TEMPORARYID_IDX").IsUnique();

            entity.HasIndex(e => e.Type, "CUSTOMFIELD_TYPE").HasAnnotation("MySql:IndexPrefixLength", new[] { 255 });

            entity.HasIndex(e => e.Viewusagerightskey, "CUSTOMFIELD_VIEWUSAGERIGHTSKEY_KEY");

            entity.HasIndex(e => e.IdGenerated, "ID_GENERATED_IDX").IsUnique();

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Amounts)
                .HasColumnType("mediumblob")
                .HasColumnName("AMOUNTS");
            entity.Property(e => e.Availableforall).HasColumnName("AVAILABLEFORALL");
            entity.Property(e => e.Builtincustomfieldid)
                .HasMaxLength(255)
                .HasColumnName("BUILTINCUSTOMFIELDID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.CustomfieldsetEncodedkeyOid)
                .HasMaxLength(32)
                .HasColumnName("CUSTOMFIELDSET_ENCODEDKEY_OID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Datatype)
                .HasMaxLength(256)
                .HasColumnName("DATATYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Editusagerightskey)
                .HasMaxLength(32)
                .HasColumnName("EDITUSAGERIGHTSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Id)
                .HasMaxLength(32)
                .HasColumnName("ID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.IdGenerated)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID_GENERATED");
            entity.Property(e => e.Indexinlist)
                .HasDefaultValueSql("'-1'")
                .HasColumnName("INDEXINLIST");
            entity.Property(e => e.Isdefault)
                .HasColumnType("bit(1)")
                .HasColumnName("ISDEFAULT");
            entity.Property(e => e.Isrequired)
                .HasColumnType("bit(1)")
                .HasColumnName("ISREQUIRED");
            entity.Property(e => e.Lastmodifieddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTMODIFIEDDATE");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("NAME")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.State)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NORMAL'")
                .HasColumnName("STATE");
            entity.Property(e => e.Temporaryid)
                .HasMaxLength(32)
                .HasColumnName("TEMPORARYID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Type)
                .HasMaxLength(256)
                .HasColumnName("TYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Unique)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("UNIQUE");
            entity.Property(e => e.Validationpattern)
                .HasMaxLength(256)
                .HasColumnName("VALIDATIONPATTERN");
            entity.Property(e => e.Valuelength)
                .HasMaxLength(256)
                .HasDefaultValueSql("'SHORT'")
                .HasColumnName("VALUELENGTH")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Values)
                .HasColumnType("mediumblob")
                .HasColumnName("VALUES");
            entity.Property(e => e.Viewusagerightskey)
                .HasMaxLength(32)
                .HasColumnName("VIEWUSAGERIGHTSKEY")
                .UseCollation("utf8mb3_bin");
        });

        modelBuilder.Entity<Customfieldvalue>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("customfieldvalue")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => new { e.Customfieldkey, e.Parentkey, e.Customfieldsetgroupindex }, "CUSTOMFIELDVALUE_COMPOSED_INDEX1").IsUnique();

            entity.HasIndex(e => e.Value, "CUSTOMFIELDVALUE_IDX").HasAnnotation("MySql:IndexPrefixLength", new[] { 255 });

            entity.HasIndex(e => e.Linkedentitykeyvalue, "CUSTOMFIELDVALUE_LINKEDENTITYKEYVALUE");

            entity.HasIndex(e => e.Parentkey, "CUSTOMFIELDVALUE_PARENTKEY");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Amount)
                .HasPrecision(50, 10)
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Customfieldkey)
                .HasMaxLength(32)
                .HasColumnName("CUSTOMFIELDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Customfieldsetgroupindex)
                .HasDefaultValueSql("'-1'")
                .HasColumnName("CUSTOMFIELDSETGROUPINDEX");
            entity.Property(e => e.Indexinlist).HasColumnName("INDEXINLIST");
            entity.Property(e => e.Linkedentitykeyvalue)
                .HasMaxLength(32)
                .HasColumnName("LINKEDENTITYKEYVALUE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Parentkey)
                .HasMaxLength(32)
                .HasColumnName("PARENTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Value)
                .HasMaxLength(2048)
                .HasColumnName("VALUE")
                .UseCollation("utf8mb3_bin");
        });

        modelBuilder.Entity<Glaccount>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("glaccount")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Currencycode, "FK_GLACCOUNT_CURRENCY_CODE");

            entity.HasIndex(e => e.Glcode, "GLACCOUNT_GLCODE").IsUnique();

            entity.HasIndex(e => e.Migrationeventkey, "GLACCOUNT_MIGRATIONEVENT");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Activated)
                .HasColumnType("bit(1)")
                .HasColumnName("ACTIVATED");
            entity.Property(e => e.Allowmanualjournalentries)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("ALLOWMANUALJOURNALENTRIES");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Currencycode)
                .HasMaxLength(32)
                .HasColumnName("CURRENCYCODE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Description)
                .HasColumnType("mediumtext")
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Glcode)
                .HasMaxLength(32)
                .HasColumnName("GLCODE");
            entity.Property(e => e.Lastmodifieddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTMODIFIEDDATE");
            entity.Property(e => e.Migrationeventkey)
                .HasMaxLength(32)
                .HasColumnName("MIGRATIONEVENTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("NAME")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Striptrailingzeros)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("STRIPTRAILINGZEROS");
            entity.Property(e => e.Type)
                .HasMaxLength(256)
                .HasColumnName("TYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Usage)
                .HasMaxLength(256)
                .HasColumnName("USAGE")
                .UseCollation("utf8mb3_bin");
        });

        modelBuilder.Entity<Gljournalentry>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("gljournalentry")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Accountkey, "ACCOUNTKEY_IDX");

            entity.HasIndex(e => e.Assignedbranchkey, "GLJOURNALENTRY_ASSIGNEDBRANCHKEY");

            entity.HasIndex(e => e.Creationdate, "GLJOURNALENTRY_CREATIONDATE_IDX");

            entity.HasIndex(e => new { e.Entrydate, e.Assignedbranchkey, e.GlaccountEncodedkeyOid }, "GLJOURNALENTRY_ENTRYDATE_BRANCH_GLACCOUNT_IDX");

            entity.HasIndex(e => e.Entryid, "GLJOURNALENTRY_ENTRYID");

            entity.HasIndex(e => e.GlaccountEncodedkeyOid, "GLJOURNALENTRY_N49");

            entity.HasIndex(e => e.Entrydate, "GLJOURNALENTRY_TRANSACTIONDATE");

            entity.HasIndex(e => e.Productkey, "PRODUCTKEY_IDX");

            entity.HasIndex(e => e.Reversalentrykey, "REVERSALENTRYKEY");

            entity.HasIndex(e => e.Transactionid, "TRANSACTIONID");

            entity.HasIndex(e => e.Userkey, "USERKEY_IDX");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountkey)
                .HasMaxLength(32)
                .HasColumnName("ACCOUNTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Amount)
                .HasPrecision(50, 10)
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Assignedbranchkey)
                .HasMaxLength(32)
                .HasColumnName("ASSIGNEDBRANCHKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Entrydate)
                .HasColumnType("datetime")
                .HasColumnName("ENTRYDATE");
            entity.Property(e => e.Entryid)
                .ValueGeneratedOnAdd()
                .HasColumnName("ENTRYID");
            entity.Property(e => e.GlaccountEncodedkeyOid)
                .HasMaxLength(32)
                .HasColumnName("GLACCOUNT_ENCODEDKEY_OID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Notes)
                .HasMaxLength(256)
                .HasColumnName("NOTES")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Productkey)
                .HasMaxLength(32)
                .HasColumnName("PRODUCTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Producttype)
                .HasMaxLength(256)
                .HasColumnName("PRODUCTTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Reversalentrykey)
                .HasMaxLength(32)
                .HasColumnName("REVERSALENTRYKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Transactionid)
                .HasMaxLength(32)
                .HasColumnName("TRANSACTIONID");
            entity.Property(e => e.Type)
                .HasMaxLength(256)
                .HasColumnName("TYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Userkey)
                .HasMaxLength(32)
                .HasColumnName("USERKEY")
                .UseCollation("utf8mb3_bin");

            entity.HasOne(d => d.AssignedbranchkeyNavigation).WithMany(p => p.Gljournalentries)
                .HasForeignKey(d => d.Assignedbranchkey)
                .HasConstraintName("GLJOURNALENTRY_FK2");

            entity.HasOne(d => d.GlaccountEncodedkeyO).WithMany(p => p.Gljournalentries)
                .HasForeignKey(d => d.GlaccountEncodedkeyOid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("GLJOURNALENTRY_FK1");

            entity.HasOne(d => d.ReversalentrykeyNavigation).WithMany(p => p.InverseReversalentrykeyNavigation)
                .HasForeignKey(d => d.Reversalentrykey)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("GLJOURNALENTRY_IBFK_1");
        });

        modelBuilder.Entity<Loanaccount>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("loanaccount")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Activationtransactionkey, "ACTIVATIONTRANSACTIONKEY_IDX");

            entity.HasIndex(e => e.Disbursementdetailskey, "DISBURSEMENTDETAILSKEY_IDX");

            entity.HasIndex(e => e.Accountarrearssettingskey, "FK_LOANACCOUNT_ACCOUNTARREARSSETTINGSKEY_SETTINGSKEY");

            entity.HasIndex(e => e.Accountholderkey, "LOANACCOUNT_ACCOUNTHOLDERKEY");

            entity.HasIndex(e => e.Accountholdertype, "LOANACCOUNT_ACCOUNTHOLDERTYPE").HasAnnotation("MySql:IndexPrefixLength", new[] { 255 });

            entity.HasIndex(e => e.Accountstate, "LOANACCOUNT_ACCOUNTSTATE").HasAnnotation("MySql:IndexPrefixLength", new[] { 255 });

            entity.HasIndex(e => e.Assignedbranchkey, "LOANACCOUNT_ASSIGNEDBRANCHKEY");

            entity.HasIndex(e => e.Assignedcentrekey, "LOANACCOUNT_ASSIGNEDCENTREKEY");

            entity.HasIndex(e => e.Assigneduserkey, "LOANACCOUNT_ASSIGNEDUSERKEY");

            entity.HasIndex(e => e.Closeddate, "LOANACCOUNT_CLOSEDDATE");

            entity.HasIndex(e => e.Id, "LOANACCOUNT_ID").IsUnique();

            entity.HasIndex(e => e.Lastmodifieddate, "LOANACCOUNT_LASTMODIFIEDDATE_IDX");

            entity.HasIndex(e => e.Lineofcreditkey, "LOANACCOUNT_LINEOFCREDITKEY");

            entity.HasIndex(e => e.Migrationeventkey, "LOANACCOUNT_MIGRATIONEVENT");

            entity.HasIndex(e => e.LoangroupEncodedkeyOid, "LOANACCOUNT_N49");

            entity.HasIndex(e => e.Principalpaymentsettingskey, "LOANACCOUNT_PRINCIPALPAYMENTACCOUNTSETTINGS_SETTINGSKEY");

            entity.HasIndex(e => new { e.Producttypekey, e.Creationdate }, "LOANACCOUNT_PRODUCTTYPEKEY_CREATIONDATE");

            entity.HasIndex(e => e.Rescheduledaccountkey, "LOANACCOUNT_RESCHEDULEDACCOUNTKEY");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountarrearssettingskey)
                .HasMaxLength(32)
                .HasColumnName("ACCOUNTARREARSSETTINGSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountholderkey)
                .HasMaxLength(32)
                .HasColumnName("ACCOUNTHOLDERKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountholdertype)
                .HasMaxLength(256)
                .HasColumnName("ACCOUNTHOLDERTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.AccountsIntegerIdx).HasColumnName("ACCOUNTS_INTEGER_IDX");
            entity.Property(e => e.Accountstate)
                .HasMaxLength(256)
                .HasColumnName("ACCOUNTSTATE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountsubstate)
                .HasMaxLength(256)
                .HasColumnName("ACCOUNTSUBSTATE");
            entity.Property(e => e.Accruedinterest)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("ACCRUEDINTEREST");
            entity.Property(e => e.Accruedpenalty)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("ACCRUEDPENALTY");
            entity.Property(e => e.Accrueinterestaftermaturity)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("ACCRUEINTERESTAFTERMATURITY");
            entity.Property(e => e.Accruelateinterest)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("ACCRUELATEINTEREST");
            entity.Property(e => e.Activationtransactionkey)
                .HasMaxLength(32)
                .HasColumnName("ACTIVATIONTRANSACTIONKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Allowoffset)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("ALLOWOFFSET");
            entity.Property(e => e.Applyinterestonprepaymentmethod)
                .HasMaxLength(256)
                .HasColumnName("APPLYINTERESTONPREPAYMENTMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Approveddate)
                .HasColumnType("datetime")
                .HasColumnName("APPROVEDDATE");
            entity.Property(e => e.Arrearstoleranceperiod).HasColumnName("ARREARSTOLERANCEPERIOD");
            entity.Property(e => e.Assignedbranchkey)
                .HasMaxLength(32)
                .HasColumnName("ASSIGNEDBRANCHKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Assignedcentrekey)
                .HasMaxLength(32)
                .HasColumnName("ASSIGNEDCENTREKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Assigneduserkey)
                .HasMaxLength(32)
                .HasColumnName("ASSIGNEDUSERKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Closeddate)
                .HasColumnType("datetime")
                .HasColumnName("CLOSEDDATE");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Defaultfirstrepaymentduedateoffset)
                .HasPrecision(50, 20)
                .HasColumnName("DEFAULTFIRSTREPAYMENTDUEDATEOFFSET");
            entity.Property(e => e.Disbursementdetailskey)
                .HasMaxLength(32)
                .HasColumnName("DISBURSEMENTDETAILSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Elementsrecalculationmethod)
                .HasMaxLength(256)
                .HasColumnName("ELEMENTSRECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Feesbalance)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("FEESBALANCE");
            entity.Property(e => e.Feesdue)
                .HasPrecision(50, 10)
                .HasColumnName("FEESDUE");
            entity.Property(e => e.Feespaid)
                .HasPrecision(50, 10)
                .HasColumnName("FEESPAID");
            entity.Property(e => e.Fixeddaysofmonth)
                .HasColumnType("mediumblob")
                .HasColumnName("FIXEDDAYSOFMONTH");
            entity.Property(e => e.Futurepaymentsacceptance)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NO_FUTURE_PAYMENTS'")
                .HasColumnName("FUTUREPAYMENTSACCEPTANCE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Graceperiod).HasColumnName("GRACEPERIOD");
            entity.Property(e => e.Graceperiodtype)
                .HasMaxLength(256)
                .HasColumnName("GRACEPERIODTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Hascustomschedule)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("HASCUSTOMSCHEDULE");
            entity.Property(e => e.Holdbalance)
                .HasPrecision(50, 10)
                .HasColumnName("HOLDBALANCE");
            entity.Property(e => e.Id)
                .HasMaxLength(32)
                .HasColumnName("ID");
            entity.Property(e => e.Interestapplicationmethod)
                .HasMaxLength(256)
                .HasColumnName("INTERESTAPPLICATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestbalance)
                .HasPrecision(50, 10)
                .HasColumnName("INTERESTBALANCE");
            entity.Property(e => e.Interestbalancecalculationmethod)
                .HasMaxLength(32)
                .HasDefaultValueSql("'PRINCIPAL_ONLY'")
                .HasColumnName("INTERESTBALANCECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestcalculationmethod)
                .HasMaxLength(256)
                .HasColumnName("INTERESTCALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestchargefrequency)
                .HasMaxLength(256)
                .HasColumnName("INTERESTCHARGEFREQUENCY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestcommission)
                .HasPrecision(50, 20)
                .HasColumnName("INTERESTCOMMISSION");
            entity.Property(e => e.Interestdue)
                .HasPrecision(50, 10)
                .HasColumnName("INTERESTDUE");
            entity.Property(e => e.Interestfromarrearsaccrued)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("INTERESTFROMARREARSACCRUED");
            entity.Property(e => e.Interestfromarrearsbalance)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("INTERESTFROMARREARSBALANCE");
            entity.Property(e => e.Interestfromarrearsdue)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("INTERESTFROMARREARSDUE");
            entity.Property(e => e.Interestfromarrearspaid)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("INTERESTFROMARREARSPAID");
            entity.Property(e => e.Interestpaid)
                .HasPrecision(50, 10)
                .HasColumnName("INTERESTPAID");
            entity.Property(e => e.Interestrate)
                .HasPrecision(50, 20)
                .HasColumnName("INTERESTRATE");
            entity.Property(e => e.Interestratereviewcount).HasColumnName("INTERESTRATEREVIEWCOUNT");
            entity.Property(e => e.Interestratereviewunit)
                .HasMaxLength(256)
                .HasColumnName("INTERESTRATEREVIEWUNIT");
            entity.Property(e => e.Interestratesource)
                .HasMaxLength(256)
                .HasDefaultValueSql("'FIXED_INTEREST_RATE'")
                .HasColumnName("INTERESTRATESOURCE");
            entity.Property(e => e.Interestroundingversion)
                .HasMaxLength(256)
                .HasDefaultValueSql("'VERSION_1'")
                .HasColumnName("INTERESTROUNDINGVERSION")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestspread)
                .HasPrecision(50, 10)
                .HasColumnName("INTERESTSPREAD");
            entity.Property(e => e.Interesttype)
                .HasMaxLength(255)
                .HasDefaultValueSql("'SIMPLE_INTEREST'")
                .HasColumnName("INTERESTTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Lastaccountappraisaldate)
                .HasColumnType("datetime")
                .HasColumnName("LASTACCOUNTAPPRAISALDATE");
            entity.Property(e => e.Lastinterestapplieddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTINTERESTAPPLIEDDATE");
            entity.Property(e => e.Lastinterestreviewdate)
                .HasColumnType("datetime")
                .HasColumnName("LASTINTERESTREVIEWDATE");
            entity.Property(e => e.Lastlockeddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTLOCKEDDATE");
            entity.Property(e => e.Lastmodifieddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTMODIFIEDDATE");
            entity.Property(e => e.Lastsettoarrearsdate)
                .HasColumnType("datetime")
                .HasColumnName("LASTSETTOARREARSDATE");
            entity.Property(e => e.Lasttaxratereviewdate)
                .HasColumnType("datetime")
                .HasColumnName("LASTTAXRATEREVIEWDATE");
            entity.Property(e => e.Latepaymentsrecalculationmethod)
                .HasMaxLength(256)
                .HasColumnName("LATEPAYMENTSRECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Lineofcreditkey)
                .HasMaxLength(32)
                .HasColumnName("LINEOFCREDITKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Loanamount)
                .HasPrecision(50, 10)
                .HasColumnName("LOANAMOUNT");
            entity.Property(e => e.LoangroupEncodedkeyOid)
                .HasMaxLength(32)
                .HasColumnName("LOANGROUP_ENCODEDKEY_OID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Loanname)
                .HasMaxLength(256)
                .HasColumnName("LOANNAME")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Loanpenaltycalculationmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NONE'")
                .HasColumnName("LOANPENALTYCALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Lockedoperations)
                .HasColumnType("mediumblob")
                .HasColumnName("LOCKEDOPERATIONS");
            entity.Property(e => e.Migrationeventkey)
                .HasMaxLength(32)
                .HasColumnName("MIGRATIONEVENTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Notes)
                .HasColumnType("mediumtext")
                .HasColumnName("NOTES");
            entity.Property(e => e.Paymentmethod)
                .HasMaxLength(256)
                .HasColumnName("PAYMENTMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Penaltybalance)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("PENALTYBALANCE");
            entity.Property(e => e.Penaltydue)
                .HasPrecision(50, 10)
                .HasColumnName("PENALTYDUE");
            entity.Property(e => e.Penaltypaid)
                .HasPrecision(50, 10)
                .HasColumnName("PENALTYPAID");
            entity.Property(e => e.Penaltyrate)
                .HasPrecision(50, 20)
                .HasColumnName("PENALTYRATE");
            entity.Property(e => e.Periodicpayment)
                .HasPrecision(50, 10)
                .HasColumnName("PERIODICPAYMENT");
            entity.Property(e => e.Prepaymentacceptance)
                .HasMaxLength(256)
                .HasDefaultValueSql("'ACCEPT_PREPAYMENTS'")
                .HasColumnName("PREPAYMENTACCEPTANCE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Prepaymentrecalculationmethod)
                .HasMaxLength(255)
                .HasColumnName("PREPAYMENTRECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Principalbalance)
                .HasPrecision(50, 10)
                .HasColumnName("PRINCIPALBALANCE");
            entity.Property(e => e.Principaldue)
                .HasPrecision(50, 10)
                .HasColumnName("PRINCIPALDUE");
            entity.Property(e => e.Principalpaid)
                .HasPrecision(50, 10)
                .HasColumnName("PRINCIPALPAID");
            entity.Property(e => e.Principalpaidinstallmentstatus)
                .HasMaxLength(255)
                .HasColumnName("PRINCIPALPAIDINSTALLMENTSTATUS")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Principalpaymentsettingskey)
                .HasMaxLength(32)
                .HasColumnName("PRINCIPALPAYMENTSETTINGSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Principalrepaymentinterval)
                .HasDefaultValueSql("'1'")
                .HasColumnName("PRINCIPALREPAYMENTINTERVAL");
            entity.Property(e => e.Producttypekey)
                .HasMaxLength(32)
                .HasColumnName("PRODUCTTYPEKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Redrawbalance)
                .HasPrecision(50, 10)
                .HasColumnName("REDRAWBALANCE");
            entity.Property(e => e.Repaymentinstallments).HasColumnName("REPAYMENTINSTALLMENTS");
            entity.Property(e => e.Repaymentperiodcount).HasColumnName("REPAYMENTPERIODCOUNT");
            entity.Property(e => e.Repaymentperiodunit)
                .HasMaxLength(256)
                .HasColumnName("REPAYMENTPERIODUNIT")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Repaymentschedulemethod)
                .HasMaxLength(256)
                .HasColumnName("REPAYMENTSCHEDULEMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Rescheduledaccountkey)
                .HasMaxLength(32)
                .HasColumnName("RESCHEDULEDACCOUNTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Scheduleduedatesmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'INTERVAL'")
                .HasColumnName("SCHEDULEDUEDATESMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Shortmonthhandlingmethod)
                .HasMaxLength(256)
                .HasColumnName("SHORTMONTHHANDLINGMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Taxrate)
                .HasPrecision(50, 10)
                .HasColumnName("TAXRATE");

            entity.HasOne(d => d.ActivationtransactionkeyNavigation).WithMany(p => p.Loanaccounts)
                .HasForeignKey(d => d.Activationtransactionkey)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("LOANACCOUNT_FK7");

            entity.HasOne(d => d.AssignedbranchkeyNavigation).WithMany(p => p.Loanaccounts)
                .HasForeignKey(d => d.Assignedbranchkey)
                .HasConstraintName("LOANACCOUNT_FK5");

            entity.HasOne(d => d.ProducttypekeyNavigation).WithMany(p => p.Loanaccounts)
                .HasForeignKey(d => d.Producttypekey)
                .HasConstraintName("LOANACCOUNT_FK3");

            entity.HasOne(d => d.RescheduledaccountkeyNavigation).WithMany(p => p.InverseRescheduledaccountkeyNavigation)
                .HasForeignKey(d => d.Rescheduledaccountkey)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("LOANACCOUNT_FK4");
        });

        modelBuilder.Entity<Loanaccounthistory>(entity =>
        {
            entity.HasKey(e => new { e.Encodedkey, e.Snapshotdate })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("loanaccounthistory")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => new { e.Id, e.Snapshotdate }, "LOANACCOUNT_ID_SNAPSHOTDATE").IsUnique();

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Snapshotdate)
                .HasDefaultValueSql("curdate()")
                .HasColumnName("SNAPSHOTDATE");
            entity.Property(e => e.Accountarrearssettingskey)
                .HasMaxLength(32)
                .HasColumnName("ACCOUNTARREARSSETTINGSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountholderkey)
                .HasMaxLength(32)
                .HasColumnName("ACCOUNTHOLDERKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountholdertype)
                .HasMaxLength(256)
                .HasColumnName("ACCOUNTHOLDERTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.AccountsIntegerIdx).HasColumnName("ACCOUNTS_INTEGER_IDX");
            entity.Property(e => e.Accountstate)
                .HasMaxLength(256)
                .HasColumnName("ACCOUNTSTATE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountsubstate)
                .HasMaxLength(256)
                .HasColumnName("ACCOUNTSUBSTATE");
            entity.Property(e => e.Accruedinterest)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("ACCRUEDINTEREST");
            entity.Property(e => e.Accruedpenalty)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("ACCRUEDPENALTY");
            entity.Property(e => e.Accrueinterestaftermaturity)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("ACCRUEINTERESTAFTERMATURITY");
            entity.Property(e => e.Accruelateinterest)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("ACCRUELATEINTEREST");
            entity.Property(e => e.Activationtransactionkey)
                .HasMaxLength(32)
                .HasColumnName("ACTIVATIONTRANSACTIONKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Allowoffset)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("ALLOWOFFSET");
            entity.Property(e => e.Applyinterestonprepaymentmethod)
                .HasMaxLength(256)
                .HasColumnName("APPLYINTERESTONPREPAYMENTMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Approveddate)
                .HasColumnType("datetime")
                .HasColumnName("APPROVEDDATE");
            entity.Property(e => e.Arrearstoleranceperiod).HasColumnName("ARREARSTOLERANCEPERIOD");
            entity.Property(e => e.Assignedbranchkey)
                .HasMaxLength(32)
                .HasColumnName("ASSIGNEDBRANCHKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Assignedcentrekey)
                .HasMaxLength(32)
                .HasColumnName("ASSIGNEDCENTREKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Assigneduserkey)
                .HasMaxLength(32)
                .HasColumnName("ASSIGNEDUSERKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Closeddate)
                .HasColumnType("datetime")
                .HasColumnName("CLOSEDDATE");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Defaultfirstrepaymentduedateoffset)
                .HasPrecision(50, 20)
                .HasColumnName("DEFAULTFIRSTREPAYMENTDUEDATEOFFSET");
            entity.Property(e => e.Disbursementdetailskey)
                .HasMaxLength(32)
                .HasColumnName("DISBURSEMENTDETAILSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Elementsrecalculationmethod)
                .HasMaxLength(256)
                .HasColumnName("ELEMENTSRECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Feesbalance)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("FEESBALANCE");
            entity.Property(e => e.Feesdue)
                .HasPrecision(50, 10)
                .HasColumnName("FEESDUE");
            entity.Property(e => e.Feespaid)
                .HasPrecision(50, 10)
                .HasColumnName("FEESPAID");
            entity.Property(e => e.Fixeddaysofmonth)
                .HasColumnType("mediumblob")
                .HasColumnName("FIXEDDAYSOFMONTH");
            entity.Property(e => e.Futurepaymentsacceptance)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NO_FUTURE_PAYMENTS'")
                .HasColumnName("FUTUREPAYMENTSACCEPTANCE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Graceperiod).HasColumnName("GRACEPERIOD");
            entity.Property(e => e.Graceperiodtype)
                .HasMaxLength(256)
                .HasColumnName("GRACEPERIODTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Hascustomschedule)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("HASCUSTOMSCHEDULE");
            entity.Property(e => e.Holdbalance)
                .HasPrecision(50, 10)
                .HasColumnName("HOLDBALANCE");
            entity.Property(e => e.Id)
                .HasMaxLength(32)
                .HasColumnName("ID");
            entity.Property(e => e.Interestapplicationmethod)
                .HasMaxLength(256)
                .HasColumnName("INTERESTAPPLICATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestbalance)
                .HasPrecision(50, 10)
                .HasColumnName("INTERESTBALANCE");
            entity.Property(e => e.Interestbalancecalculationmethod)
                .HasMaxLength(32)
                .HasDefaultValueSql("'PRINCIPAL_ONLY'")
                .HasColumnName("INTERESTBALANCECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestcalculationmethod)
                .HasMaxLength(256)
                .HasColumnName("INTERESTCALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestchargefrequency)
                .HasMaxLength(256)
                .HasColumnName("INTERESTCHARGEFREQUENCY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestcommission)
                .HasPrecision(50, 20)
                .HasColumnName("INTERESTCOMMISSION");
            entity.Property(e => e.Interestdue)
                .HasPrecision(50, 10)
                .HasColumnName("INTERESTDUE");
            entity.Property(e => e.Interestfromarrearsaccrued)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("INTERESTFROMARREARSACCRUED");
            entity.Property(e => e.Interestfromarrearsbalance)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("INTERESTFROMARREARSBALANCE");
            entity.Property(e => e.Interestfromarrearsdue)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("INTERESTFROMARREARSDUE");
            entity.Property(e => e.Interestfromarrearspaid)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("INTERESTFROMARREARSPAID");
            entity.Property(e => e.Interestpaid)
                .HasPrecision(50, 10)
                .HasColumnName("INTERESTPAID");
            entity.Property(e => e.Interestrate)
                .HasPrecision(50, 20)
                .HasColumnName("INTERESTRATE");
            entity.Property(e => e.Interestratereviewcount).HasColumnName("INTERESTRATEREVIEWCOUNT");
            entity.Property(e => e.Interestratereviewunit)
                .HasMaxLength(256)
                .HasColumnName("INTERESTRATEREVIEWUNIT");
            entity.Property(e => e.Interestratesource)
                .HasMaxLength(256)
                .HasDefaultValueSql("'FIXED_INTEREST_RATE'")
                .HasColumnName("INTERESTRATESOURCE");
            entity.Property(e => e.Interestroundingversion)
                .HasMaxLength(256)
                .HasDefaultValueSql("'VERSION_1'")
                .HasColumnName("INTERESTROUNDINGVERSION")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestspread)
                .HasPrecision(50, 10)
                .HasColumnName("INTERESTSPREAD");
            entity.Property(e => e.Interesttype)
                .HasMaxLength(255)
                .HasDefaultValueSql("'SIMPLE_INTEREST'")
                .HasColumnName("INTERESTTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Lastaccountappraisaldate)
                .HasColumnType("datetime")
                .HasColumnName("LASTACCOUNTAPPRAISALDATE");
            entity.Property(e => e.Lastinterestapplieddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTINTERESTAPPLIEDDATE");
            entity.Property(e => e.Lastinterestreviewdate)
                .HasColumnType("datetime")
                .HasColumnName("LASTINTERESTREVIEWDATE");
            entity.Property(e => e.Lastlockeddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTLOCKEDDATE");
            entity.Property(e => e.Lastmodifieddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTMODIFIEDDATE");
            entity.Property(e => e.Lastsettoarrearsdate)
                .HasColumnType("datetime")
                .HasColumnName("LASTSETTOARREARSDATE");
            entity.Property(e => e.Lasttaxratereviewdate)
                .HasColumnType("datetime")
                .HasColumnName("LASTTAXRATEREVIEWDATE");
            entity.Property(e => e.Latepaymentsrecalculationmethod)
                .HasMaxLength(256)
                .HasColumnName("LATEPAYMENTSRECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Lineofcreditkey)
                .HasMaxLength(32)
                .HasColumnName("LINEOFCREDITKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Loanamount)
                .HasPrecision(50, 10)
                .HasColumnName("LOANAMOUNT");
            entity.Property(e => e.LoangroupEncodedkeyOid)
                .HasMaxLength(32)
                .HasColumnName("LOANGROUP_ENCODEDKEY_OID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Loanname)
                .HasMaxLength(256)
                .HasColumnName("LOANNAME")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Loanpenaltycalculationmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NONE'")
                .HasColumnName("LOANPENALTYCALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Lockedoperations)
                .HasColumnType("mediumblob")
                .HasColumnName("LOCKEDOPERATIONS");
            entity.Property(e => e.Migrationeventkey)
                .HasMaxLength(32)
                .HasColumnName("MIGRATIONEVENTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Notes)
                .HasColumnType("mediumtext")
                .HasColumnName("NOTES");
            entity.Property(e => e.Paymentmethod)
                .HasMaxLength(256)
                .HasColumnName("PAYMENTMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Penaltybalance)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("PENALTYBALANCE");
            entity.Property(e => e.Penaltydue)
                .HasPrecision(50, 10)
                .HasColumnName("PENALTYDUE");
            entity.Property(e => e.Penaltypaid)
                .HasPrecision(50, 10)
                .HasColumnName("PENALTYPAID");
            entity.Property(e => e.Penaltyrate)
                .HasPrecision(50, 20)
                .HasColumnName("PENALTYRATE");
            entity.Property(e => e.Periodicpayment)
                .HasPrecision(50, 10)
                .HasColumnName("PERIODICPAYMENT");
            entity.Property(e => e.Prepaymentacceptance)
                .HasMaxLength(256)
                .HasDefaultValueSql("'ACCEPT_PREPAYMENTS'")
                .HasColumnName("PREPAYMENTACCEPTANCE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Prepaymentrecalculationmethod)
                .HasMaxLength(255)
                .HasColumnName("PREPAYMENTRECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Principalbalance)
                .HasPrecision(50, 10)
                .HasColumnName("PRINCIPALBALANCE");
            entity.Property(e => e.Principaldue)
                .HasPrecision(50, 10)
                .HasColumnName("PRINCIPALDUE");
            entity.Property(e => e.Principalpaid)
                .HasPrecision(50, 10)
                .HasColumnName("PRINCIPALPAID");
            entity.Property(e => e.Principalpaidinstallmentstatus)
                .HasMaxLength(255)
                .HasColumnName("PRINCIPALPAIDINSTALLMENTSTATUS")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Principalpaymentsettingskey)
                .HasMaxLength(32)
                .HasColumnName("PRINCIPALPAYMENTSETTINGSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Principalrepaymentinterval)
                .HasDefaultValueSql("'1'")
                .HasColumnName("PRINCIPALREPAYMENTINTERVAL");
            entity.Property(e => e.Producttypekey)
                .HasMaxLength(32)
                .HasColumnName("PRODUCTTYPEKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Redrawbalance)
                .HasPrecision(50, 10)
                .HasColumnName("REDRAWBALANCE");
            entity.Property(e => e.Repaymentinstallments).HasColumnName("REPAYMENTINSTALLMENTS");
            entity.Property(e => e.Repaymentperiodcount).HasColumnName("REPAYMENTPERIODCOUNT");
            entity.Property(e => e.Repaymentperiodunit)
                .HasMaxLength(256)
                .HasColumnName("REPAYMENTPERIODUNIT")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Repaymentschedulemethod)
                .HasMaxLength(256)
                .HasColumnName("REPAYMENTSCHEDULEMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Rescheduledaccountkey)
                .HasMaxLength(32)
                .HasColumnName("RESCHEDULEDACCOUNTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Scheduleduedatesmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'INTERVAL'")
                .HasColumnName("SCHEDULEDUEDATESMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Shortmonthhandlingmethod)
                .HasMaxLength(256)
                .HasColumnName("SHORTMONTHHANDLINGMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Taxrate)
                .HasPrecision(50, 10)
                .HasColumnName("TAXRATE");
        });

        modelBuilder.Entity<Loanproduct>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("loanproduct")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Arrearssettingskey, "FK_LOANPRODUCT_PRODUCTARREARSSETTINGS_ARREARSSETTINGS");

            entity.HasIndex(e => e.Interestratesettingskey, "INTERESTRATESETTINGS_FK");

            entity.HasIndex(e => e.Currencycode, "LOANPRODUCT_CURRENCY_FK");

            entity.HasIndex(e => e.Id, "LOANPRODUCT_ID_IDX");

            entity.HasIndex(e => e.Linkablesavingsproductkey, "LOANPRODUCT_LINKABLESAVINGSPRODUCTKEY");

            entity.HasIndex(e => e.Principalpaymentsettingskey, "LOANPRODUCT_PRINCIPALPAYMENTPRODUCTSETTINGS_SETTINGSKEY");

            entity.HasIndex(e => e.Redrawsettingskey, "LOANPRODUCT_PRODUCTREDRAWSETTINGS_REDRAWSETTINGSKEY");

            entity.HasIndex(e => e.Taxsourcekey, "LOANPRODUCT_TAXSOURCEKEY");

            entity.HasIndex(e => e.Productsecuritysettingskey, "PRODUCTSECURITYSETTINGS_KEY");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountingmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NONE'")
                .HasColumnName("ACCOUNTINGMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountinitialstate)
                .HasMaxLength(32)
                .HasDefaultValueSql("'PENDING_APPROVAL'")
                .HasColumnName("ACCOUNTINITIALSTATE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Accountlinkingenabled)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("ACCOUNTLINKINGENABLED");
            entity.Property(e => e.Accruelateinterest)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("ACCRUELATEINTEREST");
            entity.Property(e => e.Activated)
                .HasColumnType("bit(1)")
                .HasColumnName("ACTIVATED");
            entity.Property(e => e.Allowarbitraryfees)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("ALLOWARBITRARYFEES");
            entity.Property(e => e.Allowcustomrepaymentallocation)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("ALLOWCUSTOMREPAYMENTALLOCATION");
            entity.Property(e => e.Amortizationmethod)
                .HasMaxLength(32)
                .HasDefaultValueSql("'STANDARD_PAYMENTS'")
                .HasColumnName("AMORTIZATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Applyinterestonprepaymentmethod)
                .HasMaxLength(256)
                .HasColumnName("APPLYINTERESTONPREPAYMENTMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Arrearssettingskey)
                .HasMaxLength(32)
                .HasColumnName("ARREARSSETTINGSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Autocreatelinkedaccounts)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("AUTOCREATELINKEDACCOUNTS");
            entity.Property(e => e.Autolinkaccounts)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("AUTOLINKACCOUNTS");
            entity.Property(e => e.Cappingapplyaccruedchargesbeforelocking)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("CAPPINGAPPLYACCRUEDCHARGESBEFORELOCKING");
            entity.Property(e => e.Cappingconstrainttype)
                .HasMaxLength(255)
                .HasColumnName("CAPPINGCONSTRAINTTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Cappingmethod)
                .HasMaxLength(255)
                .HasColumnName("CAPPINGMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Cappingpercentage)
                .HasPrecision(50, 20)
                .HasColumnName("CAPPINGPERCENTAGE");
            entity.Property(e => e.Category)
                .HasMaxLength(256)
                .HasColumnName("CATEGORY")
                .UseCollation("utf8mb3_unicode_ci");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Currencycode)
                .HasMaxLength(3)
                .HasColumnName("CURRENCYCODE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Daysinyear)
                .HasMaxLength(256)
                .HasDefaultValueSql("'DAYS_365'")
                .HasColumnName("DAYSINYEAR");
            entity.Property(e => e.Defaultfirstrepaymentduedateoffset)
                .HasPrecision(50, 20)
                .HasColumnName("DEFAULTFIRSTREPAYMENTDUEDATEOFFSET");
            entity.Property(e => e.Defaultgraceperiod).HasColumnName("DEFAULTGRACEPERIOD");
            entity.Property(e => e.Defaultloanamount)
                .HasPrecision(50, 10)
                .HasColumnName("DEFAULTLOANAMOUNT");
            entity.Property(e => e.Defaultnuminstallments).HasColumnName("DEFAULTNUMINSTALLMENTS");
            entity.Property(e => e.Defaultpenaltyrate)
                .HasPrecision(50, 20)
                .HasColumnName("DEFAULTPENALTYRATE");
            entity.Property(e => e.Defaultprincipalrepaymentinterval).HasColumnName("DEFAULTPRINCIPALREPAYMENTINTERVAL");
            entity.Property(e => e.Defaultrepaymentperiodcount).HasColumnName("DEFAULTREPAYMENTPERIODCOUNT");
            entity.Property(e => e.Dormancyperioddays).HasColumnName("DORMANCYPERIODDAYS");
            entity.Property(e => e.Elementsrecalculationmethod)
                .HasMaxLength(256)
                .HasColumnName("ELEMENTSRECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Fixeddaysofmonth)
                .HasColumnType("mediumblob")
                .HasColumnName("FIXEDDAYSOFMONTH");
            entity.Property(e => e.Forallbranches)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("FORALLBRANCHES");
            entity.Property(e => e.Forhybridgroups)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("FORHYBRIDGROUPS");
            entity.Property(e => e.Forindividuals)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("FORINDIVIDUALS");
            entity.Property(e => e.Forpuregroups)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("FORPUREGROUPS");
            entity.Property(e => e.Foureyesprincipleloanapproval)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("FOUREYESPRINCIPLELOANAPPROVAL");
            entity.Property(e => e.Futurepaymentsacceptance)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NO_FUTURE_PAYMENTS'")
                .HasColumnName("FUTUREPAYMENTSACCEPTANCE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Graceperiodtype)
                .HasMaxLength(256)
                .HasColumnName("GRACEPERIODTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Id)
                .HasMaxLength(32)
                .HasColumnName("ID");
            entity.Property(e => e.Idgeneratortype)
                .HasMaxLength(256)
                .HasDefaultValueSql("'RANDOM_PATTERN'")
                .HasColumnName("IDGENERATORTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Idpattern)
                .HasMaxLength(256)
                .HasDefaultValueSql("'@@@@###'")
                .HasColumnName("IDPATTERN")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestaccrualcalculation)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NONE'")
                .HasColumnName("INTERESTACCRUALCALCULATION")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestaccruedaccountingmethod)
                .HasMaxLength(32)
                .HasDefaultValueSql("'NONE'")
                .HasColumnName("INTERESTACCRUEDACCOUNTINGMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestapplicationmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'ON_DISBURSEMENT'")
                .HasColumnName("INTERESTAPPLICATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestbalancecalculationmethod)
                .HasMaxLength(32)
                .HasDefaultValueSql("'PRINCIPAL_ONLY'")
                .HasColumnName("INTERESTBALANCECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestcalculationmethod)
                .HasMaxLength(256)
                .HasColumnName("INTERESTCALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestratesettingskey)
                .HasMaxLength(32)
                .HasColumnName("INTERESTRATESETTINGSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interesttype)
                .HasMaxLength(255)
                .HasDefaultValueSql("'SIMPLE_INTEREST'")
                .HasColumnName("INTERESTTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Lastmodifieddate)
                .HasColumnType("datetime")
                .HasColumnName("LASTMODIFIEDDATE");
            entity.Property(e => e.Latepaymentsrecalculationmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'INCREASE_OVERDUE_INSTALLMENTS'")
                .HasColumnName("LATEPAYMENTSRECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Lineofcreditrequirement)
                .HasMaxLength(255)
                .HasDefaultValueSql("'OPTIONAL'")
                .HasColumnName("LINEOFCREDITREQUIREMENT")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Linkablesavingsproductkey)
                .HasMaxLength(32)
                .HasColumnName("LINKABLESAVINGSPRODUCTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Loanpenaltycalculationmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NONE'")
                .HasColumnName("LOANPENALTYCALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Loanpenaltygraceperiod).HasColumnName("LOANPENALTYGRACEPERIOD");
            entity.Property(e => e.Loanproducttype)
                .HasMaxLength(255)
                .HasDefaultValueSql("'FIXED_TERM_LOAN'")
                .HasColumnName("LOANPRODUCTTYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Lockperioddays).HasColumnName("LOCKPERIODDAYS");
            entity.Property(e => e.Maxfirstrepaymentduedateoffset)
                .HasPrecision(50, 20)
                .HasColumnName("MAXFIRSTREPAYMENTDUEDATEOFFSET");
            entity.Property(e => e.Maxgraceperiod).HasColumnName("MAXGRACEPERIOD");
            entity.Property(e => e.Maxloanamount)
                .HasPrecision(50, 10)
                .HasColumnName("MAXLOANAMOUNT");
            entity.Property(e => e.Maxnumberofdisbursementtranches)
                .HasDefaultValueSql("'1'")
                .HasColumnName("MAXNUMBEROFDISBURSEMENTTRANCHES");
            entity.Property(e => e.Maxnuminstallments).HasColumnName("MAXNUMINSTALLMENTS");
            entity.Property(e => e.Maxpenaltyrate)
                .HasPrecision(50, 20)
                .HasColumnName("MAXPENALTYRATE");
            entity.Property(e => e.Minfirstrepaymentduedateoffset)
                .HasPrecision(50, 20)
                .HasColumnName("MINFIRSTREPAYMENTDUEDATEOFFSET");
            entity.Property(e => e.Mingraceperiod).HasColumnName("MINGRACEPERIOD");
            entity.Property(e => e.Minloanamount)
                .HasPrecision(50, 10)
                .HasColumnName("MINLOANAMOUNT");
            entity.Property(e => e.Minnuminstallments).HasColumnName("MINNUMINSTALLMENTS");
            entity.Property(e => e.Minpenaltyrate)
                .HasPrecision(50, 20)
                .HasColumnName("MINPENALTYRATE");
            entity.Property(e => e.Offsetpercentage)
                .HasPrecision(50, 20)
                .HasColumnName("OFFSETPERCENTAGE");
            entity.Property(e => e.Paymentmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'HORIZONTAL'")
                .HasColumnName("PAYMENTMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Prepaymentacceptance)
                .HasMaxLength(256)
                .HasDefaultValueSql("'ACCEPT_PREPAYMENTS'")
                .HasColumnName("PREPAYMENTACCEPTANCE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Prepaymentrecalculationmethod)
                .HasMaxLength(255)
                .HasColumnName("PREPAYMENTRECALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Principalpaidinstallmentstatus)
                .HasMaxLength(255)
                .HasColumnName("PRINCIPALPAIDINSTALLMENTSTATUS")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Principalpaymentsettingskey)
                .HasMaxLength(32)
                .HasColumnName("PRINCIPALPAYMENTSETTINGSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Productdescription)
                .HasColumnType("mediumtext")
                .HasColumnName("PRODUCTDESCRIPTION");
            entity.Property(e => e.Productname)
                .HasMaxLength(256)
                .HasColumnName("PRODUCTNAME")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Productsecuritysettingskey)
                .HasMaxLength(32)
                .HasColumnName("PRODUCTSECURITYSETTINGSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Redrawsettingskey)
                .HasMaxLength(32)
                .HasColumnName("REDRAWSETTINGSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Repaymentallocationorder)
                .HasColumnType("mediumblob")
                .HasColumnName("REPAYMENTALLOCATIONORDER");
            entity.Property(e => e.Repaymentcurrencyrounding)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NO_ROUNDING'")
                .HasColumnName("REPAYMENTCURRENCYROUNDING");
            entity.Property(e => e.Repaymentelementsroundingmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NO_ROUNDING'")
                .HasColumnName("REPAYMENTELEMENTSROUNDINGMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Repaymentperiodunit)
                .HasMaxLength(256)
                .HasColumnName("REPAYMENTPERIODUNIT")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Repaymentreschedulingmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NEXT_WORKING_DAY'")
                .HasColumnName("REPAYMENTRESCHEDULINGMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Repaymentscheduleeditoptions)
                .HasColumnType("mediumblob")
                .HasColumnName("REPAYMENTSCHEDULEEDITOPTIONS");
            entity.Property(e => e.Repaymentschedulemethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'FIXED'")
                .HasColumnName("REPAYMENTSCHEDULEMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Roundingrepaymentschedulemethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'NO_ROUNDING'")
                .HasColumnName("ROUNDINGREPAYMENTSCHEDULEMETHOD");
            entity.Property(e => e.Scheduleduedatesmethod)
                .HasMaxLength(256)
                .HasDefaultValueSql("'INTERVAL'")
                .HasColumnName("SCHEDULEDUEDATESMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Scheduleinterestdayscountmethod)
                .HasMaxLength(256)
                .HasColumnName("SCHEDULEINTERESTDAYSCOUNTMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Settlementoptions)
                .HasMaxLength(32)
                .HasDefaultValueSql("'FULL_DUE_AMOUNTS'")
                .HasColumnName("SETTLEMENTOPTIONS")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Shortmonthhandlingmethod)
                .HasMaxLength(256)
                .HasColumnName("SHORTMONTHHANDLINGMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Taxcalculationmethod)
                .HasMaxLength(256)
                .HasColumnName("TAXCALCULATIONMETHOD")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Taxesonfeesenabled)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("TAXESONFEESENABLED");
            entity.Property(e => e.Taxesoninterestenabled)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("TAXESONINTERESTENABLED");
            entity.Property(e => e.Taxesonpenaltyenabled)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("TAXESONPENALTYENABLED");
            entity.Property(e => e.Taxsourcekey)
                .HasMaxLength(32)
                .HasColumnName("TAXSOURCEKEY")
                .UseCollation("utf8mb3_bin");
        });

        modelBuilder.Entity<Loantransaction>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("loantransaction")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Externalid, "EXTERNALID").IsUnique();

            entity.HasIndex(e => e.Loantransactiontermskey, "LOANTRANSACTIONTERMSKEY_IDX");

            entity.HasIndex(e => e.Branchkey, "LOANTRANSACTION_BRANCHKEY");

            entity.HasIndex(e => e.Centrekey, "LOANTRANSACTION_CENTREKEY");

            entity.HasIndex(e => e.Creationdate, "LOANTRANSACTION_CREATIONDATE");

            entity.HasIndex(e => e.Entrydate, "LOANTRANSACTION_ENTRYDATE");

            entity.HasIndex(e => new { e.Entrydate, e.Reversaltransactionkey, e.Branchkey }, "LOANTRANSACTION_ENTRYDATE_REVERSALKEY_BRANCHKEY");

            entity.HasIndex(e => e.Migrationeventkey, "LOANTRANSACTION_FK10");

            entity.HasIndex(e => e.Originalcurrencycode, "LOANTRANSACTION_FK12");

            entity.HasIndex(e => e.IndexinterestrateEncodedkeyOid, "LOANTRANSACTION_FK8");

            entity.HasIndex(e => e.TaxrateEncodedkeyOid, "LOANTRANSACTION_FK9");

            entity.HasIndex(e => e.DetailsEncodedkeyOid, "LOANTRANSACTION_N49");

            entity.HasIndex(e => e.Parentaccountkey, "LOANTRANSACTION_PARENTACCOUNTKEY");

            entity.HasIndex(e => e.Parentloantransactionkey, "LOANTRANSACTION_PARENTLOANTRANSACTIONKEY");

            entity.HasIndex(e => e.Producttypekey, "LOANTRANSACTION_PRODUCTTYPEKEY");

            entity.HasIndex(e => e.Reversaltransactionkey, "LOANTRANSACTION_REVERSALTRANSACTIONKEY");

            entity.HasIndex(e => e.Transactionid, "LOANTRANSACTION_TRANSACTIONID").IsUnique();

            entity.HasIndex(e => new { e.Type, e.Entrydate, e.Branchkey }, "LOANTRANSACTION_TYPE_ENTRYDATE_BRANCHKEY_IDX");

            entity.HasIndex(e => e.Type, "LOANTRANSACTION_TYPE_IDX");

            entity.HasIndex(e => e.Userkey, "LOANTRANSACTION_USERKEY");

            entity.HasIndex(e => e.Tillkey, "TILLKEY_IDX");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Advanceposition)
                .HasPrecision(50, 10)
                .HasColumnName("ADVANCEPOSITION");
            entity.Property(e => e.Amount)
                .HasPrecision(50, 10)
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Arrearsposition)
                .HasPrecision(50, 10)
                .HasColumnName("ARREARSPOSITION");
            entity.Property(e => e.Balance)
                .HasPrecision(50, 10)
                .HasColumnName("BALANCE");
            entity.Property(e => e.Branchkey)
                .HasMaxLength(32)
                .HasColumnName("BRANCHKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Centrekey)
                .HasMaxLength(32)
                .HasColumnName("CENTREKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Comment)
                .HasMaxLength(256)
                .HasColumnName("COMMENT")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Deferredinterestamount)
                .HasPrecision(50, 10)
                .HasColumnName("DEFERREDINTERESTAMOUNT");
            entity.Property(e => e.Deferredtaxoninterestamount)
                .HasPrecision(50, 10)
                .HasColumnName("DEFERREDTAXONINTERESTAMOUNT");
            entity.Property(e => e.DetailsEncodedkeyOid)
                .HasMaxLength(32)
                .HasColumnName("DETAILS_ENCODEDKEY_OID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Entrydate)
                .HasColumnType("datetime")
                .HasColumnName("ENTRYDATE");
            entity.Property(e => e.Expectedprincipalredraw)
                .HasPrecision(50, 10)
                .HasColumnName("EXPECTEDPRINCIPALREDRAW");
            entity.Property(e => e.Externalid)
                .HasMaxLength(36)
                .HasColumnName("EXTERNALID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Feesamount)
                .HasPrecision(50, 10)
                .HasColumnName("FEESAMOUNT");
            entity.Property(e => e.Fundersinterestamount)
                .HasPrecision(50, 20)
                .HasColumnName("FUNDERSINTERESTAMOUNT");
            entity.Property(e => e.IndexinterestrateEncodedkeyOid)
                .HasMaxLength(32)
                .HasColumnName("INDEXINTERESTRATE_ENCODEDKEY_OID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Interestamount)
                .HasPrecision(50, 10)
                .HasColumnName("INTERESTAMOUNT");
            entity.Property(e => e.Interestfromarrearsamount)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("INTERESTFROMARREARSAMOUNT");
            entity.Property(e => e.Interestrate)
                .HasPrecision(50, 20)
                .HasColumnName("INTERESTRATE");
            entity.Property(e => e.Loantransactiontermskey)
                .HasMaxLength(32)
                .HasColumnName("LOANTRANSACTIONTERMSKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Migrationeventkey)
                .HasMaxLength(32)
                .HasColumnName("MIGRATIONEVENTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Organizationcommissionamount)
                .HasPrecision(50, 20)
                .HasColumnName("ORGANIZATIONCOMMISSIONAMOUNT");
            entity.Property(e => e.Originalamount)
                .HasPrecision(50, 10)
                .HasColumnName("ORIGINALAMOUNT");
            entity.Property(e => e.Originalcurrencycode)
                .HasMaxLength(3)
                .HasColumnName("ORIGINALCURRENCYCODE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Parentaccountkey)
                .HasMaxLength(32)
                .HasColumnName("PARENTACCOUNTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Parentloantransactionkey)
                .HasMaxLength(32)
                .HasColumnName("PARENTLOANTRANSACTIONKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Penaltyamount)
                .HasPrecision(50, 10)
                .HasColumnName("PENALTYAMOUNT");
            entity.Property(e => e.Principalamount)
                .HasPrecision(50, 10)
                .HasColumnName("PRINCIPALAMOUNT");
            entity.Property(e => e.Principalbalance)
                .HasPrecision(50, 10)
                .HasColumnName("PRINCIPALBALANCE");
            entity.Property(e => e.Producttypekey)
                .HasMaxLength(32)
                .HasColumnName("PRODUCTTYPEKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Redrawbalance)
                .HasPrecision(50, 10)
                .HasColumnName("REDRAWBALANCE");
            entity.Property(e => e.Reversaltransactionkey)
                .HasMaxLength(32)
                .HasColumnName("REVERSALTRANSACTIONKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Taxonfeesamount)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("TAXONFEESAMOUNT");
            entity.Property(e => e.Taxoninterestamount)
                .HasPrecision(50, 10)
                .HasColumnName("TAXONINTERESTAMOUNT");
            entity.Property(e => e.Taxoninterestfromarrearsamount)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("TAXONINTERESTFROMARREARSAMOUNT");
            entity.Property(e => e.Taxonpenaltyamount)
                .HasPrecision(50, 10)
                .HasDefaultValueSql("'0.0000000000'")
                .HasColumnName("TAXONPENALTYAMOUNT");
            entity.Property(e => e.TaxrateEncodedkeyOid)
                .HasMaxLength(32)
                .HasColumnName("TAXRATE_ENCODEDKEY_OID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Tillkey)
                .HasMaxLength(32)
                .HasColumnName("TILLKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Transactionid)
                .ValueGeneratedOnAdd()
                .HasColumnName("TRANSACTIONID");
            entity.Property(e => e.Type)
                .HasMaxLength(256)
                .HasColumnName("TYPE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Userkey)
                .HasMaxLength(32)
                .HasColumnName("USERKEY")
                .UseCollation("utf8mb3_bin");

            entity.HasOne(d => d.BranchkeyNavigation).WithMany(p => p.Loantransactions)
                .HasForeignKey(d => d.Branchkey)
                .HasConstraintName("LOANTRANSACTION_FK5");

            entity.HasOne(d => d.DetailsEncodedkeyO).WithMany(p => p.Loantransactions)
                .HasForeignKey(d => d.DetailsEncodedkeyOid)
                .HasConstraintName("LOANTRANSACTION_FK1");

            entity.HasOne(d => d.ParentaccountkeyNavigation).WithMany(p => p.Loantransactions)
                .HasForeignKey(d => d.Parentaccountkey)
                .HasConstraintName("LOANTRANSACTION_FK4");

            entity.HasOne(d => d.ParentloantransactionkeyNavigation).WithMany(p => p.InverseParentloantransactionkeyNavigation)
                .HasForeignKey(d => d.Parentloantransactionkey)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("LOANTRANSACTION_FK11");

            entity.HasOne(d => d.ProducttypekeyNavigation).WithMany(p => p.Loantransactions)
                .HasForeignKey(d => d.Producttypekey)
                .HasConstraintName("LOANTRANSACTION_PRODUCTTYPEKEY");

            entity.HasOne(d => d.ReversaltransactionkeyNavigation).WithMany(p => p.InverseReversaltransactionkeyNavigation)
                .HasForeignKey(d => d.Reversaltransactionkey)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("LOANTRANSACTION_FK2");
        });

        modelBuilder.Entity<Productaccountinghub>(entity =>
        {
            entity.HasKey(e => e.Productencodedkey).HasName("PRIMARY");

            entity.ToTable("productaccountinghub");

            entity.Property(e => e.Productencodedkey)
                .HasMaxLength(45)
                .HasColumnName("PRODUCTENCODEDKEY");
            entity.Property(e => e.Accountchart)
                .HasMaxLength(45)
                .HasColumnName("ACCOUNTCHART");
            entity.Property(e => e.Enable).HasColumnName("ENABLE");
            entity.Property(e => e.Productid)
                .HasMaxLength(45)
                .HasColumnName("PRODUCTID");
        });

        modelBuilder.Entity<Sheetparametrization>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity.ToTable("sheetparametrization");

            entity.HasIndex(e => new { e.Currentstage, e.Currentglcode, e.Currentloantrxtype }, "idx_sheetparametrization_STAGE_GLCODE_LOANTX");

            entity.HasIndex(e => new { e.Currentstage, e.Currentglcode, e.Currentloantrxtype, e.Currentjetype }, "idx_sheetparametrization_STAGE_GLCODE_LOANTX_JETYPE");

            entity.HasIndex(e => new { e.Currentstage, e.Currentloantrxtype, e.Currentjetype }, "idx_sheetparametrization_STAGE_LOANTX_JETYPE");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(45)
                .HasColumnName("ENCODEDKEY");
            entity.Property(e => e.Accountchart)
                .HasMaxLength(45)
                .HasComment("ACCOUNTCHART")
                .HasColumnName("ACCOUNTCHART");
            entity.Property(e => e.Adjust).HasColumnName("ADJUST");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Currentglcode)
                .HasMaxLength(45)
                .HasColumnName("CURRENTGLCODE");
            entity.Property(e => e.Currentjetype)
                .HasMaxLength(45)
                .HasColumnName("CURRENTJETYPE");
            entity.Property(e => e.Currentloantrxtype)
                .HasMaxLength(45)
                .HasColumnName("CURRENTLOANTRXTYPE");
            entity.Property(e => e.Currentstage)
                .HasMaxLength(45)
                .HasColumnName("CURRENTSTAGE");
            entity.Property(e => e.Enable).HasColumnName("ENABLE");
            entity.Property(e => e.Exclusionglcodes)
                .HasMaxLength(500)
                .HasColumnName("EXCLUSIONGLCODES");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(5000)
                .HasColumnName("OBSERVACIONES");
            entity.Property(e => e.Orderaccount).HasColumnName("ORDERACCOUNT");
            entity.Property(e => e.Outputglcode)
                .HasMaxLength(45)
                .HasColumnName("OUTPUTGLCODE");
            entity.Property(e => e.Outputglname)
                .HasMaxLength(100)
                .HasColumnName("OUTPUTGLNAME");
            entity.Property(e => e.Outputjetype)
                .HasMaxLength(45)
                .HasColumnName("OUTPUTJETYPE");
            entity.Property(e => e.Overdueppal).HasColumnName("OVERDUEPPAL");
            entity.Property(e => e.Principal).HasColumnName("PRINCIPAL");
            entity.Property(e => e.Taxentry).HasColumnName("TAXENTRY");
        });

        modelBuilder.Entity<Transactionchannel>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("transactionchannel")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.LoanCustomFilterConstraintKey, "FK_TRANSACTIONCHANNEL_CUSTOMFILTER_LOAN_CONSTRAINT_KEY_IDX");

            entity.HasIndex(e => e.SavingsCustomFilterConstraintKey, "FK_TRANSACTIONCHANNEL_CUSTOMFILTER_SAVINGS_CONSTRAINT_KEY_IDX");

            entity.HasIndex(e => e.Createdbyuserkey, "TRANSACTIONCHANNEL_FK1");

            entity.HasIndex(e => e.Id, "TRANSACTIONCHANNEL_U1").IsUnique();

            entity.HasIndex(e => e.Usagerightskey, "TRANSACTIONCHANNEL_USAGERIGHTS_KEY");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Activated)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("ACTIVATED");
            entity.Property(e => e.Createdbyuserkey)
                .HasMaxLength(32)
                .HasColumnName("CREATEDBYUSERKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Creationdate)
                .HasColumnType("datetime")
                .HasColumnName("CREATIONDATE");
            entity.Property(e => e.Id)
                .HasMaxLength(32)
                .HasColumnName("ID")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Index).HasColumnName("INDEX");
            entity.Property(e => e.LoanCustomFilterConstraintKey)
                .HasMaxLength(32)
                .HasColumnName("LOAN_CUSTOM_FILTER_CONSTRAINT_KEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Loanconstraintsusage)
                .HasMaxLength(255)
                .HasDefaultValueSql("'UNCONSTRAINED_USAGE'")
                .HasColumnName("LOANCONSTRAINTSUSAGE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.SavingsCustomFilterConstraintKey)
                .HasMaxLength(32)
                .HasColumnName("SAVINGS_CUSTOM_FILTER_CONSTRAINT_KEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Savingsconstraintsusage)
                .HasMaxLength(255)
                .HasDefaultValueSql("'UNCONSTRAINED_USAGE'")
                .HasColumnName("SAVINGSCONSTRAINTSUSAGE")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Usagerightskey)
                .HasMaxLength(32)
                .HasColumnName("USAGERIGHTSKEY")
                .UseCollation("utf8mb3_bin");
        });

        modelBuilder.Entity<Transactiondetail>(entity =>
        {
            entity.HasKey(e => e.Encodedkey).HasName("PRIMARY");

            entity
                .ToTable("transactiondetails")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Targetsavingsaccountkey, "TARGETSAVINGSACCOUNTKEY_IDX");

            entity.HasIndex(e => e.Transactionchannelkey, "TRANSACTIONCHANNELKEY_IDX");

            entity.Property(e => e.Encodedkey)
                .HasMaxLength(32)
                .HasColumnName("ENCODEDKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Internaltransfer)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("INTERNALTRANSFER");
            entity.Property(e => e.Targetsavingsaccountkey)
                .HasMaxLength(32)
                .HasColumnName("TARGETSAVINGSACCOUNTKEY")
                .UseCollation("utf8mb3_bin");
            entity.Property(e => e.Transactionchannelkey)
                .HasMaxLength(32)
                .HasColumnName("TRANSACTIONCHANNELKEY")
                .UseCollation("utf8mb3_bin");

            entity.HasOne(d => d.TransactionchannelkeyNavigation).WithMany(p => p.Transactiondetails)
                .HasForeignKey(d => d.Transactionchannelkey)
                .HasConstraintName("TRANSACTIONDETAILS_FK1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
