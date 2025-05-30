using System.Threading;

public static class IdGenerator
{
    private static long _currentId = 0;

    public static long GetNextId()
    {
        return Interlocked.Increment(ref _currentId);
    }
}