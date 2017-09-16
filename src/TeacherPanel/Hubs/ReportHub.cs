using Microsoft.AspNet.SignalR;

namespace TeacherPanel.Hubs
{
    public class ReportHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}
