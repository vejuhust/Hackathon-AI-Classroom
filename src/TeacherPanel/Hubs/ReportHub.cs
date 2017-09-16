namespace TeacherPanel.Hubs
{
    using Microsoft.AspNet.SignalR;

    public class ReportHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}
