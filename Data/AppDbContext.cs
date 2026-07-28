using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> People { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<PeopleTagAssociation> PeopleTagAssociations { get; set; }

        public DbSet<Venue> Venues { get; set; }

        public DbSet<Floor> Floors { get; set; }

        public DbSet<Zone> Zones { get; set; }

        public DbSet<ZonePolygonPoint> ZonePolygonPoints { get; set; }

        public DbSet<LastPosition> LastPositions { get; set; }

        public DbSet<PositionHistory> PositionHistories { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurePeople(modelBuilder);
            ConfigureTags(modelBuilder);
            ConfigurePeopleTagAssociations(modelBuilder);
            ConfigureVenues(modelBuilder);
            ConfigureFloors(modelBuilder);
            ConfigureZones(modelBuilder);
            ConfigureZonePolygonPoints(modelBuilder);
            ConfigureLastPositions(modelBuilder);
            ConfigurePositionHistories(modelBuilder);
        }

        private static void ConfigurePeople(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("tblpeople");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                    .HasColumnName("id");

                entity.Property(p => p.Name)
                    .HasColumnName("name");

                entity.Property(p => p.Phone)
                    .HasColumnName("phone");

                entity.Property(z => z.UpdateStatus)
                    .HasColumnName("update_status")
                    .HasColumnType("update_status");

                entity.Property(p => p.CreateDate)
                    .HasColumnName("create_date");

                entity.Property(p => p.LastUpdate)
                    .HasColumnName("last_update");
            });
        }

        private static void ConfigureTags(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tbltags");

                entity.HasKey(t => t.Id);

                entity.Property(t => t.Id)
                    .HasColumnName("id");

                entity.Property(t => t.Label)
                    .HasColumnName("label");

                entity.Property(t => t.Mac)
                    .HasColumnName("mac");

                entity.Property(z => z.UpdateStatus)
                     .HasColumnName("update_status")
                     .HasColumnType("update_status");

                entity.Property(t => t.CreateDate)
                    .HasColumnName("create_date");

                entity.Property(t => t.LastUpdate)
                    .HasColumnName("last_update");
            });
        }

        private static void ConfigurePeopleTagAssociations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeopleTagAssociation>(entity =>
            {
                entity.ToTable("tblpeople_tag_association");

                entity.HasKey(a => new
                {
                    a.PeopleId,
                    a.TagId
                });

                entity.Property(a => a.PeopleId)
                    .HasColumnName("people_id");

                entity.Property(a => a.TagId)
                    .HasColumnName("tag_id");

                entity.Property(a => a.CreateDate)
                    .HasColumnName("create_date");

                entity.HasOne(a => a.Person)
                    .WithMany(p => p.TagAssociations)
                    .HasForeignKey(a => a.PeopleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Tag)
                    .WithMany(t => t.PeopleAssociations)
                    .HasForeignKey(a => a.TagId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureVenues(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Venue>(entity =>
            {
                entity.ToTable("tblvenues");

                entity.HasKey(v => v.Id);

                entity.Property(v => v.Id)
                    .HasColumnName("id");

                entity.Property(v => v.Name)
                    .HasColumnName("name");

                entity.Property(v => v.City)
                    .HasColumnName("city");

                entity.Property(z => z.UpdateStatus)
                    .HasColumnName("update_status")
                    .HasColumnType("update_status");

                entity.Property(v => v.CreateDate)
                    .HasColumnName("create_date");

                entity.Property(v => v.LastUpdate)
                    .HasColumnName("last_update");
            });
        }

        private static void ConfigureFloors(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Floor>(entity =>
            {
                entity.ToTable("tblfloors");

                entity.HasKey(f => f.Id);

                entity.Property(f => f.Id)
                    .HasColumnName("id");

                entity.Property(f => f.Name)
                    .HasColumnName("name");

                entity.Property(f => f.VenueId)
                    .HasColumnName("venue_id");

                entity.Property(f => f.Level)
                    .HasColumnName("level");

                entity.Property(z => z.UpdateStatus)
                    .HasColumnName("update_status")
                    .HasColumnType("update_status");

                entity.Property(f => f.CreateDate)
                    .HasColumnName("create_date");

                entity.Property(f => f.LastUpdate)
                    .HasColumnName("last_update");

                entity.HasOne(f => f.Venue)
                    .WithMany(v => v.Floors)
                    .HasForeignKey(f => f.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureZones(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Zone>(entity =>
            {
                entity.ToTable("tblzones");

                entity.HasKey(z => z.Id);

                entity.Property(z => z.Id)
                    .HasColumnName("id");

                entity.Property(z => z.Name)
                    .HasColumnName("name");

                entity.Property(z => z.FloorId)
                    .HasColumnName("floor_id");

                entity.Property(z => z.UpdateStatus)
                    .HasColumnName("update_status")
                    .HasColumnType("update_status");

                entity.Property(z => z.CreateDate)
                    .HasColumnName("create_date");

                entity.Property(z => z.LastUpdate)
                    .HasColumnName("last_update");

                entity.HasOne(z => z.Floor)
                    .WithMany(f => f.Zones)
                    .HasForeignKey(z => z.FloorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureZonePolygonPoints(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ZonePolygonPoint>(entity =>
            {
                entity.ToTable("tblzones_polygon_points");

                entity.HasKey(p => new
                {
                    p.ZoneId,
                    p.PointIndex
                });

                entity.Property(p => p.ZoneId)
                    .HasColumnName("zone_id");

                entity.Property(p => p.PointIndex)
                    .HasColumnName("point_index");

                entity.Property(p => p.X)
                    .HasColumnName("x");

                entity.Property(p => p.Y)
                    .HasColumnName("y");

                entity.Property(p => p.CreateDate)
                    .HasColumnName("create_date");

                entity.Property(p => p.LastUpdate)
                    .HasColumnName("last_update");

                entity.HasOne(p => p.Zone)
                    .WithMany(z => z.PolygonPoints)
                    .HasForeignKey(p => p.ZoneId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureLastPositions(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LastPosition>(entity =>
            {
                entity.ToTable("tbllast_position");

                entity.HasKey(p => p.PeopleId);

                entity.Property(p => p.PeopleId)
                    .HasColumnName("people_id");

                entity.Property(p => p.VenueId)
                    .HasColumnName("venue_id");

                entity.Property(p => p.FloorId)
                    .HasColumnName("floor_id");

                entity.Property(p => p.ZoneId)
                    .HasColumnName("zone_id");

                entity.Property(p => p.X)
                    .HasColumnName("x");

                entity.Property(p => p.Y)
                    .HasColumnName("y");

                entity.Property(p => p.CreateDate)
                    .HasColumnName("create_date");

                entity.Property(p => p.LastUpdate)
                    .HasColumnName("last_update");

                entity.HasOne(p => p.Person)
                    .WithMany()
                    .HasForeignKey(p => p.PeopleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Venue)
                    .WithMany()
                    .HasForeignKey(p => p.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Floor)
                    .WithMany()
                    .HasForeignKey(p => p.FloorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Zone)
                    .WithMany()
                    .HasForeignKey(p => p.ZoneId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigurePositionHistories(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PositionHistory>(entity =>
            {
                entity.ToTable("tblposition_history");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                    .HasColumnName("id");

                entity.Property(p => p.PeopleId)
                    .HasColumnName("people_id");

                entity.Property(p => p.VenueId)
                    .HasColumnName("venue_id");

                entity.Property(p => p.FloorId)
                    .HasColumnName("floor_id");

                entity.Property(p => p.ZoneId)
                    .HasColumnName("zone_id");

                entity.Property(p => p.X)
                    .HasColumnName("x");

                entity.Property(p => p.Y)
                    .HasColumnName("y");

                entity.Property(p => p.CreateDate)
                    .HasColumnName("create_date");

                entity.HasOne(p => p.Person)
                    .WithMany()
                    .HasForeignKey(p => p.PeopleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Venue)
                    .WithMany()
                    .HasForeignKey(p => p.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Floor)
                    .WithMany()
                    .HasForeignKey(p => p.FloorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Zone)
                    .WithMany()
                    .HasForeignKey(p => p.ZoneId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}