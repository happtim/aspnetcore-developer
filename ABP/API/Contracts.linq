<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Ddd.Application.Contracts</NuGetReference>
  <Namespace>Volo.Abp.Application.Services</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

public interface IHelloWorldService : IApplicationService
{
	Task<String> GetCallHelloAsync();
}