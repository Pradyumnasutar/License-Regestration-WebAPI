using System;
using System.Collections.Generic;
using LeS_License_Registry_API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeS_License_Registry_API.Data;

public partial class LesLicenseRegistryContext : DbContext
{
    public LesLicenseRegistryContext()
    {
    }

    public LesLicenseRegistryContext(DbContextOptions<LesLicenseRegistryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LesIpAccessControl> les_ip_access_control { get; set; }

    public virtual DbSet<LesLicenseApplication> les_license_applications { get; set; }

    public virtual DbSet<LesLicenseRegistry>les_license_registry { get; set; }

    public virtual DbSet<LesLicenseUpdateLog> les_license_update_log { get; set; }
    public virtual DbSet<V_Les_License_Update_Log> v_les_license_update_log { get; set; }
    public virtual DbSet<LesLicenseControlUsers> les_license_control_users { get; set; }

    
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<LesIpAccessControl>(entity =>
    //    {
    //        entity.HasKey(e => e.Ipaccessid);

    //        entity.ToTable("LES_IP_ACCESS_CONTROL");

    //        entity.HasIndex(e => e.IpAddress, "idx_ip_lookup").IsUnique();

    //        entity.Property(e => e.Ipaccessid).HasColumnName("IPACCESSID");
    //        entity.Property(e => e.AccessType)
    //            .HasMaxLength(10)
    //            .HasColumnName("ACCESS_TYPE");
    //        entity.Property(e => e.CreatedDate)
    //            .HasColumnType("datetime")
    //            .HasColumnName("CREATED_DATE");
    //        entity.Property(e => e.IpAddress)
    //            .HasMaxLength(45)
    //            .HasColumnName("IP_ADDRESS");
    //        entity.Property(e => e.Remarks)
    //            .HasMaxLength(255)
    //            .HasColumnName("REMARKS");
    //        entity.Property(e => e.UpdatedDate)
    //            .HasColumnType("datetime")
    //            .HasColumnName("UPDATED_DATE");
    //    });

    //    modelBuilder.Entity<LesLicenseApplication>(entity =>
    //    {
    //        entity.HasKey(e => e.Licenseapplicationid);

    //        entity.ToTable("LES_LICENSE_APPLICATIONS");

    //        entity.Property(e => e.Licenseapplicationid).HasColumnName("LICENSEAPPLICATIONID");
    //        entity.Property(e => e.ApplicationName)
    //            .HasMaxLength(50)
    //            .HasColumnName("APPLICATION_NAME");
    //        entity.Property(e => e.ApplicationVersion)
    //            .HasMaxLength(50)
    //            .HasColumnName("APPLICATION_VERSION");
    //        entity.Property(e => e.LastAccessedDate)
    //            .HasColumnType("datetime")
    //            .HasColumnName("LAST_ACCESSED_DATE");
    //        entity.Property(e => e.LastAccessedIp)
    //            .HasMaxLength(50)
    //            .HasColumnName("LAST_ACCESSED_IP");
    //        entity.Property(e => e.Licenseid).HasColumnName("LICENSEID");

    //        entity.HasOne(d => d.License).WithMany(p => p.LesLicenseApplications)
    //            .HasForeignKey(d => d.Licenseid)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK_LES_LICENSE_APPLICATIONS_LES_LICENSE_REGISTRY");
    //    });

    //    modelBuilder.Entity<LesLicenseRegistry>(entity =>
    //    {
    //        entity.HasKey(e => e.Licenseid);

    //        entity.ToTable("LES_LICENSE_REGISTRY");

    //        entity.Property(e => e.Licenseid).HasColumnName("LICENSEID");
    //        entity.Property(e => e.ActivatedBy)
    //            .HasMaxLength(100)
    //            .HasColumnName("ACTIVATED_BY");
    //        entity.Property(e => e.ActivatedDate)
    //            .HasColumnType("datetime")
    //            .HasColumnName("ACTIVATED_DATE");
    //        entity.Property(e => e.ActivationKey)
    //            .HasMaxLength(100)
    //            .HasColumnName("ACTIVATION_KEY");
    //        entity.Property(e => e.BypassLicense).HasColumnName("BYPASS_LICENSE");
    //        entity.Property(e => e.CustomerName)
    //            .HasMaxLength(50)
    //            .HasColumnName("CUSTOMER_NAME");
    //        entity.Property(e => e.ExpiryDate)
    //            .HasColumnType("datetime")
    //            .HasColumnName("EXPIRY_DATE");
    //        entity.Property(e => e.LicensePeriod).HasColumnName("LICENSE_PERIOD");
    //        entity.Property(e => e.MachineName)
    //            .HasMaxLength(100)
    //            .HasColumnName("MACHINE_NAME");
    //        entity.Property(e => e.Remarks)
    //            .HasColumnType("ntext")
    //            .HasColumnName("REMARKS");
    //        entity.Property(e => e.RevokedBy)
    //            .HasMaxLength(100)
    //            .HasColumnName("REVOKED_BY");
    //        entity.Property(e => e.RevokedDate)
    //            .HasColumnType("datetime")
    //            .HasColumnName("REVOKED_DATE");
    //        entity.Property(e => e.Status)
    //            .HasMaxLength(50)
    //            .HasColumnName("STATUS");
    //    });

    //    modelBuilder.Entity<LesLicenseUpdateLog>(entity =>
    //    {
    //        entity.HasKey(e => e.Logid);

    //        entity.ToTable("LES_LICENSE_UPDATE_LOG");

    //        entity.Property(e => e.Logid).HasColumnName("LOGID");
    //        entity.Property(e => e.LicenseStatus)
    //            .HasMaxLength(50)
    //            .HasColumnName("LICENSE_STATUS");
    //        entity.Property(e => e.Licenseid).HasColumnName("LICENSEID");
    //        entity.Property(e => e.Remarks)
    //            .HasMaxLength(255)
    //            .HasColumnName("REMARKS");
    //        entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
    //        entity.Property(e => e.UpdatedDate)
    //            .HasColumnType("datetime")
    //            .HasColumnName("UPDATED_DATE");

    //        entity.HasOne(d => d.License).WithMany(p => p.LesLicenseUpdateLogs)
    //            .HasForeignKey(d => d.Licenseid)
    //            .HasConstraintName("FK_LES_LICENSE_UPDATE_LOG_LES_LICENSE_REGISTRY");
    //    });

    //    OnModelCreatingPartial(modelBuilder);
    //}

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
