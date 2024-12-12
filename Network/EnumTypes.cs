namespace comp_netwrks_course_work
{
    public enum NodeType
    {
        Red,
        Green,
        Blue,
        Disabled
    }

    public enum ConnectionType
    {
        Satellite,
        Duplex,
        HalfDuplex,
        Custom,
        Random,
        Disabled
    }

    public enum Direction
    {
        Bidirectional,
        DirectionalNode1Node2,
        DirectionalNode2Node1,
        DirectionalUndefined
    }

    public enum MessageConnectionType
    {
        UDP,
        TCP
    }

    public enum PacketType
    {
        Datagram,
        VirtualChannel
    }
}
