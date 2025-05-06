using Supabase;

namespace ZinzotNet.Services
{
    public interface ISupabaseService
    {
        Supabase.Client Client { get; }
    }
}