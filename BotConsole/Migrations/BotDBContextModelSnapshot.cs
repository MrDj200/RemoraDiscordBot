﻿// <auto-generated />
using System;
using BotConsole.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BotConsole.Migrations
{
    [DbContext(typeof(BotDBContext))]
    partial class BotDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.4");

            modelBuilder.Entity("BotConsole.Database.SavedMessageModel", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthorID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ChannelID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GuildID")
                        .HasColumnType("TEXT");

                    b.Property<string>("InvokerID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MessageID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("SavedMessages");
                });
#pragma warning restore 612, 618
        }
    }
}