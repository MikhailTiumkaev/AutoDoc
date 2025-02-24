﻿// <auto-generated />
using System;
using AutoDocApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoDocApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250224081545_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AutoDocApi.Models.Payload", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("PayloadLocation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TodoTaskId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TodoTaskId");

                    b.ToTable("Payloads");
                });

            modelBuilder.Entity("AutoDocApi.Models.TodoTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TodoTasks");
                });

            modelBuilder.Entity("AutoDocApi.Models.Payload", b =>
                {
                    b.HasOne("AutoDocApi.Models.TodoTask", null)
                        .WithMany("Payloads")
                        .HasForeignKey("TodoTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AutoDocApi.Models.TodoTask", b =>
                {
                    b.Navigation("Payloads");
                });
#pragma warning restore 612, 618
        }
    }
}
