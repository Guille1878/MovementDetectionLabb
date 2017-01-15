using Microsoft.Azure.Devices;
using System.Text;
using System.Threading.Tasks;

namespace SendingDataToAzure
{
    public class SendingData : ISendingData
    {

        private string deviceId = "WillesRaspbarryDevId";

        private ServiceClient serviceClient;

        public SendingData(string deviceId, string deviceConnectionString)
        {
            this.deviceId = deviceId;
            serviceClient = ServiceClient.CreateFromConnectionString(deviceConnectionString);
        }


        public async Task SendAsync(string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            await serviceClient.SendAsync(deviceId, commandMessage);

        }

    }
}
