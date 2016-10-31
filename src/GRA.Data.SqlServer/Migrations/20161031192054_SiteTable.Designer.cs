using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GRA.Data.SqlServer;

namespace GRA.Data.SqlServer.Migrations
{
    [DbContext(typeof(SqlServerContext))]
    [Migration("20161031192054_SiteTable")]
    partial class SiteTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GRA.Data.Model.Site", b =>
                {
                    b.Property<string>("Path");

                    b.Property<string>("Domain");

                    b.Property<string>("Name");

                    b.HasKey("Path");

                    b.ToTable("Sites");
                });
        }
    }
}
