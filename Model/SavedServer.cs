namespace ACCess.Model
{
    public class SavedServer
    {
        public string Address { get; set; }
        public string? Description { get; set; }
        public ushort Order { get; set; }

        public SavedServer(string address)
        {
            Address = address;
        }
        public SavedServer(string address, string description)
        {
            Address = address;
            Description = description;
        }
    }
}
