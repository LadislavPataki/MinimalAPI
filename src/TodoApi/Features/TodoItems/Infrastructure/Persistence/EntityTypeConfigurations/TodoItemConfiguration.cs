using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApi.Features.TodoItems.DomainModels;

namespace TodoApi.Features.TodoItems.Infrastructure.Persistence.EntityTypeConfigurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(x => x.Id);
    }
}