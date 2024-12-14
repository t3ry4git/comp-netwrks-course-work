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

        public Simulator Simulator
        {
            get => default;
            set
            {
            }
        }

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


        public ResultSimulator Simulate(Node node1, Node node2, MessageConnectionType type, int sizeService, int sizeInform, int countInform, int max_error)
        {
            int messageSize = 0;
            int packetCount = 0;
            int countService = 0;
            int time = 0;
            bool found = true;
            List<Connection> cons = new List<Connection>();
            if (node1.Number != node2.Number)
            {
                var route = FordFulkerson.GetData(node1, node2, 0,new List<Node>(), null, max_error);
                int max_flow = route.flow;
                cons = route.connections;
                found = route.found;
                do
                {
                    route = FordFulkerson.GetData(node1, node2, 0, new List<Node>(), null, max_error);
                    if (route.flow > max_flow && route.found == true)
                    {
                        cons = route.connections;
                        max_flow = route.flow;
                        found = route.found;
                    }
                } while (route.found == true);
                foreach (var conny in Network.Connections)
                {
                    conny.ResetFlow();
                    conny.WeightUsed = 0;
                    conny.Highlighted = false;
                }
                if (found)
                {
                    if (type == MessageConnectionType.TCP)
                    {
                        // Step 1 handshake
                        foreach (Connection conn in cons)
                        {
                            var (retime, resendCount) = SendPacket(conn, sizeService);
                            time += retime;
                            countService += resendCount;
                        }

                        // Step 2 handshake
                        foreach (Connection conn in cons)
                        {
                            var (retime, resendCount) = SendPacket(conn, sizeService);
                            time += retime;
                            countService += resendCount;
                        }

                        // Step 3 handshake
                        foreach (Connection conn in cons)
                        {
                            var (retime, resendCount) = SendPacket(conn, sizeService);
                            time += retime;
                            countService += resendCount;
                        }

                        // Send info packets
                        for (int i = 0; i < countInform; i++)
                        {
                            // Send DATA
                            foreach (Connection conn in cons)
                            {
                                var (retime, resendCount) = SendPacket(conn, sizeInform);
                                time += retime;
                                countService += resendCount;
                            }

                            // Send ACK
                            foreach (Connection conn in cons)
                            {
                                var (retime, resendCount) = SendPacket(conn, sizeService);
                                time += retime;
                                countService += resendCount;
                            }
                        }

                        // Step FIN
                        foreach (Connection conn in cons)
                        {
                            var (retime, resendCount) = SendPacket(conn, sizeService);
                            time += retime;
                            countService += resendCount;
                        }
                        // Step ACK
                        foreach (Connection conn in cons)
                        {
                            var (retime, resendCount) = SendPacket(conn, sizeService);
                            time += retime;
                            countService += resendCount;
                        }
                        // Step FIN
                        foreach (Connection conn in cons)
                        {
                            var (retime, resendCount) = SendPacket(conn, sizeService);
                            time += retime;
                            countService += resendCount;
                        }
                        // Step ACK
                        foreach (Connection conn in cons)
                        {
                            var (retime, resendCount) = SendPacket(conn, sizeService);
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
                                var (retime, resendCount) = SendPacket(conn, sizeInform);
                                time += retime;
                            }
                            // Sending response
                            foreach (Connection conn in cons)
                            {
                                var (retime, resendCount) = SendPacket(conn, sizeInform);
                                time += retime;
                            }
                        }
                    }
                    packetCount = countInform + countService;
                    messageSize = (sizeInform * countInform) + (sizeService * countService);
                    return new ResultSimulator(type, node1, node2, messageSize, packetCount, sizeService, sizeInform, countService, countInform, time);
                }
                else
                {
                    return new ResultSimulator(type, node1, node2, 0, 0, 0, 0, 0, 0, int.MaxValue);
                }
            }
            else
            {
                return new ResultSimulator(type, node1, node2, 0, 0, 0, 0, 0, 0, 0);
            }
        }

        private (int retime, int resendCount) SendPacket(Connection conn, int sizeInform, int time = 0, int resended = 1)
        {
            time += conn.Weight + (sizeInform / conn.Weight);
            bool delivery = random.NextDouble() > conn.ChanceOfError;
            if (delivery)
            {
                if (conn.Type == ConnectionType.Duplex)
                {
                    time /= 2;
                }
                return (time + conn.Weight + (sizeInform / conn.Weight), resended);
            }
            else
                return SendPacket(conn, sizeInform, time, resended + 1);
        }

    }
}
