using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RazorLogin.test2;

public partial class tempContClos : DbContext
{
    public tempContClos()
    {
    }

    public tempContClos(DbContextOptions<tempContClos> options)
        : base(options)
    {
    }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Closing> Closings { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Dependant> Dependants { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Enclosure> Enclosures { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<FoodStore> FoodStores { get; set; }

    public virtual DbSet<GiftShop> GiftShops { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Zookeeper> Zookeepers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:zoodotnet.database.windows.net,1433;Initial Catalog=zooDB;Persist Security Info=False;User ID=login;Password=Zoo1234!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.AnimalId).HasName("PK_Animals_Animal_ID");

            entity.HasIndex(e => e.AnimalId, "Animals$Animal_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.ZookeeperId, "Animals_ibfk_1");

            entity.HasIndex(e => e.EnclosureId, "Animals_ibfk_2");

            entity.Property(e => e.AnimalId)
                .ValueGeneratedNever()
                .HasColumnName("Animal_ID");
            entity.Property(e => e.AnimalDob)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Animal_DOB");
            entity.Property(e => e.AnimalName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Animal_Name");
            entity.Property(e => e.AnimalSex)
                .HasMaxLength(1)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Animal_Sex");
            entity.Property(e => e.ArrivalDate)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Arrival_date");
            entity.Property(e => e.Diet)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.EnclosureId).HasColumnName("Enclosure_ID");
            entity.Property(e => e.EndangeredStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Endangered_status");
            entity.Property(e => e.FeedingTime)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Feeding_time");
            entity.Property(e => e.MedicalNotes)
                .HasMaxLength(200)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Medical_notes");
            entity.Property(e => e.Species)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.ZookeeperId).HasColumnName("Zookeeper_ID");

            entity.HasOne(d => d.Enclosure).WithMany(p => p.Animals)
                .HasForeignKey(d => d.EnclosureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Animals$Animals_ibfk_2");

            entity.HasOne(d => d.Zookeeper).WithMany(p => p.Animals)
                .HasForeignKey(d => d.ZookeeperId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Animals$Animals_ibfk_1");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Closing>(entity =>
        {
            entity.HasKey(e => e.ClosingId).HasName("PK__Closings__1D417B3F30CFE06C");

            entity.HasIndex(e => e.ClosingId, "UQ__Closings__1D417B3E8415ED77").IsUnique();

            entity.Property(e => e.ClosingId).HasColumnName("Closing_ID");
            entity.Property(e => e.ClosingsEnd)
                .HasPrecision(0)
                .HasColumnName("Closings_end");
            entity.Property(e => e.ClosingsReason)
                .HasMaxLength(50)
                .HasColumnName("Closings_reason");
            entity.Property(e => e.ClosingsStart)
                .HasPrecision(0)
                .HasColumnName("Closings_start");
            entity.Property(e => e.EnclosureId).HasColumnName("Enclosure_ID");

            entity.HasOne(d => d.Enclosure).WithMany(p => p.Closings)
                .HasForeignKey(d => d.EnclosureId)
                .HasConstraintName("FK_Enclosure_Closing");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK_Customer_Customer_ID");

            entity.ToTable("Customer");

            entity.HasIndex(e => e.CustomerId, "Customer$Customer_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CustomerEmail, "CustomerEmailUNIQUE").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("Customer_ID");
            entity.Property(e => e.CustomerAddress)
                .HasMaxLength(120)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Customer_Address");
            entity.Property(e => e.CustomerDob)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Customer_DOB");
            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(45)
                .HasColumnName("Customer_Email");
            entity.Property(e => e.CustomerFirstName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Customer_First_name");
            entity.Property(e => e.CustomerLastName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Customer_Last_name");
            entity.Property(e => e.MembershipEndDate).HasColumnName("Membership_end_date");
            entity.Property(e => e.MembershipStartDate).HasColumnName("Membership_start_date");
            entity.Property(e => e.MembershipType)
                .HasMaxLength(20)
                .HasColumnName("Membership_type");

            entity.HasMany(d => d.FoodStores).WithMany(p => p.Customers)
                .UsingEntity<Dictionary<string, object>>(
                    "CustomerFoodStore",
                    r => r.HasOne<FoodStore>().WithMany()
                        .HasForeignKey("FoodStoreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("customer_Food_store$customer_Food_store_ibfk_2"),
                    l => l.HasOne<Customer>().WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("customer_Food_store$customer_Food_store_ibfk_1"),
                    j =>
                    {
                        j.HasKey("CustomerId", "FoodStoreId").HasName("PK_customer_Food_store_Customer_ID");
                        j.ToTable("customer_Food_store");
                        j.HasIndex(new[] { "FoodStoreId" }, "customer_Food_store_ibfk_2");
                        j.IndexerProperty<int>("CustomerId").HasColumnName("Customer_ID");
                        j.IndexerProperty<int>("FoodStoreId").HasColumnName("Food_store_ID");
                    });

            entity.HasMany(d => d.Shops).WithMany(p => p.Customers)
                .UsingEntity<Dictionary<string, object>>(
                    "CustomerGiftShop",
                    r => r.HasOne<GiftShop>().WithMany()
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("customer_Gift_shop$customer_Gift_shop_ibfk_2"),
                    l => l.HasOne<Customer>().WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("customer_Gift_shop$customer_Gift_shop_ibfk_1"),
                    j =>
                    {
                        j.HasKey("CustomerId", "ShopId").HasName("PK_customer_Gift_shop_Customer_ID");
                        j.ToTable("customer_Gift_shop");
                        j.HasIndex(new[] { "ShopId" }, "customer_Gift_shop_ibfk_2");
                        j.IndexerProperty<int>("CustomerId").HasColumnName("Customer_ID");
                        j.IndexerProperty<int>("ShopId").HasColumnName("Shop_ID");
                    });
        });

        modelBuilder.Entity<Dependant>(entity =>
        {
            entity.HasKey(e => e.DepndantId).HasName("PK_Dependant_Depndant_ID");

            entity.ToTable("Dependant");

            entity.HasIndex(e => e.DepndantId, "Dependant$Depndant_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.EmployeeId, "Dependant_ibfk_1");

            entity.Property(e => e.DepndantId)
                .ValueGeneratedNever()
                .HasColumnName("Depndant_ID");
            entity.Property(e => e.DependentDob)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Dependent_DOB");
            entity.Property(e => e.DependentName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Dependent_Name");
            entity.Property(e => e.DependentSex)
                .HasMaxLength(1)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Dependent_Sex");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.HealthcareTier)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Healthcare_tier");
            entity.Property(e => e.Relationship)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)");

            entity.HasOne(d => d.Employee).WithMany(p => p.Dependants)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Dependant$Dependant_ibfk_1");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK_Employee_Employee_ID");

            entity.ToTable("Employee", tb => tb.HasTrigger("check_employee_over_18"));

            entity.HasIndex(e => e.EmployeeId, "Employee$Employee_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Ssn, "Employee$SSN_UNIQUE").IsUnique();

            entity.HasIndex(e => e.EmployeeEmail, "EmployeeEmailUNIQUE").IsUnique();

            entity.HasIndex(e => e.SupervisorId, "Employee_ibfk_1");

            entity.HasIndex(e => e.ShopId, "Employee_ibfk_2");

            entity.HasIndex(e => e.FoodStoreId, "Employee_ibfk_3");

            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.DateOfEmployment)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Date_of_employment");
            entity.Property(e => e.Degree)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.EmployeeAddress)
                .HasMaxLength(120)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_Address");
            entity.Property(e => e.EmployeeDob)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_DOB");
            entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(45)
                .IsFixedLength()
                .HasColumnName("Employee_Email");
            entity.Property(e => e.EmployeeFirstName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_First_name");
            entity.Property(e => e.EmployeeLastName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_Last_name");
            entity.Property(e => e.EmployeePhoneNumber)
                .HasMaxLength(10)
                .HasColumnName("Employee_Phone_number");
            entity.Property(e => e.EmployeeSalary)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Employee_Salary");
            entity.Property(e => e.FoodStoreId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Food_store_ID");
            entity.Property(e => e.ShopId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Shop_ID");
            entity.Property(e => e.Ssn).HasColumnName("SSN");
            entity.Property(e => e.SupervisorId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Supervisor_ID");

            entity.HasOne(d => d.FoodStore).WithMany(p => p.Employees)
                .HasForeignKey(d => d.FoodStoreId)
                .HasConstraintName("Employee$Employee_ibfk_3");

            entity.HasOne(d => d.Shop).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ShopId)
                .HasConstraintName("Employee$Employee_ibfk_2");

            entity.HasOne(d => d.Supervisor).WithMany(p => p.Employees)
                .HasForeignKey(d => d.SupervisorId)
                .HasConstraintName("Employee$Employee_ibfk_1");
        });

        modelBuilder.Entity<Enclosure>(entity =>
        {
            entity.HasKey(e => e.EnclosureId).HasName("PK_Enclosure_Enclosure_ID");

            entity.ToTable("Enclosure");

            entity.HasIndex(e => e.EnclosureId, "Enclosure$Enclosure_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.ZookeeperId, "Enclosure_ibfk_1");

            entity.Property(e => e.EnclosureId)
                .ValueGeneratedNever()
                .HasColumnName("Enclosure_ID");
            entity.Property(e => e.AnimalId).HasColumnName("Animal_ID");
            entity.Property(e => e.EnclosureCleaningTime).HasColumnName("Enclosure_Cleaning_time");
            entity.Property(e => e.EnclosureCloseTime).HasColumnName("Enclosure_Close_time");
            entity.Property(e => e.EnclosureDepartment)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Enclosure_Department");
            entity.Property(e => e.EnclosureFeedingTime).HasColumnName("Enclosure_Feeding_time");
            entity.Property(e => e.EnclosureName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Enclosure_Name");
            entity.Property(e => e.EnclosureOpenTime).HasColumnName("Enclosure_Open_time");
            entity.Property(e => e.IsClosed).HasDefaultValue(false);
            entity.Property(e => e.OccupancyStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Occupancy_Status");
            entity.Property(e => e.ZookeeperId).HasColumnName("Zookeeper_ID");

            entity.HasOne(d => d.Zookeeper).WithMany(p => p.Enclosures)
                .HasForeignKey(d => d.ZookeeperId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Enclosure$Enclosure_ibfk_1");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK_Event_Event_ID");

            entity.ToTable("Event", tb => tb.HasTrigger("PreventDuplicateEvent"));

            entity.HasIndex(e => e.EventId, "Event$Event_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.EventEmployeeRepId, "Event_ibfk_1");

            entity.Property(e => e.EventId)
                .ValueGeneratedNever()
                .HasColumnName("Event_ID");
            entity.Property(e => e.EventDate).HasColumnName("Event_date");
            entity.Property(e => e.EventEmployeeRepId).HasColumnName("Event_employee_rep_ID");
            entity.Property(e => e.EventEndTime).HasColumnName("Event_end_time");
            entity.Property(e => e.EventLocation)
                .HasMaxLength(70)
                .IsFixedLength()
                .HasColumnName("Event_Location");
            entity.Property(e => e.EventName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Event_name");
            entity.Property(e => e.EventStartTime).HasColumnName("Event_start_time");

            entity.HasOne(d => d.EventEmployeeRep).WithMany(p => p.Events)
                .HasForeignKey(d => d.EventEmployeeRepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event$Event_ibfk_1");
        });

        modelBuilder.Entity<FoodStore>(entity =>
        {
            entity.HasKey(e => e.FoodStoreId).HasName("PK_Food_store_Food_store_ID");

            entity.ToTable("Food_store");

            entity.HasIndex(e => e.FoodStoreId, "Food_store$Food_store_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.FoodStoreLocation, "Food_store$Food_store_locationl_UNIQUE").IsUnique();

            entity.HasIndex(e => e.FoodStoreName, "Food_store$Food_store_name_UNIQUE").IsUnique();

            entity.Property(e => e.FoodStoreId)
                .ValueGeneratedNever()
                .HasColumnName("Food_store_ID");
            entity.Property(e => e.FoodStoreCloseTime)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Food_store_close_time");
            entity.Property(e => e.FoodStoreCustomerCapacity)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Food_store_customer_capacity");
            entity.Property(e => e.FoodStoreLocation)
                .HasMaxLength(45)
                .HasColumnName("Food_store_location");
            entity.Property(e => e.FoodStoreName)
                .HasMaxLength(45)
                .HasColumnName("Food_store_name");
            entity.Property(e => e.FoodStoreOpenTime)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Food_store_open_time");
            entity.Property(e => e.FoodStoreType)
                .HasMaxLength(9)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Food_store_type");
            entity.Property(e => e.FoodStoreYearToDateSales).HasColumnName("Food_store_year_to_date_sales");
        });

        modelBuilder.Entity<GiftShop>(entity =>
        {
            entity.HasKey(e => e.ShopId).HasName("PK_Gift_shop_Shop_ID");

            entity.ToTable("Gift_shop", tb => tb.HasTrigger("InvalidClosingTime_GiftShop"));

            entity.HasIndex(e => e.ShopId, "Gift_shop$Shop_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.GiftShopLocation, "Gift_shop$Shop_Location_UNIQUE").IsUnique();

            entity.HasIndex(e => e.GiftShopName, "Gift_shop$Shop_Name_UNIQUE").IsUnique();

            entity.Property(e => e.ShopId)
                .ValueGeneratedNever()
                .HasColumnName("Shop_ID");
            entity.Property(e => e.GiftShopCloseTime).HasColumnName("Gift_shop_close_time");
            entity.Property(e => e.GiftShopLocation)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Gift_shop_Location");
            entity.Property(e => e.GiftShopName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Gift_shop_Name");
            entity.Property(e => e.GiftShopOpenTime).HasColumnName("Gift_shop_open_time");
            entity.Property(e => e.GiftShopYearToDateSales).HasColumnName("Gift_shop_year_to_date_sales");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK_Item_item_ID");

            entity.ToTable("Item");

            entity.HasIndex(e => e.FoodStoreId, "FK_Food_store_id");

            entity.HasIndex(e => e.ItemId, "Item$item_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.ShopId, "Item_ibfk_1");

            entity.Property(e => e.ItemId)
                .ValueGeneratedNever()
                .HasColumnName("item_ID");
            entity.Property(e => e.FoodStoreId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Food_store_ID");
            entity.Property(e => e.ItemCount).HasColumnName("Item_count");
            entity.Property(e => e.ItemName)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Item_name");
            entity.Property(e => e.ItemPrice).HasColumnName("item_price");
            entity.Property(e => e.RestockDate).HasColumnName("Restock_date");
            entity.Property(e => e.ShopId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Shop_ID");

            entity.HasOne(d => d.FoodStore).WithMany(p => p.Items)
                .HasForeignKey(d => d.FoodStoreId)
                .HasConstraintName("FK__Item__Food_store__7167D3BD");

            entity.HasOne(d => d.Shop).WithMany(p => p.Items)
                .HasForeignKey(d => d.ShopId)
                .HasConstraintName("FK__Item__Shop_ID__7073AF84");

            entity.HasMany(d => d.FoodStores).WithMany(p => p.ItemsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "ItemFoodStore",
                    r => r.HasOne<FoodStore>().WithMany()
                        .HasForeignKey("FoodStoreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("item_Food_store$item_Food_store_ibfk_2"),
                    l => l.HasOne<Item>().WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("item_Food_store$item_Food_store_ibfk_1"),
                    j =>
                    {
                        j.HasKey("ItemId", "FoodStoreId").HasName("PK_item_Food_store_Item_ID");
                        j.ToTable("item_Food_store");
                        j.HasIndex(new[] { "FoodStoreId" }, "item_Food_store_ibfk_2");
                        j.IndexerProperty<int>("ItemId").HasColumnName("Item_ID");
                        j.IndexerProperty<int>("FoodStoreId").HasColumnName("Food_store_ID");
                    });

            entity.HasMany(d => d.Shops).WithMany(p => p.ItemsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "ItemGiftShop",
                    r => r.HasOne<GiftShop>().WithMany()
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("item_Gift_shop$item_Gift_shop_ibfk_2"),
                    l => l.HasOne<Item>().WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("item_Gift_shop$item_Gift_shop_ibfk_1"),
                    j =>
                    {
                        j.HasKey("ItemId", "ShopId").HasName("PK_item_Gift_shop_Item_ID");
                        j.ToTable("item_Gift_shop");
                        j.HasIndex(new[] { "ShopId" }, "item_Gift_shop_ibfk_2");
                        j.IndexerProperty<int>("ItemId").HasColumnName("Item_ID");
                        j.IndexerProperty<int>("ShopId").HasColumnName("Shop_ID");
                    });
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.ManagerId).HasName("PK_Manager_Manager_ID");

            entity.ToTable("Manager");

            entity.HasIndex(e => e.EmployeeId, "Manager$Employee_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.ManagerId, "Manager$Manager_ID_UNIQUE").IsUnique();

            entity.Property(e => e.ManagerId)
                .ValueGeneratedNever()
                .HasColumnName("Manager_ID");
            entity.Property(e => e.Department).HasMaxLength(45);
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.ManagerEmploymentDate)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Manager_employment_date");

            entity.HasOne(d => d.Employee).WithOne(p => p.Manager)
                .HasForeignKey<Manager>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Manager$Manager_ibfk_1");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK_Purchase_Purchase_ID");

            entity.ToTable("Purchase");

            entity.HasIndex(e => e.PurchaseId, "Purchase$Purchase_ID_UNIQUE").IsUnique();

            entity.Property(e => e.PurchaseId)
                .ValueGeneratedNever()
                .HasColumnName("Purchase_ID");
            entity.Property(e => e.CustomerId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Customer_ID");
            entity.Property(e => e.NumItems)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("num_items");
            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Purchase_date");
            entity.Property(e => e.PurchaseMethod)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Purchase_method");
            entity.Property(e => e.PurchaseTime)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Purchase_time");
            entity.Property(e => e.StoreId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Store_ID");
            entity.Property(e => e.TotalPurchasesPrice)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Total_purchases_price");

            entity.HasOne(d => d.Customer).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("Purchase$Purchase_ibfk_1");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK_Ticket_Ticket_ID");

            entity.ToTable("Ticket");

            entity.HasIndex(e => e.TicketId, "Ticket$Ticket_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.PurchaseId, "Ticket_ibfk_1");

            entity.Property(e => e.TicketId)
                .ValueGeneratedNever()
                .HasColumnName("Ticket_ID");
            entity.Property(e => e.PurchaseId).HasColumnName("Purchase_ID");
            entity.Property(e => e.TicketEntryTime)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Ticket_Entry_time");
            entity.Property(e => e.TicketPrice).HasColumnName("Ticket_Price");
            entity.Property(e => e.TicketPurchaseDate)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Ticket_Purchase_date");
            entity.Property(e => e.TicketType)
                .HasMaxLength(7)
                .HasColumnName("Ticket_Type");

            entity.HasOne(d => d.Purchase).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Ticket$Ticket_ibfk_1");
        });

        modelBuilder.Entity<Zookeeper>(entity =>
        {
            entity.HasKey(e => e.ZookeeperId).HasName("PK_Zookeeper_Zookeeper_ID");

            entity.ToTable("Zookeeper");

            entity.HasIndex(e => e.EmployeeId, "Zookeeper$Employee_ID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.ZookeeperId, "Zookeeper$Zookeeper_ID_UNIQUE").IsUnique();

            entity.Property(e => e.ZookeeperId)
                .ValueGeneratedNever()
                .HasColumnName("Zookeeper_ID");
            entity.Property(e => e.AssignedDepartment)
                .HasMaxLength(45)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Assigned_department");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.LastTrainingDate)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Last_training_date");
            entity.Property(e => e.NumAssignedCages)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Num_Assigned_cages");
            entity.Property(e => e.TrainingRenewalDate)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Training_renewal_date");

            entity.HasOne(d => d.Employee).WithOne(p => p.Zookeeper)
                .HasForeignKey<Zookeeper>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Zookeeper$Zookeeper_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
