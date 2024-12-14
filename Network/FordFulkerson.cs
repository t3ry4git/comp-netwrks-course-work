namespace comp_netwrks_course_work
{
    public static class FordFulkerson
    {
        public static TimedFordFulkersonWindow TimedFordFulkersonWindow
        {
            get => default;
            set
            {
            }
        }

        static bool NotAllConnectionsInVisited(List<Connection> connections, Node node)
        {
            return connections.Count == node.Connections.Count && !connections.Except(node.Connections).Any();
        }

        public static (bool found, List<Connection> connections, int flow) GetData(Node source, Node sink, int prev_max, List<Node> visited_nodes, Connection? visit = null, int max_error = 0)
        {
            
            if (source == sink)
            {
                return (true, new List<Connection>(), prev_max);
            }


            List<Connection> visited = new List<Connection>();
            if (visit != null)
            {
                visited.Add(visit);
            }
            bool all_busy = false;

            while (!NotAllConnectionsInVisited(visited, source) && !all_busy)
            {
                int max_flow = 0;
                Connection? max_connection = null;
                Direction max_direction = Direction.DirectionalUndefined;
                foreach (var connection in source.Connections)
                {
                    (Direction direct, int flow) = connection.GetMaxFlow(source, prev_max);
                    if (flow > max_flow && !visited.Contains(connection) && !(visited_nodes.Contains(connection.Node1) || visited_nodes.Contains(connection.Node2)))
                    {
                        max_flow = flow;
                        max_connection = connection;
                        max_direction = direct;
                    }
                }
                visited_nodes.Add(source);
                if (max_connection != null)
                {
                    bool good = max_connection.SendFlow(int.Min(max_flow, prev_max == 0 ? max_flow : prev_max), max_direction, max_error);
                    (bool rfound, List<Connection> rconnections, int rflow) =
                        GetData((max_direction == Direction.DirectionalNode1Node2) ?
                        max_connection.Node2 : max_connection.Node1, sink,
                        int.Min(max_flow, prev_max == 0 ? max_flow : prev_max),
                        visited_nodes,
                        max_connection,
                        max_error);
                    if (rfound && (good || max_error == -1))
                    {
                        rconnections.Add(max_connection);
                        max_connection.ResetFlow();
                        int count = rconnections.Count(conn => conn == max_connection);
                        (Direction d, int v) = max_connection.GetMaxFlow(source, prev_max);
                        if (v >= count * rflow)
                        {
                            max_connection.SendFlow(rflow * count, max_direction, max_error);
                            return (rfound, rconnections, rflow);
                        }
                        else
                        {
                            visited.Add(max_connection);
                            visited_nodes.Remove(source);
                        }
                    }
                    else
                    {
                        max_connection.RevertFlow();
                        visited_nodes.Remove(source);
                        visited.Add(max_connection);
                    }
                }
                else
                {
                    visited_nodes.Add(source);
                    all_busy = true;
                }
            }
            visited_nodes.Add(source);
            return (false, visited, 0);
        }
    }
}
