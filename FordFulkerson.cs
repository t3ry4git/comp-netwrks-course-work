using comp_netwrks_course_work;
using System.Windows.Ink;

public class FordFulkerson
{

    public FordFulkerson()
    {

    }
    static bool NotAllConnectionsInVisited(List<Connection> connections, Node node)
    {
        return connections.Count == node.Connections.Count && !connections.Except(node.Connections).Any();
    }

    public static (bool found, List<Connection> connections, int flow) GetData(Node source, Node sink, int prev_max, Connection? visit = null)
    {
        if (source == sink)
        {
            return (true, new List<Connection>(), prev_max);
        }


        List<Connection> visited = new List<Connection>();
        if (visit != null) { 
        visited.Add(visit);
        }
        bool all_busy = false;

        while (!NotAllConnectionsInVisited(visited, source) && !all_busy) {
            int max_flow = 0;
            Connection? max_connection = null;
            Direction max_direction = Direction.DirectionalUndefined;
            foreach (var connection in source.Connections)
            {
                (Direction direct, int flow) = connection.getMaxFlow(source, prev_max);
                if (flow > max_flow && !visited.Contains(connection))
                {
                    max_flow = flow;
                    max_connection = connection;
                    max_direction = direct;
                }
            }
            if (max_connection != null)
            {
                max_connection.SendFlow(int.Min(max_flow, prev_max == 0 ? max_flow : prev_max), max_direction);
                (bool rfound, List<Connection> rconnections, int rflow) = GetData((max_direction == Direction.DirectionalNode1Node2) ? max_connection.Node2 : max_connection.Node1, sink, int.Min(max_flow, prev_max == 0 ? max_flow : prev_max),max_connection);
                if (rfound)
                {
                    rconnections.Add(max_connection);
                    max_connection.ResetFlow();
                    int count = rconnections.Count(conn => conn == max_connection);
                    (Direction d, int v) = max_connection.getMaxFlow(source,prev_max);
                    if (v >= count * rflow)
                    {
                        max_connection.SendFlow(rflow * count, max_direction);
                        return (rfound, rconnections, rflow);
                    }
                    else
                    {
                        visited.Add(max_connection);
                    }
                }
                else
                {
                    max_connection.RevertFlow();
                    visited.Add(max_connection);
                }
            }
            else
                all_busy = true;
        }
        return (false, visited, 0);
    }

}
