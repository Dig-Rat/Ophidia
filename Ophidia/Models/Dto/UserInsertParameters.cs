namespace Ophidia.Models.Dto
{
    public class UserInsertParameters
    {
        private string _Username = "";

        public string Username { get => _Username; set => _Username = value.Trim(); }
    }
}
