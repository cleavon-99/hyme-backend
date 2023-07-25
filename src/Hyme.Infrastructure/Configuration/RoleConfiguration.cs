﻿using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyme.Infrastructure.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasConversion(id => id.Value, value => new RoleId(value));
            builder.Property(r => r.Name).HasMaxLength(50);         
        }
    }
}
