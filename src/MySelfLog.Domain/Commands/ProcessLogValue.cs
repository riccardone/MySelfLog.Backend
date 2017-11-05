using Evento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySelfLog.Domain.Commands
{
    public class ProcessLogValue : Command
    {
        public int Value { get; }
        public int MmolValue { get; }
        public int SlowTerapy { get; }
        public int FastTerapy { get; }
        public int Calories { get; }
        public string ProfileName { get; }
        public string ProfileId { get; }
        public string ProfileNickName { get; }
        public ProcessLogValue(int value, int mmolValue, int slowTerapy, int fastTerapy, int calories, string profileName, string profileId, string profileNickName)
        {
            Value = value;
            MmolValue = mmolValue;
            SlowTerapy = slowTerapy;
            FastTerapy = fastTerapy;
            Calories = calories;
            ProfileName = profileName;
            ProfileId = profileId;
            ProfileNickName = profileNickName;
        }
    }
}
