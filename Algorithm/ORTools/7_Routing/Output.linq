<Query Kind="Statements">
  <NuGetReference>Google.OrTools</NuGetReference>
  <Namespace>Google.OrTools.ConstraintSolver</Namespace>
</Query>


 static List<List<int>> GetRoutes(in Assignment solution,
										 in RoutingModel routing,
										 in RoutingIndexManager manager)
{
	// Get vehicle routes and store them in a two dimensional list, whose  
	// i, j entry is the node for the jth visit of vehicle i.  
	var routes = new List<List<int>>(manager.GetNumberOfVehicles());

	// Get routes.  
	for (int vehicleId = 0; vehicleId < manager.GetNumberOfVehicles(); ++vehicleId)
	{
		var route = new List<int>();
		long index = routing.Start(vehicleId);
		route.Add(manager.IndexToNode(index));
		while (!routing.IsEnd(index))
		{
			index = solution.Value(routing.NextVar(index));
			route.Add(manager.IndexToNode(index));
		}
		routes.Add(route);
	}
	return routes;
}

 static void DisplayRoutes(List<List<int>> routes)
{
	for (int vehicleId = 0; vehicleId < routes.Count; ++vehicleId)
	{
		string output = $"Route {vehicleId}:[ ";
		
		for (int j = 1; j < routes[vehicleId].Count; ++j)
		{
			output += routes[vehicleId][j] + " ";
		}
		Console.WriteLine( output + "]");
	}
}

/// <summary>
///   Print the solution.
/// </summary>
static void TSPPrintSolution(in RoutingModel routing, in RoutingIndexManager manager, in Assignment solution)
{
	Console.WriteLine("Objective: {0}", solution.ObjectiveValue());
	// Inspect solution.
	Console.WriteLine("Route:");
	long routeDistance = 0;
	var index = routing.Start(0);
	while (routing.IsEnd(index) == false)
	{
		Console.Write("{0} -> ", manager.IndexToNode((int)index));
		var previousIndex = index;
		index = solution.Value(routing.NextVar(index));
		routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
	}
	Console.WriteLine("{0}", manager.IndexToNode((int)index));
	Console.WriteLine("Route distance: {0}m", routeDistance);
}