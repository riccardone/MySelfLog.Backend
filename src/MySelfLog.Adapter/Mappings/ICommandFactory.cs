using Evento;
using MySelfLog.Domain.Commands;

namespace MySelfLog.Adapter.Mappings
{
    public interface ICommandFactory
    {
        CreateDiary BuildCreateDiary();
        LogFood BuildLogFood();
        LogTerapy BuildLogTerapy();
        LogValue BuildLogValue();
    }
}
