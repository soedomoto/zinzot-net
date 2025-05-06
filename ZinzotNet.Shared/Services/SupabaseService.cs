namespace ZinzotNet.Services
{
    public class SupabaseService : ISupabaseService
    {
        public Supabase.Client Client { get; }
        public SupabaseService()
        {
            Client = new Supabase.Client(Constants.SupabaseUrl, Constants.SupabaseAnonKey);
        }
    }
}