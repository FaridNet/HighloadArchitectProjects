using NBomber.Contracts.Stats;
using NBomber.CSharp;

namespace PerfomanceTests.Tests;

public static class Test1
{
    public static void Run()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("perfomance_scenario", async context =>
        {
            var response = await httpClient            
                .GetAsync("http://localhost:5156/user/search?SearchByFirstName=Александр&SearchByLastName=Барсуков");

            return response.IsSuccessStatusCode
                ? Response.Ok()
                : Response.Fail();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(rate: 50,
                              interval: TimeSpan.FromSeconds(1),
                              during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

    }
}
