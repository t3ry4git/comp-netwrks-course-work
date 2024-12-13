namespace comp_netwrks_course_work
{
    public class ResultSimulator
    {
        public MessageConnectionType Type { get; set; }
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public int MessageSize { get; set; }
        public int PacketCount { get; set; }
        public int SizeService { get; set; }
        public int SizeInform { get; set; }
        public int CountService { get; set; }
        public int CountInform { get; set; }
        public int Time { get; set; }

        public ResultSimulator(MessageConnectionType type, Node node1, Node node2, int messageSize, int packetCount, int sizeService, int sizeInform, int countService, int countInform, int time)
        {
            Type = type;
            Node1 = node1;
            Node2 = node2;
            MessageSize = messageSize;
            PacketCount = packetCount;
            SizeService = sizeService;
            SizeInform = sizeInform;
            CountService = countService;
            CountInform = countInform;
            Time = time;
        }
    }



    public class Simulator
    {
        NetworkAnalyzer Network;
        Random random = new Random();
        public Simulator(NetworkAnalyzer network)
        {
            Network = network;
        }
        public ResultSimulator Simulate(Node node1, Node node2, MessageConnectionType type, int sizeService, int sizeInform, int countInform, double chanceDelivery)
        {
            int messageSize = 0;
            int packetCount = 0;
            int countService = 0;
            int time = 0;

            List<Node> connects = ConnectionManipulator.GetOptimalPath(node1, node2, Network);
            List<Connection> cons = GetConnectionsBetweenNodes(connects);
            if (type == MessageConnectionType.TCP)
            {
                // Step 1 handshake
                foreach (Connection conn in cons)
                {
                    var (retime, resendCount) = SendPacket(conn, sizeService, chanceDelivery);
                    time += retime;
                    countService += resendCount;
                }

                // Step 2 handshake
                foreach (Connection conn in cons)
                {
                    var (retime, resendCount) = SendPacket(conn, sizeService, chanceDelivery);
                    time += retime;
                    countService += resendCount;
                }

                // Step 3 handshake
                foreach (Connection conn in cons)
                {
                    var (retime, resendCount) = SendPacket(conn, sizeService, chanceDelivery);
                    time += retime;
                    countService += resendCount;
                }

                // Send info packets
                for (int i = 0; i < countInform; i++)
                {
                    // Send DATA
                    foreach (Connection conn in cons)
                    {
                        var (retime, resendCount) = SendPacket(conn, sizeInform, chanceDelivery);
                        time += retime;
                        countService += resendCount;
                    }

                    // Send ACK
                    foreach (Connection conn in cons)
                    {
                        var (retime, resendCount) = SendPacket(conn, sizeService, chanceDelivery);
                        time += retime;
                        countService += resendCount;
                    }
                }

                // Step FIN
                foreach (Connection conn in cons)
                {
                    var (retime, resendCount) = SendPacket(conn, sizeService, chanceDelivery);
                    time += retime;
                    countService += resendCount;
                }
                // Step ACK
                foreach (Connection conn in cons)
                {
                    var (retime, resendCount) = SendPacket(conn, sizeService, chanceDelivery);
                    time += retime;
                    countService += resendCount;
                }
                // Step FIN
                foreach (Connection conn in cons)
                {
                    var (retime, resendCount) = SendPacket(conn, sizeService, chanceDelivery);
                    time += retime;
                    countService += resendCount;
                }
                // Step ACK
                foreach (Connection conn in cons)
                {
                    var (retime, resendCount) = SendPacket(conn, sizeService, chanceDelivery);
                    time += retime;
                    countService += resendCount;
                }
            }
            else
            {
                for (int i = 0; i < countInform; i++)
                {
                    //Sending request
                    foreach (Connection conn in cons)
                    {
                        var (retime, resendCount) = SendPacket(conn, sizeInform, chanceDelivery);
                        time += retime;
                    }
                    // Sending response
                    foreach (Connection conn in cons)
                    {
                        var (retime, resendCount) = SendPacket(conn, sizeInform, chanceDelivery);
                        time += retime;
                    }
                }
            }
            packetCount = countInform + countService;
            messageSize = (sizeInform * countInform) + (sizeService * countService);
            return new ResultSimulator(type, node1, node2, messageSize, packetCount, sizeService, sizeInform, countService, countInform, time);
        }

        // Not only UDP send request/response, also TCP when handshake and FIN/ACK
        private (int retime, int resendCount) SendPacket(Connection conn, int sizeInform, double chanceDelivery, int time = 0, int resended = 1)
        {
            time += conn.Weight + (sizeInform / conn.Weight);
            bool delivery = random.NextDouble() > chanceDelivery;
            if (delivery)
                return (time + conn.Weight + (sizeInform / conn.Weight), resended);
            else
                return SendPacket(conn, sizeInform, chanceDelivery, time, resended + 1);
        }

        private List<Connection> GetConnectionsBetweenNodes(List<Node> nodes)
        {
            var connections = new List<Connection>();
            foreach (var node in nodes)
                foreach (var connection in node.GetConnections())
                    if (!connections.Contains(connection))
                        if (nodes.Contains(connection.Node1) && nodes.Contains(connection.Node2))
                            connections.Add(connection);

            return connections;
        }

    }
}
