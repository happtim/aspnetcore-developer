<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.SettingManagement.EntityFrameworkCore</NuGetReference>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore</Namespace>
  <Namespace>Volo.Abp.SettingManagement.EntityFrameworkCore</Namespace>
</Query>


public class SettingManagementDbContext : AbpDbContext<SettingManagementDbContext>
{
	public SettingManagementDbContext(DbContextOptions<SettingManagementDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		
		modelBuilder.ConfigureSettingManagement();
	}
}