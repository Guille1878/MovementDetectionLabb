using System.Threading.Tasks;

namespace SendingDataToAzure
{
    public interface ISendingData
    {
        Task SendAsync(string message);
    }
}