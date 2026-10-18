using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Models;

public partial class CharityContext : DbContext
{
    public CharityContext()
    {
    }

    public CharityContext(DbContextOptions<CharityContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<FinanceTransaction> FinanceTransactions { get; set; }

    public virtual DbSet<HelpRequest> HelpRequests { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<Volunteer> Volunteers { get; set; }

    public virtual DbSet<VolunteerShift> VolunteerShifts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=charitynew;Username=postgres;Password=291205nana");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Beneficiary>(entity =>
        {
            entity.HasKey(e => e.BeneficiaryId).HasName("beneficiary_pkey");

            entity.ToTable("beneficiary");

            entity.HasIndex(e => e.PassportNumber, "beneficiary_passport_number_key").IsUnique();

            entity.HasIndex(e => e.UserId, "beneficiary_user_id_key").IsUnique();

            entity.Property(e => e.BeneficiaryId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("beneficiary_id");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(20)
                .HasColumnName("passport_number");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Beneficiary)
                .HasForeignKey<Beneficiary>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("beneficiary_user_id_fkey");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("contract_pkey");

            entity.ToTable("contract");

            entity.HasIndex(e => e.ProjectId, "idx_contract_project");

            entity.Property(e => e.ContractId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("contract_id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SigningDate).HasColumnName("signing_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'чинний'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Subject).HasColumnName("subject");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

            entity.HasOne(d => d.Project).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contract_project_id_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contract_supplier_id_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employee_pkey");

            entity.ToTable("employee");

            entity.HasIndex(e => e.ContactPhone, "employee_contact_phone_key").IsUnique();

            entity.HasIndex(e => e.Login, "employee_login_key").IsUnique();

            entity.Property(e => e.EmployeeId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("employee_id");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(20)
                .HasColumnName("contact_phone");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
        });

        modelBuilder.Entity<FinanceTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("finance_transaction_pkey");

            entity.ToTable("finance_transaction");

            entity.HasIndex(e => e.ProjectId, "idx_finance_project");

            entity.Property(e => e.TransactionId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("transaction_id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Purpose).HasColumnName("purpose");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("transaction_date");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.Contract).WithMany(p => p.FinanceTransactions)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("finance_transaction_contract_id_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.FinanceTransactions)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("finance_transaction_employee_id_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.FinanceTransactions)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("finance_transaction_project_id_fkey");
        });

        modelBuilder.Entity<HelpRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("help_request_pkey");

            entity.ToTable("help_request");

            entity.HasIndex(e => e.BeneficiaryId, "idx_request_beneficiary");

            entity.HasIndex(e => e.EmployeeId, "idx_request_employee");

            entity.Property(e => e.RequestId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("request_id");
            entity.Property(e => e.BeneficiaryId).HasColumnName("beneficiary_id");
            entity.Property(e => e.DeliveryAddress)
                .HasMaxLength(255)
                .HasColumnName("delivery_address");
            entity.Property(e => e.DocumentPath)
                .HasMaxLength(500)
                .HasColumnName("document_path");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.NeedDescription).HasColumnName("need_description");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("registration_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'нове'::character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.Beneficiary).WithMany(p => p.HelpRequests)
                .HasForeignKey(d => d.BeneficiaryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("help_request_beneficiary_id_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.HelpRequests)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("help_request_employee_id_fkey");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("project_pkey");

            entity.ToTable("project");

            entity.HasIndex(e => e.RequestId, "project_request_id_key").IsUnique();

            entity.Property(e => e.ProjectId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("project_id");
            entity.Property(e => e.ApprovedBudget)
                .HasPrecision(12, 2)
                .HasColumnName("approved_budget");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'планується'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.Employee).WithMany(p => p.Projects)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("project_employee_id_fkey");

            entity.HasOne(d => d.Request).WithOne(p => p.Project)
                .HasForeignKey<Project>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("project_request_id_fkey");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("supplier_pkey");

            entity.ToTable("supplier");

            entity.HasIndex(e => e.EdrpouCode, "supplier_edrpou_code_key").IsUnique();

            entity.Property(e => e.SupplierId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("supplier_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(200)
                .HasColumnName("company_name");
            entity.Property(e => e.Contacts).HasColumnName("contacts");
            entity.Property(e => e.EdrpouCode)
                .HasMaxLength(10)
                .HasColumnName("edrpou_code");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_account_pkey");

            entity.ToTable("user_account");

            entity.HasIndex(e => e.Phone, "user_account_phone_key").IsUnique();

            entity.Property(e => e.UserId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("user_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Volunteer>(entity =>
        {
            entity.HasKey(e => e.VolunteerId).HasName("volunteer_pkey");

            entity.ToTable("volunteer");

            entity.HasIndex(e => e.UserId, "volunteer_user_id_key").IsUnique();

            entity.Property(e => e.VolunteerId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("volunteer_id");
            entity.Property(e => e.Rating)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Volunteer)
                .HasForeignKey<Volunteer>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("volunteer_user_id_fkey");
        });

        modelBuilder.Entity<VolunteerShift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("volunteer_shift_pkey");

            entity.ToTable("volunteer_shift");

            entity.HasIndex(e => e.ProjectId, "idx_shift_project");

            entity.HasIndex(e => e.VolunteerId, "idx_shift_volunteer");

            entity.Property(e => e.ShiftId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("shift_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.HoursWorked).HasColumnName("hours_worked");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ShiftDate).HasColumnName("shift_date");
            entity.Property(e => e.VolunteerId).HasColumnName("volunteer_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.VolunteerShifts)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("volunteer_shift_employee_id_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.VolunteerShifts)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("volunteer_shift_project_id_fkey");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.VolunteerShifts)
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("volunteer_shift_volunteer_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
