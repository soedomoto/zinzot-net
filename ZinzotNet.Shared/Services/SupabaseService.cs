namespace ZinzotNet.Services
{
    public class SupabaseService : ISupabaseService
    {
        public Supabase.Client Client { get; }
        public SupabaseService()
        {
            var url = "https://ehqlobynfjypylnzojyp.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImVocWxvYnluZmp5cHlsbnpvanlwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzY1NTE2NTUsImV4cCI6MjA1MjEyNzY1NX0.uG6YciZ8tx3RolzxEDvIk3DEgLY4aMP2L3uCKdJKNvU";
            Client = new Supabase.Client(url, key);
        }
    }
}