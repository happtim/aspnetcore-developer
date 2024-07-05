<Query Kind="Statements">
  <NuGetReference>KdTree</NuGetReference>
  <Namespace>KdTree</Namespace>
  <Namespace>KdTree.Math</Namespace>
</Query>

var tree = new KdTree<float, int>(3, new FloatMath());
tree.Add(new[] { 50.0f, 80.0f , 10f}, 100);
tree.Add(new[] { 20.0f, 10.0f , 10f}, 201);
tree.Add(new[] { 20.0f, 10.0f , 10f}, 202);


var nodes = tree.GetNearestNeighbours(new[] { 30.0f, 20.0f ,0.1f}, 2);

nodes.Dump();