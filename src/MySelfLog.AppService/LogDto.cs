namespace MySelfLog.AppService
{
    public class LogDto
    {
        public int Value { get; protected set; }
        public int MmolValue { get; protected set; }
        public int SlowTerapy { get; protected set; }
        public int FastTerapy { get; protected set; }
        public int Calories { get; protected set; }
        public string ProfileName { get; protected set; }
        public string ProfileId { get; protected set; }
        public string ProfileNickName { get; protected set; }
    }
}
